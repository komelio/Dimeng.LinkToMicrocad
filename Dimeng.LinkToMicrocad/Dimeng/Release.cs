using Dimeng.LinkToMicrocad.Logging;
using Dimeng.WoodEngine.Entities;
using Dimeng.WoodEngine.Entities.Machines.Tools;
using SpreadsheetGear;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Dimeng.LinkToMicrocad
{
    public class Release : IMVLibrary
    {
        public static Release GetRelease(string path)
        {
            return new Release(path);
        }

        private string folderPath;
        private Release(string path)
        {
            Logger.GetLogger().Debug("Init Release from directory:" + path);

            this.folderPath = path;

            //getReleaseNumber(path);

            this.Library = Path.Combine(folderPath, "Products");
            Logger.GetLogger().Debug(string.Format("Product Library:{0}", this.Library));
            this.MicrovellumData = folderPath;
            Logger.GetLogger().Debug(string.Format("MicrovellumData:{0}", this.MicrovellumData));
            this.Subassemblies = Path.Combine(folderPath, "Subassemblies");
            Logger.GetLogger().Debug(string.Format("Subassemblies:{0}", this.Subassemblies));
            this.Template = Path.Combine(folderPath, "Template");
            Logger.GetLogger().Debug(string.Format("Template:{0}", this.Template));
            this.Toolfiles = Path.Combine(folderPath, "Toolfiles");
            Logger.GetLogger().Debug(string.Format("Toolfiles:{0}", this.Toolfiles));

            loadToolfile();
        }

        private void loadToolfile()
        {
            var files = Directory.GetFiles(this.Toolfiles, "*.tlfx");
            if (files.Length == 0)
            {
                throw new Exception("Toolfiles number is 0!");
            }

            var toolfile = new ToolFile();

            IWorkbook wb = Factory.GetWorkbook(files[0]);
            IRange cells = wb.Worksheets[0].Cells;

            for (int i = 0; i < cells.Rows.RowCount; i++)
            {
                if (cells[i, 0].Text == string.Empty)
                    break;

                Tool tool = new Tool();
                tool.Description = cells[i, 0].Text;
                tool.ToolName = cells[i, 1].Text;
                tool.Diameter = Convert.ToDouble(cells[i, 2].Text);
                //tool.FaceNumber = int.Parse(cells[i, 3].Text);
                tool.ToolType = string.IsNullOrEmpty(cells[i, 8].Text) ? ToolType.Router : (ToolType)(int.Parse(cells[i, 8].Text));

                toolfile.Tools.Add(tool);
            }

            CurrentToolFile = toolfile;
        }

        private void getReleaseNumber(string path)
        {
            string folderName = path.Substring(path.LastIndexOf("\\") + 1);
            int index = folderName.IndexOf("_");
            if (index <= 0)
            {
                throw new Exception("Error when load Release information:" + folderName);
            }

            this.ReleaseNumber = folderName.Substring(index + 1);
            Logger.GetLogger().Debug(string.Format("ReleaseNumber:{0}", this.ReleaseNumber));
        }

        public string Library { get; private set; }
        public string MicrovellumData { get; private set; }
        public string Subassemblies { get; private set; }
        public string Template { get; private set; }
        public string Toolfiles { get; private set; }

        public string ReleaseNumber { get; private set; }

        public ToolFile CurrentToolFile { get; private set; }
    }
}
