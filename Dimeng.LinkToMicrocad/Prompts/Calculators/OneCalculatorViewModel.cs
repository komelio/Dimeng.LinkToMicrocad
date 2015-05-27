using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Prompts
{
    public class OneCalculatorViewModel : ViewModelBase
    {
        public string Name { get; set; }

        private string v;
        public string Value
        {
            get { return v; }
            set
            {
                v = value;
                base.RaisePropertyChanged("Value");
            }
        }

        private bool isEqual = false;
        public bool IsEqual
        {
            get { return isEqual; }
            set
            {
                isEqual = value;
                base.RaisePropertyChanged("IsEqual");
            }

        }

        public PromptItem PromptItem { get; set; }
    }
}
