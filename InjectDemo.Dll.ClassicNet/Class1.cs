using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace InjectDemo.Dll.ClassicNet
{
    /// <summary>
    /// .NET 4.0 Injection Sample App
    /// </summary>
    public class Class1
    {
        /// <summary>
        /// Entrypoint Method.
        /// This Method MUST be declared as "static int" and have "pwzArgmument" Parameter. Ref: https://docs.microsoft.com/en-us/dotnet/framework/unmanaged-api/hosting/iclrruntimehost-executeindefaultappdomain-method
        /// </summary>
        /// <param name="pwzArgument">Optional argument to pass in.</param>
        /// <returns>Integer Exit Code</returns>
        static int EntryPoint(String pwzArgument)
        {
            // show modal message box
            MessageBox.Show(
                ".NET 4 Managed DLL Injected Successfully.\n\n" +
                "Running Inside: [" + System.Diagnostics.Process.GetCurrentProcess().ProcessName + "] Process\n\n" +
                (String.IsNullOrEmpty(pwzArgument) ? "No Argument Received" : "Received Argument: [" + pwzArgument + "]"));

            return 0;
        }
    }
}
