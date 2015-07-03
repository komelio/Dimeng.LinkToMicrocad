﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace ADLauncher
{
    public class ViewModel : ViewModelBase
    {
        public ViewModel(string orderNumber)
            : this()
        {
            this.ProjectName = orderNumber;
        }

        public ViewModel()
        {
            CreateProjectCommand = new RelayCommand(this.createNew, this.canCreateNew);
        }

        private bool canCreateNew()
        {
            if (this.projectName.Length > 0)
            {
                string path = Path.Combine(ADProjectsPath, this.projectName);
                if (Directory.Exists(path))
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        private void createNew()
        {
            string erpxml = Path.Combine(ADTempPath, "erp.xml");
            using (FileStream fs = new FileStream(erpxml, FileMode.CreateNew, FileAccess.Write, FileShare.Write))
            using (StreamWriter sw = new StreamWriter(fs, Encoding.Default))
            {
                sw.WriteLine(@"<?xml version=""1.0"" encoding=""UTF-8"" ?>");
                sw.WriteLine("<root>");
                sw.WriteLine(string.Format(@"<f Name=""{0}"" Description=""{1}"" o=""N"" /> ", this.projectName, this.projectDescription));
                sw.WriteLine(@"</root>");
            }
            OnCreate();
        }

        public string ADTempPath { get; set; }
        private string adProjectPath;
        public string ADProjectsPath
        {
            get { return adProjectPath; }
            set
            {
                adProjectPath = value;
                loadProjects();
                base.RaisePropertyChanged("ADProjectPath");
            }
        }

        private void loadProjects()
        {
            this.Projects = new List<ProjectViewModel>();
            DirectoryInfo di = new DirectoryInfo(this.adProjectPath);
            foreach (var d in di.GetDirectories())
            {
                var p = new ProjectViewModel(d.Name, d.LastWriteTime.ToString("yyyy-MM-dd HH:mm"));
                p.OpenProjectCommand = new RelayCommand<ProjectViewModel>(this.openProject);
                this.Projects.Add(p);
            }
        }

        private void openProject(ProjectViewModel vm)
        {
            string erpxml = Path.Combine(ADTempPath, "erp.xml");
            using (FileStream fs = new FileStream(erpxml, FileMode.CreateNew, FileAccess.Write, FileShare.Write))
            using (StreamWriter sw = new StreamWriter(fs, Encoding.Default))
            {
                sw.WriteLine(@"<?xml version=""1.0"" encoding=""UTF-8"" ?>");
                sw.WriteLine("<root>");
                sw.WriteLine(string.Format(@"<f Name=""{0}"" Description=""{1}"" o=""M"" /> ", vm.Name, ""));
                sw.WriteLine(@"</root>");
            }
            OnCreate();
        }

        private string projectName = string.Empty;
        public string ProjectName
        {
            get { return projectName; }
            set
            {
                projectName = value;
                base.RaisePropertyChanged("ProjectName");
            }
        }

        private string projectDescription = string.Empty;
        public string ProjectDescription
        {
            get { return projectDescription; }
            set
            {
                projectDescription = value;
                base.RaisePropertyChanged("ProjectDescription");
            }
        }

        public List<ProjectViewModel> Projects { get; private set; }

        public RelayCommand CreateProjectCommand { get; private set; }

        public Action OnCreate { get; set; }
    }
}