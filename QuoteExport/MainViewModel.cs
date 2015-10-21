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
using SpreadsheetGear;
using Microsoft.Win32;
using System.Diagnostics;

namespace QuoteExport
{
    public class MainViewModel : ViewModelBase
    {
        private readonly BackgroundWorker worker;
        public MainViewModel(string xmlFileName)
        {
            PauchieProducts = new List<PauchieProduct>();
            PauchieHardwares = new List<PauchieHardware>();

            StartCommand = new RelayCommand(this.startWork);
            ShowConfiguration = new RelayCommand(this.showConfiguration);
            BrowserCommand = new RelayCommand(this.browser);
            StartCADCommand = new RelayCommand(this.startCAD, this.canStartCAD);
            init(xmlFileName);

            this.worker = new BackgroundWorker();
            this.worker.DoWork += this.DoWork;
            this.worker.ProgressChanged += this.ProgressChanged;
        }
        private string cadDirPath;
        private string cadEXEPath;
        private void startCAD()
        {
            RegistryKey key = Registry.CurrentUser
                .OpenSubKey(@"Software\Microvellum\Microvellum Toolbox\R67\Toolbox-Standard\ApplicationSettings");
            string jobpath = key.GetValue("Jobs").ToString();
            jobpath = Path.Combine(jobpath, (new DirectoryInfo(this.currentProjectPath)).Name);

            if (Directory.Exists(jobpath))
            {
                Directory.Delete(jobpath, true);
            }

            IOHelper.CopyDirectory(Path.Combine(this.currentProjectPath, "DMS"), jobpath);

            Process myProcess = new Process();

            myProcess.StartInfo.UseShellExecute = true;
            // You can start any process, HelloWorld is a do-nothing example.
            myProcess.StartInfo.FileName = this.cadEXEPath;
            myProcess.StartInfo.CreateNoWindow = true;
            myProcess.StartInfo.WorkingDirectory = this.cadDirPath;
            myProcess.Start();
            //System.Diagnostics.Process.Start(this.cadEXEPath);
        }

