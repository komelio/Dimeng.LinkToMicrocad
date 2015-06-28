using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Xml.Linq;
using QuoteExport.Entities;
using Dapper;
using System.Data.OleDb;
using System.IO;
using System.Windows;
using Dimeng.FTP;
using QuoteExport.Properties;
using System.ComponentModel;

namespace QuoteExport
{
    public class MainViewModel : ViewModelBase
    {
        private readonly BackgroundWorker worker;
        public MainViewModel(string xmlFileName)
        {
            StartCommand = new RelayCommand(this.startWork);
            ShowConfiguration = new RelayCommand(this.showConfiguration);
            BrowserCommand = new RelayCommand(this.browser);
            init(xmlFileName);

            this.worker = new BackgroundWorker();
            this.worker.DoWork += this.DoWork;
            this.worker.ProgressChanged += this.ProgressChanged;
        }

        private void browser()
        {
            System.Diagnostics.Process.Start("Explorer.exe", "/select,\"" + Path.Combine(currentProjectPath, "DMS") + "\"");
        }

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.ProgressValue = e.ProgressPercentage;
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            this.ProgressValue = 0;

            try
            {
                FTPclient client = new FTPclient(
                    Properties.Settings.Default.FTPServer,
                    Settings.Default.FTPUser,
                    Settings.Default.FTPPassword);

                string folderName = (new DirectoryInfo(currentProjectPath)).Name;
                client.FtpCreateDirectory(@"/DASSMDATA/" + folderName);
                client.FtpCreateDirectory(@"/DASSMDATA/" + folderName + "/BOM Output");
                client.FtpCreateDirectory(@"/DASSMDATA/" + folderName + "/Machinings");

                this.ProgressMax = Directory.GetFiles(Path.Combine(currentProjectPath, "DMS", "Output"), "*", SearchOption.AllDirectories).Length;
                //MessageBox.Show(progressMax.ToString());

                foreach (var product in Products)
                {
                    string pathToXLS = Path.Combine(currentProjectPath, "DMS", "Output", product.Handle, product.Handle + ".xlsx");
                    string pathToUploadXLS = string.Format(@"/DASSMDATA/{0}/BOM Output/{1}.xlsx", folderName, product.Handle);

                    client.Upload(pathToXLS, pathToUploadXLS);
                    this.ProgressValue++;

                    string pathToCSV = Path.Combine(currentProjectPath, "DMS", "Output", product.Handle, "Machinings");
                    foreach (var f in Directory.GetFiles(pathToCSV, "*.csv"))
                    {
                        string filename = (new FileInfo(f)).Name;
                        client.Upload(f, string.Format(@"/DASSMDATA/{0}/Machinings/{1}", folderName, filename));
                        this.ProgressValue++;
                    }
                }


                var service = new Pauchie.PauchieWebServiceService();
                service.doDASSM("hello", Products.Count, @"/DASSMData/" + folderName);
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
            MessageBox.Show("上传完毕");
        }

        private void showConfiguration()
        {
            ConfigurationForm form = new ConfigurationForm();
            form.ShowDialog();
        }

        private void init(string xmlFileName)
        {
            XElement xml = XElement.Load(xmlFileName);

            var global = from e in xml.Elements("Global")
                         select e;

            CurrentProjectPath = global.SingleOrDefault().Attribute("Path").Value;

            this.Products = new List<Product>();
            var units = from e in xml.Elements("Inter")
                        where e.Attribute("Manufacturer").Value == "dms"
                        select e;

            string connectStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Path.Combine(this.currentProjectPath, "DMS", "ProductList.mdb");
            foreach (var x in units)
            {
                using (OleDbConnection conn = new OleDbConnection(connectStr))
                {
                    conn.Open();

                    string selectStr = string.Format("Select * from ProductList Where Handle='{0}'", x.Attribute("DMID").Value.Replace("_", ""));
                    var product = conn.Query<Product>(selectStr, null).SingleOrDefault<Product>();
                    if (product == null)
                    {
                        MessageBox.Show(selectStr);
                        continue;
                    }

                    Products.Add(product);
                }
            }

            using (OleDbConnection conn = new OleDbConnection(connectStr))
            {
                conn.Open();
                StringBuilder sb = new StringBuilder();
                foreach (var p in Products)
                {
                    sb.Append(string.Format("'{0}'", p.Handle));
                    if (Products.IndexOf(p) != Products.Count - 1)
                    { sb.Append(","); }
                }
                string delStr = string.Format("Delete from ProductList Where Handle NOT IN ({0})", sb.ToString());
                OleDbCommand cmd = new OleDbCommand(delStr, conn);
                cmd.ExecuteNonQuery();
            }

        }

        private void startWork()
        {
            try
            {
                worker.RunWorkerAsync();
            }
            catch (Exception error)
            {
                MessageBox.Show("上传过程中发生了错误!" + error.Message + error.StackTrace);
            }
        }

        private int progressValue;
        public int ProgressValue
        {
            get { return progressValue; }
            set
            {
                progressValue = value;
                base.RaisePropertyChanged("ProgressValue");
            }
        }

        private double progressMax = 100;
        public double ProgressMax
        {
            get { return progressMax; }
            set
            {
                progressMax = value;
                base.RaisePropertyChanged("ProgressMax");
            }
        }

        private string currentProjectPath;

        public string CurrentProjectPath
        {
            get { return currentProjectPath; }
            set
            {
                currentProjectPath = value;
                base.RaisePropertyChanged("CurrentProjectPath");
            }
        }

        public List<Product> Products { get; private set; }

        public RelayCommand StartCommand { get; set; }
        public RelayCommand ShowConfiguration { get; set; }
        public RelayCommand BrowserCommand { get; set; }
    }
}
