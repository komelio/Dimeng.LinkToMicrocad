using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Windows.Forms;

namespace Updater
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            getJson();
        }

        private void getJson()
        {
            HttpWebRequest request = HttpWebRequest.Create("http://localhost:46959/releases/LatestReleaseVersion") as HttpWebRequest;
            request.Method = "GET";
            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            byte[] buf = new byte[1024];
            int len = stream.Read(buf, 0, 1024);
            //string str = Encoding.ASCII.GetString(buf, 0, len); 
            string str = System.Text.Encoding.GetEncoding("gb2312").GetString(buf);
        }
    }
}
