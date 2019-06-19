# DNCI - Dot Net Code Injector
DNCI allows the injection of .Net code (.exe or .dll) remotely in unmanaged processes in windows.

# 1. Project Structure
The project is structured in:

  * **DNCI.Injector.Library** - Injection library. Contains all injection components and logic;
  * **DNCI.Injector.Runner**  - Command line utility for injection;
  * **DNCIClrLoader**         - C++ MicroCode to Load the .NET assembly into memory;
  * **InjectDemo.Console.ClassicNet** - Demo Classic .Net Console Application to be injected;
  * **InjectDemo.Console.DotNetCore** - Demo .Net Core Console Application to be injected;
  * **InjectDemo.Dll.ClassicNet** - Demo Classic .Net DLL to be injected;

# 2. Documentation and Usage:

## 2.1. Command Line Utility Documentation
 * Parameters:
   *   --help                                         Show help information
   * --assemblyFile <SOURCE_FILE_PATH>              Target .NET Classic DLL File
   * --className <TARGET_CLASS_NAME>                The fully qualified type name of the managed assembly
   * --methodName <TARGET_CLASS_ENTRYPOINT_METHOD>  The name of the managed method to execute. EX: EntryPoint (This method should be 'public static int')
   * --argument <ENTRYPOINT_METHOD_ARGUMENT>        An optional argument to pass in to the managed function
   * --targetMode <TARGET_MODE>                     Injection Target Mode (BruteForce, PID, ProcessName)
   * --pid <TARGET_PROCESS_ID>                      Target Process ID
   * --processName <TARGET_PROCESS_Name>            Target Process Name

### Examples
**Inject Classic .Net Console Application into Notepad++** 

This example used the *InjectDemo.Console.ClassicNet* .exe file.
DNCI.Injector.Runner.exe --assemblyFile "<PATH_TO_FILE>\InjectDemo.Console.ClassicNet.exe" --className InjectDemo.Console.ClassicNet.Program --methodName EntryPoint --targetMode=processName --processName notepad++ --argument "OK BOY"

**Inject Classic .Net Console Application into Process with ID 66** 

This example used the *InjectDemo.Console.ClassicNet* .exe file.
DNCI.Injector.Runner.exe --assemblyFile "<PATH_TO_FILE>\InjectDemo.Console.ClassicNet.exe" --className InjectDemo.Console.ClassicNet.Program --methodName EntryPoint --targetMode=PID --pid 66 --argument "OK BOY"

**Try to Inject Classic .Net Console Application into any Running Process** 

This example used the *InjectDemo.Console.ClassicNet* .exe file.
DNCI.Injector.Runner.exe --assemblyFile "<PATH_TO_FILE>\InjectDemo.Console.ClassicNet.exe" --className InjectDemo.Console.ClassicNet.Program --methodName EntryPoint --targetMode=BruteForce --argument "OK BOY"



## 2.2. Injection Library Documentation
The injection library was designed to be used by any .Net program. In fact, the DNCI Command Line Utility do uses the DNCI Library it self.

  * Classes
    * **Injector** - Main injector componente;
    * **InjectorConfiguration** - Configuration model. Created to be an abstract model between the *Injector* and they consumers;
    * **InjectorConfigurationBuilder** - Fluent builder for the *InjectionConfiguration* model;
    * **InjectorResult** - Result model. Created to be an abstract model between the *Injector* and they consumers;
    * **InjectorResultStatus** - Enum with all possible injection status;
    
## Building Parameters ##
**Inject Classic .Net Console Application into Remote Process** 

```csharp
Injector.Library.InjectorConfiguration config = Injector.Library.InjectorConfigurationBuilder
    .Instance()
    .InjectThisClrBinary(@"<PATH_TO_BINARY>\InjectDemo.Console.ClassicNet.exe")
    .ClrClassName("InjectDemo.Console.ClassicNet.Program")
    .ClrMethodName("EntryPoint")
    .WithArguments("OK - It Works Baby")
    .InjectOnProcess("cmd") // Try to Inject on cmd process
    .InjectOnProcess("chrome") // Try to Inject on chrome process
    .InjectOnProcess("cmd.exe") // Try to Inject on cmd.exe process
    .InjectOnProcess("calc") // Try to Inject on calc process
    .InjectOnProcess("notepad++")  // Try to Inject on notepad++ process
    .Build();
```

**Brute Force to Inject Classic .Net DLL Application into Any Available Process** 

```csharp
Injector.Library.InjectorConfiguration config = Injector.Library.InjectorConfigurationBuilder
   .Instance()
   .InjectThisClrBinary(@"<PATH_TO_BINARY>\InjectDemo.Dll.ClassicNet.dll")
   .ClrClassName("InjectDemo.Dll.ClassicNet.Class1")
   .ClrMethodName("EntryPoint")
   .WithArguments("OK - It Works Baby")
   .InjectWithBruteForce()
   .Build();
```

## Running the Injector

```csharp
// Create Injector Instance
DNCI.Injector.Library.Injector injector = new Library.Injector(configBuilderconfig);

// Execute the Injection
List<InjectorResult> result = injector.Run();

// Print Injection Result on Console
foreach (InjectorResult res in result)
{
    Console.WriteLine(res);
}
```
