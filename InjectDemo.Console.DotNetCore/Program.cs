using System;
using System.Runtime.InteropServices;

namespace InjectDemo.Console.DotNetCore
{
    /// <summary>
    /// .NET 4.0 Injection Sample App
    /// </summary>
    public class Program
    {
        [DllImport("User32.dll", CharSet = CharSet.Unicode)]
        public static extern int MessageBox(IntPtr h, string m, string c, int type);

        /// <summary>
        /// Entrypoint Method.
        /// This Method MUST be declared as "static int" and have "pwzArgmument" Parameter. Ref: https://docs.microsoft.com/en-us/dotnet/framework/unmanaged-api/hosting/iclrruntimehost-executeindefaultappdomain-method
        /// </summary>
        /// <param name="pwzArgument">Optional argument to pass in.</param>
        /// <returns>Integer Exit Code</returns>
        static int EntryPoint(String pwzArgument)
        {
            // show modal message box
            MessageBox(IntPtr.Zero,
                ".DotNetCore Managed App Injected Successfully.\n\n" +
                "Running Inside: [" + System.Diagnostics.Process.GetCurrentProcess().ProcessName + "] Process\n\n" +
                (String.IsNullOrEmpty(pwzArgument) ? "No Argument Received" : "Received Argument: [" + pwzArgument + "]")
                , "", 0
            );

            return 0;
        }

        /// <summary>
        /// Main method. Invoked when app is run standalone.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            EntryPoint("Called by Main");
        }
    }
}
