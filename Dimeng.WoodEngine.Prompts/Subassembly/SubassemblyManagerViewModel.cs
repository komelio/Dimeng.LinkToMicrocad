using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SpreadsheetGear;

using Dimeng.WoodEngine.Entities;
using Autodesk.AutoCAD.Geometry;
using Dimeng.WoodEngine.Business;
using Dimeng.WoodEngine.CommonSpreadControls;
using GalaSoft.MvvmLight.Messaging;
using Dimeng.WoodEngine.Entities.Events;
using System.Windows;

namespace Dimeng.WoodEngine.Prompts
{
    public class SubassemblyManagerViewModel : ViewModelBase
    {
        Logger logger;
        App app;
        IWorkbook book;
        Project project;
        string productHandle;
        SubassemblyHelper helper;

        public SubassemblyManagerViewModel(Logger logger, App app, SubassemblyHelper subHelper)
        {
            this.logger = logger;
            this.app = app;
            this.helper = subHelper;

            Subassemblies = new ObservableCollection<SubassemblyItem>();

            this.ModifyFormulaCommand = new RelayCommand<PropertyElement>(this.ModifySelectedFormula, this.CanModify);
            this.SelectInsertPointCommmand = new RelayCommand(this.SelectPoint, this.IsCurrentNull);
            this.AddNewSubassemblyCommand = new RelayCommand<Object>(this.AddNewSub);
            this.ShowSubassemblyPromptCommand = new RelayCommand(this.ShowPrompt, this.IsCurrentNull);
            this.TurnSubIntoUniqueCommand = new RelayCommand(this.TurnSubIntoUnique, this.IsCurrentUnique);
            this.SelectRangeSizeCommand = new RelayCommand(this.SelectRange, this.IsCurrentNull);
            this.CopySubCommand = new RelayCommand(this.CopySub, this.IsCurrentNull);
            this.DeleteSubCommand = new RelayCommand(this.DelSub, this.IsCurrentNull);
        }

        public void SetData(IWorkbook book, Project project, string productHandle)
        {
            this.book = book;
            this.project = project;
            this.productHandle = productHandle;

            LoadSubassemblies();

            LoadBookSubassemblies();
        }

        #region Private Method
        private void DelSub()
        {
            book.Worksheets["Subassemblies"].Cells[this.SelectedItem.RowIndex, 0].EntireRow.Delete();

            //TODO:重新命名编号文件

            this.Subassemblies.Remove(this.SelectedItem);
        }

        private void CopySub()
        {
            throw new NotImplementedException();
        }

        private void SelectRange()
        {
            logger.Debug("点击选择range按钮");

            Document acDoc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            Editor acEditor = acDoc.Editor;

            //如果没有记录，那么就提示如何放置产品，并在选择的点上进行绘制
            Point3d ptStart = Point3d.Origin;
            PromptPointResult rpr = acEditor.GetPoint("选择起点\n");
            if (rpr.Status == PromptStatus.OK)
            {
                ptStart = rpr.Value;
                Point3d ptEnd = Point3d.Origin;
                PromptCornerOptions options = new PromptCornerOptions("选择终点", ptStart);
                PromptPointResult i = acEditor.GetCorner(options);
                if (i.Status == PromptStatus.OK)
                {
                    ptEnd = i.Value;

                    double width = Math.Abs(ptStart.X - ptEnd.X);
                    double height = Math.Abs(ptStart.Y - ptEnd.Y);

                    this.SelectedItem.Width.PropertyValue = Math.Round(width, 3).ToString();
                    this.SelectedItem.Height.PropertyValue = Math.Round(height, 3).ToString();
                }
            }

            logger.Debug("未选择产品的放置点");
            return;
        }
        private void TurnSubIntoUnique()
        {
            logger.Debug("执行将共享组件转换为独立组件命令");

            if (this.SelectedItem.IsUnique)
                return;

            string filePath = Path.Combine(project.JobPath, "Subassemblies", this.SelectedItem.Name + ".cutx");
            logger.Debug("组件可能文件名1：" + filePath);
            string filePath2 = Path.Combine(project.JobPath, "Subassemblies", string.Format("{0}_({1}){2}.cutx", this.productHandle, this.SelectedItem.Name, this.SelectedItem.RowIndex + 1));
            logger.Debug("组件可能文件名2：" + filePath);

            if (File.Exists(filePath))
            {
                if (!File.Exists(filePath2))
                {
                    File.Copy(filePath, filePath2);
                }

                this.SelectedItem.IsUnique = true;
            }
            else
            {
                var subs = Directory.GetFiles(app.CurrentConfiguration.PathToSubassemblies, this.SelectedItem.Name + ".cutx", SearchOption.AllDirectories);
                if (subs.Length == 0)
                {
                    logger.Warn("未从产品库中找到组件");
                    MessageBox.Show("未找到组件文件:" + this.SelectedItem.Name);
                    return;
                }

                if (!File.Exists(filePath2))
                {
                    File.Copy(subs[0], filePath2);
                    this.SelectedItem.IsUnique = true;
                }
            }
        }

