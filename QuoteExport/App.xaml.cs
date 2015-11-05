using QuoteExport.ERP;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace QuoteExport
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (e.Args.Length == 0)
            {
                MessageBox.Show("Parameter should not be blank!");
                this.Shutdown();
            }
            else
            {
                MainWindow2 mainWindow = new MainWindow2(e.Args[0]);
                mainWindow.ShowDialog();
            }
        }
    }
}
