using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Prompts
{
    public class PromptValueToCheckedConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string v = (string)value;
            double r;
            if (double.TryParse(v, out r))
            {
                if (r == 1)
                    return true;
                else return false;
            }
            else return false;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool v = (bool)value;
            if (v)
                return "1";
            else return "0";
        }
    }
}
