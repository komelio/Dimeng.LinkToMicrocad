using SpreadsheetGear;
using SpreadsheetGear.Windows.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Prompts
{
    public class SubassemblyViewModel
    {
        public SubassemblyViewModel(FileInfo fi)
        {
            Name = fi.Name.Replace(fi.Extension, "");
            CutxPath = fi.FullName;

            var jpgs = fi.Directory.GetFiles(Name + ".jpg", SearchOption.AllDirectories);
            if (jpgs.Length > 0)
            {
                JPGPath = jpgs[0].FullName;
            }

            var wmfs = fi.Directory.GetFiles(Name + ".jpg", SearchOption.AllDirectories);
            if (wmfs.Length > 0)
            {
                WMFPath = wmfs[0].FullName;
            }
        }

        public string Name { get; set; }
        public string CutxPath { get; set; }
        public string JPGPath { get; set; }
        public string WMFPath { get; set; }
    }
}
