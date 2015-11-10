using QuoteExport.Properties;
using QuoteExport.Push;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

namespace QuoteExport.ERP
{
    public class PushConnector
    {
        public static string GetToken()
        {
            try
            {
                Push.WebServiceSoapClient client
                    = new Push.WebServiceSoapClient(new BasicHttpBinding(), new EndpointAddress("http://www.pauchie.com.cn:81/pauchie/Furnit/WebService.asmx"));
                string result = client.GetToken("DM01", "DM01");

                XElement xml = XElement.Parse(result);

                return xml.Elements("DataSource").SingleOrDefault().Value;
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
                throw error;
            }
        }

        public static bool OrderDesign(string adProjectName, out string message)
        {
            try
            {
                string[] words = adProjectName.Split('-');

                Push.WebServiceSoapClient client
                    = new Push.WebServiceSoapClient(new BasicHttpBinding(), new EndpointAddress("http://www.pauchie.com.cn:81/pauchie/Furnit/WebService.asmx"));
                string result = client.OrderDesign(Settings.Default.ClientToken, words[0], int.Parse(words[1]));

                XElement xml = XElement.Parse(result);
                if (xml.Elements("Success").SingleOrDefault().Value == "Y")
                {
                    message = "Success!";
                    return true;
                }
                else
                {
                    message = xml.Elements("Message").SingleOrDefault().Value;
                    return false;
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
                throw error;
            }
        }

        public static bool OrderCanEdit(string adProjectName, out string message)
        {
            try
            {
                //MessageBox.Show(string.Format("{0}/{1}/",adProjectName,Settings.Default.ClientToken));
                string[] words = adProjectName.Split('-');

                Push.WebServiceSoapClient client
                    = new Push.WebServiceSoapClient(new BasicHttpBinding(), new EndpointAddress("http://www.pauchie.com.cn:81/pauchie/Furnit/WebService.asmx"));
                string result = client.OrderCanEdit(Settings.Default.ClientToken, words[0], int.Parse(words[1]));

                XElement xml = XElement.Parse(result);
                //MessageBox.Show(xml.Elements("DataSource").SingleOrDefault().Value);
                if (xml.Elements("DataSource").SingleOrDefault().Value == "Y")
                {
                    message = "Success!";
                    return true;
                }
                else
                {
                    message = xml.Elements("Message").SingleOrDefault().Value;
                    return false;
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
                throw error;
            }
        }

        internal static bool GetToken(string username, string password, out string token)
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
    }
}
