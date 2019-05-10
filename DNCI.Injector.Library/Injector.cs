using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using DNCI.Injector.Library;
namespace DNCI.Injector.Library
{
    /// <summary>
    /// Main Injector
    /// </summary>
    public static class Injector
    {
        // TODO: make one with Process Name and Resolve-it automatically


        /// <summary>
        /// Inject .NET Assembly into Remote Process
        /// </summary>
        /// <param name="methodName">The name of the managed method to execute. EX: EntryPoint (This method should be 'public static int')</param>
        /// <param name="assemblyFileLocation">The fully qualified path of the managed assembly to inject inside of the remote process. EX: C:\\MyExecutable.exe</param>
        /// <param name="typeName">The fully qualified type name of the managed assembly. EX: MyExecutable.MyClass</param>
        /// <param name="argumentString">An optional argument to pass in to the managed function</param>
        /// <param name="targetProcessName">The process name of the process to inject. EX: notepad.exe</param>
        /// <returns></returns>
        public static int InjectWithProcessName(String methodName, String assemblyFileLocation, String typeName, String argumentString, String targetProcessName)
        {
            // TODO: Better Error Handling
            return InjectWithPid(methodName, assemblyFileLocation, typeName, argumentString, Process.GetProcessesByName(targetProcessName)[0].Id);
        }

        /// <summary>
        /// Inject .NET Assembly into Remote Process
        /// </summary>
        /// <param name="methodName">The name of the managed method to execute. EX: EntryPoint (This method should be 'public static int')</param>
        /// <param name="assemblyFileLocation">The fully qualified path of the managed assembly to inject inside of the remote process. EX: C:\\MyExecutable.exe</param>
        /// <param name="typeName">The fully qualified type name of the managed assembly. EX: MyExecutable.MyClass</param>
        /// <param name="argumentString">An optional argument to pass in to the managed function</param>
        /// <param name="targetProcessId">The process ID of the process to inject. EX: 1569</param>
        /// <returns></returns>
        public static int InjectWithPid(String methodName, String assemblyFileLocation, String typeName, String argumentString, Int32 targetProcessId) 
        {


            //// enable debug privileges
            //EnablePrivilege(SE_DEBUG_NAME, TRUE);

            //// get handle to remote process
            //HANDLE hProcess = OpenProcess(PROCESS_ALL_ACCESS, FALSE, g_processId);              <<< OK

            //// inject bootstrap.dll into the remote process
            //FARPROC fnLoadLibrary = GetProcAddress(GetModuleHandle(L"Kernel32"), "LoadLibraryW");       <<< OK

            //Inject(hProcess, fnLoadLibrary, GetBootstrapPath()); // ret val on x86 is the base addr; on x64 addr gets truncated	 <<< OK

            //// add the function offset to the base of the module in the remote process
            //DWORD_PTR hBootstrap = GetRemoteModuleHandle(g_processId, BOOTSTRAP_DLL);           << OK

            //DWORD_PTR offset = GetFunctionOffset(GetBootstrapPath(), "ImplantDotNetAssembly");
            //DWORD_PTR fnImplant = hBootstrap + offset;

            //// build argument; use DELIM as tokenizer
            //wstring argument = g_moduleName + DELIM + g_typeName + DELIM + g_methodName + DELIM + g_Argument;

            //// inject the managed assembly into the remote process
            //Inject(hProcess, (LPVOID)fnImplant, argument);

            //// unload bootstrap.dll out of the remote process
            //FARPROC fnFreeLibrary = GetProcAddress(GetModuleHandle(L"Kernel32"), "FreeLibrary");
            //CreateRemoteThread(hProcess, NULL, 0, (LPTHREAD_START_ROUTINE)fnFreeLibrary, (LPVOID)hBootstrap, NULL, 0);

            //// close process handle
            //CloseHandle(hProcess);



            // Open and get handle of the process - with required privileges
            IntPtr targetProcessHandle = NativeExecution.OpenProcess(
                NativeConstants.PROCESS_ALL_ACCESS, 
                false,
                targetProcessId
            );

            // Resolve LoadLibraryW function pointer into Kernel32 address space
            IntPtr loadLibraryWAddr = NativeExecution.GetProcAddress(
                NativeExecution.GetModuleHandle("kernel32.dll"),
                "LoadLibraryW"
            );

            // Inject DNCIClrLoader into Remote Process
            Inject(targetProcessHandle, loadLibraryWAddr, @"C:\projetos\DNCI\Debug\DNCIClrLoader.dll"); // TODO: Achar em que lugar colocar, Talvez junto	

            // Inject a DLL into a process using the CreateRemoteThread method

            // Find the Loaded Module Pointer into Remote Process Memory

            // Find the LoadDNA Function Point into Remote Process Memory
            IntPtr x = FindRemoteModuleHandle(targetProcessHandle, "DNCIClrLoader.dll");

            return -1;
        }
  
        /// <summary>
        /// Find the "moduleName" into Remote Process
        /// </summary>
        /// <param name="targetProcessHandle">Target Process Handler</param>
        /// <param name="moduleName">Desired Module Name</param>
        /// <returns></returns>
        private static IntPtr FindRemoteModuleHandle(IntPtr targetProcessHandle, String moduleName)
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
                Console.WriteLine(moduleEntry.szModule);
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
        private static Int32 Inject(IntPtr processHandle, IntPtr functionPointer, String parameters)
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
