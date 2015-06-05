using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace QuoteExport
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            StartCommand = new RelayCommand(this.startWork);
        }

        private void startWork()
        {
            throw new NotImplementedException();
        }

        private double progressValue;
        public double ProgressValue
        {
            get { return progressValue; }
            set
            {
                progressValue = value;
                base.RaisePropertyChanged("ProgressValue");
            }
        }

        public RelayCommand StartCommand { get; set; }
    }
}
