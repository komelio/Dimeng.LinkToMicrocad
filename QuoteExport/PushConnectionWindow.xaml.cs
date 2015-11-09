using QuoteExport.ERP;
using QuoteExport.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace QuoteExport
{
    /// <summary>
    /// PushConnectionWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PushConnectionWindow : Window
    {
        public PushConnectionWindow()
        {
            InitializeComponent();

            tbUserName.Text = Settings.Default.ClientId;
            passwordbox.Password = Settings.Default.ClientPassword;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.tbUserName.Text) || string.IsNullOrEmpty(this.passwordbox.Password))
            {
                MessageBox.Show("用户或密码不能为空");
                return;
            }

            string token;
            if (PushConnector.GetToken(this.tbUserName.Text, passwordbox.Password, out token))
            {
                Settings.Default.ClientId = this.tbUserName.Text;
                Settings.Default.ClientPassword = passwordbox.Password;
                Settings.Default.ClientToken = token;
                Settings.Default.Save();
                this.DialogResult = true;
            }
            else
            {
                MessageBox.Show("用户名或密码错误");
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
