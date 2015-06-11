using SpreadsheetGear;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Dimeng.LinkToMicrocad.Prompts.Subassemblies
{
    public class PropertyElement : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private IRange _cell;
        public PropertyElement(IRange cell)
        {
            _cell = cell;
        }

        public string PropertyValue
        {
            get
            {
                return _cell.Text;
            }
            set
            {
                _cell.Value = value;
                OnPropertyChanged("PropertyValue");
                OnPropertyChanged("PropertyFormula");
            }
        }

        public string PropertyFormula
        {
            get
            {
                return _cell.Formula;
            }
            set
            {
                _cell.Value = value;
                //PropertyValue = _cell.Text;
                OnPropertyChanged("PropertyFormula");
                OnPropertyChanged("PropertyValue");
            }
        }

        public bool IsModifiedByFormula { get; set; }

        private void OnPropertyChanged(string word)
        {
            if (PropertyChanged != null)
            {
                this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(word));
            }
        }

        public override string ToString()
        {
            return this.PropertyValue;
        }
    }
}
