using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ADLauncher
{
    public class ParamHelper
    {
        public static string GetOrderNumber(string par)
        {
            string value = par.Substring(12);
            string[] values = value.Split('&');

            return values[0];
        }
    }
}
