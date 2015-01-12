using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Dimeng.LinkToMicrocad.Web.Models
{
    public class MVProduct
    {
        [Required(ErrorMessage="Please enter your name")]
        public string Name { get; set; }
        [Required(ErrorMessage="Please enter a double value above 0")]
        public double DefaultWidth { get; set; }
        public double DefaultHeight { get; set; }
        public double DefaultDepth { get; set; }

        [Required(ErrorMessage="Make sure if it is enabled")]
        public bool? IsEnabled { get; set; }
    }
}