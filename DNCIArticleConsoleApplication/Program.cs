/****************************************************************/
/* WARNING: this code should be used only for research purpose. */
/* Author: Th3 0bservator                                       */
/* https://www.theobservator.net/about                          */
/****************************************************************/

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

/// <summary>
/// This is a POC Console Application. All required components are declared here and this code do not handle any type of error.
/// Please, check the Main method to verify the program operation.
/// </summary>
namespace DNCIArticleConsoleApplication
{
    class Program
    {
        // privileges
         const int PROCESS_TERMINATE = 0x00000001;
         const int PROCESS_CREATE_THREAD = 0x00000002;
         const int PROCESS_SET_SESSIONID = 0x00000004;
         const int PROCESS_VM_OPERATION = 0x00000008;
         const int PROCESS_VM_READ = 0x00000010;
         const int PROCESS_VM_WRITE = 0x00000020;
         const int PROCESS_DUP_HANDLE = 0x00000040;
         const int PROCESS_CREATE_PROCESS = 0x00000080;
         const int PROCESS_SET_QUOTA = 0x00000100;
         const int PROCESS_SET_INFORMATION = 0x00000200;
         const int PROCESS_QUERY_INFORMATION = 0x00000400;
         const int STANDARD_RIGHTS_REQUIRED = 0x000F0000;
         const int SYNCHRONIZE = 0x00100000;
         const int PROCESS_ALL_ACCESS = PROCESS_TERMINATE | PROCESS_CREATE_THREAD | PROCESS_SET_SESSIONID | PROCESS_VM_OPERATION |
          PROCESS_VM_READ | PROCESS_VM_WRITE | PROCESS_DUP_HANDLE | PROCESS_CREATE_PROCESS | PROCESS_SET_QUOTA |
          PROCESS_SET_INFORMATION | PROCESS_QUERY_INFORMATION | STANDARD_RIGHTS_REQUIRED | SYNCHRONIZE | 0xFFFF;

        // used for memory allocation
         const uint MEM_COMMIT = 0x00001000;
         const uint MEM_RESERVE = 0x00002000;
         const uint MEM_RELEASE = 0x00008000;
         const uint PAGE_READWRITE = 0x00000040;

        // used for WaitForSingleObject
         const uint INFINITE = 0xFFFFFFFF;

        // used for CreateToolhelp32Snapshot
         const UInt32 TH32CS_SNAPMODULE = 0x00000008;

        [DllImport("kernel32.dll")]
         static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
         static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
         static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
         static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
         static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out UIntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
         static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

        [DllImport("kernel32.dll")]
         static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, out IntPtr lpThreadId);

