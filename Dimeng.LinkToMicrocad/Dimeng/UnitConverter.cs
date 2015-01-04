using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.LinkToMicrocad.Dimeng
{
    internal class UnitConverter
    {
        public static double GetValueFromString(string str)
        {
            try
            {
                str = str.ToUpper();

                if (str.EndsWith("MM"))
                {
                    return double.Parse(str.Substring(0, str.Length - 2));
                }
                else if (str.EndsWith("INCH"))
                {
                    return double.Parse(str.Substring(0, str.Length - 4)) * 25.4;
                }
                else if (str.EndsWith("CM"))
                {
                    return double.Parse(str.Substring(0, str.Length - 2)) * 10;
                }
                else if (str.EndsWith("M"))
                {
                    return double.Parse(str.Substring(0, str.Length - 1)) * 100;
                }

                throw new Exception("未知的单位:" + str);
            }
            catch (Exception error)
            {
                throw new Exception("转换数据时发生了错误:" + str, error);
            }
        }
    }
}
