using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Dimeng.WoodEngine.Prompts
{
    public class SubQtyToColorConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string v = value.ToString();
            double value1;
            if (double.TryParse(v, out value1))
            {
                if (value1 == 1)
                {
                    return "White";
                }
                else
                {
                    return "Red";
                }
            }

            return "White";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
