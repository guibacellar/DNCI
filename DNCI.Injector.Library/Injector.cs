using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using DNCI.Injector.Library.NativeCode;

namespace DNCI.Injector.Library
{
    /// <summary>
    /// Main Injector
    /// </summary>
    public class Injector
    {
        public static readonly int STATUS_OK = 0;

        public static readonly int STATUS_VALIDATION_TARGETPROCESS_CONFIGURATION_CONFLICT = 1000;

        public static readonly int STATUS_TARGET_PROCESS_NOT_FOUND = 9000;
        public static readonly int STATUS_UNABLE_TO_GET_PROCESS_HANDLE = 9001;
        public static readonly int STATUS_UNABLE_TO_FIND_INJECTOR_HANDLE = 9002;
        public static readonly int STATUS_MASTER_ERROR = 9999;
        private readonly InjectorConfiguration configuration;
        private string moduleTempFileName;

        /// <summary>
        /// Initialize Injector with Configuration
        /// </summary>
        /// <param name="configuration">Configurations</param>
        public Injector(InjectorConfiguration configuration)
        {
            this.configuration = configuration;
        }
        
        /// <summary>
        /// Execute the Injector
        /// </summary>
        /// <returns></returns>
        public int Run()
        {
            // Check ProcessID and ProcessName Conflict
            if (this.configuration.TargetProcessNames.Count > 0 && this.configuration.TargetProcessIds.Count > 0)
            {
                return STATUS_VALIDATION_TARGETPROCESS_CONFIGURATION_CONFLICT;
            }

            int hResult = STATUS_TARGET_PROCESS_NOT_FOUND;

            try
            {
                // Resolve ProcessName to ProcessId (if Applicable)
                if (this.configuration.TargetProcessNames.Count > 0) { hResult = RunForProcessNames(); }
                else { hResult = ExecuteForProcessIds(); }

            } catch
            {
                return STATUS_MASTER_ERROR;
            }

            return hResult;
        }

        /// <summary>
        /// Execute using Process Id List
        /// </summary>
        /// <param name="hResult"></param>
        /// <returns></returns>
        private int ExecuteForProcessIds()
        {
            int hResult = STATUS_TARGET_PROCESS_NOT_FOUND;

            foreach (Int32 processId in this.configuration.TargetProcessIds)
            {
                hResult = this.DoInject(processId);

                if (hResult == STATUS_OK)
                {
                    break;
                }
            }

            return hResult;
        }

        /// <summary>
        /// Execute using Process Name List
        /// </summary>
        /// <returns></returns>
        private int RunForProcessNames()
        {
            int hResult = STATUS_TARGET_PROCESS_NOT_FOUND;

            foreach (String processName in this.configuration.TargetProcessNames)
            {
                hResult = this.InjectWithProcessName(processName);

                if (hResult == STATUS_OK)
                {
                    break;
                }
            }

            return hResult;
        }

        /// <summary>
        /// Inject .NET Assembly into Remote Process using Process Name as Parameter
        /// </summary>
        /// <param name="targetProcessName">The process Name of the process to inject. EX: notepad++</param>
        /// <returns></returns>
        public int InjectWithProcessName(String targetProcessName)
        {
            // Get Possible Process
            Process[] targetProcessList = Process.GetProcessesByName(targetProcessName);

            // Check if Process Exists
            if (targetProcessList == null || targetProcessList.Length == 0)
            {
                return STATUS_MASTER_ERROR;
            }

            return this.DoInject(targetProcessList[0].Id);
        }

        /// <summary>
        /// Inject .NET Assembly into Remote Process
        /// </summary>
        /// <returns></returns>
        private int DoInject(Int32 targetProcessId) 
        {
            // Copy DNCIClrLoader.dll into Temporary Folder with Random Name
            String dnciLoaderLibraryPath = WriteLoaderToDisk();

            // Open and get handle of the process - with required privileges
            IntPtr targetProcessHandle = NativeExecution.OpenProcess(
                NativeConstants.PROCESS_ALL_ACCESS,
                false,
                targetProcessId
            );

            // Check Process Handle 
            if (targetProcessHandle == null || targetProcessHandle == IntPtr.Zero)
            {
                return STATUS_UNABLE_TO_GET_PROCESS_HANDLE;
            }

            // Find the LoadDNA Function Point into Remote Process Memory
            IntPtr dnciModuleHandle = DNCIClrLoader(targetProcessHandle, dnciLoaderLibraryPath, Path.GetFileName(dnciLoaderLibraryPath));

            // Check Injector Handle
            if (dnciModuleHandle == null || dnciModuleHandle == IntPtr.Zero)
            {
                return STATUS_UNABLE_TO_FIND_INJECTOR_HANDLE;
            }

            // Inject Managed Assembly
            LoadManagedAssemblyOnRemoteProcess(targetProcessHandle, dnciModuleHandle, this.configuration.MethodName, this.configuration.AssemblyFileLocation, this.configuration.TypeName, this.configuration.ArgumentString, dnciLoaderLibraryPath);

            // Erase Modules from Target Process
            EraseRemoteModules(targetProcessHandle, dnciModuleHandle);

            // Close Remote Process Handle
            NativeExecution.CloseHandle(targetProcessHandle);

            // Remove Temporary File
            try { 
                File.Delete(dnciLoaderLibraryPath);
            } catch { }

            return STATUS_OK;
        }

