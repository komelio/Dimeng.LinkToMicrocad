using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Media;

using SpreadsheetGear;
using GalaSoft.MvvmLight;
using Dimeng.LinkToMicrocad.Logging;
using Dimeng.WoodEngine.Spread;
using Dimeng.WoodEngine.Entities;

namespace Dimeng.WoodEngine.Prompts
{
    public class PromptsViewModel : ViewModelBase
    {
        private IRange PromptCells;

        private PromptsViewModel()
        {
            PromptsChangedControlType = new List<PromptItem>();
            PromptTabs = new List<string>();
            Calcuators = new List<CalculatorItem>();
        }

        public PromptsViewModel(string name,
                                string imagePath,
                                double width,
                                double height,
                                double depth,
                                Project project)
            : this()
        {
            this.Width = width;
            Logger.GetLogger().Debug(string.Format("Width:{0}", width));

            this.Height = height;
            Logger.GetLogger().Debug(string.Format("Height:{0}", height));

            this.Depth = depth;
            Logger.GetLogger().Debug(string.Format("Depth:{0}", depth));

            this.Name = name;
            Logger.GetLogger().Debug(string.Format("Name:{0}", name));

            this.ProductImagePath = imagePath + ".jpg";
            Logger.GetLogger().Debug(string.Format("ProductImagePath:{0}.jpg", imagePath));
        }

        public PromptsViewModel(string cutxFilePath,
                                string globalFilePath,
                                string cutpartsFilePath,
                                string edgebandFilePath,
                                string hardwareFilePath,
                                string doorstyleFilePath,
                                string name,
                                string imagePath,
                                double width,
                                double height,
                                double depth)
            : this()
        {
            this.Width = width;
            this.Height = height;
            this.Depth = depth;
            this.Name = name;
            this.ProductImagePath = imagePath + ".jpg";

            var bookSet = SpreadHelper.GetProductBaseicBookSet(cutxFilePath, globalFilePath,
                cutpartsFilePath, hardwareFilePath, doorstyleFilePath, edgebandFilePath);

            Book = bookSet.Workbooks["L"];
            var sheet = Book.Worksheets["Prompts"];
            PromptCells = sheet.Cells;

            loadCellPrompts();

            if (PromptTabs.Count == 0)
            {
                PromptTabs.Add("主标签");
            }

            //this.SpecificationGroups = project.SpecificationGroups;
            //this.SelectedSGroup = this.SpecificationGroups[0];
            //this.Qty = 1;

            //var fi = new FileInfo(cutxFilePath);
            //this.Description = fi.Name.Replace(fi.Extension, "");

            //this.ProductHandle = (new Random()).Next(1000000000, 1999999999).ToString();
        }


        private void loadCellPrompts()
        {
            //读取Tabs
            for (int i = 0; i < PromptCells.Rows.RowCount; i++)
            {
                string TabName = PromptCells[i, 13].Text;
                if (!string.IsNullOrEmpty(TabName))
                {                   
                    //logger.Debug("增加Tab:" + TabName);
                    this.PromptTabs.Add(TabName);
                }
                else
                    break;
            }

            //读取计算器
            for (int i = 0; i < PromptCells.Rows.RowCount; i++)
            {
                string CalName = PromptCells[i, 17].Text;
                if (!string.IsNullOrEmpty(CalName))
                {
                    //logger.Debug("增加计算器:" + CalName);

                    var strs = CalName.Split('|');
                    if (strs.Length < 3)
                        continue;

                    string name = strs[0];

                    double upperBound;
                    if (!double.TryParse(strs[1], out upperBound))
                    { continue; }

                    double gap;
                    if (!double.TryParse(strs[2], out gap))
                    { continue; }

                    Calcuators.Add(new CalculatorItem()
                    {
                        Name = name,
                        UpperBound = upperBound,
                        Gap = gap,
                        Index = i
                    });
                }
                else
                    break;
            }

            for (int i = 0; i < PromptCells.Rows.RowCount; i++)
            {
                var name = PromptCells[i, 0].Text;

                if (string.IsNullOrEmpty(name))
                {
                    //logger.Info("空行中断读取,行号:" + i.ToString());
                    break;
                }

                //忽略宽度、高度、深度三个变量
                if (i == 0 || i == 1 || i == 2)
                { continue; }

                string value = PromptCells[i, 1].Text;
                string controlType = PromptCells[i, 2].Text;
                string helpMessage = PromptCells[i, 3].Text;
                string verifyCode = PromptCells[i, 4].Text;
                string comboString = PromptCells[i, 5].Text;
                string color = PromptCells[i, 7].Text;
                string picture = PromptCells[i, 9].Text;
                string visible = PromptCells[i, 10].Text;
                string hideInReport = PromptCells[i, 11].Text;
                string tabIndex = PromptCells[i, 12].Text;
                string calculatorIndex = PromptCells[i, 16].Text;


                //logger.Debug(string.Format("Prompt变量数据:{0}/{1}/{2}/{3}/{4}/{5}/{6}/{7}/{8}/{9}/{10}/{11}",
                //                           name,
                //                           value,
                //                           controlType,
                //                           helpMessage,
                //                           verifyCode,
                //                           comboString,
                //                           color,
                //                           picture,
                //                           visible,
                //                           hideInReport,
                //                           tabIndex,
                //                           calculatorIndex));

                PromptItem prompt = new PromptItem(name, value, controlType, helpMessage, verifyCode, comboString, color, picture, visible, hideInReport, tabIndex, calculatorIndex, i, this);
                Prompts.Add(prompt);
            }
        }

