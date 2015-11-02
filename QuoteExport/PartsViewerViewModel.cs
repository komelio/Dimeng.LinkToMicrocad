using GalaSoft.MvvmLight;
using QuoteExport.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuoteExport
{
    public class PartsViewerViewModel : ViewModelBase
    {
        public PartsViewerViewModel(List<PauchiePart> parts)
        {
            this.Parts = parts;
        }

        public List<PauchiePart> Parts { get; private set; }
    }
}
