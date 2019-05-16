using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DNCI.Injector.Runner
{

    class Program
    {
        static void Main(string[] args)
        {
            Injector.Library.InjectorConfiguration config = Injector.Library.InjectorConfigurationBuilder
                .Instance()
                .InjectThisClrBinary(@"C:\Users\guiba\Downloads\FrameworkInjection\InjectExample\bin\Debug\InjectExample.exe")
                .ClrClassName("InjectExample.Program")
                .ClrMethodName("EntryPoint")
                .WithArguments("OK - It Works Baby")
                .InjectOnProcess("cmd")
                .InjectOnProcess("cmd.exe")
                .InjectOnProcess("calc")
                .InjectOnProcess("notepad++")
                .build();

            DNCI.Injector.Library.Injector injector = new Library.Injector(config);

            Console.WriteLine(
                injector.Run()
            );
        }
    }
}
