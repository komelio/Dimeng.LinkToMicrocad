using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ADLauncher
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainWindow window = new MainWindow();
            UpdateWindow uw = new UpdateWindow();
            if (uw.ShowDialog() == true)
            {
                if (e.Args.Length >= 1)
                {
                    window = new MainWindow(ParamHelper.GetOrderNumber(e.Args[0]));
                    window.ShowDialog();
                }
                else
                {
                    window.ShowDialog();
                }
            }
            this.Shutdown();
        }
    }
}
