using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        [Key]
        public int MaterialId { get; set; }

        public string Name { get; set; }

        [ForeignKey("TextureId")]
        public Texture Texture { get; set; }

        public int? TextureId { get; set; }//可以为空
    }
}
