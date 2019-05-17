using DNCI.Injector.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DNCI.Injector.Runner
{

    class Program
    {
        // TODO:  Write Host for .NET CORE. REF: https://docs.microsoft.com/pt-br/dotnet/core/tutorials/netcore-hosting
        static void Main(string[] args)
        {
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

            // Classic .NET DLL
            Injector.Library.InjectorConfiguration config = Injector.Library.InjectorConfigurationBuilder
               .Instance()
               .InjectThisClrBinary(@"C:\projetos\DNCI\InjectDemo.Dll.ClassicNet\bin\Debug\InjectDemo.Dll.ClassicNet.dll")
               .ClrClassName("InjectDemo.Dll.ClassicNet.Class1")
               .ClrMethodName("EntryPoint")
               .WithArguments("OK - It Works Baby")
               .InjectWithBruteForce()
               .Build();

            DNCI.Injector.Library.Injector injector = new Library.Injector(config);

            List<InjectorResult> result = injector.Run();

            foreach (InjectorResult res in result)
            {
                Console.WriteLine(res);
            }
        }
    }
}
