using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Prompts
{
    public class PromptValueToIntConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string v = (string)value;
            double r;
            if (double.TryParse(v, out r))
            {
                try
                {
                    return System.Convert.ToInt32(r).ToString();
                }
                catch
                {
                    return v;
                }
            }
            else return v;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
