﻿using System;
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
            if (e.Args.Length >= 1)
            {
                MainWindow window = new MainWindow(ParamHelper.GetOrderNumber(e.Args[0]));
                window.ShowDialog();
            }
            else
            {
                MainWindow window = new MainWindow();
                window.ShowDialog();
            }
        }
    }
}
