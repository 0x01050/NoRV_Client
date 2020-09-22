using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoRV
{
    class Logger
    {
        private static string prefix = "NoRV";
        public static void info(string message, string reason = "")
        {
            if(String.IsNullOrEmpty(reason))
                Console.WriteLine(prefix + ": " + message);
            else
                Console.WriteLine(prefix + ": " + message + " -> " + reason);
        }
    }
}
