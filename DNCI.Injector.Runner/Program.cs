using DNCI.Injector.Library;
using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;

namespace DNCI.Injector.Runner
{

    class Program
    {
        static int Main(string[] args)
        {
            var app = new CommandLineApplication();

            app.HelpOption("--help");

            // Set Command Line Options
            CommandOption<String> assemblyFile = app.Option<String>("--assemblyFile <SOURCE_FILE_PATH>", "Target .NET Classic DLL File", CommandOptionType.SingleValue).IsRequired();
            CommandOption<String> className = app.Option<String>("--className <TARGET_CLASS_NAME>", "The fully qualified type name of the managed assembly", CommandOptionType.SingleValue).IsRequired();
            CommandOption<String> methodName = app.Option<String>("--methodName <TARGET_CLASS_ENTRYPOINT_METHOD>", "The name of the managed method to execute. EX: EntryPoint (This method should be 'public static int')", CommandOptionType.SingleValue).IsRequired();
            CommandOption<String> argument = app.Option<String>("--argument <ENTRYPOINT_METHOD_ARGUMENT>", "An optional argument to pass in to the managed function", CommandOptionType.SingleValue);
            CommandOption<String> targetMode = app.Option<String>("--targetMode <TARGET_MODE>", "Injection Target Mode (BruteForce, PID, ProcessName)", CommandOptionType.SingleValue).IsRequired();
            CommandOption<Int32> targetPid = app.Option<Int32>("--pid <TARGET_PROCESS_ID>", "Target Process ID", CommandOptionType.SingleValue);
            CommandOption<String> targetProcessName = app.Option<String>("--processName <TARGET_PROCESS_Name>", "Target Process Name", CommandOptionType.SingleValue);

            // Parse Options
            app.OnExecute(() =>
            {
            });

            int hResult = app.Execute(args);

            if (hResult != 0)
            {
                Console.WriteLine("");
                Console.WriteLine(app.GetHelpText());

                return hResult;
            }

            // Create Builder
            Injector.Library.InjectorConfigurationBuilder configBuilder = Injector.Library.InjectorConfigurationBuilder
               .Instance()
               .InjectThisClrBinary(assemblyFile.Value())
               .ClrClassName(className.Value())
               .ClrMethodName(methodName.Value());

            // Optional Args
            if (!String.IsNullOrEmpty(argument.Value()))
            {
                configBuilder.WithArguments(argument.Value().Trim());
            }

            // Target Mode
            switch(targetMode.Value().ToUpper())
            {
                case "PROCESSNAME": configBuilder.InjectOnProcess(targetProcessName.Value()); break;
                case "PID": configBuilder.InjectOnProcess(Int32.Parse(targetPid.Value())); break;
                default: configBuilder.InjectWithBruteForce(); break;
            }

            // Execute the Injector
            DNCI.Injector.Library.Injector injector = new Library.Injector(configBuilder.Build());

            List<InjectorResult> result = injector.Run();

            foreach (InjectorResult res in result)
            {
                Console.WriteLine(res);
            }

            return 0;
        }
    }
}



// Classic .NET Console Application
//Injector.Library.InjectorConfiguration config = Injector.Library.InjectorConfigurationBuilder
//    .Instance()
//    .InjectThisClrBinary(@"C:\projetos\DNCI\InjectDemo.Console.ClassicNet\bin\Debug\InjectDemo.Console.ClassicNet.exe")
//    .ClrClassName("InjectDemo.Console.ClassicNet.Program")
//    .ClrMethodName("EntryPoint")
//    .WithArguments("OK - It Works Baby")
//    .InjectOnProcess("cmd")
//    .InjectOnProcess("chrome")
//    .InjectOnProcess("cmd.exe")
//    .InjectOnProcess("calc")
//    .InjectOnProcess("notepad++")
//    .Build();

//// Classic .NET DLL
//Injector.Library.InjectorConfiguration config = Injector.Library.InjectorConfigurationBuilder
//   .Instance()
//   .InjectThisClrBinary(@"C:\projetos\DNCI\InjectDemo.Dll.ClassicNet\bin\Debug\InjectDemo.Dll.ClassicNet.dll")
//   .ClrClassName("InjectDemo.Dll.ClassicNet.Class1")
//   .ClrMethodName("EntryPoint")
//   .WithArguments("OK - It Works Baby")
//   .InjectWithBruteForce()
//   .Build();
