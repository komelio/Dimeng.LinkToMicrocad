using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace QuoteExport
{
    public class ProductImageConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string reference = (string)value;
            if (string.IsNullOrEmpty(reference))
            {
                return "images/notFound.png";
            }

            string path = Path.Combine(@"c:\\Microcad Software\\autodecco_studio 11\Catalog\Dms\Photos\dmsobj", reference + ".jpg");
            if (!File.Exists(path))
            {
                return "images/notFound.png";
            }

            return path;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
