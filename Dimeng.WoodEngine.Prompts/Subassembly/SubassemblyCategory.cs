using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Prompts
{
    public class SubassemblyCategory
    {
        public SubassemblyCategory(DirectoryInfo di)
        {
            Subassemblies = new List<SubassemblyViewModel>();

            Name = di.Name;

            LoadFiles(di);
        }

        private void LoadFiles(DirectoryInfo di)
        {
            var files = di.GetFiles("*.cutx");
            foreach(var fi in files)
            {
                Subassemblies.Add(new SubassemblyViewModel(fi));
            }
        }
        public List<SubassemblyViewModel> Subassemblies { get; set; }
        public string Name { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }
}
