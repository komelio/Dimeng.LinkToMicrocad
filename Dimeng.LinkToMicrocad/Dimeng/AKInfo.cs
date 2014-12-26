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
        public string Product { get; set; }
        public string SerialNumber { get; set; }
        public string KeyNumber { get; set; }

        public static AKInfo GetInfo()
        {
            try
            {
                AKInfo akInfo = new AKInfo();

                RegistryKey hkml = Registry.LocalMachine;
                RegistryKey akr14 = hkml.OpenSubKey("SOFTWARE\\Microcad\\autodecco_studio\\R11", true);

                akInfo.Path = akr14.GetValue("").ToString();
                akInfo.Country = akr14.GetValue("Country").ToString();
                akInfo.MeterToUnit = akr14.GetValue("MeterToUnit").ToString();
                akInfo.Product = akr14.GetValue("Product").ToString();
                akInfo.SerialNumber = akr14.GetValue("SerialNumber").ToString();
                akInfo.KeyNumber = akr14.GetValue("T").ToString();

                return akInfo;
            }
            catch (System.Exception error)
            {
                throw new Exception("获取当前Microcad产品的注册表信息时发生错误，请检查是否拥有足够的权限读取注册表", error);
            }
        }
    }
}
