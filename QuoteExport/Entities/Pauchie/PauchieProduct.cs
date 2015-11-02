using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuoteExport.Entities
{
    public class PauchieProduct
    {
        public PauchieProduct()
        {
            BomId = Guid.NewGuid().ToString();
            Hardwares = new List<PauchieHardware>();
            Parts = new List<PauchiePart>();
            IsExport = true;

            ShowPartsCommand = new RelayCommand(this.showParts,
                () =>
                {
                    if (this.PartsCounter == 0)
                    {
                        return false;
                    }
                    else return true;
                });

            ShowHardwaresCommand = new RelayCommand(this.showHwrs,
                () =>
                {
                    if (this.HardwaresCounter == 0)
                    {
                        return false;
                    }
                    else return true;
                });
        }

        void showHwrs()
        {
            HardwaresViewer hv = new HardwaresViewer(this.Hardwares);
            hv.ShowDialog();
        }

        void showParts()
        {
            PartsViewer pv = new PartsViewer(this.Parts);
            pv.ShowDialog();
        }

        public string OrderNumber { get; set; }
        public int LineNumber { get; set; }
        public int Qty { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public List<PauchiePart> Parts { get; private set; }
        public List<PauchieHardware> Hardwares { get; private set; }
        public List<PauchieDrawerSubassembly> Drawers { get; private set; }
        public string Color { get; set; }
        public string BomId { get; set; }
        public string ItmId { get; set; }

        public bool IsExport { get; set; }
        public bool IsDataMatch { get; set; }

        public double Width { get; set; }//实际的宽度，下同
        public double Height { get; set; }
        public double Depth { get; set; }
        public int PartsCounter { get; set; }
        public int HardwaresCounter { get; set; }

        public RelayCommand ShowPartsCommand { get; private set; }
        public RelayCommand ShowHardwaresCommand { get; private set; }
    }
}
