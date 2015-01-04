using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Media;

using SpreadsheetGear;
using GalaSoft.MvvmLight;

namespace Dimeng.WoodEngine.Prompts
{
    public class PromptsViewModel : ViewModelBase
    {
        private IRange PromptCells;

        private PromptsViewModel(Logger logger, Project project)
        {
            //this.logger = logger;
            this.Project = project;
            PromptsChangedControlType = new List<PromptItem>();
            PromptTabs = new List<string>();
            Calcuators = new List<CalculatorItem>();
        }

        /// <summary>
        /// 对应新产品的情况
        /// </summary>
        /// <param name="cutxFilePath">library里产品的cutx路径</param>
        /// <param name="project">要使用产品的project</param>
        /// <param name="logger">日志记录类</param>
        public PromptsViewModel(string cutxFilePath, Project project, logger logger)
            : this(logger, project)
        {
            var bookSet = (new WorkBookSetProvider(logger))
                    .GetDefaultSpecificationGroupWorkBookSet(cutxFilePath, project);

            book = bookSet.Workbooks["L"];
            var sheet = book.Worksheets["Prompts"];
            PromptCells = sheet.Cells;

            LoadCellPrompts();

            if (PromptTabs.Count == 0)
            {
                PromptTabs.Add("主标签");
            }

            this.SpecificationGroups = project.SpecificationGroups;
            this.SelectedSGroup = this.SpecificationGroups[0];
            this.Qty = 1;

            var fi = new FileInfo(cutxFilePath);
            this.Description = fi.Name.Replace(fi.Extension, "");

            this.ProductHandle = (new Random()).Next(1000000000, 1999999999).ToString();
        }

        /// <summary>
        /// 供组件使用的，不需要project的信息
        /// </summary>
        /// <param name="cutxFilePath"></param>
        /// <param name="books"></param>
        /// <param name="//logger"></param>
        public PromptsViewModel(string cutxFilePath, string w, string h, string d, IWorkbookSet books, //logger //logger) :
            this(logger, null)
        {
            book = books.Workbooks.Open(cutxFilePath);
            book.FullName = "S";

            var sheet = book.Worksheets["Prompts"];
            PromptCells = sheet.Cells;

            PromptCells[0, 1].Value = w;
            PromptCells[1, 1].Value = h;
            PromptCells[2, 1].Value = d;

            loadCellPrompts();

            if (PromptTabs.Count == 0)
            {
                PromptTabs.Add("主标签");
            }
        }

        private void loadCellPrompts()
        {
            //logger.Debug("读取Prompts表中的变量清单");

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

        private int qty = 1;
        public int Qty
        {
            get { return qty; }
            set
            {
                qty = value;
                base.RaisePropertyChanged("Qty");
            }
        }

        private string description;
        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                base.RaisePropertyChanged("Description");
            }
        }

        private SpecificationGroup selectedSGroup;
        public SpecificationGroup SelectedSGroup
        {
            get { return selectedSGroup; }
            set
            {
                selectedSGroup = value;
                base.RaisePropertyChanged("SelectedSGroup");
            }
        }

        private List<SpecificationGroup> specificationGroups;
        public List<SpecificationGroup> SpecificationGroups
        {
            get { return specificationGroups; }
            set
            {
                specificationGroups = value;
                base.RaisePropertyChanged("SpecificationGroups");
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


        public double Width
        {
            get
            {
                IRange cells = book.Worksheets["Prompts"].Cells;

                double width;
                if (double.TryParse(cells[0, 1].Text, out width))
                {
                    return width;
                }
                else
                {
                    return 0;
                }
            }
        }

        public double Height
        {
            get
            {
                IRange cells = book.Worksheets["Prompts"].Cells;

                double width;
                if (double.TryParse(cells[1, 1].Text, out width))
                {
                    return width;
                }
                else
                {
                    return 0;
                }
            }
        }

        public double Depth
        {
            get
            {
                IRange cells = book.Worksheets["Prompts"].Cells;

                double width;
                if (double.TryParse(cells[2, 1].Text, out width))
                {
                    return width;
                }
                else
                {
                    return 0;
                }
            }
        }

        private ImageSource image;
        public ImageSource Image
        {
            get { return image; }
            set
            {
                image = value;
                base.RaisePropertyChanged("Image");
            }
        }

        public string ProductHandle { get; private set; }

        public Action<List<PromptItem>> ControlTypeChangedAction { get; set; }
        public IWorkbook book { get; private set; }
        public Project Project { get; private set; }
    }
}
