using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using Dapper;

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

            loadProjectInfo();

            loadSpecificationGroups();

            loadProducts();

            //this.ProjectInfo.JobName = (new DirectoryInfo(path)).Name;
        }

        private void loadProducts()
        {
            string connectStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + this.ProductListMDBPath;
            using (OleDbConnection conn = new OleDbConnection(connectStr))
            {
                conn.Open();

                this.Products = conn.Query<Product>
                    ("Select * from ProductList", null).ToList<Product>();
            }
        }

        /// <summary>
        /// 读取订单的规格组信息
        /// </summary>
        private void loadSpecificationGroups()
        {
            //string connectStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + this.MicrovellumProjectPath;
            string connectStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + this.MicrovellumProjectPath;
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
    }
}
