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
        public int Id { get; set; }
        public string Name { get; set; }

        [ForeignKey("Texture")]
        public int TextureId { get; set; }
        public Texture Texture { get; set; }
    }
}
