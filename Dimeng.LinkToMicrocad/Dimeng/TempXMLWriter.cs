using Dimeng.LinkToMicrocad.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Dimeng.LinkToMicrocad
{
    internal class TempXMLWriter
    {
        public void WriteFile(string xmlPath, AKProduct product)
        {
            Logger.GetLogger().Info("Writing back temp.xml file......");

            XDocument doc = new XDocument();
            XElement xml = new XElement("Root");

            foreach (var ui in product.UIVars)
            {
                if (ui.Value != null)
                {
                    xml.Add(new XElement("UIVar",
                                new XAttribute("Value", ui.Value),
                                new XAttribute("Name", ui.Name)));
                }
            }

            xml.Add(getTab(product));
            doc.Add(xml);
            doc.Save(xmlPath);
        }

        public XElement getTab(AKProduct product)
        {
            XElement tabNode = new XElement("Tab");
            tabNode.Add(new XAttribute("Name", product.Tab.Name));
            tabNode.Add(new XAttribute("Units", "mm"));
            tabNode.Add(new XAttribute("DWG", product.Tab.DWG));
            tabNode.Add(new XAttribute("Photo", product.Tab.Photo));
            tabNode.Add(new XAttribute("File", "DMS.xml"));
            tabNode.Add(new XAttribute("Path", "f:\\ak12\\x64\\catalog\\dms"));
            tabNode.Add(new XAttribute("Description", product.Tab.Description));
            tabNode.Add(new XAttribute("DMID", product.Tab.DMID));
            tabNode.Add(new XAttribute("ID", product.Tab.ID));
            tabNode.Add(new XAttribute("Kind", "kDiMengObj"));
            tabNode.Add(new XAttribute("Category", "Accessory"));

            tabNode.Add(new XElement("Var",
                            new XAttribute("Value", product.Tab.VarX.ToString() + "mm"),
                            new XAttribute("Name", "X"),
                            new XAttribute("Type", "InputX")),
                        new XElement("Var",
                            new XAttribute("Value", product.Tab.VarY.ToString() + "mm"),
                            new XAttribute("Name", "Y"),
                            new XAttribute("Type", "InputY")),
                        new XElement("Var",
                            new XAttribute("Value", product.Tab.VarZ.ToString() + "mm"),
                            new XAttribute("Name", "Z"),
                            new XAttribute("Type", "InputZ")),
                        new XElement("Var",
                            new XAttribute("Value", product.Tab.VarElevation + "mm"),
                            new XAttribute("Name", "Elevation"),
                            new XAttribute("Type", "InputE"))
                        );
            return tabNode;
        }
    }
}
