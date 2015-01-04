using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Dimeng.WoodEngine.Prompts
{
    public class SubImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is SubassemblyViewModel)
            {
                var vm = value as SubassemblyViewModel;
                
                if(File.Exists(vm.JPGPath))
                {
                    return new BitmapImage(new Uri(vm.JPGPath));
                }
                else if(File.Exists(vm.WMFPath))
                {
                    return new BitmapImage(new Uri(vm.WMFPath));
                }
            }
           
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
