using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuoteExport.Dimensions
{
    public class Part
    {
        public Part()
        {
            Vdrillings = new List<VdrillSeq>();
            Hdrillings = new List<HdrillSeq>();
            Routes = new List<RouteSeq>();
        }
        public BorderSeq Border { get; set; }
        public List<VdrillSeq> Vdrillings { get; private set; }
        public List<HdrillSeq> Hdrillings { get; private set; }
        public List<RouteSeq> Routes { get; private set; }

        private string filename;
        public string FileName
        {
            get
            { return filename; }
            set
            {

            }
        }
    }
}
