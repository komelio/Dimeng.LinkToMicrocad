using Dimeng.LinkToMicrocad.Logging;
using Dimeng.WoodEngine.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Dimeng.LinkToMicrocad
{
    public class DataExporter
    {
        Product product;
        public DataExporter(Product product)
        {
            this.product = product;
        }

        public void Output()
        {
            string path = Path.Combine(product.Project.JobPath, "Output", product.Handle);
            Logger.GetLogger().Debug(path);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            else
            {
                //删除原有的所有内容
                var files = Directory.GetFiles(path);
                foreach (var f in files)
                {
                    File.Delete(f);
                }
            }

            outputMachinings(path);

            outputPartsInfo(path);
        }

        private void outputPartsInfo(string path)
        {
            string pathToPartInfo = path;
            if (!Directory.Exists(pathToPartInfo))
            {
                Directory.CreateDirectory(pathToPartInfo);
            }
            else
            {
                foreach (var f in Directory.GetFiles(pathToPartInfo,"*.xml"))
                {
                    File.Delete(f);
                }
            }

            var partsExporter = new PartInfoExporter(this.product, pathToPartInfo);
            partsExporter.Export();
        }

        private void outputMachinings(string path)
        {
            string pathToMachineCode = Path.Combine(path, "Machinings");
            if (!Directory.Exists(pathToMachineCode))
            {
                Directory.CreateDirectory(pathToMachineCode);
            }
            else
            {
                //先删除掉原有的csv文件
                foreach (var f in Directory.GetFiles(pathToMachineCode))
                {
                    File.Delete(f);
                }
            }
            foreach (var part in this.product.CombinedParts)
            {
                MachiningExporter me = new MachiningExporter(part, pathToMachineCode);
                me.Export();
            }
        }
    }
}
