using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ADLauncher
{
    public class UpdateItem
    {
        public string Path { get; set; }
        public string MD5 { get; set; }

        public string GetFolder()
        {
            if (string.IsNullOrEmpty(Path))
            {
                return string.Empty;
            }

            if (Path.IndexOf("/") > -1)
            {
                return Path.Substring(0, Path.LastIndexOf("/") + 1);
            }

            return Path;
        }
    }
}
