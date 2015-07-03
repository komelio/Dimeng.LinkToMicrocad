using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ADLauncher
{
    public class ProjectViewModel
    {
        public ProjectViewModel(string name, string time)
        {
            this.Name = name;
            this.Time = time;
        }
        public string Name { get; set; }
        public string Time { get; set; }
        public RelayCommand<ProjectViewModel> OpenProjectCommand { get; set; }
    }
}
