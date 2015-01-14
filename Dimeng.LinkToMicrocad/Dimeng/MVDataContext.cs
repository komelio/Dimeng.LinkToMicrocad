using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Dimeng.LinkToMicrocad
{
    public class MVDataContext
    {
        /// <summary>
        /// Maintain Microvellum data information
        /// </summary>
        /// <returns></returns>
        public static MVDataContext GetContext(string microcadProductPath)
        {
            string path = Path.Combine(microcadProductPath, "Dimeng");

            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException(string.Format("未找到路径{0}", path));
            }

            return new MVDataContext(path);
        }

        private string basePath;
        private string releasesPath;

        protected MVDataContext(string path)
        {
            this.basePath = path;

            init();
        }

        private void init()
        {
            initReleases();
        }

        private void initReleases()
        {
            Logging.Logger.GetLogger().Debug("Begin loading releases......");

            releasesPath = Path.Combine(basePath, "Releases");

            this.Releases = new List<Release>();
            DirectoryInfo di = new DirectoryInfo(releasesPath);
            foreach (var d in di.GetDirectories("Release_*"))
            {
                Releases.Add(Release.GetRelease(d.FullName));
            }
        }

        public List<Release> Releases { get; private set; }

        public Release GetLatestRelease()
        {
            //TODO:计算版本号大小，返回最新的版本
            return Releases[0];
        }

        public string GetProduct(string productName)
        {
            Logging.Logger.GetLogger().Debug("Searching the MV data for product:" + productName);

            try
            {
                if (this.Releases.Count == 0)
                {
                    throw new Exception("MV data not found!");
                }

                var release = this.Releases[0];//todo:找到对应的版本的数据

                string library = release.Library;
                string[] files = Directory.GetFiles(library, productName + ".cutx", SearchOption.AllDirectories);
                if (files.Length == 0)
                {
                    throw new Exception("Product data not found:" + productName);
                }

                return files[0];
            }
            catch (Exception error)
            {
                throw new Exception(string.Format("未找到产品[{0}]数据", productName), error);
            }
        }
    }
}
