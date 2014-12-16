// (C) Copyright 2014 by  
//
using System;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using Microsoft.Win32;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.Linq;

// This line is not mandatory, but improves loading performances
[assembly: CommandClass(typeof(Dimeng.LinkToMicrocad.Commands))]

namespace Dimeng.LinkToMicrocad
{
    public class Commands
    {
        /// <summary>
        /// Read temp.xml and popup a prompt window for product options.
        /// Then draw a autocad block dwg file.
        /// </summary>
        [CommandMethod("AK", "New_dm", CommandFlags.Modal)]
        public void NewBlock()
        {
            var dmHelper = new DimengHelper();
            dmHelper.ShowPromptAndDrawBlock();
        }

        /// <summary>
        /// Read temp.xml and then directly draw a autocad block dwg file.
        /// </summary>
        [CommandMethod("AK", "Edit_dm", CommandFlags.Modal)]
        public void EditBlock()
        {
            var dmHelper = new DimengHelper();
            dmHelper.EditAndDrawBlock();
        }

        [CommandMethod("TestEnvironment", CommandFlags.Modal)]
        public void TestEnvironment()
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                RegistryKey hkml = Registry.LocalMachine;
                RegistryKey akr14 = hkml.OpenSubKey("SOFTWARE\\Microcad\\autokitchen\\R14", true);

                sb.AppendLine(akr14.GetValue("").ToString());
                sb.AppendLine(akr14.GetValue("Country").ToString());
                sb.AppendLine(akr14.GetValue("MeterToUnit").ToString());
                sb.AppendLine(akr14.GetValue("Product").ToString());
                sb.AppendLine(akr14.GetValue("SerialNumber").ToString());
                sb.AppendLine(akr14.GetValue("T").ToString());

                System.Windows.MessageBox.Show(sb.ToString());

            }
            catch (System.Exception error)
            {
                System.Windows.MessageBox.Show(error.Message);
            }
        }

        [CommandMethod("TestCreateTempDWG", CommandFlags.Modal)]
        public void TestDWG()
        {
            try
            {
                string a14Path = @"c:\microcad software\autodecco_studio 11";
                //RegistryKey hkml = Registry.LocalMachine;
                //RegistryKey akr14 = hkml.OpenSubKey("SOFTWARE\\Microcad\\autokitchen\\R14", true);
                //a14Path = akr14.GetValue("").ToString();

                string folderPath = Path.Combine(a14Path, "temp");
                folderPath = Path.Combine(folderPath, "dms");

                if (!Directory.Exists(folderPath))
                {
                    System.Windows.MessageBox.Show("Directory Not Found:" + folderPath);
                    return;
                }

                string tempxml = Path.Combine(folderPath, "temp.xml");
                if (File.Exists(tempxml))
                {
                    System.Windows.MessageBox.Show("File Not Found:" + tempxml);
                    return;
                }

                double width = 0;
                double height = 0;
                double depth = 0;

                var doc = XElement.Load(tempxml).Elements("Tab").Elements("Var");
                foreach (var x in doc)
                {
                    if (x.Attribute("Name").Value == "X")
                    {
                        string text = x.Attribute("Value").Value;
                        width = parseDouble(text);
                    }
                    if (x.Attribute("Name").Value == "Y")
                    {
                        string text = x.Attribute("Value").Value;
                        width = parseDouble(text);
                    }
                    if (x.Attribute("Name").Value == "Z")
                    {
                        string text = x.Attribute("Value").Value;
                        width = parseDouble(text);
                    }
                }

            }
            catch (System.Exception error)
            {
                System.Windows.MessageBox.Show(error.Message);
            }
        }

        private double parseDouble(string text)
        {
            return 0;

            if (text.EndsWith("mm"))
            {

            }
        }
    }

}
