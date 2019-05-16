using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DNCI.Injector.Library
{
    /// <summary>
    /// Builder for <see cref="InjectorConfiguration"/>
    /// </summary>
    public sealed class InjectorConfigurationBuilder
    {
        private InjectorConfiguration configuration;

        /// <summary>
        /// Private Constructor
        /// </summary>
        private InjectorConfigurationBuilder()
        {
            this.configuration = new InjectorConfiguration();
            this.configuration.TargetProcessNames = new List<string>();
            this.configuration.TargetProcessIds = new List<int>();
        }

        /// <summary>
        /// The process name of the process to inject. Cumulative Option.  
        /// </summary>
        /// <param name="processName">notepad.exe</param>
        /// <returns></returns>
        public InjectorConfigurationBuilder InjectOnProcess(String processName)
        {
            this.configuration.TargetProcessNames.Add(processName);
            return this;
        }

        /// <summary>
        /// The process id of the process to inject. Cumulative Option.  
        /// </summary>
        /// <param name="processName">notepad.exe</param>
        /// <returns></returns>
        public InjectorConfigurationBuilder InjectOnProcess(Int32 processId)
        {
            this.configuration.TargetProcessIds.Add(processId);
            return this;
        }

        /// <summary>
        /// An optional argument to pass in to the managed function
        /// </summary>
        /// <param name="argumentString">STRING</param>
        /// <returns></returns>
        public InjectorConfigurationBuilder WithArguments(String argumentString)
        {
            this.configuration.ArgumentString = argumentString;
            return this;
        }

        /// <summary>
        /// The fully qualified path of the managed assembly to inject inside of the remote process. 
        /// </summary>
        /// <param name="assemblyFileLocation">C:\\MyExecutable.exe</param>
        /// <returns></returns>
        public InjectorConfigurationBuilder InjectThisClrBinary(String assemblyFileLocation)
        {
            this.configuration.AssemblyFileLocation = assemblyFileLocation;
            return this;
        }

        /// <summary>
        /// The name of the managed method to execute. EX: EntryPoint (This method should be 'public static int') 
        /// </summary>
        /// <param name="methodName">EntryPoint</param>
        /// <returns></returns>
        public InjectorConfigurationBuilder ClrMethodName(String methodName)
        {
            this.configuration.MethodName = methodName;
            return this;
        }

        /// <summary>
        /// The fully qualified type name of the managed assembly.  
        /// </summary>
        /// <param name="methodName">MyExecutable.MyClass</param>
        /// <returns></returns>
        public InjectorConfigurationBuilder ClrClassName(String className)
        {
            this.configuration.TypeName = className;
            return this;
        }


        /// <summary>
        /// Get a new InjectorConfigurationBuilder Instance
        /// </summary>
        /// <returns></returns>
        public static InjectorConfigurationBuilder Instance()
        {
            return new InjectorConfigurationBuilder();
        }

        /// <summary>
        /// Return Configuration Object
        /// </summary>
        /// <returns></returns>
        public InjectorConfiguration build()
        {
            return this.configuration;
        }
    }
}
