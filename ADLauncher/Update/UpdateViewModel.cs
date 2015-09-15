using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;

namespace ADLauncher
{
    public class UpdateViewModel : ViewModelBase
    {
        private readonly BackgroundWorker worker;

        public UpdateViewModel()
        {
            this.worker = new BackgroundWorker();
            this.worker.DoWork += this.DoWork;
            this.worker.ProgressChanged += this.ProgressChanged;
            this.worker.RunWorkerCompleted += worker_RunWorkerCompleted;

        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.CloseWindow();
        }

        public void StartUpdate()
        {
            this.worker.RunWorkerAsync();
        }

        public Action CloseWindow { get; set; }

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.ProgressValue = e.ProgressPercentage;
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            this.ProgressValue = 0;

            try
            {
                UpdateVersion verLocal = checkLocalVersion();

                UpdateVersion verNewest = checkRemoteVersion();

                if (verNewest.IsNewerThan(verLocal))
                {
                    UpdateHelper uhelper = new UpdateHelper(verNewest,
                        Directory.GetCurrentDirectory());

                    //step1 获取文件清单
                    uhelper.GetFileList();
                    this.ProgressMax = uhelper.Items.Count;

                    //step2 创建临时目录，比对清单和当前的文件的md5值，开始下载
                    foreach (var item in uhelper.Items)
                    {
                        ProgressText = item.Path;
                        try
                        {
                            uhelper.Download(item);
                        }
                        catch (Exception error2)
                        {
                            throw new Exception("下载文件" + item.Path + "失败." + error2.Message);
                        }
                        this.ProgressValue++;
                    }

                    //step4 写入最新的版本号到当前目录
                    writeVersionNumber(verNewest.VersionNumber);
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }

        private UpdateVersion checkRemoteVersion()
        {
            string tempFile = Path.GetTempFileName();

            FTPclient client = new FTPclient(
                "www.dm-software.com",
                "Microcad",
                "Paco1234");

            if (client.Download(@"/Data/Version.xml", tempFile, true))
            {
                FileStream fs = new FileStream(tempFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                StreamReader sr = new StreamReader(fs, Encoding.Default);
                string v = sr.ReadLine();
                sr.Close();
                fs.Close();

                File.Delete(tempFile);

                return new UpdateVersion(v);
            }

            throw new Exception("未找到所需的数据");
        }

        private UpdateVersion checkLocalVersion()
        {
            string folder = System.IO.Directory.GetCurrentDirectory();
            string file = Path.Combine(folder, "version");
            if (!File.Exists(file))
            {
                return new UpdateVersion("1985.01.18.001");
            }

            FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
            StreamReader sr = new StreamReader(fs, Encoding.Default);
            string line;
            line = sr.ReadLine();
            sr.Close();
            fs.Close();

            return new UpdateVersion(line);
        }

        private void writeVersionNumber(string num)
        {
            string folder = System.IO.Directory.GetCurrentDirectory();
            string file = Path.Combine(folder, "version");

            FileStream fs = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.Write);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);
            sw.WriteLine(num);
            sw.Close();
            fs.Close();
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
        private string progressText;

        public string ProgressText
        {
            get { return progressText; }
            set
            {
                progressText = value;
                base.RaisePropertyChanged("ProgressText");
            }
        }

    }
}
