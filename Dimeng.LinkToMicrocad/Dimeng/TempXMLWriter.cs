using Dimeng.LinkToMicrocad.Logging;
using Dimeng.WoodEngine.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Dimeng.LinkToMicrocad
{
    internal class TempXMLWriter
    {
        public void WriteFile(string xmlPath, AKProduct akproduct, IEnumerable<string> materialList)
        {
            Logger.GetLogger().Info("Writing back temp.xml file......");

            XDocument doc = new XDocument();
            XElement xml = new XElement("Root");

            foreach (var ui in akproduct.UIVars)
            {
                if (ui.Value != null)
                {
                    xml.Add(new XElement("UIVar",
                                new XAttribute("Value", ui.Value),
                                new XAttribute("Name", ui.Name)));
                }
            }

            xml.Add(getTab(akproduct, materialList));
            xml.Add(getCategoryMaterial(materialList));
            doc.Add(xml);
            doc.Save(xmlPath);
        }

        public XElement getTab(AKProduct akproduct, IEnumerable<string> materialList)
        {
            XElement tabNode = new XElement("Tab");
            tabNode.Add(new XAttribute("Name", akproduct.Tab.Name));
            tabNode.Add(new XAttribute("Units", "mm"));
            tabNode.Add(new XAttribute("DWG", akproduct.Tab.DWG));
            tabNode.Add(new XAttribute("Photo", akproduct.Tab.Photo));
            tabNode.Add(new XAttribute("File", "DMS.xml"));
            tabNode.Add(new XAttribute("Path", akproduct.GetUIVarValue("CatalogPath")));
            tabNode.Add(new XAttribute("Description", akproduct.Tab.Description));
            tabNode.Add(new XAttribute("DMID", akproduct.Tab.DMID));
            tabNode.Add(new XAttribute("ID", akproduct.Tab.ID));
            tabNode.Add(new XAttribute("Kind", "kDiMengObj"));
            tabNode.Add(new XAttribute("Category", "Accessory"));

            tabNode.Add(new XElement("Var",
                            new XAttribute("Value", akproduct.Tab.VarX.ToString() + "mm"),
                            new XAttribute("Name", "X"),
                            new XAttribute("Type", "InputX")),
                        new XElement("Var",
                            new XAttribute("Value", akproduct.Tab.VarY.ToString() + "mm"),
                            new XAttribute("Name", "Y"),
                            new XAttribute("Type", "InputY")),
                        new XElement("Var",
                            new XAttribute("Value", akproduct.Tab.VarZ.ToString() + "mm"),
                            new XAttribute("Name", "Z"),
                            new XAttribute("Type", "InputZ")),
                        new XElement("Var",
                            new XAttribute("Value", akproduct.Tab.VarElevation + "mm"),
                            new XAttribute("Name", "Elevation"),
                            new XAttribute("Type", "InputE"))
                        );

            int i = 0;
            foreach (var m in materialList)
            {
                var texture = Context.GetContext().MVDataContext.GetTexture(m);

                if (texture != null)
                {
                    tabNode.Add(new XElement("G",
                                    new XAttribute("Name", "material" + i.ToString()),
                                    new XAttribute("HM", 1),
                                    new XAttribute("Layer", m),
                                    new XAttribute("Text", m),
                                    new XAttribute("Material", "material" + i.ToString()),
                                    new XAttribute("A_41", texture.ImageName),
                                    new XAttribute("A_47", texture.A47)
                               ));

                    i++;
                }
            }

            return tabNode;
        }

        public XElement getCategoryMaterial(IEnumerable<string> materialList)
        {
            XElement xml = new XElement("Category",
                                new XAttribute("Name", "Material Catalog"),
                                new XAttribute("K", "MaterialCatalog"));
            XElement xmlGroups = new XElement("Groups");


            int i = 0;
            foreach (var m in materialList)
            {
                var texture = Context.GetContext().MVDataContext.GetTexture(m);

                if (texture != null)
                {
                    xmlGroups.Add(new XElement("G",
                                    new XAttribute("Name", "material" + i.ToString()),
                                    new XAttribute("HM", 1),
                                    new XAttribute("Layer", m),
                                    new XAttribute("Text", m),
                                    new XAttribute("Material", "material" + i.ToString()),
                                    new XAttribute("A_41", texture.ImageName),
                                    new XAttribute("A_47", texture.A47)
                               ));

                    i++;
                }
            }
            xml.Add(xmlGroups);
            return xml;
        }
    }
}
