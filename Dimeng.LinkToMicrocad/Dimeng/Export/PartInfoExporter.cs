using Dimeng.WoodEngine.Entities;
using Dimeng.WoodEngine.Entities.MachineTokens;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Dimeng.LinkToMicrocad
{
    public class PartInfoExporter
    {
        Product product;
        string path;
        public PartInfoExporter(Product product, string path)
        {
            this.product = product;
            this.path = path;
        }

        public void Export()
        {
            XDocument doc = new XDocument(new XDeclaration("1.0", "utf8", "no"));

            XElement xProduct = new XElement("Product",
                new XAttribute("Description", product.Description),
                new XAttribute("Comments", product.Comments),
                new XAttribute("Width", product.Width),
                new XAttribute("Height", product.Height),
                new XAttribute("Depth", product.Depth));

            XElement xParts = new XElement("Parts");
            xProduct.Add(xParts);
            foreach (var part in this.product.Parts.Where(it=>!it.Material.IsFake))
            {
                XElement xml = getPartXElement(part);
                xParts.Add(xml);
            }

            XElement xHwrs = new XElement("Hardwares");
            xProduct.Add(xHwrs);
            foreach (var hw in this.product.Hardwares)
            {
                xHwrs.Add(getHwrXElement(hw));
            }

            XElement xSubs = new XElement("Subassemblies");
            xProduct.Add(xSubs);
            foreach (var sub in this.product.Subassemblies)
            {
                XElement xml = getSubXElement(sub);
                xSubs.Add(xml);
            }

            //计算机加工指令带来的三合一、活动层板孔数量
            var list = new List<Hardware>();
            foreach (var part in this.product.CombinedParts.Where(it=>!it.Material.IsFake))
            {
                getPartTokenHardwares(part, list);
            }
            foreach (var hw in list)
            {
                xHwrs.Add(getHwrXElement(hw));
            }

            doc.Add(xProduct);
            doc.Save(Path.Combine(this.path, "parts.xml"));
        }

        private void getPartTokenHardwares(Part part, List<Hardware> list)
        {
            foreach (var hd in part.HDrillings)
            {
                if (hd.Token != null)
                {
                    if (hd.Token.Token.IndexOf("三合一") > -1)
                    {
                        addHardwareToList("三合一拉杆", "", "个", 1, list);
                    }
                    else if (hd.Token.Token.IndexOf("木榫") > -1)
                    {
                        addHardwareToList("木榫", "", "个", 1, list);
                    }
                }
            }

            foreach (var vd in part.VDrillings)
            {
                if (vd.Token == null)
                {
                    continue;
                }

                if (vd.Token is CAMLOCKMachiningToken)
                {
                    var camlock = vd.Token as CAMLOCKMachiningToken;
                    if (vd.Token.Token.IndexOf("三合一") > -1)
                    {
                        if (vd.Diameter == camlock.CamFaceBoreDiameter)
                        {
                            addHardwareToList("三合一锁扣盖", "", "个", 1, list);
                            addHardwareToList("三合一锁扣", "", "个", 1, list);
                        }
                        else if (vd.Diameter == camlock.FaceBoreDiameter)
                        {
                            addHardwareToList("三合一胶粒", "", "个", 1, list);
                        }
                    }
                    else if (vd.Token.Token.IndexOf("二合一") > -1)
                    {
                        if (vd.Diameter == camlock.CamFaceBoreDiameter)
                        {
                            addHardwareToList("二合一", "", "个", 1, list);
                        }
                    }
                }
                else if (vd.Token.Token.IndexOf("活动层板") > -1)
                {
                    addHardwareToList("活动层板托", "", "个", 1, list);
                }
            }
        }

        private void addHardwareToList(string name, string sku, string unit, int qty, List<Hardware> hwrs)
        {
            var hw = hwrs.Find(it => it.Name.ToUpper() == name.ToUpper());
            if (hw == null)
            {
                hwrs.Add(new Hardware() { Name = name, Qty = qty });
            }
            else
            {
                hw.Qty += qty;
            }
        }

        private XElement getSubXElement(Subassembly sub)
        {
            XElement xml = new XElement("Subassembly");
            xml.Add(new XAttribute("SubName", sub.Description));
            xml.Add(new XAttribute("Qty", sub.Qty));
            xml.Add(new XAttribute("Width", sub.Width));
            xml.Add(new XAttribute("Height", sub.Height));
            xml.Add(new XAttribute("Depth", sub.Depth));
            xml.Add(new XAttribute("RotationAngle", sub.Rotation));

            XElement xParts = new XElement("Parts");
            xml.Add(xParts);
            foreach (var part in sub.Parts.Where(it=>!it.Material.IsFake))
            {
                xParts.Add(getPartXElement(part));
            }

            XElement xHwrs = new XElement("Hardwares");
            xml.Add(xHwrs);
            foreach (var hw in sub.Hardwares)
            {
                xHwrs.Add(getHwrXElement(hw));
            }

            XElement xSubs = new XElement("Subassemblies");
            xml.Add(xSubs);
            foreach (var ssub in sub.Subassemblies)
            {
                xSubs.Add(getSubXElement(ssub));
            }

            return xml;
        }

        private XElement getHwrXElement(Hardware hw)
        {
            XElement xml = new XElement("Hardware");
            xml.Add(new XAttribute("HardwareName", hw.Name));
            xml.Add(new XAttribute("Qty", hw.Qty));
            xml.Add(new XAttribute("Width", hw.Width));
            xml.Add(new XAttribute("Height", hw.Height));
            xml.Add(new XAttribute("Depth", hw.Depth));
            xml.Add(new XAttribute("Rotation", hw.AssociatedRotation));
            xml.Add(new XAttribute("Comment", hw.Comment));
            xml.Add(new XAttribute("Comment2", hw.Comment2));
            xml.Add(new XAttribute("Comment3", hw.Comment3));
            xml.Add(new XAttribute("HardwareCode", hw.HardwareType.HardwareCode ?? string.Empty));
            return xml;
        }

        private static XElement getPartXElement(Part part)
        {
            XElement xml = new XElement("Part");
            xml.Add(new XAttribute("PartName", part.PartName),
                new XAttribute("Qty", part.Qty),
                new XAttribute("Width", part.Width),
                new XAttribute("Length", part.Length),
                new XAttribute("CutWidth", part.CutWidth),
                new XAttribute("CutLength", part.CutLength),
                new XAttribute("Material", part.Material.Name),
                new XAttribute("MaterialCode", part.Material.Code),
                new XAttribute("Thickness", part.Thickness),
                new XAttribute("EBW1", part.EBW1.Name),
                new XAttribute("EBW1Thick", part.EBW1.Thickness),
                new XAttribute("EBW1Code", part.EBW1.Code),
                new XAttribute("EBW2", part.EBW2.Name),
                new XAttribute("EBW2Thick", part.EBW1.Thickness),
                new XAttribute("EBW2Code", part.EBW1.Code),
                new XAttribute("EBL1", part.EBL1.Name),
                new XAttribute("EBL1Thick", part.EBW1.Thickness),
                new XAttribute("EBL1Code", part.EBW1.Code),
                new XAttribute("EBL2", part.EBL2.Name),
                new XAttribute("EBL2Thick", part.EBW1.Thickness),
                new XAttribute("EBL2Code", part.EBW1.Code),
                new XAttribute("Comment", part.Comment),
                new XAttribute("Comment2", part.Comment2),
                new XAttribute("Comment3", part.Comment3),
                new XAttribute("FileName", part.FileName),
                new XAttribute("Face6FileName",part.Face6FileName)
                );
            return xml;
        }
    }
}