        private bool canStartCAD()
        {
            try
            {
                RegistryKey key = Registry.CurrentUser
                    .OpenSubKey(@"Software\Microvellum\Microvellum ToolBox\R67\Toolbox-Standard");
                string path = key.GetValue("Last AutoCAD Path").ToString();
                if (!Directory.Exists(path))
                {
                    return false;
                }
                this.cadDirPath = path;
                this.cadEXEPath = Path.Combine(path, "ACAD.exe");
                return true;
            }
            catch
            {
                return false;
            }
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
                //和普世合作的部分
                string erpFolder = Path.Combine(currentProjectPath, "DMS", "ERP");
                if (Directory.Exists(erpFolder))
                {
                    //清空已有文件
                    foreach (var fi in Directory.GetFiles(erpFolder))
                    {
                        File.Delete(fi);
                    }
                }
                else
                {
                    Directory.CreateDirectory(erpFolder);
                }

                for (int i = 0; i < this.Products.Count; i++)
                {
                    Product product = this.Products[i];
                    PauchieConverter converter = new PauchieConverter(product);
                    PauchieProduct pProduct = converter.GetPauchieProduct();
                    pProduct.LineNumber = i + 1;//号码
                    PauchieProducts.Add(pProduct);
                }

                PauchieExporter exporter = new PauchieExporter(this.PauchieProducts, erpFolder);
                exporter.Export();

                //之前和启程合作的部分

                //FTPclient client = new FTPclient(
                //    Properties.Settings.Default.FTPServer,
                //    Settings.Default.FTPUser,
                //    Settings.Default.FTPPassword);

                //string folderName = (new DirectoryInfo(currentProjectPath)).Name;
                //client.FtpCreateDirectory(@"/DASSMDATA/" + folderName);
                //client.FtpCreateDirectory(@"/DASSMDATA/" + folderName + "/BOM Output");
                //client.FtpCreateDirectory(@"/DASSMDATA/" + folderName + "/Machinings");

                //this.ProgressMax = Directory.GetFiles(Path.Combine(currentProjectPath, "DMS", "Output"), "*", SearchOption.AllDirectories).Length;
                ////MessageBox.Show(progressMax.ToString());

                //foreach (var product in Products)
                //{
                //    string pathToXLS = Path.Combine(currentProjectPath, "DMS", "Output", product.Handle, product.Handle + ".xlsx");
                //    string pathToUploadXLS = string.Format(@"/DASSMDATA/{0}/BOM Output/{1}.xlsx", folderName, product.Handle);

                //    client.Upload(pathToXLS, pathToUploadXLS);
                //    this.ProgressValue++;

                //    string pathToCSV = Path.Combine(currentProjectPath, "DMS", "Output", product.Handle, "Machinings");
                //    foreach (var f in Directory.GetFiles(pathToCSV, "*.csv"))
                //    {
                //        string filename = (new FileInfo(f)).Name;
                //        client.Upload(f, string.Format(@"/DASSMDATA/{0}/Machinings/{1}", folderName, filename));
                //        this.ProgressValue++;
                //    }
                //}


                //var service = new Pauchie.PauchieWebServiceService();
                //service.doDASSM("hello", Products.Count, @"/DASSMData/" + folderName);
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message + error.StackTrace);
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
            try
            {
                XElement xml = XElement.Load(xmlFileName);

                var global = from e in xml.Elements("Global")
                             select e;

                CurrentProjectPath = global.SingleOrDefault().Attribute("Path").Value;

                loadProducts(xml);

                loadMouldings(xml);

                loadDeccos(xml);

                //把moulding的数据以五金的形式插入到mv的工作任务中
                //todo：只为了演示使用，将来要剔除
                InsertMouldingProduct();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message + error.StackTrace);
            }

        }

        private void loadDeccos(XElement xml)
        {
            this.Deccos = new List<Decco>();
            foreach (var xd in xml.Elements("Decco"))
            {
                Decco decco = new Decco();
                decco.Name = xd.Attribute("Name").Value;
                decco.Width = double.Parse(xd.Attribute("X").Value);
                decco.Height = double.Parse(xd.Attribute("Z").Value);
                decco.Depth = double.Parse(xd.Attribute("Y").Value);
                decco.Reference = xd.Attribute("Reference").Value;

                this.Deccos.Add(decco);
            }
        }

        private void InsertMouldingProduct()
        {
            string connectStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source="
                + Path.Combine(this.currentProjectPath, "DMS", "ProductList.mdb");

            string binFileName = getBinFilename();

            using (OleDbConnection conn = new OleDbConnection(connectStr))
            {
                conn.Open();

                string selectStr = string.Format("Select * from ProductList Where Description='{0}'", "装饰线条");
                var product = conn.Query<Product>(selectStr, null).SingleOrDefault<Product>();


                if (product == null)//只能是null，因为quoteExport执行的第一步就是把图形里没有的产品都干掉了
                {
                    //MessageBox.Show("HHH");
                    int count = 9999;//特定的号码
                    string id = Guid.NewGuid().ToString();
                    string filename = string.Format("{0}.cutx", count);
                    //插入这个产品数据条
                    string insertStr = string.Format("Insert Into ProductList (ItemNumber,Description,Qty,Width,Height,Depth,MatFile,FileName,Handle,ReleaseNumber,Parent1) Values('{0}','{1}',{2},{3},{4},{5},'{6}','{7}','{8}','{9}','{10}')",
                        string.Format("{0}.00", count),
                        "装饰线条",
                        1,
                        100,
                        100,
                        100,
                        binFileName,
                        filename,
                        id,
                        "UnNamed",
                        "Phase 1");

                    var cmd = new OleDbCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = insertStr;
                    cmd.ExecuteNonQuery();

                    product = new Product();
                    product.ReleaseNumber = "UnNamed";
                    product.Qty = 1;
                    product.MatFile = binFileName;
                    product.FileName = filename;
                }

                string fullPath = Path.Combine(this.currentProjectPath, "DMS", product.FileName);
                IWorkbook book;
                if (File.Exists(fullPath))
                {
                    book = Factory.GetWorkbook(fullPath);
                }
                else
                {
                    book = Factory.GetWorkbook();
                    book.Worksheets[0].Name = "CutParts";
                    ISheet s1 = book.Worksheets.Add();
                    s1.Name = "HardwareParts";
                    ISheet s2 = book.Worksheets.Add();
                    s2.Name = "Subassemblies";
                    ISheet s3 = book.Worksheets.Add();
                    s3.Name = "Prompts";
                    ISheet s4 = book.Worksheets.Add();
                    s4.Name = "Machining";
                    IRange promptCells = book.Worksheets["Prompts"].Cells;
                    promptCells[0, 0].Value = "Width";
                    promptCells[0, 1].Value = 100;
                    promptCells[1, 0].Value = "Height";
                    promptCells[1, 1].Value = 100;
                    promptCells[2, 0].Value = "Depth";
                    promptCells[2, 1].Value = 100;
                }

                //更新这个产品数据条
                IRange range = book.Worksheets["HardwareParts"].Cells;
                for (int i = 0; i < this.Mouldings.Count; i++)
                {
                    range[i, 16].Value = this.Mouldings[i].Description;
                    range[i, 17].Value = 1;
                    range[i, 18].Value = this.Mouldings[i].Length;
                    range[i, 26].Value = this.Mouldings[i].Kind;
                }
                range[this.Mouldings.Count, 26].Value = string.Empty;//断空行

                book.SaveAs(fullPath, FileFormat.OpenXMLWorkbook);
            }
        }

        private int getCount()
        {
            string path = Path.Combine(this.CurrentProjectPath, "DMS", "counter.txt");

            if (File.Exists(path))
            {
                int value;

                using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                using (StreamReader sr = new StreamReader(fs, Encoding.Default))
                {
                    value = int.Parse(sr.ReadLine());//读取
                    sr.Close();
                    fs.Close();
                }

                value++;
                using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                using (StreamWriter sw = new StreamWriter(fs, Encoding.Default))
                {
                    sw.WriteLine(value.ToString());
                    sw.Close();
                    fs.Close();
                }


                return value;
            }
            else//直接创建一个新的
            {
                using (FileStream fs = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite))
                using (StreamWriter sw = new StreamWriter(fs, Encoding.Default))
                {
                    sw.WriteLine("1");
                    sw.Close();
                    fs.Close();
                }
                return 1;
            }
        }

        private string getBinFilename()
        {
            string[] files = Directory.GetFiles(Path.Combine(this.currentProjectPath, "DMS"), "*.bin", SearchOption.TopDirectoryOnly);
            if (files.Length == 0)
            {
                throw new Exception("当前任务未找到合适的规格组");
            }

            string f = (new FileInfo(files[0])).Name;
            return f.Substring(0, f.IndexOf(".bin"));
        }

        private void loadMouldings(XElement xml)
        {
            this.Mouldings = new List<Moulding>();
            var mouldings = from e in xml.Elements("Moulding")
                            select e;
            foreach (var xMoulding in mouldings)
            {
                Moulding moulding = new Moulding();
                moulding.Description = xMoulding.Attribute("Name").Value;
                string kind = xMoulding.Attribute("Kind").Value;
                if (kind == "kCrown")
                {
                    moulding.Kind = "顶线";
                }
                else if (kind == "kBaseBoard")
                {
                    moulding.Kind = "踢脚线";
                }
                else if (kind == "kCounter")
                {
                    moulding.Kind = "台面";
                }
                else if (kind == "kBackSplash")
                {
                    moulding.Kind = "后挡水";
                }

                moulding.Material = xMoulding.Elements("G").SingleOrDefault().Attribute("Material").Value;

                int count = 0;
                double dist = 0;
                double x = 0, y = 0;
                foreach (var xpoint in xMoulding.Elements("Point"))
                {
                    if (count == 0)
                    {
                        x = UnitConverter.GetValueFromString(xpoint.Attribute("X").Value);
                        y = UnitConverter.GetValueFromString(xpoint.Attribute("Y").Value);
                    }
                    else
                    {
                        double x1 = UnitConverter.GetValueFromString(xpoint.Attribute("X").Value);
                        double y1 = UnitConverter.GetValueFromString(xpoint.Attribute("Y").Value);

                        dist = Math.Sqrt(Math.Pow(x - x1, 2) + Math.Pow(y - y1, 2));
                        dist = Math.Round(dist, 1);
                    }
                    count++;
                }

                moulding.Length = dist;

                Mouldings.Add(moulding);
            }
        }

        private void loadProducts(XElement xml)
        {
            this.Products = new List<Product>();
            var units = from e in xml.Elements("Inter")
                        where e.Attribute("Manufacturer").Value == "dms"
                        select e;

            string connectStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source="
                + Path.Combine(this.currentProjectPath, "DMS", "ProductList.mdb");
            foreach (var x in units)
            {
                using (OleDbConnection conn = new OleDbConnection(connectStr))
                {
                    conn.Open();

                    string[] dmids = x.Attribute("DMID").Value.Replace("_", "").Split('-');

                    string id = dmids[0];
                    string timestamp = (dmids.Length > 1) ? dmids[1] : string.Empty;

                    string selectStr = string.Format("Select * from ProductList Where Handle='{0}'", id);
                    var product = conn.Query<Product>(selectStr, null).SingleOrDefault<Product>();
                    if (product == null)
                    {
                        MessageBox.Show(selectStr);
                        continue;
                    }

                    ProductHelper.LoadProduct(product,
                        Path.Combine(currentProjectPath, "DMS", "Output", product.Handle));

                    string timestampPath = Path.Combine(this.currentProjectPath, "DMS", "Output", product.Handle, timestamp + ".time");

                    if (File.Exists(timestampPath))
                    {
                        product.IsDataMatch = true;
                    }
                    else
                    {
                        //MessageBox.Show(timestampPath);
                        product.IsDataMatch = false;
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
        public List<Moulding> Mouldings { get; private set; }
        public List<Decco> Deccos { get; private set; }
        public List<PauchieProduct> PauchieProducts { get; private set; }
        public List<PauchieHardware> PauchieHardwares { get; private set; }
        public RelayCommand StartCommand { get; set; }
        public RelayCommand ShowConfiguration { get; set; }
        public RelayCommand BrowserCommand { get; set; }
        public RelayCommand StartCADCommand { get; set; }
    }
}
