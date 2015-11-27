using Dimeng.LinkToMicrocad.Logging;
using Dimeng.WoodEngine.Entities;
using Dimeng.WoodEngine.Prompts;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SpreadsheetGear;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace Dimeng.LinkToMicrocad.Prompts.Subassemblies
{
    public class SubassembliesViewModel : ViewModelBase
    {
        IRange cells;

        public SubassembliesViewModel(IRange subCells)
        {
            Items = new ObservableCollection<SubassemblyViewModel>();
            this.ShowPromptCommand = new RelayCommand(this.showPrompt);
            this.ShowPropertyCommand = new RelayCommand(this.showProperty);
            this.DeleteCommand = new RelayCommand(this.delete);
            this.CopyCommand = new RelayCommand(this.copy);
            this.cells = subCells;

            for (int i = 0; i < cells.Rows.RowCount; i++)
            {
                var row = cells.Rows[i, 0].EntireRow;
                string name = cells.Rows[i, 16].Text;

                if (string.IsNullOrEmpty(name))
                    break;

                SubassemblyViewModel item = new SubassemblyViewModel();
                item.Handle = new PropertyElement(row[0, 1]);
                item.Name = new PropertyElement(row[0, 16]);
                item.Qty = new PropertyElement(row[0, 17]);
                item.Width = new PropertyElement(row[0, 18]);
                item.Height = new PropertyElement(row[0, 19]);
                item.Depth = new PropertyElement(row[0, 20]);
                item.XOrigin = new PropertyElement(row[0, 29]);
                item.YOrigin = new PropertyElement(row[0, 30]);
                item.ZOrigin = new PropertyElement(row[0, 31]);
                item.ZRotation = new PropertyElement(row[0, 34]);
                item.RowIndex = i;

                this.Items.Add(item);
            }
        }

        public ObservableCollection<SubassemblyViewModel> Items { get; private set; }
        public RelayCommand ShowPromptCommand { get; private set; }
        public RelayCommand ShowPropertyCommand { get; private set; }
        public RelayCommand DeleteCommand { get; private set; }
        public RelayCommand CopyCommand { get; private set; }
        private SubassemblyViewModel selectedItem;
        public SubassemblyViewModel SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;
                base.RaisePropertyChanged("SelectedItem");
            }
        }

        public IMVLibrary Library { get; set; }
        public string ProjectPath { get; set; }
        public string Handle { get; set; }

        private void showPrompt()
        {
            if (this.selectedItem == null)
            {
                Logger.GetLogger().Debug("当前SelectedItem为null");
                return;
            }
            else
            {
                //第一步，获取组件的cutx文件，可能是目前已经存在的，也可能是需要到库里查找的
                string subFilename1 = Path.Combine(this.ProjectPath, "Subassemblies",
                    string.Format("{0}_({1}){2}.cutx", this.Handle, SelectedItem.Name, SelectedItem.RowIndex + 1));
                string subFilename = subFilename1;

                if (!File.Exists(subFilename1))
                {
                    Logger.GetLogger().Debug(subFilename1 + " not Found");

                    string subFilename2 = Path.Combine(this.ProjectPath, "Subassemblies", string.Format("{0}.cutx", selectedItem.Name));
                    if (!File.Exists(subFilename2))
                    {
                        Logger.GetLogger().Debug(subFilename2 + " not found and start searching.");

                        string[] files = Directory.GetFiles(Library.Subassemblies, string.Format("{0}.cutx", selectedItem.Name), SearchOption.AllDirectories);
                        if (files.Length == 0)
                        {
                            throw new Exception("Can`t find subasembliy from library:" + selectedItem.Name);
                        }

                        File.Copy(files[0], subFilename1);
                    }
                    else
                    {
                        subFilename = subFilename2;
                    }
                }

                //第二步，把数据放入组件中，启动组件提示屏
                var book = BookSet.Workbooks.Open(subFilename);
                book.FullName = "S";

                double w = double.Parse(this.selectedItem.Width.PropertyValue);
                double h = double.Parse(this.selectedItem.Height.PropertyValue);
                double d = double.Parse(this.selectedItem.Depth.PropertyValue);

                book.Worksheets["Prompts"].Cells[0, 1].Value = w;
                book.Worksheets["Prompts"].Cells[1, 1].Value = h;
                book.Worksheets["Prompts"].Cells[2, 1].Value = d;

                PromptsViewModel viewmodel = new PromptsViewModel(BookSet,
                this.selectedItem.Name.PropertyValue, w, h,
                d, 0, this.Library, "S", this.ProjectPath, this.Handle);

                PromptWindow prompt = new PromptWindow();
                prompt.ViewModel = viewmodel;
                prompt.ShowDialog();

                selectedItem.Width.PropertyValue = book.Worksheets["Prompts"].Cells[0, 1].Value.ToString();
                selectedItem.Height.PropertyValue = book.Worksheets["Prompts"].Cells[1, 1].Value.ToString();
                selectedItem.Depth.PropertyValue = book.Worksheets["Prompts"].Cells[2, 1].Value.ToString();

                //第三部，保存组件的数据，从bookset中删去S
                book.SaveAs(subFilename, FileFormat.OpenXMLWorkbook);
                book.Close();
            }
        }

        public IWorkbookSet BookSet { get; set; }

        public void showProperty()
        {
            if (this.selectedItem == null)
            {
                Logger.GetLogger().Debug("当前SelectedItem为null");
                return;
            }
            else
            {
                SubPropertyWindow window = new SubPropertyWindow(this.selectedItem);
                window.ShowDialog();
            }
        }

        private void delete()
        {
            if (this.selectedItem == null)
            {
                return;
            }
            else
            {
                this.selectedItem.Qty.PropertyValue = "0";
            }
        }

        void copy()
        {
            if (this.selectedItem == null)
            {
                return;
            }
            else
            {
                //第一步，获取组件的cutx文件，可能是目前已经存在的，也可能是需要到库里查找的
                string subFilename1 = Path.Combine(this.ProjectPath, "Subassemblies",
                    string.Format("{0}_({1}){2}.cutx", this.Handle, SelectedItem.Name, SelectedItem.RowIndex + 1));
                string subFilename = subFilename1;

                if (!File.Exists(subFilename1))
                {
                    Logger.GetLogger().Debug(subFilename1 + " not Found");

                    string subFilename2 = Path.Combine(this.ProjectPath, "Subassemblies", string.Format("{0}.cutx", selectedItem.Name));
                    if (!File.Exists(subFilename2))
                    {
                        Logger.GetLogger().Debug(subFilename2 + " not found and start searching.");

                        string[] files = Directory.GetFiles(Library.Subassemblies, string.Format("{0}.cutx", selectedItem.Name), SearchOption.AllDirectories);
                        if (files.Length == 0)
                        {
                            throw new Exception("Can`t find subasembliy from library:" + selectedItem.Name);
                        }

                        File.Copy(files[0], subFilename1);
                    }
                    else
                    {
                        subFilename = subFilename2;
                    }
                }

                //第二步，复制数据
                for (int i = 0; i < this.cells.RowCount; i++)
                {
                    string name = this.cells[i, 16].Text;
                    if (!string.IsNullOrEmpty(name))
                    {
                        continue;
                    }

                    this.cells[i, 16].EntireRow.Insert(InsertShiftDirection.Down);

                    this.cells[i, 16].Value = this.SelectedItem.Name.PropertyValue;
                    this.cells[i, 17].Value = this.SelectedItem.Qty.PropertyValue;
                    this.cells[i, 18].Value = this.SelectedItem.Width.PropertyValue;
                    this.cells[i, 19].Value = this.SelectedItem.Height.PropertyValue;
                    this.cells[i, 20].Value = this.SelectedItem.Depth.PropertyValue;
                    this.cells[i, 29].Value = this.SelectedItem.XOrigin.PropertyValue;
                    this.cells[i, 30].Value = this.SelectedItem.YOrigin.PropertyValue;
                    this.cells[i, 31].Value = this.SelectedItem.ZOrigin.PropertyValue;
                    this.cells[i, 34].Value = this.SelectedItem.ZRotation.PropertyValue;
                    //this.cells[i + 1, 16].Value = string.Empty;//防止下一行为非空行导致错误

                    IRange row = this.cells[i, 16].EntireRow;

                    SubassemblyViewModel item = new SubassemblyViewModel();
                    //item.Handle = new PropertyElement(row[0, 1]);
                    item.Name = new PropertyElement(row[0, 16]);
                    item.Qty = new PropertyElement(row[0, 17]);
                    item.Width = new PropertyElement(row[0, 18]);
                    item.Height = new PropertyElement(row[0, 19]);
                    item.Depth = new PropertyElement(row[0, 20]);
                    item.XOrigin = new PropertyElement(row[0, 29]);
                    item.YOrigin = new PropertyElement(row[0, 30]);
                    item.ZOrigin = new PropertyElement(row[0, 31]);
                    item.ZRotation = new PropertyElement(row[0, 34]);
                    item.RowIndex = i;

                    this.Items.Add(item);
                    break;
                }
            }
        }
    }
}
