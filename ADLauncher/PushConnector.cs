using ADLauncher.PushSoft;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Windows;
using System.Xml.Linq;

namespace ADLauncher
{
    public class PushHelper
    {
        public static bool GetToken(string username, string password, out string token)
        {
            try
            {
                WebServiceSoapClient client
                    = new WebServiceSoapClient(new BasicHttpBinding(), new EndpointAddress("http://www.pauchie.com.cn:81/pauchie/Furnit/WebService.asmx"));
                string result = client.GetToken(username, password);

                XElement xml = XElement.Parse(result);

                if (xml.Elements("Success").SingleOrDefault().Value == "Y")
                {
                    token = xml.Elements("DataSource").SingleOrDefault().Value;
                    return true;
                }
                else
                {
                    token = string.Empty;
                    return false;
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
                throw error;
            }
        }

        public static bool OrderCanEdit(string token, string order, string linenumber)
        {
            try
            {
                WebServiceSoapClient client
                    = new WebServiceSoapClient(new BasicHttpBinding(), new EndpointAddress("http://www.pauchie.com.cn:81/pauchie/Furnit/WebService.asmx"));
                string result = client.OrderCanEdit(token, order, int.Parse(linenumber));

                XElement xml = XElement.Parse(result);
                //MessageBox.Show(xml.Elements("DataSource").SingleOrDefault().Value);
                if (xml.Elements("DataSource").SingleOrDefault().Value == "Y")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
                throw error;
            }
        }

        public static bool OrderCancelDesign(string token, string order, string linenumber)
        {
            try
            {
                WebServiceSoapClient client
                    = new WebServiceSoapClient(new BasicHttpBinding(), new EndpointAddress("http://www.pauchie.com.cn:81/pauchie/Furnit/WebService.asmx"));
                string result = client.OrderCancelDesign(token, order, int.Parse(linenumber));

                XElement xml = XElement.Parse(result);

                if (xml.Elements("Success").SingleOrDefault().Value == "Y")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
                throw error;
            }
        }
    }
}
