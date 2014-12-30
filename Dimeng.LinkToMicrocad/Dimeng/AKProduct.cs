using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Dimeng.LinkToMicrocad
{
    internal class AKProduct
    {
        public TabInfo Tab { get; set; }
        IEnumerable<UIVar> UIVars { get; set; }

        internal static AKProduct Load(string tempXMLFilePath)
        {
            AKProduct product = new AKProduct();

            XElement xml = XElement.Load(tempXMLFilePath);
            product.UIVars = from n in xml.Elements("UIVar")
                             select new UIVar(
                                    (string)n.Attribute("Name"),
                                    (string)n.Attribute("Value")
                                 );


            product.Tab = new TabInfo();

            var tab = from t in xml.Elements("Tab")
                      select t;
            product.Tab.DWG = tab.SingleOrDefault().Attribute("DWG").Value;

            var vars = from t in xml.Elements("Tab").Elements("Var")
                       select t;

           

            foreach (var v in vars)
            {
                switch (v.Attribute("Name").Value.ToString())
                {
                    case "X":
                        product.Tab.VarX = double.Parse(v.Attribute("Value").Value.Replace("mm",""));
                        break;
                    case "Y":
                        product.Tab.VarY = double.Parse(v.Attribute("Value").Value.Replace("mm", ""));
                        break;
                    case "Z":
                        product.Tab.VarZ = double.Parse(v.Attribute("Value").Value.Replace("mm", ""));
                        break;
                }
            }

            return product;
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
        }

        public override string ToString()
        {
            return string.Format("{0}/{1}", Name, Value);
        }
    }
}