        [DllImport("kernel32.dll", SetLastError = true)]
         static extern UInt32 WaitForSingleObject(IntPtr hHandle, UInt32 dwMilliseconds);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
         static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, uint dwFreeType);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
         static extern unsafe bool VirtualFreeEx(IntPtr hProcess, byte* pAddress, int size, uint freeType);

        [DllImport("Kernel32", ExactSpelling = true, CharSet = CharSet.Auto)]
         static extern bool GetExitCodeThread(IntPtr hHandle, out int lpdwExitCode);

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
         static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)]string lpFileName);

        [DllImport("kernel32.dll", SetLastError = true)]
         static extern uint GetProcessId(IntPtr handle);

        [DllImport("kernel32.dll", SetLastError = true)]
         static extern bool CloseHandle(IntPtr hHandle);

        [DllImport("kernel32.dll")]
         static extern bool Module32First(IntPtr hSnapshot, ref MODULEENTRY32 lpme);

        [DllImport("kernel32.dll")]
         static extern bool Module32Next(IntPtr hSnapshot, ref MODULEENTRY32 lpme);

        [DllImport("kernel32.dll", SetLastError = true)]
         static extern IntPtr CreateToolhelp32Snapshot(SnapshotFlags dwFlags, uint th32ProcessID);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
         static extern bool FreeLibrary(IntPtr hModule);

        [Flags]
        public enum SnapshotFlags : uint
        {
            HeapList = 0x00000001,
            Process = 0x00000002,
            Thread = 0x00000004,
            Module = 0x00000008,
            Module32 = 0x00000010,
            Inherit = 0x80000000,
            All = 0x0000001F
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct MODULEENTRY32
        {
            public uint dwSize;
            public uint th32ModuleID;
            public uint th32ProcessID;
            public uint GlblcntUsage;
            public uint ProccntUsage;
            public IntPtr modBaseAddr;
            public uint modBaseSize;
            IntPtr hModule;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szModule;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szExePath;
        };

static int Main(string[] args)
{

    /*** Configuration Variables ***/
    String targetProcessName = "notepad++";     // Define Target Process
    String clrLoaderLibraryPath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "DNCIClrLoader.dll");   // Get CLR Runtime Loader Path
    String clrLoaderLibraryFileName = "DNCIClrLoader.dll";  // CLR Runtime Loader DLL Name

    String targetDotNetAssemblyPath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "InjectDemo.Console.ClassicNet.exe");
    String targetDotNetAssemblyEntryPointAssemblyType = "InjectDemo.Console.ClassicNet.Program";
    String targetDotNetAssemblyEntryPointMethodName = "EntryPoint";
    String targetDotNetAssemblyEntryPointMethodParameters = "Parameter String OK";

    // Find the Process Info
    Int32 targetProcessId = Process.GetProcessesByName(targetProcessName)[0].Id;
       


    /*** Open and get handle of the process - with required privileges ***/ 
    IntPtr targetProcessHandle = OpenProcess(PROCESS_ALL_ACCESS, false, targetProcessId);
    if (targetProcessHandle == null || targetProcessHandle == IntPtr.Zero)
    {
        return -1;
    }



    /*** Inject CLR Runtime Loader into Remote Process ***/
    Inject(targetProcessHandle, GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryW"), clrLoaderLibraryPath);



    /*** Get Module (C++ CLR Runtime Loader) Handle ***/
    IntPtr clrRuntimeLoaderHandle = FindRemoteModuleHandle(targetProcessHandle, clrLoaderLibraryFileName);
    if (clrRuntimeLoaderHandle == null || clrRuntimeLoaderHandle == IntPtr.Zero)
    {
        return -2;
    }



    /*** Load .NET Assembly into Remote Process ***/

    // Find LoadDNA Function from C++ CLR Runtime Loader into Remote Process Memory
    uint targetOffset = GetFunctionOffSet(clrLoaderLibraryPath, "LoadDNA");

    // Compute OffSet into Remote Target
    uint remoteTargetOffSet = targetOffset + (uint)clrRuntimeLoaderHandle.ToInt32();

    // Build LoadDNA Function Arguments
    String loadDnaArgs = targetDotNetAssemblyPath + "\t" + targetDotNetAssemblyEntryPointAssemblyType + "\t" + targetDotNetAssemblyEntryPointMethodName + "\t" + targetDotNetAssemblyEntryPointMethodParameters;

    // Inject .NET Assembly using LoadDNA Function on DNCIClrLoader.dll
    Inject(targetProcessHandle, new IntPtr(remoteTargetOffSet), loadDnaArgs);



    /*** Remove Module from Remote Process ***/

    // Close Remote Process Handle
    CloseHandle(targetProcessHandle);

    return 0;
}


        /// <summary>
        /// Get Target Function OffSet
        /// </summary>
        /// <param name="libraryPath">Full Library Path</param>
        /// <param name="targetFunctionName"></param>
        /// <returns></returns>
        static uint GetFunctionOffSet(String libraryPath, String targetFunctionName)
        {
            // Load the Library
            IntPtr libHandle = LoadLibrary(libraryPath);

            // Get Target Function Address
            IntPtr functionPtr = GetProcAddress(libHandle, targetFunctionName);

            // Compute the OffSet Between the Library Base Address and the Target Function inside the Binary
            uint offset = (uint)functionPtr.ToInt32() - (uint)libHandle.ToInt32();

            // Unload Library from Memory
            FreeLibrary(libHandle);

            return offset;
        }

        /// <summary>
        /// Find the "moduleName" into Remote Process
        /// </summary>
        /// <param name="targetProcessHandle">Target Process Handler</param>
        /// <param name="moduleName">Desired Module Name</param>
        /// <returns></returns>
        static IntPtr FindRemoteModuleHandle(IntPtr targetProcessHandle, String moduleName)
        {
            MODULEENTRY32 moduleEntry = new MODULEENTRY32()
            {
                dwSize = (uint)Marshal.SizeOf(typeof(MODULEENTRY32))
            };

            uint targetProcessId = GetProcessId(targetProcessHandle);

            IntPtr snapshotHandle = CreateToolhelp32Snapshot(
                SnapshotFlags.Module | SnapshotFlags.Module32,
                targetProcessId
            );

            // Check if is Valid
            if (!Module32First(snapshotHandle, ref moduleEntry))
            {
                CloseHandle(snapshotHandle);
                return IntPtr.Zero;
            }

            // Enumerate all Modules until find the "moduleName"
            while (Module32Next(snapshotHandle, ref moduleEntry))
            {
                if (moduleEntry.szModule == moduleName)
                {
                    break;
                }
            }

            // Close the Handle
            CloseHandle(snapshotHandle);

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
        static Int32 Inject(IntPtr processHandle, IntPtr functionPointer, String parameters)
        {
            // Set Array to Write
            byte[] toWriteData = Encoding.Unicode.GetBytes(parameters);

            // Compute Required Space on Remote Process
            uint requiredRemoteMemorySize = (uint)(
                (toWriteData.Length) * Marshal.SizeOf(typeof(char))
            ) + (uint)Marshal.SizeOf(typeof(char));

            // Alocate Required Memory Space on Remote Process
            IntPtr allocMemAddress = VirtualAllocEx(
                processHandle,
                IntPtr.Zero,
                requiredRemoteMemorySize,
                MEM_RESERVE | MEM_COMMIT,
                PAGE_READWRITE
            );

            // Write Argument on Remote Process
            UIntPtr bytesWritten;
            bool success = WriteProcessMemory(
                processHandle,
                allocMemAddress,
                toWriteData,
                requiredRemoteMemorySize,
                out bytesWritten
            );

            // Create Remote Thread
            IntPtr createRemoteThread = CreateRemoteThread(
                processHandle,
                IntPtr.Zero,
                0,
                functionPointer,
                allocMemAddress,
                0,
                IntPtr.Zero
            );

            // Wait Thread to Exit
            WaitForSingleObject(createRemoteThread, INFINITE);

            // Release Memory in Remote Process
            VirtualFreeEx(processHandle, allocMemAddress, 0, MEM_RELEASE);

            // Get Thread Exit Code
            Int32 exitCode;
            GetExitCodeThread(createRemoteThread, out exitCode);

            // Close Remote Handle
            CloseHandle(createRemoteThread);

            return exitCode;
        }

    }
}