        private bool IsCurrentUnique()
        {
            if (this.SelectedItem == null)
                return false;

            if (this.SelectedItem.IsUnique)
                return false;
            else return true;
        }

        private void ShowPrompt()
        {
            logger.Debug("显示组件提示屏命令");

            string filePath = helper.GetSubassembly(project, this.SelectedItem.Name.PropertyValue, this.SelectedItem.RowIndex, this.productHandle);

            //如果当前任务里没有这个组件的cutx文件，则需要从组件库中查找并拷贝
            if (!File.Exists(filePath))
            {
                MessageBox.Show("未找到对应的组件文件！");
                return;
            }

            SubPromptWindow spw = new SubPromptWindow(logger, app, filePath, book.WorkbookSet, this.SelectedItem);
            if (spw.ShowDialog() == true)//对于组件修改来说，要进行保存
            {
                this.SelectedItem.Width.PropertyValue = spw.Manager.Width.ToString();
                this.SelectedItem.Height.PropertyValue = spw.Manager.Height.ToString();
                this.SelectedItem.Depth.PropertyValue = spw.Manager.Depth.ToString();

                book.WorkbookSet.Workbooks["S"].SaveAs(filePath, FileFormat.OpenXMLWorkbook);
                book.WorkbookSet.Workbooks[filePath.Substring(filePath.LastIndexOf("\\") + 1)].Close();//关掉，释放之
            }
            else
            {
                book.WorkbookSet.Workbooks["S"].Close();//关掉，释放之
            }
        }

        private void AddNewSub(Object obj)
        {
            if (obj is SubassemblyViewModel)
            {
                var sub = obj as SubassemblyViewModel;

                IRange cells = book.Worksheets["Subassemblies"].Cells;

                for (int i = 0; i < cells.Rows.RowCount; i++)
                {
                    string name = cells.Rows[i, 16].Text;
                    var row = cells.Rows[i, 0].EntireRow;

                    //与读取相反，这里是新加入行，所以直到空行才开始写数据
                    if (!string.IsNullOrEmpty(name))
                        continue;

                    row[0, 16].Value = sub.Name;
                    row[0, 17].Value = 1;
                    row[0, 18].Value = 0;
                    row[0, 19].Value = 0;
                    row[0, 20].Value = 0;
                    row[0, 29].Value = 0;
                    row[0, 30].Value = 0;
                    row[0, 31].Value = 0;
                    row[0, 34].Value = 0;

                    SubassemblyItem item = new SubassemblyItem();
                    item.Name = new PropertyElement(row[0, 16]);
                    item.Qty = new PropertyElement(row[0, 17]);
                    item.Width = new PropertyElement(row[0, 18]);
                    item.Height = new PropertyElement(row[0, 19]);
                    item.Depth = new PropertyElement(row[0, 20]);
                    item.XOrigin = new PropertyElement(row[0, 29]);
                    item.YOrigin = new PropertyElement(row[0, 30]);
                    item.ZOrigin = new PropertyElement(row[0, 31]);
                    item.ZRotation = new PropertyElement(row[0, 34]);
                    item.RowIndex = row.Row;
                    item.IsUnique = true;

                    this.Subassemblies.Add(item);

                    CopySubAssemblyFile(sub, row.Row);

                    break;
                }
            }
        }

        private void CopySubAssemblyFile(SubassemblyViewModel sub, int rowIndex)
        {
            logger.Debug("开始从组件库中拷贝cutx文件");

            string file = string.Format("{0}_({1}){2}.cutx",
                                        productHandle,
                                        sub.Name,
                                        rowIndex + 1);

            file = Path.Combine(project.JobPath, "Subassemblies", file);

            logger.Debug("完整文件路径:" + file);

            try
            {
                //TODO:这里用了覆盖拷贝，是否有风险？
                File.Copy(sub.CutxPath, file, true);
            }
            catch (System.Exception error)
            {
                logger.Error("拷贝文件时发生错误：" + error.Message + Environment.NewLine + error.StackTrace);
                throw error;
            }
        }

