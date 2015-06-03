using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Dimeng.LinkToMicrocad.Web.ViewModels
{
    public class ProductViewModel
    {
        public ProductViewModel()
        {
            Elevation = 0;
        }
        public int Id { get; set; }

        [Display(Name = "产品名称")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "产品分类")]
        [Required]
        public string Category { get; set; }

        [Display(Name = "默认离地高度")]
        public double Elevation { get; set; }

        [Display(Name = "CUTX文件")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "请上传CUTX文件")]
        public HttpPostedFileBase File { get; set; }

    }
}