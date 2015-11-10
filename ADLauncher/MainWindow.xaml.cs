using ADLauncher.Properties;
using Ionic.Zip;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
        string od;
        string line;

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

        public ViewModel Viewmodel
        {
            get { return this.DataContext as ViewModel; }
            set
            {
                this.DataContext = value;
            }
        }

        public MainWindow(string od, string lineNumber)
        {
            InitializeComponent();

            this.od = od;
            this.line = lineNumber;

            initADInfo();

            var viewmodel = new ViewModel(od);
            //viewmodel.IsConnected = false;
            viewmodel.ADTempPath = this.adTempPath;
            viewmodel.OnCreate = this.close;
            viewmodel.ADProjectsPath = System.IO.Path.Combine(adPath, "Projects");
            this.DataContext = viewmodel;

            this.ContentRendered += MainWindow_ContentRendered;
            this.orderNumber = string.Format("{0}-{1}", od, lineNumber);
        }

        private void MainWindow_ContentRendered(object sender, EventArgs e)
        {
            string path = System.IO.Path.Combine(adPath, "Projects", orderNumber);

            //step1：查找订单数据
            if (!PushHelper.OrderCanEdit(Settings.Default.ClientToken, od, line))
            {
                if (Directory.Exists(path))
                {
                    if (MessageBox.Show("本地已存在任务数据" + orderNumber + ",是否进行覆盖?", "班尔奇", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        downloadProjectData(path);
                    }
                }
                else
                {
                    downloadProjectData(path);
                }

                this.Viewmodel.OpenProject(this.orderNumber);
            }
            else
            {
                if (!System.IO.Directory.Exists(path))
                {
                    //todo：下载订单数据

                    //创建订单数据
                    this.Viewmodel.CreateNewProject(this.orderNumber);
                }
                else
                {
                    //MessageBox.Show("OpenProject!");
                    //打开订单数据
                    this.Viewmodel.OpenProject(this.orderNumber);
                }
            }
        }

        private void downloadProjectData(string path)
        {
            FTPclient client = new FTPclient(Settings.Default.FTPServer, Settings.Default.FTPClient, Settings.Default.FTPPassword);
            client.Download("/" + od + "/" + line + "/Project/project_temp.dwg", System.IO.Path.Combine(path, "project.dwg"), true);

            string tempDMS = System.IO.Path.GetTempFileName();
            client.Download("/" + od + "/" + line + "/Project/DMS.zip", tempDMS, true);

            using (ZipFile zipfile = new ZipFile(tempDMS, Encoding.Default))
            {
                zipfile.ExtractAll(System.IO.Path.Combine(path, "DMS"));
            }
        }

        private void close()
        {
            this.Close();
            Process pro = Process.Start(adExePath, @"/u");
        }

        private void initADInfo()
        {
            try
            {
                RegistryKey hkml = Registry.LocalMachine;
                RegistryKey akr14 = hkml.OpenSubKey("SOFTWARE\\Microcad\\autodecco_studio\\R11", true);

                string path = akr14.GetValue("").ToString();
                adPath = path;
                adTempPath = System.IO.Path.Combine(path, "Temp");
                adExePath = System.IO.Path.Combine(path, "ad11s.exe");
            }
            catch
            {
                MessageBox.Show("获取Autodecco信息时发生错误，请以系统管理员权限运行程序");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
