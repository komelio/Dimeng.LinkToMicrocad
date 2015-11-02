using QuoteExport.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace QuoteExport
{
    public class ProductHelper
    {
        public static void LoadProduct(Product product, string folderPath)
        {
            string filename = Path.Combine(folderPath, "parts.xml");
            if (!File.Exists(filename))
            {
                return;
            }

            product.Parts.Clear();
            product.Hardwares.Clear();
            product.Subassemblies.Clear();

            XElement xml = XElement.Load(filename);
            var partsX = from p in xml.Elements("Parts").Elements("Part")
                         select p;
            foreach (var px in partsX)
            {
                Part part = loadPartXElement(px, product);
                product.Parts.Add(part);
            }

            var hwrsX = from h in xml.Elements("Hardwares").Elements("Hardware")
                        select h;
            foreach (var hx in hwrsX)
            {
                Hardware hw = loadHardwareXElement(hx);
                product.Hardwares.Add(hw);
            }

            var subsX = from s in xml.Elements("Subassemblies").Elements("Subassembly")
                        select s;
            foreach (var sx in subsX)
            {
                Subassembly sub = loadSubXElement(sx);
                product.Subassemblies.Add(sub);
            }


            //get total qty
            product.PartsCounter = 0; product.HardwaresCounter = 0;
            foreach(var part in product.Parts)
            {
                product.PartsCounter += part.Qty;
            }
            foreach(var hw in product.Hardwares)
            {
                product.HardwaresCounter += hw.Qty;
            }
            foreach(var sub in product.Subassemblies)
            {
                foreach (var part in sub.Parts)
                {
                    product.PartsCounter += part.Qty;
                }
                foreach (var hw in sub.Hardwares)
                {
                    product.HardwaresCounter += hw.Qty;
                }
                foreach(var sub2 in sub.Subassemblies)
                {
                    foreach (var part in sub2.Parts)
                    {
                        product.PartsCounter += part.Qty;
                    }
                    foreach (var hw in sub2.Hardwares)
                    {
                        product.HardwaresCounter += hw.Qty;
                    }
                }
            }
        }

        private static Subassembly loadSubXElement(XElement sx)
        {
            Subassembly sub = new Subassembly();
            sub.Description = sx.Attribute("SubName").Value;
            sub.Qty = int.Parse(sx.Attribute("Qty").Value);
            sub.Width = double.Parse(sx.Attribute("Width").Value);
            sub.Height = double.Parse(sx.Attribute("Height").Value);
            sub.Depth = double.Parse(sx.Attribute("Depth").Value);

            foreach (var px in sx.Elements("Parts").Elements("Part"))
            {
                Part part = loadPartXElement(px, sub);
                sub.Parts.Add(part);
                
            }
            foreach (var hx in sx.Elements("Hardwares").Elements("Hardware"))
            {
                Hardware hw = loadHardwareXElement(hx);
                sub.Hardwares.Add(hw);
            }
            foreach (var sx2 in sx.Elements("Subassemblies").Elements("Subassembly"))
            {
                Subassembly s2 = loadSubXElement(sx2);
                sub.Subassemblies.Add(s2);
            }
            return sub;
        }

        private static Hardware loadHardwareXElement(XElement hx)
        {
            Hardware hwr = new Hardware();
            hwr.Name = hx.Attribute("HardwareName").Value;
            hwr.Qty = int.Parse(hx.Attribute("Qty").Value);
            hwr.Width = double.Parse(hx.Attribute("Width").Value);
            hwr.Height = double.Parse(hx.Attribute("Height").Value);
            hwr.Depth = double.Parse(hx.Attribute("Depth").Value);
            hwr.Comment = hx.Attribute("Comment").Value;
            hwr.Comment2 = hx.Attribute("Comment2").Value;
            hwr.Comment3 = hx.Attribute("Comment3").Value;
            hwr.HardewareCode = hx.Attribute("HardwareCode").Value;
            return hwr;
        }

        private static Part loadPartXElement(XElement px, IProduct parent)
        {
            Part part = new Part();
            part.Parent = parent;
            part.PartName = px.Attribute("PartName").Value;
            part.Qty = int.Parse(px.Attribute("Qty").Value);
            part.Width = double.Parse(px.Attribute("Width").Value);
            part.Length = double.Parse(px.Attribute("Length").Value);
            part.CutWidth = double.Parse(px.Attribute("CutWidth").Value);
            part.CutLength = double.Parse(px.Attribute("CutLength").Value);
            part.Material = px.Attribute("Material").Value;
            part.MaterialCode = px.Attribute("MaterialCode").Value;
            part.Thickness = double.Parse(px.Attribute("Thickness").Value);
            part.EBW1 = px.Attribute("EBW1").Value;
            part.EBW1Thick = double.Parse(px.Attribute("EBW1Thick").Value);
            part.EBW1Code = px.Attribute("EBW1Code").Value;
            part.EBW2 = px.Attribute("EBW2").Value;
            part.EBW2Thick = double.Parse(px.Attribute("EBW2Thick").Value);
            part.EBW2Code = px.Attribute("EBW2Code").Value;
            part.EBL1 = px.Attribute("EBL1").Value;
            part.EBL1Thick = double.Parse(px.Attribute("EBL1Thick").Value);
            part.EBL1Code = px.Attribute("EBL1Code").Value;
            part.EBL2 = px.Attribute("EBL2").Value;
            part.EBL2Thick = double.Parse(px.Attribute("EBL2Thick").Value);
            part.EBL2Code = px.Attribute("EBL2Code").Value;
            part.Comment = px.Attribute("Comment").Value;
            part.Comment2 = px.Attribute("Comment2").Value;
            part.Comment3 = px.Attribute("Comment3").Value;
            part.FileName = px.Attribute("FileName").Value;
            part.Face6FileName = px.Attribute("Face6FileName").Value;
            return part;
        }
    }
}
