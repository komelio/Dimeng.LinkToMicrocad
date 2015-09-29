using System;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using Dimeng.LinkToMicrocad;
using Dimeng.LinkToMicrocad.Logging;
using System.Windows;

[assembly: CommandClass(typeof(Dimeng.Commands))]

namespace Dimeng
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
            Logger.GetLogger().Info("Call command 'New_dm'");

            try
            {
                var dmHelper = new DimengHelper();
                dmHelper.ShowPromptAndDrawBlock();
            }
            catch (System.Exception error)
            {
                Logger.GetLogger().Error(error);
                BugReportWindow bug = new BugReportWindow(new BugReportViewModel(error.Message));
                bug.ShowDialog();
            }

            //MessageBox.Show("new_dm");
        }

        /// <summary>
        /// Read temp.xml and then directly draw a autocad block dwg file.
        /// </summary>
        [CommandMethod("AK", "Edit_dm", CommandFlags.Modal)]
        public void EditBlock()
        {
            Logger.GetLogger().Info("Call command 'Edit_dm'");

            try
            {
                var dmHelper = new DimengHelper();
                dmHelper.ShowPromptAndDrawBlock();
            }
            catch (System.Exception error)
            {
                Logger.GetLogger().Error(error);
                BugReportWindow bug = new BugReportWindow(new BugReportViewModel(error.Message));
                bug.ShowDialog();
            }

            //MessageBox.Show("new_dm");
        }

        [CommandMethod("AK", "Del_dm", CommandFlags.Modal)]
        public void DelProduct()
        {
            Logger.GetLogger().Info("Call command 'Del_dm'");
            //MessageBox.Show("Del_dm");
            try
            {
                var dmHelper = new DimengHelper();
                dmHelper.DeleteProduct();
            }
            catch (System.Exception error)
            {
                Logger.GetLogger().Error(error);
                BugReportWindow bug = new BugReportWindow(new BugReportViewModel(error.Message));
                bug.ShowDialog();
            }
        }

        [CommandMethod("AK", "Copy_dm", CommandFlags.Modal)]
        public void CopyProduct()
        {
            Logger.GetLogger().Info("Call command 'Copy_dm'");
            //MessageBox.Show("Copy_dm");
            try
            {
                var dmHelper = new DimengHelper();
                dmHelper.CopyProduct();
            }
            catch (System.Exception error)
            {
                Logger.GetLogger().Error(error);
                BugReportWindow bug = new BugReportWindow(new BugReportViewModel(error.Message));
                bug.ShowDialog();
            }
        }
    }
}
