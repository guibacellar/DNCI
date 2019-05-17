using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DNCI.Injector.Library.Exception
{
    /// <summary>
    /// Exception for Invalid Configuration
    /// </summary>
    public sealed class InvalidConfigurationException : System.Exception
    {
        public InvalidConfigurationException(string message) : base(message)
        {
        }
    }
}
