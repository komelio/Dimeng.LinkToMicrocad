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
            //UpdateWindow uw = new UpdateWindow();
            //if (uw.ShowDialog() == true)
            //{
            if (e.Args.Length >= 1)
            {
                try
                {
                    MainWindow window = new MainWindow(
                        ParamHelper.GetOrderNumber(e.Args[0])
                        );
                    window.ShowDialog();
                    this.Shutdown();
                }
                catch (Exception error)
                {
                    MessageBox.Show(error.Message + error.StackTrace);
                }
            }
            else
            {
                MainWindow window = new ADLauncher.MainWindow();
                window.ShowDialog();
                this.Shutdown();
            }
            //}

        }
    }
}
