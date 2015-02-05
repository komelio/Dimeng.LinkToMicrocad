using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Dimeng.LinkToMicrocad;
using Dimeng.WoodEngine.Entities;
using System.IO;

namespace Dimeng.LinkToMicrocad
{
    internal class AKProduct
    {
        public TabInfo Tab { get; set; }
        public List<UIVar> UIVars { get; set; }

        internal static AKProduct Load(string tempXMLFilePath)
        {
            Logging.Logger.GetLogger().Debug("Getting cabinet information from temp.xml file...");
            Logging.Logger.GetLogger().Debug("File path:" + tempXMLFilePath);

            AKProduct product = new AKProduct();

            XElement xml = XElement.Load(tempXMLFilePath);
            var uivars = from n in xml.Elements("UIVar")
                         select new UIVar(
                                (string)n.Attribute("Name"),
                                (string)n.Attribute("Value")
                             );

            product.UIVars = uivars.ToList();


            product.Tab = new TabInfo();

            var tab = from t in xml.Elements("Tab")
                      select t;
            var tabNode = tab.SingleOrDefault();
            product.Tab.Name = tabNode.Attribute("Name").Value;
            product.Tab.DWG = tabNode.Attribute("DWG").Value;
            product.Tab.ID = tabNode.Attribute("ID").Value;
            product.Tab.DMID = tabNode.Attribute("DMID").Value;
            product.Tab.Photo = tabNode.Attribute("Photo").Value;
            product.Tab.Description = tabNode.Attribute("Descprition") == null ?
                                        string.Empty : tabNode.Attribute("Description").Value;
            product.Tab.CatalogPath = tabNode.Attribute("Path").Value;

            var vars = from t in xml.Elements("Tab").Elements("Var")
                       select t;

            foreach (var v in vars)
            {
                switch (v.Attribute("Name").Value.ToString())
                {
                    case "X":
                        product.Tab.VarX = UnitConverter.GetValueFromString(v.Attribute("Value").Value);
                        break;
                    case "Y":
                        product.Tab.VarY = UnitConverter.GetValueFromString(v.Attribute("Value").Value);
                        break;
                    case "Z":
                        product.Tab.VarZ = UnitConverter.GetValueFromString(v.Attribute("Value").Value);
                        break;
                    case "Elevation":
                        product.Tab.VarElevation = UnitConverter.GetValueFromString(v.Attribute("Value").Value);
                        break;
                }
            }

            return product;
        }

        public string GetUIVarValue(string parameter)
        {
            return this.UIVars.Where(it => it.Name == parameter)
                                            .SingleOrDefault()
                                            .Value;
        }
    }

    internal class TabInfo
    {
        public string Name { get; set; }
        public string ID { get; set; }
        public string DMID { get; set; }
        public string Description { get; set; }
        public string CatalogPath { get; set; }
        public string Photo { get; set; }
        public string DWG { get; set; }

        public double VarX { get; set; }
        public double VarY { get; set; }
        public double VarZ { get; set; }
        public double VarElevation { get; set; }
    }

    internal class UIVar
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public UIVar(string name, string value)
        {
            if (string.IsNullOrEmpty(name))
                throw new Exception("UIVar变量名称不能为空");

            this.Name = name;
            this.Value = value;

            Logging.Logger.GetLogger().Debug(string.Format("UIVar:{0}/{1}", this.Name, this.Value));
        }

        public override string ToString()
        {
            return string.Format("{0}/{1}", Name, Value);
        }
    }
}
