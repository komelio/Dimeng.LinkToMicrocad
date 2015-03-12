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
        public int Id { get; set; }
        [Required]
        public int Name { get; set; }
    }
}
