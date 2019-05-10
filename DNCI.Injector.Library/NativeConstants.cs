using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DNCI.Injector.Library
{
    /// <summary>
    /// Constants for Native Code Execution
    /// </summary>
    internal static class NativeConstants
    {

        // privileges
        internal const int PROCESS_TERMINATE = 0x00000001;
        internal const int PROCESS_CREATE_THREAD = 0x00000002;
        internal const int PROCESS_SET_SESSIONID     = 0x00000004;
        internal const int PROCESS_VM_OPERATION = 0x00000008;
        internal const int PROCESS_VM_READ       = 0x00000010;
        internal const int PROCESS_VM_WRITE      = 0x00000020;
        internal const int PROCESS_DUP_HANDLE = 0x00000040;
        internal const int PROCESS_CREATE_PROCESS = 0x00000080;
        internal const int PROCESS_SET_QUOTA = 0x00000100;
        internal const int PROCESS_SET_INFORMATION   = 0x00000200;
        internal const int PROCESS_QUERY_INFORMATION = 0x00000400;
        internal const int STANDARD_RIGHTS_REQUIRED = 0x000F0000;
        internal const int SYNCHRONIZE = 0x00100000;
        internal const int PROCESS_ALL_ACCESS = PROCESS_TERMINATE | PROCESS_CREATE_THREAD | PROCESS_SET_SESSIONID | PROCESS_VM_OPERATION |
          PROCESS_VM_READ | PROCESS_VM_WRITE | PROCESS_DUP_HANDLE | PROCESS_CREATE_PROCESS | PROCESS_SET_QUOTA |
          PROCESS_SET_INFORMATION | PROCESS_QUERY_INFORMATION | STANDARD_RIGHTS_REQUIRED | SYNCHRONIZE | 0xFFFF;



        // used for memory allocation
        internal const uint MEM_COMMIT = 0x00001000;
        internal const uint MEM_RESERVE = 0x00002000;
        internal const uint MEM_RELEASE = 0x00008000;
        internal const uint PAGE_READWRITE = 0x00000040;

        // used for WaitForSingleObject
        internal const uint INFINITE = 0xFFFFFFFF;

        // used for CreateToolhelp32Snapshot
        internal const UInt32 TH32CS_SNAPMODULE = 0x00000008;
    }
}
