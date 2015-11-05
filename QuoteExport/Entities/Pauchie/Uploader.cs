using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dimeng.FTP;

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

        public void Upload(string sourcepath,string destinPath)
        {
            FTPclient client = new FTPclient(server, user, pw);
            client.Upload(sourcepath, destinPath);
        }
    }
}
