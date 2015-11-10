using ADLauncher.Properties;
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

namespace ADLauncher
{
    /// <summary>
    /// PushConectionWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PushConnectionWindow : Window
    {
        string orderNumber;
        string lineNumber;
        public PushConnectionWindow(string ordernumber, string linenumber)
        {
            InitializeComponent();
            this.orderNumber = ordernumber;
            this.lineNumber = linenumber;
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
            if (PushHelper.GetToken(this.tbUserName.Text, passwordbox.Password, out token))
            {
                Settings.Default.ClientId = this.tbUserName.Text;
                Settings.Default.ClientPassword = passwordbox.Password;
                Settings.Default.ClientToken = token;
                Settings.Default.Save();

                if (!PushHelper.OrderCanEdit(token, this.orderNumber, this.lineNumber))
                {
                    if (MessageBox.Show("当前订单状态为已设计，是否要重置订单状态？否则退出程序", "Warning", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        if (PushHelper.OrderCancelDesign(token, this.orderNumber, this.lineNumber))
                        { this.DialogResult = true; }
                        else
                        {
                            this.DialogResult = false;
                        }
                    }
                    this.DialogResult = false;
                }
                else
                {
                    this.DialogResult = true;
                }
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
