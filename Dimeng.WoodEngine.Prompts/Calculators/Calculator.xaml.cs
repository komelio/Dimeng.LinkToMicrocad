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

namespace Dimeng.WoodEngine.Prompts
{
    /// <summary>
    /// Calculator.xaml 的交互逻辑
    /// </summary>
    public partial class Calculator : Window
    {
        private Calculator()
        {
            InitializeComponent();
        }

        public Calculator(CalculatorItem calItem, IEnumerable<PromptItem> prompts)
            :this()
        {
            this.DataContext = new CalculatorViewModel(calItem,prompts);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
