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
    }
}
