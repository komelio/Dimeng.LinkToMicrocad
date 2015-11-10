using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace QuoteExport.Entities
{
    public class ProjectExporter
    {
        string projectPath;
        public ProjectExporter(string projectPath)
        {
            this.projectPath = projectPath;
            this.Files = new List<string>();
        }

        public List<string> Files { get; private set; }

        public void GetFiles()
        {
            string projectdwg = Path.Combine(projectPath, "project_Temp.dwg");
            File.Copy(Path.Combine(projectPath, "project.dwg"), projectdwg, true);
            Files.Add(projectdwg);

            string dmsZip = Path.Combine(projectPath, "DMS.zip");
            if (File.Exists(dmsZip))
            {
                File.Delete(dmsZip);
            }
            using (ZipFile zipFile = new ZipFile())
            {
                zipFile.AlternateEncoding = Encoding.Default;//.UseUnicodeAsNecessary = true;
                zipFile.AlternateEncodingUsage = ZipOption.AsNecessary;
                zipFile.AddDirectory(Path.Combine(projectPath, "DMS"));
                zipFile.Save(dmsZip);
            }
            Files.Add(dmsZip);

            string pictures = Path.Combine(projectPath, "Pictures.zip");
            if (File.Exists(pictures))
            {
                File.Delete(pictures);
            }
            using (ZipFile zipFile = new ZipFile())
            {
                zipFile.AlternateEncoding = Encoding.Default;
                zipFile.AlternateEncodingUsage = ZipOption.AsNecessary;
                zipFile.AddDirectory(Path.Combine(projectPath, "images"));
                zipFile.AddDirectory(Path.Combine(projectPath, "panoramas"));
                zipFile.Save(pictures);
            }
            Files.Add(pictures);
        }
    }
}
