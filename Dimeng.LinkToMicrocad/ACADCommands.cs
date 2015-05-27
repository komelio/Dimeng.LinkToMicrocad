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
using System.Windows;

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
            Logging.Logger.GetLogger().Info("Call command 'New_dm'");

            try
            {
                var dmHelper = new DimengHelper();
                dmHelper.ShowPromptAndDrawBlock();
            }
            catch (System.Exception error)
            {
                Logging.Logger.GetLogger().Error(error);
            }

            //MessageBox.Show("New_dm");
        }

        /// <summary>
        /// Read temp.xml and then directly draw a autocad block dwg file.
        /// </summary>
        [CommandMethod("AK", "Edit_dm", CommandFlags.Modal )]
        public void EditBlock()
        {
            Logging.Logger.GetLogger().Info("Call command 'Edit_dm'");

            //Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            //Editor ed = doc.Editor;

            //PromptPointOptions option = new PromptPointOptions("mm");
            //PromptPointResult ppr = ed.GetPoint(option);
            //if (ppr.Status == PromptStatus.OK)
            //    MessageBox.Show(ppr.Value.ToString());


            try
            {
                var dmHelper = new DimengHelper();
                dmHelper.ShowPromptAndDrawBlock();
            }
            catch (System.Exception error)
            {
                Logging.Logger.GetLogger().Error(error);
            }

          
            //MessageBox.Show("Edit_dm");
        }

        [CommandMethod("AK", "Del_dm", CommandFlags.Modal)]
        public void DelProduct()
        {
            Logging.Logger.GetLogger().Info("Call command 'Del_dm'");

            try
            {
                var dmHelper = new DimengHelper();
                dmHelper.DeleteProduct();
            }
            catch (System.Exception error)
            {
                Logging.Logger.GetLogger().Error(error);
            }
        }

        [CommandMethod("AK","showdebug",CommandFlags.Modal)]
        public void Showdebug()
        {
            Dimeng.LinkToMicrocad.Logging.ConsoleHelper.Show();
        }
    }

}
