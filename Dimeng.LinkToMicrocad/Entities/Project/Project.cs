using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using Dapper;
using SpreadsheetGear;

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

            //this.ProjectInfo.JobName = (new DirectoryInfo(path)).Name;
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
            string filename = id + ".cutx";

            string connstr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + this.ProductListMDBPath;
            using (OleDbConnection conn = new OleDbConnection(connstr))
            {
                conn.Open();
                string cmdtext = string.Format("Insert Into ProductList (ItemNumber,Description,Qty,Width,Height,Depth,MatFile,FileName,Handle,ReleaseNumber,Parent1) Values('{0}','{1}',{2},{3},{4},{5},'{6}','{7}','{8}','{9}','{10}')",
                    string.Format("{0}.00", Products.Count + 1),
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

                ////add product cutx file to project
                IWorkbook book = Factory.GetWorkbook(libraryProductPath);
                //var cells = book.Worksheets["Prompts"].Cells;
                //cells[0, 1].Value = width;
                //cells[1, 1].Value = height;
                //cells[2, 1].Value = depth;
                book.SaveAs(Path.Combine(JobPath, filename), FileFormat.OpenXMLWorkbook);

                return product;
            }
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
        }

        public void UpdateProduct(Product product)
        {
            string connstr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + this.ProductListMDBPath;
            using (OleDbConnection conn = new OleDbConnection(connstr))
            {
                conn.Open();
                string query = "Width='{1}',Height='{2}',Depth='{3}'";
                if(product.Comments!=null && product.Comments.Length>0)
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
    }
}