        private bool IsCurrentNull()
        {
            if (this.SelectedItem != null)
            { return true; }
            else return false;
        }

        private void SelectPoint()
        {
            Document acDoc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            Editor acEditor = acDoc.Editor;

            //如果没有记录，那么就提示如何放置产品，并在选择的点上进行绘制
            PromptPointResult rpr = acEditor.GetPoint("选择一点放置组件\n");
            if (rpr.Status == PromptStatus.OK)
            {
                logger.Debug("选择角点：" + rpr.Value.ToString());

                this.SelectedItem.XOrigin.PropertyFormula = rpr.Value.X.ToString();
                this.SelectedItem.YOrigin.PropertyFormula = rpr.Value.Y.ToString();
                this.SelectedItem.ZOrigin.PropertyFormula = rpr.Value.Z.ToString();
                return;
            }

            logger.Debug("未选择产品的放置点");
            return;
        }

        private bool CanModify(object arg)
        {
            if (this.selectedItem == null)
                return false;
            else return true;
        }

        private void ModifySelectedFormula(PropertyElement property)
        {
            //this.CurrentProperty = property;
            Messenger.Default.Send<ToModifyCellFormulaEvent>(new ToModifyCellFormulaEvent() { Element = property });
        }

        private void LoadBookSubassemblies()
        {
            logger.Debug("开始读取当前产品中的组件清单：");

            Subassemblies.Clear();

            IRange cells = book.Worksheets["Subassemblies"].Cells;

            for (int i = 0; i < cells.Rows.RowCount; i++)
            {
                var row = cells.Rows[i, 0].EntireRow;
                string name = cells.Rows[i, 16].Text;

                if (string.IsNullOrEmpty(name))
                    break;

                SubassemblyItem item = new SubassemblyItem();
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

                string file = string.Format("{3}\\Subassemblies\\{0}_({1}){2}.cutx",
                                        productHandle,
                                        item.Name.PropertyValue,
                                        i + 1,
                                        project.JobPath);
                if (File.Exists(file))
                {
                    item.IsUnique = true;
                }
                else item.IsUnique = false;

                this.Subassemblies.Add(item);

                logger.Debug(string.Format("读取组件：{0}/{1}/{2}/{3}/{4}/{5}/{6}/{7}/{8}/{9}",
                                           item.Name,
                                           item.Qty,
                                           item.Width,
                                           item.Height,
                                           item.Depth,
                                           item.XOrigin,
                                           item.YOrigin,
                                           item.ZOrigin,
                                           item.ZRotation,
                                           item.Handle));
            }

            logger.Debug("结束读取当前产品中的组件清单。");
        }

        private void LoadSubassemblies()
        {
            var subPath = app.CurrentConfiguration.PathToSubassemblies;
            var di = new DirectoryInfo(subPath);
            var tempList = new List<SubassemblyCategory>();

            foreach (var d in di.GetDirectories())
            {
                tempList.Add(new SubassemblyCategory(d));
            }

            Categories = tempList;
        }
        #endregion

        #region BindingProperty
        private List<SubassemblyCategory> categories;
        public List<SubassemblyCategory> Categories
        {
            get { return categories; }
            set
            {
                categories = value;
                base.RaisePropertyChanged("Categories");
            }
        }

        private ObservableCollection<SubassemblyItem> subassemblies;
        public ObservableCollection<SubassemblyItem> Subassemblies
        {
            get { return subassemblies; }
            set { subassemblies = value; }
        }

        private SubassemblyItem selectedItem;
        public SubassemblyItem SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;
                base.RaisePropertyChanged("SelectedItem");
            }
        }

        #endregion

        #region Commands
        public RelayCommand<PropertyElement> ModifyFormulaCommand { get; private set; }

        public RelayCommand<Object> AddNewSubassemblyCommand { get; private set; }

        public RelayCommand SelectInsertPointCommmand { get; private set; }

        public RelayCommand ShowSubassemblyPromptCommand { get; private set; }

        public RelayCommand TurnSubIntoUniqueCommand { get; private set; }

        public RelayCommand SelectRangeSizeCommand { get; private set; }

        public RelayCommand CopySubCommand { get; private set; }

        public RelayCommand DeleteSubCommand { get; private set; }
        #endregion
    }
}
