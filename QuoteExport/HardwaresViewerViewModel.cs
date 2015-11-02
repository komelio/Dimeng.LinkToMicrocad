using QuoteExport.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuoteExport
{
    public class HardwaresViewerViewModel
    {
        public HardwaresViewerViewModel(List<PauchieHardware> hwrs)
        {
            this.Hardwares = hwrs;
        }

        public List<PauchieHardware> Hardwares { get; private set; }
    }
}
