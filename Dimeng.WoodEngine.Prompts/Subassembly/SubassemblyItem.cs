using Dimeng.WoodEngine.CommonSpreadControls;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Prompts
{
    public class SubassemblyItem : ViewModelBase
    {
        private bool isUnique = false;

        public bool IsUnique
        {
            get { return isUnique; }
            set
            {
                isUnique = value;
                base.RaisePropertyChanged("IsUnique");
            }
        }

        private PropertyElement name;

        public PropertyElement Name
        {
            get { return name; }
            set
            {
                name = value;
                base.RaisePropertyChanged("Name");
            }
        }

        private PropertyElement qty;

        public PropertyElement Qty
        {
            get { return qty; }
            set
            {
                qty = value;
                base.RaisePropertyChanged("Qty");
            }
        }

        private PropertyElement width;

        public PropertyElement Width
        {
            get { return width; }
            set
            {
                width = value;
                base.RaisePropertyChanged("Width");
            }
        }

        private PropertyElement height;

        public PropertyElement Height
        {
            get { return height; }
            set
            {
                height = value;
                base.RaisePropertyChanged("Height");
            }
        }

        private PropertyElement depth;

        public PropertyElement Depth
        {
            get { return depth; }
            set
            {
                depth = value;
                base.RaisePropertyChanged("Depth");
            }
        }

        private PropertyElement xOrigin;

        public PropertyElement XOrigin
        {
            get { return xOrigin; }
            set
            {
                xOrigin = value;
                base.RaisePropertyChanged("XOrigin");
            }
        }

        private PropertyElement yOrigin;

        public PropertyElement YOrigin
        {
            get { return yOrigin; }
            set
            {
                yOrigin = value;
                base.RaisePropertyChanged("YOrigin");
            }
        }

        private PropertyElement zOrigin;

        public PropertyElement ZOrigin
        {
            get { return zOrigin; }
            set
            {
                zOrigin = value;
                base.RaisePropertyChanged("ZOrigin");
            }
        }

        private PropertyElement zRotation;

        public PropertyElement ZRotation
        {
            get { return zRotation; }
            set
            {
                zRotation = value;
                base.RaisePropertyChanged("ZRotation");
            }
        }

        private PropertyElement handle;

        public PropertyElement Handle
        {
            get { return handle; }
            set
            {
                handle = value;
                base.RaisePropertyChanged("Handle");
            }
        }

        public int RowIndex { get; set; }
    }
}
