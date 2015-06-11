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
        public IRange PromptCells;

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
                                Project project,
                                IMVLibrary library)
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

            this.Library = library;
            Logger.GetLogger().Debug(string.Format("Library:{0}", library.Library));
        }

        public PromptsViewModel(IWorkbookSet bookset,
                                string name,
                                double width,
                                double height,
                                double depth,
                                double elevation,
                                IMVLibrary library,
                                string title,
                                string projectPath,
                                string handle)
            : this()
        {
            this.Width = width;
            this.Height = height;
            this.Depth = depth;
            this.Name = name;
            this.Library = library;
            this.Elevation = elevation;
            this.BookSet = bookset;
            this.ProjectPath = projectPath;
            this.Handle = handle;

            Book = BookSet.Workbooks[title];
            var sheet = Book.Worksheets["Prompts"];
            PromptCells = sheet.Cells;

            loadCellPrompts();

            if (PromptTabs.Count == 0)
            {
                PromptTabs.Add("主标签");
            }
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

            ReloadCalculators();

            bool belowBlankRow = false;
            for (int i = 0; i < PromptCells.Rows.RowCount; i++)
            {
                var name = PromptCells[i, 0].Text;

                if (string.IsNullOrEmpty(name))
                {
                    belowBlankRow = true;
                    //logger.Info("空行中断读取,行号:" + i.ToString());
                    if (string.IsNullOrEmpty(PromptCells[i + 1, 0].Text.Trim()))
                    { break; }
                }

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

                if (i < 4)
                {
                    Logger.GetLogger().Debug(string.Format("Prompt变量数据:{0}/{1}/{2}/{3}/{4}/{5}/{6}/{7}/{8}/{9}/{10}/{11}",
                                               name,
                                               value,
                                               controlType,
                                               helpMessage,
                                               verifyCode,
                                               comboString,
                                               color,
                                               picture,
                                               visible,
                                               hideInReport,
                                               tabIndex,
                                               calculatorIndex));
                }
                PromptItem prompt = new PromptItem(name, value, controlType, helpMessage, verifyCode, comboString, color, picture, visible, hideInReport, tabIndex, calculatorIndex, i, this);
                prompt.IsBelowBlankRow = belowBlankRow;
                Prompts.Add(prompt);
            }
        }

        public void ReloadCalculators()
        {
            Logger.GetLogger().Debug("清空计算器");
            this.Calcuators.Clear();
            //读取计算器
            for (int i = 0; i < PromptCells.Rows.RowCount; i++)
            {
                string CalName = PromptCells[i, 17].Text;
                if (!string.IsNullOrEmpty(CalName))
                {
                    Logger.GetLogger().Debug("增加计算器:" + CalName);

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
            Logger.GetLogger().Debug("-------Change Value:" + prompt.Name + "/" + prompt.PromptValue);

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

        private double elevation;
        public double Elevation
        {
            get { return elevation; }
            set
            {
                elevation = value;
                base.RaisePropertyChanged("Elevation");
            }
        }

        public string ProjectPath { get; private set; }

        public Action<List<PromptItem>> ControlTypeChangedAction { get; set; }
        public IWorkbook Book { get; private set; }
        public IWorkbookSet BookSet { get; private set; }
        public IMVLibrary Library { get; private set; }
        //public Project Project { get; private set; }

        public string Handle { get; set; }
    }
}
