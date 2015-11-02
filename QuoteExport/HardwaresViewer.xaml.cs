using QuoteExport.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace QuoteExport
{
    /// <summary>
    /// HardwaresViewer.xaml 的交互逻辑
    /// </summary>
    public partial class HardwaresViewer : Window
    {
        public HardwaresViewer(List<PauchieHardware> hwrs)
        {
            InitializeComponent();

            this.DataContext = new HardwaresViewerViewModel(hwrs);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
