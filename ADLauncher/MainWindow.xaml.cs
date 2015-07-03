﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ADLauncher
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        string orderNumber;
        string adPath;
        string adTempPath;
        string adExePath;

        public MainWindow()
        {
            InitializeComponent();
            initADInfo();

            var viewmodel = new ViewModel();
            viewmodel.ADTempPath = this.adTempPath;
            viewmodel.OnCreate = this.close;
            viewmodel.ADProjectsPath = System.IO.Path.Combine(adPath, "Projects");
            this.DataContext = viewmodel;
        }

        public MainWindow(string od)
            : this()
        {
            this.orderNumber = od;
        }

        private void close()
        {
            this.Close();
            Process.Start(adExePath, @"/u");
        }

        private void initADInfo()
        {
            RegistryKey hkml = Registry.LocalMachine;
            RegistryKey akr14 = hkml.OpenSubKey("SOFTWARE\\Microcad\\autodecco_studio\\R11", true);

            string path = akr14.GetValue("").ToString();
            adPath = path;
            adTempPath = System.IO.Path.Combine(path, "Temp");
            adExePath = System.IO.Path.Combine(path, "ad11s.exe");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
