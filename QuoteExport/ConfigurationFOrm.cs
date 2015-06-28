using QuoteExport.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace QuoteExport
{
    public partial class ConfigurationForm : Form
    {
        public ConfigurationForm()
        {
            InitializeComponent();

            this.tbFTPServer.Text = Settings.Default.FTPServer;
            this.tbFTPUser.Text = Settings.Default.FTPUser;
            this.tbFTPPass.Text = Settings.Default.FTPPassword;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Settings.Default.FTPServer = tbFTPServer.Text;
            Settings.Default.FTPUser = tbFTPUser.Text;
            Settings.Default.FTPPassword = tbFTPPass.Text;
            Settings.Default.Save();

            this.DialogResult = DialogResult.OK;
        }
    }
}
