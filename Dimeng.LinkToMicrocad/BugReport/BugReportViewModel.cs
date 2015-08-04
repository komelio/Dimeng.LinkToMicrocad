using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Dimeng.LinkToMicrocad
{
    public class BugReportViewModel : ViewModelBase
    {
        public BugReportViewModel(string error)
        {
            this.errorMessage = error;
            this.SendReportCommand = new RelayCommand(this.sendReport);
        }

        private void sendReport()
        {
            try
            {
                FTPclient client = new FTPclient(
                    "www.dm-software.com",
                    "microcad",
                    "Paco1234");

                string id = DateTime.Now.ToString("yyyyMMdd");
                client.FtpCreateDirectory("/Logs/" + id);

                string filename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Appdata", "local", "dimeng", "logs", "ad.log");

                if (!File.Exists(filename))
                {
                    throw new FileNotFoundException("未找到日志文件!" + filename);
                }

                string file = Path.GetTempFileName();
                File.Copy(filename, file, true);
                client.Upload(file, "/logs/" + id + "/" + string.Format("ad_{0}.log", Guid.NewGuid()));

                MessageBox.Show("上传完毕");
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }
        private string errorMessage;

        public string ErrorMessage
        {
            get { return errorMessage; }
            set
            {
                errorMessage = value;
                base.RaisePropertyChanged("ErrorMessage");
            }
        }


        public RelayCommand SendReportCommand { get; private set; }
    }
}
