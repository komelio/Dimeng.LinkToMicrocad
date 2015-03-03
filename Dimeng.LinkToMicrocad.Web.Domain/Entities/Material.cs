using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dimeng.LinkToMicrocad.Web.Domain.Entities
{
    /// <summary>
    /// 对应MV的材料
    /// </summary>
    public class Material
    {
        public string Name { get; set; }
        
        /// <summary>
        /// 贴图
        /// </summary>
        public Texture Texture { get; set; }
    }
}