        public List<PromptItem> Prompts = new List<PromptItem>();
        public List<string> PromptTabs { get; private set; }

        /// <summary>
        /// 变量修改触发重新检查数据
        /// </summary>
        /// <param name="prompt">那个被修改的变量，要把修改值写回表格中</param>
        public void ReloadValues(PromptItem prompt)
        {
            PromptCells[prompt.RowNumber, 1].Value = prompt.PromptValue;

            PromptsChangedControlType.Clear();

            foreach (var p in Prompts)
            {
                if (p == prompt)
                    continue;

                string name = PromptCells[p.RowNumber, 0].Text;
                string value = PromptCells[p.RowNumber, 1].Text;
                string controlType = PromptCells[p.RowNumber, 2].Text;
                string helpMessage = PromptCells[p.RowNumber, 3].Text;
                string verifyCode = PromptCells[p.RowNumber, 4].Text;
                string comboString = PromptCells[p.RowNumber, 5].Text;
                string color = PromptCells[p.RowNumber, 7].Text;
                string picture = PromptCells[p.RowNumber, 9].Text;
                string visible = PromptCells[p.RowNumber, 10].Text;
                string hideInReport = PromptCells[p.RowNumber, 11].Text;
                string tabIndex = PromptCells[p.RowNumber, 12].Text;
                string calculatorIndex = PromptCells[p.RowNumber, 16].Text;  //TODO:这里和之前的逻辑貌似是重叠了？

                //标记一下，防止循环更新导致死循环
                p.IsPassive = true;
                p.LoadProperty(name, value, controlType, helpMessage, verifyCode, comboString, color, picture, visible, hideInReport, tabIndex, calculatorIndex);
                p.IsPassive = false;
            }

            ControlTypeChangedAction(PromptsChangedControlType);
            //ControlTypeChanged(PromptsChangedControlType);
        }

        public List<PromptItem> PromptsChangedControlType { get; private set; }

        public List<CalculatorItem> Calcuators { get; private set; }

        public void RecordControlTypeChange(PromptItem prompt)
        {
            PromptsChangedControlType.Add(prompt);
        }

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                base.RaisePropertyChanged("Name");
            }
        }

        private string comments = string.Empty;
        public string Comments
        {
            get { return comments; }
            set
            {
                comments = value;
                base.RaisePropertyChanged("Comments");
            }
        }

        private string itemNumber;
        public string ItemNumber
        {
            get { return itemNumber; }
            set
            {
                itemNumber = value;
                base.RaisePropertyChanged("ItemNumber");
            }
        }

        private double width;
        public double Width
        {
            get { return width; }
            set
            {
                width = value;
                Logger.GetLogger().Debug(string.Format("Width:{0}", width));
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
                Logger.GetLogger().Debug(string.Format("Height:{0}", height));
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
                Logger.GetLogger().Debug(string.Format("Depth:{0}", depth));
                base.RaisePropertyChanged("Depth");
            }
        }

        private string productImagePath;

        public string ProductImagePath
        {
            get { return productImagePath; }
            set
            {
                productImagePath = value;
                Logger.GetLogger().Debug(string.Format("ProductImagePath:{0}", productImagePath));
                base.RaisePropertyChanged("ProductImagePath");
            }
        }

        public Action<List<PromptItem>> ControlTypeChangedAction { get; set; }
        public IWorkbook Book { get; private set; }
        //public Project Project { get; private set; }
    }
}
