using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dimeng.LinkToMicrocad.Web.Domain.Entities
{
    /// <summary>
    /// 对应AutoDecco的材质
    /// </summary>
    public class Texture
    {
        [Key]
        public int TextureId { get; set; }

        [Required(ErrorMessage = "Please enter a description")]
        public string Name { get; set; }
        

        public string ImageName { get; set; }
    }
}
