using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dimeng.FTP;
using System.Windows;

namespace QuoteExport.Entities
{
    public class Uploader
    {
        string server;
        string user;
        string pw;

        public Uploader(string ftpserver, string user, string password)
        {
            this.server = ftpserver;
            this.user = user;
            this.pw = password;
        }

        public void Upload(string sourcepath, string destinPath)
        {
            FTPclient client = new FTPclient(server, user, pw);
            client.Upload(sourcepath, destinPath);
        }

        public void DeleteDirectoryFiles(string path)
        {
            if (path == "/")
            {
                throw new Exception("不能删除根目录的内容！");
            }

            FTPclient client = new FTPclient(server, user, pw);
            if (client.FtpDirectoryExists(path))
            {
                foreach (var file in client.ListDirectory(path))
                {
                    if (!client.FtpDelete(path + "/" + file))
                    {
                        MessageBox.Show(file);
                    }
                }
                //client.FtpDeleteDirectory(path);
            }
        }
    }
}
