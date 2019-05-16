using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DNCI.Injector.Library
{
    /// <summary>
    /// Contains the Configuration and Parameters for the Injector
    /// </summary>
    public sealed class InjectorConfiguration
    {
        /// <summary>
        /// The name of the managed method to execute. EX: EntryPoint (This method should be 'public static int')
        /// </summary>
        public String MethodName { get; set; }

        /// <summary>
        /// The fully qualified path of the managed assembly to inject inside of the remote process. EX: C:\\MyExecutable.exe
        /// </summary>
        public String AssemblyFileLocation { get; set; }

        /// <summary>
        /// The fully qualified type name of the managed assembly. EX: MyExecutable.MyClass
        /// </summary>
        public String TypeName { get; set; }

        /// <summary>
        /// An optional argument to pass in to the managed function
        /// </summary>
        public String ArgumentString { get; set; }

        /// <summary>
        /// The process name of the process to inject. EX: notepad.exe
        /// </summary>
        public List<String> TargetProcessNames { get; set; }

        /// <summary>
        /// The process ID of the process to inject. EX: 1598
        /// </summary>
        public List<Int32> TargetProcessIds { get; set; }
    }
}

