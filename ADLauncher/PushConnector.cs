using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace ADLauncher
{
    public class PushHelper
    {
        public static string GetToken()
        {
            try
            {
                PushSoft.WebServiceSoapClient client = new PushSoft.WebServiceSoapClient();
                return client.GetToken("DM01", "DM01");
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
                throw error;
            }
        }

        public static void SaveToken(string token)
        {
            string path = Directory.GetCurrentDirectory();
            path = Path.Combine(path, "pushsoft.txt");
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            else
            {
                using (FileStream fs = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.Write))
                using (StreamWriter sw = new StreamWriter(fs, Encoding.Default))
                {
                    sw.WriteLine(token);
                    sw.Close();
                    fs.Close();
                }
            }
        }

        public static string LoadToken()
        {
            string path = Directory.GetCurrentDirectory();
            path = Path.Combine(path, "pushsoft.txt");

            using (FileStream fs = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.Write))
            using (StreamReader sr = new StreamReader(fs, Encoding.Default))
            {
                string line = sr.ReadLine();

                sr.Close();
                fs.Close();

                return line;
            }
        }
    }
}
