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
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class PartsViewer : Window
    {
        public PartsViewer(List<PauchiePart> parts)
        {
            InitializeComponent();

            this.DataContext = new PartsViewerViewModel(parts);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