        /// <summary>
        /// Write the C++ Loader into Disk
        /// </summary>
        /// <returns>Temporary File Path </returns>
        private string WriteLoaderToDisk()
        {
            if (this.moduleTempFileName == null)
            {
                this.moduleTempFileName = Path.GetTempFileName().Replace(".tmp", ".dll");

                using (FileStream fs = new FileStream(this.moduleTempFileName, FileMode.Create))
                {
                    byte[] rawBytes = Convert.FromBase64String(ASCIIEncoding.ASCII.GetString(
                            Properties.Resources.DNCIClrLoader
                        )
                    );
                    fs.Write(rawBytes, 0, rawBytes.Length);

                    fs.Flush();
                    fs.Close();
                }
            }

            return moduleTempFileName;
        }

        /// <summary>
        /// Removes from Target Process Memory the DNCIClrLoader
        /// </summary>
        /// <param name="targetProcessHandle">Target Process Handle</param>
        /// <param name="dnciModuleHandle">DNCIClrLoader Module Handle</param>
        private void EraseRemoteModules(IntPtr targetProcessHandle, IntPtr dnciModuleHandle)
        {
            // Resolve FreeLibrary function pointer into kernel32 address space
            IntPtr freeLibraryHandle = NativeExecution.GetProcAddress(NativeExecution.GetModuleHandle("Kernel32"), "FreeLibrary");

            // Unload DNCIClrLoader.dll from Remote Process
            NativeExecution.CreateRemoteThread(targetProcessHandle, IntPtr.Zero, 0, freeLibraryHandle, dnciModuleHandle, 0, IntPtr.Zero);
        }

        /// <summary>
        /// Inject the DNCLClrLoader.dll into Target Process Memory
        /// </summary>
        /// <param name="targetProcessHandle">Target Process Handle</param>
        /// <param name="injectorLibraryFilePath">DNCIClrLoader.dll File Path</param>
        /// <param name="moduleName">Name of Module (usually, FILE_NAME.dll)</param>
        /// <returns></returns>
        private IntPtr DNCIClrLoader(IntPtr targetProcessHandle, String injectorLibraryFilePath, String moduleName)
        {
            // Resolve LoadLibraryW function pointer into Kernel32 address space
            IntPtr loadLibraryWAddr = NativeExecution.GetProcAddress(
                NativeExecution.GetModuleHandle("kernel32.dll"),
                "LoadLibraryW"
            );

            // Inject DNCIClrLoader into Remote Process
            Inject(targetProcessHandle, loadLibraryWAddr, injectorLibraryFilePath);

            // Find the LoadDNA Function Point into Remote Process Memory
            return FindRemoteModuleHandle(targetProcessHandle, moduleName);
        }

        /// <summary>
        /// Inject Managed Assembly into Remote Unmanaged Process using DNCIClrLoader as Bridge
        /// </summary>
        /// <param name="targetProcessHandle">Target Process Handle</param>
        /// <param name="dnciModuleHandle">DNCIClrLoader Module Handle</param>
        /// <param name="methodName">The name of the managed method to execute. EX: EntryPoint (This method should be 'public static int')</param>
        /// <param name="assemblyFileLocation">The fully qualified path of the managed assembly to inject inside of the remote process. EX: C:\\MyExecutable.exe</param>
        /// <param name="typeName">The fully qualified type name of the managed assembly. EX: MyExecutable.MyClass</param>
        /// <param name="argumentString">An optional argument to pass in to the managed function</param>
        /// <param name="injectorLibraryFilePath">DNCIClrLoader.dll File Path</param>
        private void LoadManagedAssemblyOnRemoteProcess(IntPtr targetProcessHandle, IntPtr dnciModuleHandle, String methodName, String assemblyFileLocation, String typeName, String argumentString, String injectorLibraryFilePath)
        {
            // Find Target Function OffSet
            uint targetOffset = GetFunctionOffSet(injectorLibraryFilePath, "LoadDNA");

            // Compute OffSet into Remote Target
            uint remoteTargetOffSet = targetOffset + (uint) dnciModuleHandle.ToInt32();

            // Build LoadDNA Function Arguments
            String loadDnaArgs = assemblyFileLocation + "\t" + typeName + "\t" + methodName + "\t" + argumentString;

            // Inject .NET Assembly using LoadDNA Function on DNCIClrLoader.dll
            Inject(targetProcessHandle, new IntPtr(remoteTargetOffSet), loadDnaArgs);
        }

