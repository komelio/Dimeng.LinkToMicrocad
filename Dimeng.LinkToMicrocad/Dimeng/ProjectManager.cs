using Dimeng.LinkToMicrocad.Logging;
using Dimeng.WoodEngine.Entities;
using SpreadsheetGear;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;

namespace Dimeng.LinkToMicrocad
{
    public class ProjectManager
    {
        public static Project CreateOrOpenProject(string projectPath)
        {
            if (!Directory.Exists(projectPath))
            {
                Directory.CreateDirectory(projectPath);
                Logging.Logger.GetLogger().Debug(string.Format("Directory {0} not found and is created", projectPath));
            }

            if (!ValidateProjectPath(projectPath))
            {
                //copy files from template
                Logger.GetLogger().Debug("Copy files from template folder");
                string templatePath = Context.GetContext().MVDataContext.GetLatestRelease().Template;
                IOHelper.CopyDirectory(templatePath, projectPath);

                //validate the mdb files and copy file from the resources if not existed
                validateAndCopyProjectMDBFiles(projectPath);
            }

            return GetProjectByPath(projectPath);
        }

        private static void validateAndCopyProjectMDBFiles(string projectPath)
        {
            string opmdb = Path.Combine(projectPath, "OverdrivePro.mdb");
            if (!File.Exists(opmdb))
            {
                using (FileStream fs = new FileStream(opmdb, FileMode.CreateNew, FileAccess.Write, FileShare.Write))
                {
                    fs.Write(Properties.Resources.OverdrivePro, 0, Properties.Resources.OverdrivePro.Length);
                }

                Logger.GetLogger().Info("Copy OverDrivePro.mdb from resources.");
            }

            string mvmdb = Path.Combine(projectPath, "MicrovellumProject.mdb");
            if (!File.Exists(mvmdb))
            {
                using (FileStream fs = new FileStream(mvmdb, FileMode.CreateNew, FileAccess.Write, FileShare.Write))
                {
                    fs.Write(Properties.Resources.MicrovellumProject, 0, Properties.Resources.MicrovellumProject.Length);
                }
                Logger.GetLogger().Info("Copy MicrovellumProject.mdb from resources.");
            }

            string productList = Path.Combine(projectPath, "ProductList.mdb");
            if (!File.Exists(productList))
            {
                using (FileStream fs = new FileStream(productList, FileMode.CreateNew, FileAccess.Write, FileShare.Write))
                {
                    fs.Write(Properties.Resources.ProductList, 0, Properties.Resources.ProductList.Length);
                }
                Logger.GetLogger().Info("Copy ProductList.mdb from resources.");
            }
        }

        public static bool ValidateProjectPath(string projectPath)
        {
            string opmdb = Path.Combine(projectPath, "OverdrivePro.mdb");
            if (!File.Exists(opmdb))
            {
                return false;
            }

            string mvmdb = Path.Combine(projectPath, "MicrovellumProject.mdb");
            if (!File.Exists(mvmdb))
            {
                return false;
            }

            string productList = Path.Combine(projectPath, "ProductList.mdb");
            if (!File.Exists(productList))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 获取Project
        /// 如果是空目录，则创建目录，拷贝相应的数据
        /// 如果是已有内容的目录，则根据目录获取
        /// </summary>
        /// <param name="path">工作任务的路径</param>
        /// <returns></returns>
        public static Project GetProjectByPath(string path)
        {
            var project = new Project();
            project.JobPath = path;

            return project;
        }

        private static void insertBlankProjectInfo(string connstr, string projectName)
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

        public static void AddProduct(string description, string comments, string dmId,
            SpecificationGroup sgroup,
            double width, double height, double depth,
            IWorkbook book, Project project)
        {
            Logger.GetLogger().Info("Add Product to current project");

            string connstr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + project.ProductListMDBPath;
            using (OleDbConnection conn = new OleDbConnection(connstr))
            {
                conn.Open();
                string cmdtext = string.Format("Insert Into ProductList (ItemNumber,Description,Qty,Width,Height,Depth,MatFile,FileName,Handle,ReleaseNumber,Parent1) Values('{0}','{1}',{2},{3},{4},{5},'{6}','{7}','{8}','{9}','{10}')",
                    "1.00",
                    description,
                    1,
                    width,
                    height,
                    depth,
                    sgroup.Name,
                    dmId + ".cutx",
                    dmId,
                    "UnNamed",
                    "Phase 1");
                var cmd = new OleDbCommand();
                cmd.Connection = conn;
                cmd.CommandText = cmdtext;
                cmd.ExecuteNonQuery();
            }

            var savepath = Path.Combine(project.JobPath, dmId + ".cutx");
            book.SaveAs(savepath, FileFormat.OpenXMLWorkbook);//因为是经过各种运算得到的L，所以是book来SaveAs，而不是FileCopy过来

            return;
        }
    }
}
