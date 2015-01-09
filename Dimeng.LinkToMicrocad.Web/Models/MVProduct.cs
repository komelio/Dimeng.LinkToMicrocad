using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dimeng.LinkToMicrocad.Web.Models
{
    public class MVProduct
    {
        public string Name { get; set; }
        public double DefaultWidth { get; set; }
        public double DefaultHeight { get; set; }
        public double DefaultDepth { get; set; }
        public bool? IsEnabled { get; set; }
    }
}