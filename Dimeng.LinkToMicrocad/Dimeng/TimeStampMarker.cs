using Dimeng.LinkToMicrocad.Logging;
using Dimeng.WoodEngine.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Dimeng.LinkToMicrocad
{
    public class TimeStampMarker
    {
        /// <summary>
        /// 生成一个时间戳保存在硬盘上，这样导出QuoteExport导出时可以通过时间戳验证数据是否是一致的
        /// </summary>
        /// <param name="prodakt"></param>
        public static void Mark(Product prodakt)
        {
            string path = Path.Combine(prodakt.Project.JobPath, "Output", prodakt.Handle);
            string filePath = Path.Combine(path, DateTime.Now.ToString("yyyyMMddHHmm") + ".time");
            Logger.GetLogger().Debug(path);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            else
            {
                //删除原有的所有内容
                var files = Directory.GetFiles(path, "*.time");
                foreach (var f in files)
                {
                    File.Delete(f);
                }
            }

            using (FileStream fs = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write, FileShare.Write))
            using (StreamWriter sw = new StreamWriter(fs, Encoding.Default))
            {
                sw.WriteLine("Created by XSP!");
            }
        }
    }
}
