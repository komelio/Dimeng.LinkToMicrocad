using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Dimeng.LinkToMicrocad;
using Dimeng.WoodEngine.Entities;
using System.IO;
using Autodesk.AutoCAD.Geometry;
using Dimeng.LinkToMicrocad.Logging;

namespace Dimeng.LinkToMicrocad
{
    public class AKProduct
    {
        public TabInfo Tab { get; set; }
        public TabInfo TabA { get; set; }
        public List<UIVar> UIVars { get; set; }
        public SubInfo SubInfo { get; set; }

        public static AKProduct Load(string tempXMLFilePath)
        {
            Logging.Logger.GetLogger().Info("Getting cabinet information from temp.xml file...");
            Logging.Logger.GetLogger().Debug("File path:" + tempXMLFilePath);

            AKProduct product = new AKProduct();

            XElement xml = XElement.Load(tempXMLFilePath);
            var uivars = from n in xml.Elements("UIVar").Where(it => it.Attribute("Name") != null)
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
            Logger.GetLogger().Info("Produt Name:" + product.Tab.Name);

            product.Tab.DWG = tabNode.Attribute("DWG").Value;
            product.Tab.ID = tabNode.Attribute("ID").Value;
            Logger.GetLogger().Info("Produt ID:" + product.Tab.ID);

            product.Tab.DMID = tabNode.Attribute("DMID").Value.Replace("_", "");
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
                        Logger.GetLogger().Info("Produt X:" + product.Tab.VarX.ToString());
                        break;
                    case "Y":
                        product.Tab.VarY = UnitConverter.GetValueFromString(v.Attribute("Value").Value);
                        Logger.GetLogger().Info("Produt Y:" + product.Tab.VarY.ToString());
                        break;
                    case "Z":
                        product.Tab.VarZ = UnitConverter.GetValueFromString(v.Attribute("Value").Value);
                        Logger.GetLogger().Info("Produt Z:" + product.Tab.VarZ.ToString());
                        break;
                    case "Elevation":
                        product.Tab.VarElevation = UnitConverter.GetValueFromString(v.Attribute("Value").Value);
                        Logger.GetLogger().Info("Produt Elevation:" + product.Tab.VarElevation.ToString());
                        break;
                }
            }

            getSubInfo(product);

            //组件的具体信息
            var tabA = from t in xml.Elements("TabA")
                       select t;
            var tabANode = tabA.SingleOrDefault();
            if (tabANode != null)
            {
                product.TabA = new TabInfo();
                product.TabA.Name = tabANode.Attribute("Name").Value;
                Logger.GetLogger().Info("Produt Name:" + product.TabA.Name);

                product.TabA.DWG = tabANode.Attribute("DWG").Value;
                product.TabA.ID = tabANode.Attribute("ID").Value;
                Logger.GetLogger().Info("Produt ID:" + product.TabA.ID);

                product.TabA.DMID = tabANode.Attribute("DMID").Value.Replace("_", "");
                product.TabA.Photo = tabANode.Attribute("Photo").Value;
                product.TabA.Description = tabANode.Attribute("Descprition") == null ?
                                            string.Empty : tabANode.Attribute("Description").Value;
                product.TabA.CatalogPath = tabANode.Attribute("Path").Value;
                var varsA = from t in xml.Elements("TabA").Elements("Var")
                            select t;

                foreach (var v in varsA)
                {
                    switch (v.Attribute("Name").Value.ToString())
                    {
                        case "X":
                            product.TabA.VarX = UnitConverter.GetValueFromString(v.Attribute("Value").Value);
                            Logger.GetLogger().Info("Produt X:" + product.TabA.VarX.ToString());
                            break;
                        case "Y":
                            product.TabA.VarY = UnitConverter.GetValueFromString(v.Attribute("Value").Value);
                            Logger.GetLogger().Info("Produt Y:" + product.TabA.VarY.ToString());
                            break;
                        case "Z":
                            product.TabA.VarZ = UnitConverter.GetValueFromString(v.Attribute("Value").Value);
                            Logger.GetLogger().Info("Produt Z:" + product.TabA.VarZ.ToString());
                            break;
                        case "Elevation":
                            product.TabA.VarElevation = UnitConverter.GetValueFromString(v.Attribute("Value").Value);
                            Logger.GetLogger().Info("Produt Elevation:" + product.TabA.VarElevation.ToString());
                            break;
                    }
                }
            }

            return product;
        }

        private static void getSubInfo(AKProduct product)
        {
            var mainA = product.UIVars.Find(it => it.Name == "MainA");
            if (mainA == null)
            {
                return;
            }

            mainA.Value = mainA.Value.Replace("_", "");//因为所有id都不能包含下划线

            var refPoint = product.UIVars.Find(it => it.Name == "RefPoint");
            var position = product.UIVars.Find(it => it.Name == "Position");
            var vx = product.UIVars.Find(it => it.Name == "vX");
            var vy = product.UIVars.Find(it => it.Name == "vY");
            var vz = product.UIVars.Find(it => it.Name == "vZ");

            SubInfo subinfo = new SubInfo();
            subinfo.MainA = mainA.Value;
            subinfo.Position = getPoint(position.Value);
            subinfo.RefPoint = getPoint(refPoint.Value);
            subinfo.VX = getVector(vx.Value);
            subinfo.VY = getVector(vy.Value);
            subinfo.VZ = getVector(vz.Value);

            product.SubInfo = subinfo;
        }

        private static Vector3d getVector(string array)
        {
            string[] ar = array.Split(',');
            double x = double.Parse(ar[0]);
            double y = double.Parse(ar[1]);
            double z = double.Parse(ar[2]);
            return new Vector3d(x, y, z);
        }

        private static Point3d getPoint(string array)
        {
            string[] ar = array.Split(',');
            double x = double.Parse(ar[0]);
            double y = double.Parse(ar[1]);
            double z = double.Parse(ar[2]);
            return new Point3d(x, y, z);
        }

        public string GetUIVarValue(string parameter)
        {
            return this.UIVars.Where(it => it.Name == parameter)
                                            .SingleOrDefault()
                                            .Value;
        }
    }

    public class SubInfo
    {
        public string MainA { get; set; }
        public Point3d RefPoint { get; set; }
        public Point3d Position { get; set; }
        public Vector3d VX { get; set; }
        public Vector3d VY { get; set; }
        public Vector3d VZ { get; set; }
    }

    public class TabInfo
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

    public class UIVar
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