        /// <summary>
        /// Get Target Function OffSet
        /// </summary>
        /// <param name="libraryPath">Full Library Path</param>
        /// <param name="targetFunctionName"></param>
        /// <returns></returns>
        private uint GetFunctionOffSet(String libraryPath, String targetFunctionName)
        {
            // Load the Library
            IntPtr libHandle = NativeExecution.LoadLibrary(libraryPath);

            // Get Target Function Address
            IntPtr functionPtr = NativeExecution.GetProcAddress(libHandle, targetFunctionName);

            // Compute the OffSet Between the Library Base Address and the Target Function inside the Binary
            uint offset = (uint) functionPtr.ToInt32() - (uint) libHandle.ToInt32();

            // Unload Library from Memory
            NativeExecution.FreeLibrary(libHandle);

            return offset;
        }
  
        /// <summary>
        /// Find the "moduleName" into Remote Process
        /// </summary>
        /// <param name="targetProcessHandle">Target Process Handler</param>
        /// <param name="moduleName">Desired Module Name</param>
        /// <returns></returns>
        private IntPtr FindRemoteModuleHandle(IntPtr targetProcessHandle, String moduleName)
        {
            NativeStructures.MODULEENTRY32 moduleEntry = new NativeStructures.MODULEENTRY32() {
                dwSize = (uint)Marshal.SizeOf(typeof(NativeStructures.MODULEENTRY32))
            };

            uint targetProcessId = NativeExecution.GetProcessId(targetProcessHandle);

            IntPtr snapshotHandle = NativeExecution.CreateToolhelp32Snapshot(
                NativeStructures.SnapshotFlags.Module | NativeStructures.SnapshotFlags.Module32, 
                targetProcessId
            );

            // Check if is Valid
            if (!NativeExecution.Module32First(snapshotHandle, ref moduleEntry))
            {
                NativeExecution.CloseHandle(snapshotHandle);
                return IntPtr.Zero;
            }

            // Enumerate all Modules until find the "moduleName"
            while (NativeExecution.Module32Next(snapshotHandle, ref moduleEntry))
            {
                if (moduleEntry.szModule == moduleName)
                {
                    break;
                }
            }

            // Close the Handle
            NativeExecution.CloseHandle(snapshotHandle);


            // Return if Success on Search
            if (moduleEntry.szModule == moduleName)
            {
                return moduleEntry.modBaseAddr;
            }

            return IntPtr.Zero;
        }

        /// <summary>
        /// Inject the "functionPointer" with "parameters" into Remote Process
        /// </summary>
        /// <param name="processHandle">Remote Process Handle</param>
        /// <param name="functionPointer">LoadLibraryW Function Pointer</param>
        /// <param name="clrLoaderFullPath">DNCIClrLoader.exe Full Path</param>
        private Int32 Inject(IntPtr processHandle, IntPtr functionPointer, String parameters)
        {
            // Set Array to Write
            byte[] toWriteData = Encoding.Unicode.GetBytes(parameters);

            // Compute Required Space on Remote Process
            uint requiredRemoteMemorySize = (uint)(
                (toWriteData.Length) * Marshal.SizeOf(typeof(char))
            ) + (uint)Marshal.SizeOf(typeof(char));

            // Alocate Required Memory Space on Remote Process
            IntPtr allocMemAddress = NativeExecution.VirtualAllocEx(
                processHandle,
                IntPtr.Zero,
                requiredRemoteMemorySize,
                NativeConstants.MEM_RESERVE | NativeConstants.MEM_COMMIT,
                NativeConstants.PAGE_READWRITE
            );

            // Write Argument on Remote Process
            UIntPtr bytesWritten;
            bool success = NativeExecution.WriteProcessMemory(
                processHandle, 
                allocMemAddress,
                toWriteData, 
                requiredRemoteMemorySize, 
                out bytesWritten
            );

            // Create Remote Thread
            IntPtr createRemoteThread = NativeExecution.CreateRemoteThread(
                processHandle,
                IntPtr.Zero, 
                0, 
                functionPointer, 
                allocMemAddress, 
                0,
                IntPtr.Zero
            );
    
            // Wait Thread to Exit
            NativeExecution.WaitForSingleObject(createRemoteThread, NativeConstants.INFINITE);

            // Release Memory in Remote Process
            NativeExecution.VirtualFreeEx(processHandle, allocMemAddress, 0, NativeConstants.MEM_RELEASE);

            // Get Thread Exit Code
            Int32 exitCode;
            NativeExecution.GetExitCodeThread(createRemoteThread, out exitCode);

            // Close Remote Handle
            NativeExecution.CloseHandle(createRemoteThread);

            return exitCode;
        }

    }
}
