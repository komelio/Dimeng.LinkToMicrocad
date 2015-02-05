using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Dimeng.WoodEngine.Business
{
    /// <summary>
    /// Singleton
    /// </summary>
    public class ModelAssemblyLoader
    {
        private static readonly ModelAssemblyLoader instance = new ModelAssemblyLoader();
        public static ModelAssemblyLoader GetInstance()
        {
            return instance;
        }

        protected ModelAssemblyLoader()
        {
            this.Assembly = Assembly.Load("Dimeng.WoodEngine.Entities");
            this.Types = this.Assembly.GetTypes().ToList();
        }

        public Assembly Assembly { get; private set; }
        public List<Type> Types { get; private set; }
    }
}
