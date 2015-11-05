using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ADLauncher
{
    public class ParamHelper
    {
        public static string GetOrderNumber(string par, out string lineNumber)
        {
            string value = par.Substring(3);
            value = value.TrimEnd('/');
            string[] values = value.Split(' ');

            string w = values[0];
            lineNumber = values[1];
            return w;
        }
    }
}
