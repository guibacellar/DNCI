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
            DNCI.Injector.Library.Injector.InjectWithProcessName("EntryPoint", @"C:\Users\guiba\Downloads\FrameworkInjection\InjectExample\bin\Debug\InjectExample.exe", "InjectExample.Program", "OK", "notepad++");
        }
    }
}
