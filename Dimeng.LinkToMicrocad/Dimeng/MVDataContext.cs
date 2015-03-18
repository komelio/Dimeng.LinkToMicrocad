using Dimeng.LinkToMicrocad.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Dimeng.LinkToMicrocad
{
    public class MVDataContext
    {
        /// <summary>
        /// Maintain Microvellum data information
        /// </summary>
        /// <returns></returns>
        public static MVDataContext GetContext(string catalogPath)
        {
            if (!Directory.Exists(catalogPath))
            {
                throw new DirectoryNotFoundException(string.Format("未找到路径{0}", catalogPath));
            }

            return new MVDataContext(catalogPath);
        }

        private string basePath;

        protected MVDataContext(string path)
        {
            this.basePath = path;

            initReleases();

            loadMaterialMappings();
        }

        private void initReleases()
        {
            Logging.Logger.GetLogger().Debug("Begin loading releases......");

            this.Releases = new List<Release>();

            Releases.Add(Release.GetRelease(this.basePath));
        }

        public List<Release> Releases { get; private set; }

        public Release GetLatestRelease()
        {
            //TODO:计算版本号大小，返回最新的版本
            return Releases[0];
        }

        public string GetProduct(string productId)
        {
            Logging.Logger.GetLogger().Debug("Searching the MV data for product:" + productId);

            try
            {
                if (this.Releases.Count == 0)
                {
                    throw new Exception("MV data not found!");
                }

                var release = this.Releases[0];//todo:找到对应的版本的数据

                string library = release.Library;
                string[] files = Directory.GetFiles(library, productId + ".cutx", SearchOption.AllDirectories);
                if (files.Length == 0)
                {
                    throw new Exception("Product data not found:" + productId);
                }

                return files[0];
            }
            catch (Exception error)
            {
                throw new Exception(string.Format("未找到产品[{0}]数据", productId), error);
            }
        }

        public Texture GetTexture(string materialName)
        {
            var texture = this.textures.Find(it => it.Material.ToUpper() == materialName.ToUpper());
            return texture;
        }

        private void loadMaterialMappings()
        {
            Logging.Logger.GetLogger().Debug("Start reading material-texture list...");

            textures.Clear();

            string xmlFile = Path.Combine(this.basePath, "materials.xml");
            Logger.GetLogger().Debug(xmlFile);

            XElement xml = XElement.Load(xmlFile);
            var materials = from e in xml.Elements("Material")
                            select e;

            foreach (var m in materials)
            {
                Logger.GetLogger().Debug(m.ToString());
                Texture texture = new Texture()
                {
                    ImageName = m.Attribute("Texture").Value,
                    Material = m.Attribute("Name").Value
                };

                textures.Add(texture);
            }

           
        }

        private List<Texture> textures = new List<Texture>();
    }
}
