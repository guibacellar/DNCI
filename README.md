# DNCI
Dot Net Code Injector

# Project Structure
The project is structured in:

  * **DNCI.Injector.Library** - Injection library. Contains all injection components and logic;
  * **DNCI.Injector.Runner**  - Command line utility for injection.
  * **DNCIClrLoader**         - C++ MicroCode to Load the .NET assembly into memory;
  * **InjectDemo.Console.ClassicNet** - Demo Classic .Net Console Application to be injected;
  * **InjectDemo.Console.DotNetCore** - Demo .Net Core Console Application to be injected;
  * **InjectDemo.Dll.ClassicNet** - Demo Classic .Net DLL to be injected;

# Documentation and Usage:

## Command Line Utility Documentation
 * Parameters:
   *   --help                                         Show help information
   * --assemblyFile <SOURCE_FILE_PATH>              Target .NET Classic DLL File
   * --className <TARGET_CLASS_NAME>                The fully qualified type name of the managed assembly
   * --methodName <TARGET_CLASS_ENTRYPOINT_METHOD>  The name of the managed method to execute. EX: EntryPoint (This method should be 'public static int')
   * --argument <ENTRYPOINT_METHOD_ARGUMENT>        An optional argument to pass in to the managed function
   * --targetMode <TARGET_MODE>                     Injection Target Mode (BruteForce, PID, ProcessName)
   * --pid <TARGET_PROCESS_ID>                      Target Process ID
   * --processName <TARGET_PROCESS_Name>            Target Process Name

#### Example
**Inject Classic .Net Console Application into Notepad++** 

This example used the *InjectDemo.Console.ClassicNet* .exe file.
DNCI.Injector.Runner.exe --assemblyFile "<PATH_TO_FILE>\InjectDemo.Console.ClassicNet.exe" --className InjectDemo.Console.ClassicNet.Program --methodName EntryPoint --targetMode=processName --processName notepad++ --argument "OK BOY"

**Inject Classic .Net Console Application into Process with ID 66** 

This example used the *InjectDemo.Console.ClassicNet* .exe file.
DNCI.Injector.Runner.exe --assemblyFile "<PATH_TO_FILE>\InjectDemo.Console.ClassicNet.exe" --className InjectDemo.Console.ClassicNet.Program --methodName EntryPoint --targetMode=PID --pid 66 --argument "OK BOY"

**Try to Inject Classic .Net Console Application into any Running Process** 

This example used the *InjectDemo.Console.ClassicNet* .exe file.
DNCI.Injector.Runner.exe --assemblyFile "<PATH_TO_FILE>\InjectDemo.Console.ClassicNet.exe" --className InjectDemo.Console.ClassicNet.Program --methodName EntryPoint --targetMode=BruteForce --argument "OK BOY"



## Injection Library Documentation
