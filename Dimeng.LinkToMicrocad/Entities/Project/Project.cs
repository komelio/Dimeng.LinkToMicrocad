using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using Dapper;
using SpreadsheetGear;
using Dimeng.LinkToMicrocad;
using Autodesk.AutoCAD.Geometry;

namespace Dimeng.WoodEngine.Entities
{
    public class Project
    {
        private string jobPath;
        public string JobPath
        {
            get { return jobPath; }
            set
            {
                jobPath = value;
                init(value);
            }
        }

        private void init(string path)
        {
            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException("未找到文件夹" + path);
            }

            ManufacturingDataPath = Path.Combine(path, "Manufacturing Data");
            ProductListMDBPath = Path.Combine(path, "ProductList.mdb");
            OverdriveProMDBPath = Path.Combine(path, "OverdrivePro.mdb");
            MicrovellumProjectPath = Path.Combine(path, "MicrovellumProject.mdb");
            SubassembliesPath = Path.Combine(path, "Subassemblies");

            checkPaths();

            loadProjectInfo();

            loadSpecificationGroups();

            loadProducts();

            loadCounter();

            //this.ProjectInfo.JobName = (new DirectoryInfo(path)).Name;
        }

        private void loadCounter()
        {
            Counter = 0;

            string path = Path.Combine(this.jobPath, "counter.txt");
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (StreamReader sr = new StreamReader(fs, Encoding.Default))
                {
                    string value = sr.ReadLine();
                    if (String.IsNullOrEmpty(value))
                    {
                        return;
                    }

                    int v;
                    if (int.TryParse(value, out v))
                    {
                        Counter = v;
                    }
                    else
                    {
                        return;
                    }
                }
            }
        }
        private int addCounter()
        {
            Counter++;
            string path = Path.Combine(this.jobPath, "counter.txt");

            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
            using (StreamWriter sw = new StreamWriter(fs, Encoding.Default))
            {
                sw.WriteLine(Counter.ToString());
            }

            return Counter;
        }


        private void checkPaths()
        {
            if (!Directory.Exists(this.SubassembliesPath))
            {
                Directory.CreateDirectory(this.SubassembliesPath);
            }
        }

        private void loadProducts()
        {
            string connectStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + this.ProductListMDBPath;
            //string connectStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + this.ProductListMDBPath;
            using (OleDbConnection conn = new OleDbConnection(connectStr))
            {
                conn.Open();

                this.Products = conn.Query<Product>
                    ("Select * from ProductList", null).ToList<Product>();

                this.Products.ForEach(it => it.Project = this);

                if (this.Products == null)
                {
                    this.Products = new List<Product>();
                }
            }
        }

        /// <summary>
        /// 读取订单的规格组信息
        /// </summary>
        private void loadSpecificationGroups()
        {
            string connectStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + this.MicrovellumProjectPath;
            //string connectStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + this.MicrovellumProjectPath;
            using (OleDbConnection conn = new OleDbConnection(connectStr))
            {
                conn.Open();

                this.SpecificationGroups = conn.Query<SpecificationGroup>
                    ("Select * from SpecificationGroups", null).ToList<SpecificationGroup>();
            }
        }

        /// <summary>
        /// 读取OverDrivePro.mdb数据库中的数据
        /// </summary>
        private void loadProjectInfo()
        {
            string connectStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + this.OverdriveProMDBPath;
            //string connectStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + this.OverdriveProMDBPath;

            using (OleDbConnection conn = new OleDbConnection(connectStr))
            {
                conn.Open();

                var info = conn.Query<ProjectInfo>("select * from Jobs", null)
                                           .SingleOrDefault();

                //Products.ForEach(a => a.Job = job);//设定每个产品的工作任务

                if (info == null)
                {
                    this.insertBlankProjectInfo(connectStr, (new DirectoryInfo(this.jobPath)).Name);//插入一个空的记录
                    this.ProjectInfo = new ProjectInfo();
                }
            }
        }

        private void insertBlankProjectInfo(string connstr, string projectName)
        {
            using (OleDbConnection conn = new OleDbConnection(connstr))
            {
                conn.Open();
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = conn;
                cmd.CommandText = string.Format("Insert into Jobs (JobName) values ('{0}')", projectName);
                cmd.ExecuteNonQuery();
            }
        }

        public string ManufacturingDataPath { get; private set; }
        public string ProductListMDBPath { get; private set; }
        public string OverdriveProMDBPath { get; private set; }
        public string MicrovellumProjectPath { get; private set; }
        public string SubassembliesPath { get; private set; }
        public List<SpecificationGroup> SpecificationGroups { get; private set; }
        public ProjectInfo ProjectInfo { get; private set; }
        public int Counter { get; private set; }

        public override string ToString()
        {
            return ProjectInfo.JobName;
        }

        public List<Product> Products { get; set; }

        /// <summary>
        /// Judge if the project has the product by handle(DMID)
        /// </summary>
        /// <param name="id">DMID from temp.xml</param>
        /// <returns></returns>
        public bool HasProduct(string id)
        {
            if (this.Products == null || this.Products.Count == 0)
            { return false; }

            return this.Products.Any(it => it.Handle.ToUpper() == id.ToUpper());
        }

        public Product AddProduct(string name, string id, double width, double height, double depth, string libraryProductPath)
        {
            int counter = addCounter();

            string filename = counter + ".cutx";

            string connstr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + this.ProductListMDBPath;
            using (OleDbConnection conn = new OleDbConnection(connstr))
            {
                conn.Open();
                string cmdtext = string.Format("Insert Into ProductList (ItemNumber,Description,Qty,Width,Height,Depth,MatFile,FileName,Handle,ReleaseNumber,Parent1) Values('{0}','{1}',{2},{3},{4},{5},'{6}','{7}','{8}','{9}','{10}')",
                    string.Format("{0}.00", counter),
                    name,
                    1,
                    width,
                    height,
                    depth,
                    this.SpecificationGroups[0].Name,
                    filename,
                    id,
                    "UnNamed",
                    "Phase 1");
                var cmd = new OleDbCommand();
                cmd.Connection = conn;
                cmd.CommandText = cmdtext;
                cmd.ExecuteNonQuery();

                Product product = conn.Query<Product>("Select * from ProductList Where Handle='" + id + "'")
                                      .SingleOrDefault();
                product.Project = this;
                this.Products.Add(product);

                IWorkbook book = Factory.GetWorkbook(libraryProductPath);
                book.SaveAs(Path.Combine(JobPath, filename), FileFormat.OpenXMLWorkbook);

                return product;
            }
        }

        public string AddSubToProduct(Product product, AKProduct aksub, string libraryPath, ref int line)
        {
            string[] files = Directory.GetFileSystemEntries(libraryPath, aksub.TabA.ID + ".cutx", SearchOption.AllDirectories);
            if (files.Length == 0)
            {
                throw new Exception("Subassembly not found:" + aksub.TabA.Name);
            }

            var book = Factory.GetWorkbook(product.GetProductCutxFileName());
            var cells = book.Worksheets["Subassemblies"].Cells;
            for (int i = 0; i < cells.Rows.RowCount; i++)
            {
                if (string.IsNullOrEmpty(cells[i, 16].Text.Trim()))
                {
                    cells[i, 16].Value = aksub.TabA.Name;
                    cells[i, 17].Value = 1;
                    cells[i, 18].Value = aksub.TabA.VarX;
                    cells[i, 19].Value = aksub.TabA.VarZ;
                    cells[i, 20].Value = aksub.TabA.VarY;

                    Point3d ptRef = new Point3d(aksub.SubInfo.RefPoint.X, aksub.SubInfo.RefPoint.Y, aksub.SubInfo.RefPoint.Z);
                    Point3d ptPos = new Point3d(aksub.SubInfo.Position.X, aksub.SubInfo.Position.Y, aksub.SubInfo.Position.Z);

                    Vector3d vector = ptRef - ptPos;

                    var vt = vector.TransformBy(Matrix3d.AlignCoordinateSystem(
                        ptRef,
                        aksub.SubInfo.VX,
                        aksub.SubInfo.VY,
                        aksub.SubInfo.VZ,
                        Point3d.Origin,
                        Vector3d.XAxis,
                        Vector3d.YAxis,
                        Vector3d.ZAxis
                        ));


                    cells[i, 29].Value = vt.X;
                    cells[i, 30].Value = vt.Y;
                    cells[i, 31].Value = vt.Z - aksub.Tab.VarElevation;
                    book.Save();

                    line = i;

                    string path = Path.Combine(this.jobPath, "Subassemblies", string.Format("{0}_({1}){2}.cutx", product.Handle, aksub.TabA.Name, i + 1));

                    File.Copy(files[0], path);

                    //拷贝后，把长宽高输入进去
                    var bookS = Factory.GetWorkbook(path);
                    var cellsS = bookS.Worksheets["Prompts"].Cells;
                    cellsS[0, 1].Value = aksub.TabA.VarX;
                    cellsS[1, 1].Value = aksub.TabA.VarZ;
                    cellsS[2, 1].Value = aksub.TabA.VarY;
                    bookS.Save();

                    return path;
                }
            }

            throw new Exception("Add subassembly to product error!");
        }

        public void DeleteProduct(string productId)
        {
            string connstr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + this.ProductListMDBPath;
            using (OleDbConnection conn = new OleDbConnection(connstr))
            {
                conn.Open();
                string cmdtext = string.Format("Delete From ProductList Where Handle='{0}'", productId);
                var cmd = new OleDbCommand();
                cmd.Connection = conn;
                cmd.CommandText = cmdtext;
                cmd.ExecuteNonQuery();
            }

            var p = this.Products.Find(it => it.Handle == productId);
            if (p != null)
            {
                this.Products.Remove(p);
            }
        }

        public void UpdateProduct(Product product)
        {
            string connstr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + this.ProductListMDBPath;
            using (OleDbConnection conn = new OleDbConnection(connstr))
            {
                conn.Open();
                string query = "Width='{1}',Height='{2}',Depth='{3}'";
                if (product.Comments != null && product.Comments.Length > 0)
                {
                    query = query + ",Comments='" + product.Comments + "'";
                }
                string cmdtext = string.Format("Update ProductList Set " + query + " Where Handle='{0}'", product.Handle, product.Width, product.Height, product.Depth);
                var cmd = new OleDbCommand();
                cmd.Connection = conn;
                cmd.CommandText = cmdtext;
                cmd.ExecuteNonQuery();
            }
        }

        internal void CopyProduct(string from, string to)
        {
            string connstr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + this.ProductListMDBPath;
            using (OleDbConnection conn = new OleDbConnection(connstr))
            {
                conn.Open();

                Product product = conn.Query<Product>("Select * from ProductList Where Handle='" + from + "'")
                                      .SingleOrDefault();
                product.Project = this;

                int counter = addCounter();
                //拷贝产品
                File.Copy(Path.Combine(this.jobPath, product.FileName),
                    Path.Combine(this.jobPath, counter + ".cutx"));
                //拷贝组件
                DirectoryInfo di = new DirectoryInfo(this.SubassembliesPath);
                foreach (var file in di.GetFiles(from + "*"))
                {
                    int index = file.Name.IndexOf('_');
                    string backName = file.Name.Substring(index+1);
                    string newName = Path.Combine(this.SubassembliesPath, string.Format("{0}_{1}", to, backName));
                    file.CopyTo(newName, false);
                }

                string cmdtext = string.Format("Insert Into ProductList (ItemNumber,Description,Qty,Width,Height,Depth,MatFile,FileName,Handle,ReleaseNumber,Parent1) Values('{0}','{1}',{2},{3},{4},{5},'{6}','{7}','{8}','{9}','{10}')",
                    string.Format("{0}.00", counter),
                    product.Description,
                    1,
                    product.Width,
                    product.Height,
                    product.Depth,
                    this.SpecificationGroups[0].Name,
                    counter + ".cutx",
                    to,
                    "UnNamed",
                    "Phase 1");
                var cmd = new OleDbCommand();
                cmd.Connection = conn;
                cmd.CommandText = cmdtext;
                cmd.ExecuteNonQuery();
            }
        }
    }
}
