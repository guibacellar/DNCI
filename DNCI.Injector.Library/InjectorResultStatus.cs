using System;

namespace DNCI.Injector.Library
{
    /// <summary>
    /// Injector Result Status Enum
    /// </summary>
    public enum InjectorResultStatus : Int32
    {
        OK = 0,
        NOT_PROCESSED = 9,
        TARGET_PROCESS_NOT_FOUND = 9000,
        UNABLE_TO_GET_PROCESS_HANDLE = 9001,
        UNABLE_TO_FIND_INJECTOR_HANDLE = 9002,
        MASTER_ERROR = 9999
    }
}
