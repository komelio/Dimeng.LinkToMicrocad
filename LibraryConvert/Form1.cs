using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace LibraryConvert
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            tbVersionNumber.Text = DateTime.Now.ToString("yyyy.MM.dd.001");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string finalPath = @"C:\Users\xspxs_000\Desktop\output\"+tbVersionNumber.Text;
            if(!Directory.Exists(finalPath))
            {
                Directory.CreateDirectory(finalPath);
            }

            string path1 = @"C:\Users\xspxs_000\Desktop\提供给AD的模型库";
            DirectoryInfo di = new DirectoryInfo(path1);
            var dis = di.GetDirectories();

            List<Category> categories = new List<Category>();
            foreach (var d in dis)
            {
                Category pc = new Category();
                pc.Name = d.Name;
                categories.Add(pc);

                foreach (var dc in d.GetDirectories())
                {
                    Category c = new Category();
                    c.Name = dc.Name;
                    pc.Categories.Add(c);

                    foreach (var f in dc.GetFiles("*.cutx", SearchOption.TopDirectoryOnly))
                    {
                        string filename = f.Name.Replace(".cutx", "");
                        Item item = new Item();

                        if (d.Name.StartsWith("0"))
                        {
                            item.Name = filename.Substring(0, filename.Length - 5);
                            item.Id = filename.Substring(filename.Length - 5);
                        }
                        else
                        {
                            item.Name = filename.Substring(0, filename.Length - 6);
                            item.Id = filename.Substring(filename.Length - 6);
                        }
                        c.Items.Add(item);

                        File.Copy(f.FullName, Path.Combine(finalPath, "Library", "Products", item.Id + ".cutx"), true);

                        string pic = Path.Combine(f.Directory.ToString(), filename + ".jpg");
                        if (File.Exists(pic))
                        {
                            File.Copy(pic, Path.Combine(finalPath, "Photos", "dmsobj", item.Id + ".jpg"), true);
                        }
                    }
                }
            }

            string subPath = @"C:\Users\xspxs_000\Desktop\组件";
            DirectoryInfo di2 = new DirectoryInfo(subPath);
            var dis2 = di2.GetDirectories();

            List<Category> subCategories = new List<Category>();
            int counter = 0;
            foreach (var d in dis2)
            {
                Category ct = new Category();
                ct.Name = d.Name;
                foreach (var f in d.GetFiles("*.cutx"))
                {
                    counter++;
                    string filename = f.Name.Replace(".cutx", "");

                    Item item = new Item();
                    item.Id = counter.ToString();
                    item.Name = filename;

                    ct.Items.Add(item);

                    File.Copy(f.FullName, Path.Combine(finalPath, "Library", "Products", item.Id + ".cutx"), true);

                    string pic = Path.Combine(f.Directory.ToString(), filename + ".jpg");
                    if (File.Exists(pic))
                    {
                        File.Copy(pic, Path.Combine(finalPath, "Photos", "dmsobj", item.Id + ".jpg"), true);
                    }
                }

                subCategories.Add(ct);
            }

            XDocument doc = new XDocument();
            XElement root = new XElement("Root");
            XElement dimengCategory = new XElement("Category",
                                                new XAttribute("K", "kDiMengObj"),
                                                new XAttribute("Name", "DiMengObj"),
                                                new XAttribute("Levels", "3"));
            foreach (var c in categories)
            {
                var category = new XElement("I",
                                   new XAttribute("Name", c.Name),
                                   new XAttribute("ToolTip", c.Name),
                                   new XAttribute("Photo", c.Name)
                                   );
                foreach (var dc in c.Categories)
                {
                    var category2 = new XElement("I",
                                   new XAttribute("Name", dc.Name),
                                   new XAttribute("ToolTip", dc.Name),
                                   new XAttribute("Photo", dc.Name)
                                   );
                    foreach (var p in dc.Items)
                    {
                        if (c.Categories.IndexOf(dc) == 0 && dc.Items.IndexOf(p) == 0)
                        {
                            category.Attribute("Photo").Value = p.Id.ToString();
                        }

                        double elevation = (p.Name.IndexOf("吊") > -1) ? 2200 : 0;
                        var productXml = new XElement("I",
                                            new XAttribute("Name", p.Name),
                                            new XAttribute("Photo", p.Id),
                                            new XAttribute("DWG", p.Id),
                                            new XAttribute("DMID", ""),
                                            new XAttribute("Units", "mm"),
                                            new XAttribute("Elevation", elevation),
                                            new XAttribute("Z", 0),
                                            new XAttribute("Y", 0),
                                            new XAttribute("X", 0),
                                            new XAttribute("LongText", string.Empty),
                                            new XAttribute("ID", p.Id)
                                            );

                        category2.Add(productXml);
                    }
                    category.Add(category2);
                }

                dimengCategory.Add(category);
            }
            var subCategory = new XElement("I",
                                   new XAttribute("Name", "组件库"),
                                   new XAttribute("ToolTip", "组件库"),
                                   new XAttribute("Photo", "组件库")
                                   );
            foreach (var sc in subCategories)
            {
                var category2 = new XElement("I",
                                   new XAttribute("Name", sc.Name),
                                   new XAttribute("ToolTip", sc.Name),
                                   new XAttribute("Photo", sc.Name)
                               );
                foreach (var p in sc.Items)
                {
                    var productXml = new XElement("I",
                                        new XAttribute("SubA", 1),
                                        new XAttribute("Name", p.Name),
                                        new XAttribute("Photo", p.Id),
                                        new XAttribute("DWG", p.Id),
                                        new XAttribute("DMID", ""),
                                        new XAttribute("Units", "mm"),
                                        new XAttribute("Elevation", 0),
                                        new XAttribute("Z", 0),
                                        new XAttribute("Y", 0),
                                        new XAttribute("X", 0),
                                        new XAttribute("LongText", string.Empty),
                                        new XAttribute("ID", p.Id)
                                        );

                    category2.Add(productXml);
                }
                subCategory.Add(category2);
            }
            dimengCategory.Add(subCategory);

            root.Add(dimengCategory);
            doc.Add(root);

            doc.Save(Path.Combine(finalPath, "Dms.xml"));



            generateList(finalPath);
        }

        private void generateList(string finalPath)
        {
            string filePath = Path.Combine(finalPath, "list.txt");

            FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Write);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);

            DirectoryInfo di = new DirectoryInfo(finalPath);
            Uri baseUri = new Uri(finalPath);

            foreach(FileInfo fi in di.GetFiles("*.*",SearchOption.AllDirectories))
            {
                if(fi.Name=="list.txt")
                { continue; }

                Uri targetUri = new Uri(fi.FullName);
                string p = baseUri.MakeRelativeUri(targetUri).ToString().Replace("%20"," ");
                string md5code = string.Empty;

                MD5 md5 = MD5.Create();
                using (var stream = File.OpenRead(fi.FullName))
                {
                    md5code = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToUpper();
                }

                sw.WriteLine(string.Format("{0}|{1}", p, md5code));
            }

            sw.Close();
            fs.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string finalPath = @"C:\Users\xspxs_000\Desktop\output";
            DirectoryInfo dc = new DirectoryInfo(@"C:\Users\xspxs_000\Desktop\ad\Products");
            foreach (var f in dc.GetFiles("*.cutx", SearchOption.AllDirectories))
            {
                string filename = f.Name.Replace(".cutx", "");
                Item item = new Item();

                if (f.Directory.Name.StartsWith("0"))
                {
                    item.Name = filename.Substring(0, filename.Length - 5);
                    item.Id = filename.Substring(filename.Length - 5);
                }
                else
                {
                    item.Name = filename.Substring(0, filename.Length - 6);
                    item.Id = filename.Substring(filename.Length - 6);
                }

                File.Copy(f.FullName, Path.Combine(finalPath, "Library", "Products", item.Id + ".cutx"), true);

                //string pic = Path.Combine(f.Directory.ToString(), filename + ".jpg");
                //if (File.Exists(pic))
                //{
                //    File.Copy(pic, Path.Combine(finalPath, "Photos", "dmsobj", item.Id + ".jpg"), true);
                //}
            }
        }
    }

    public class Category
    {
        public string Name { get; set; }
        public List<Item> Items = new List<Item>();
        public List<Category> Categories = new List<Category>();
    }

    public class Item
    {
        public string Name { get; set; }
        public string Id { get; set; }
    }
}
