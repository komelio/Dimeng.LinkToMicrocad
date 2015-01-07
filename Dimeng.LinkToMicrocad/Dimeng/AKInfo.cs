using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.LinkToMicrocad
{
    public class AKInfo
    {
        public string Path { get; set; }
        public string Country { get; set; }
        public string MeterToUnit { get; set; }
        public string SerialNumber { get; set; }

        public static AKInfo GetInfo()
        {
            try
            {
                Logging.Logger.GetLogger().Debug("Getting current product information from registry...");

                AKInfo akInfo = new AKInfo();

                RegistryKey hkml = Registry.LocalMachine;
                RegistryKey akr14 = hkml.OpenSubKey("SOFTWARE\\Microcad\\autodecco_studio\\R11", true);

                akInfo.Path = akr14.GetValue("").ToString();
                Logging.Logger.GetLogger().Debug(string.Format("Path:{0}", akInfo.Path));

                akInfo.Country = akr14.GetValue("Country").ToString();
                Logging.Logger.GetLogger().Debug(string.Format("Country:{0}", akInfo.Country));

                akInfo.MeterToUnit = akr14.GetValue("MeterToUnit").ToString();
                Logging.Logger.GetLogger().Debug(string.Format("MeterToUnit:{0}", akInfo.MeterToUnit));

                akInfo.SerialNumber = akr14.GetValue("SerialNumber").ToString();
                Logging.Logger.GetLogger().Debug(string.Format("SerialNumber:{0}", akInfo.SerialNumber));

                Logging.Logger.GetLogger().Debug("Registry reading finished.");

                return akInfo;
            }
            catch (System.Exception error)
            {
                throw new Exception("获取当前Microcad产品的注册表信息时发生错误，请检查是否拥有足够的权限读取注册表", error);
            }
        }
    }
}
