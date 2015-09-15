using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace ADLauncher
{
    public class UpdateHelper
    {
        string baseFolderPath;
        UpdateVersion version;
        public List<UpdateItem> Items = new List<UpdateItem>();
        public UpdateHelper(UpdateVersion remoteVersion, string path)
        {
            this.version = remoteVersion;
            this.baseFolderPath = path;
        }

        public bool GetFileList()
        {
            FTPclient client = new FTPclient(
                "www.dm-software.com",
                "Microcad",
                "Paco1234");

            string tempFile = Path.GetTempFileName();

            if (client.Download(string.Format(@"/Data/{0}/list.txt", version.VersionNumber), tempFile, true))
            {
                FileStream fs = new FileStream(tempFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                StreamReader sr = new StreamReader(fs, Encoding.Default);
                string line;

                while ((line = sr.ReadLine()) != null)
                {
                    string[] lines = line.Split('|');
                    string path = lines[0];
                    string md5 = lines[1];

                    Items.Add(new UpdateItem() { Path = path, MD5 = md5 });
                }

                sr.Close();
                fs.Close();

                File.Delete(tempFile);

                return true;
            }

            return false;
        }

        public bool Download(UpdateItem item)
        {
            string destinFile = Path.Combine(this.baseFolderPath, item.Path);
            if (File.Exists(destinFile))
            {
                MD5 md5 = MD5.Create();
                using (var stream = File.OpenRead(destinFile))
                {
                    string result = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToUpper();
                    if (item.MD5 == result)
                    {
                        return false;
                    }
                }
            }


            FTPclient client = new FTPclient(
                "www.dm-software.com",
                "Microcad",
                "Paco1234");

            string tempFolder = Path.GetTempPath();

            string folderName = item.GetFolder();
            folderName = Path.Combine(tempFolder, "DMAD", folderName);
            if (!Directory.Exists(folderName))
            {
                Directory.CreateDirectory(folderName);
            }

            string filename = Path.Combine(tempFolder, "DMAD", item.Path);
            if (client.Download(string.Format(@"/Data/{0}/{1}", version.VersionNumber, item.Path), filename, true))
            {
                string dFile = Path.Combine(this.baseFolderPath, item.Path);
                File.Copy(filename, dFile, true);
                return true;
            }

            return false;
        }


        private void deleteFolder(string FolderName)
        {
            DirectoryInfo dir = new DirectoryInfo(FolderName);

            foreach (FileInfo fi in dir.GetFiles())
            {
                fi.Delete();
            }

            foreach (DirectoryInfo di in dir.GetDirectories())
            {
                deleteFolder(di.FullName);
                di.Delete();
            }
        }
    }
}
