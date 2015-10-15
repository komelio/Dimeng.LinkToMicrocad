using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuoteExport
{
    public class ProductViewModel : ViewModelBase
    {
        public ProductViewModel()
        {
            ShowProductDataCommand = new RelayCommand(this.showData);
        }

        private string description;

        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                base.RaisePropertyChanged("Description");
            }
        }

        private int qty;

        public int Qty
        {
            get { return qty; }
            set
            {
                qty = value;
                base.RaisePropertyChanged("Qty");
            }
        }

        private double width;
        public double Width
        {
            get { return width; }
            set
            {
                width = value;
                base.RaisePropertyChanged("Width");
            }
        }

        private double height;
        public double Height
        {
            get { return height; }
            set
            {
                height = value;
                base.RaisePropertyChanged("Height");
            }
        }

        private double depth;

        public double Depth
        {
            get { return depth; }
            set
            {
                depth = value;
                base.RaisePropertyChanged("Depth");
            }
        }

        private void showData()
        {

        }

        public RelayCommand ShowProductDataCommand { get; private set; }
    }
}
