using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Dimeng.LinkToMicrocad.Logging;

namespace Dimeng.WoodEngine.Prompts
{
    public class PromptItem : INotifyPropertyChanged, IDataErrorInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string name = string.Empty;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        private string promptValue = string.Empty;
        public string PromptValue
        {
            get { return promptValue; }
            set
            {
                if (value != promptValue)
                {
                    Logger.GetLogger().Debug(string.Format("Set {0} value {1}", name, value));
                    promptValue = value;
                    OnPropertyChanged("PromptValue");
                }
            }
        }

        private ControlType controlType = ControlType.TextBox;
        public ControlType ControlType
        {
            get { return controlType; }
            set
            {
                if (controlType != value)//只有不相等的才更新，避免反复触发
                {
                    controlType = value;
                    OnPropertyChanged("ControlType");

                    if (PropertyChanged != null && !IsLoadFirstTime)
                    {
                        Manager.RecordControlTypeChange(this);
                    }
                }
            }
        }

        private int tabIndex = 0;
        public int TabIndex
        {
            get
            {
                return tabIndex;
            }
            set
            {
                tabIndex = value;
                OnPropertyChanged("TabIndex");
            }
        }

        private int calIndex = 99;
        public int CalculatorIndex
        {
            get { return calIndex; }
            set
            {
                calIndex = value;
                //OnPropertyChanged("CalculatorIndex");
            }
        }

        private string helpMessage = string.Empty;
        public string HelpMessage
        {
            get { return helpMessage; }
            set
            {
                helpMessage = value;
                OnPropertyChanged("HelpMessage");
            }
        }

        private string comboBoxItemsString = string.Empty;
        public string ComboBoxItemsString
        {
            get { return comboBoxItemsString; }
            set
            {
                comboBoxItemsString = value;
                OnPropertyChanged("ComboBoxItemsString");
            }
        }

        private int colorIndex = 0;
        public int ColorIndex
        {
            get { return colorIndex; }
            set
            {
                colorIndex = value;
                OnPropertyChanged("ColorIndex");
            }
        }

        private string verifyCode = string.Empty;
        public string VerifyCode
        {
            get { return verifyCode; }
            set
            {
                verifyCode = value;

                OnPropertyChanged("VerifyCode");
            }
        }

        private string picture = string.Empty;
        public string Picture
        {
            get;
            set;
        }

        private bool visible = true;
        public bool Visible
        {
            get { return visible; }
            set
            {
                visible = value;
                OnPropertyChanged("Visible");
            }
        }

        public PromptItem(string name,
                      string value,
                      string controlType,
                      string helpMessage,
                      string verifyCode,
                      string comboString,
                      string color,
                      string picture,
                      string visible,
                      string hideInReport,
                      string tabIndex,
                      string calculatorIndex,
                      int i,
                      PromptsViewModel manager)
        {
            this.RowNumber = i;
            this.Manager = manager;
            LoadProperty(name, value, controlType, helpMessage, verifyCode, comboString, color, picture, visible, hideInReport, tabIndex, calculatorIndex);
            IsLoadFirstTime = false;
        }
        private ControlType oldControlType;//记录一下原有的controltype，以决定在改变可变类型时，是否触发wpf的变动机制

        public void LoadProperty(string name, string value, string controlType, string helpMessage, string verifyCode, string comboString, string color, string picture, string visible, string hideInReport, string tabIndex, string calculatorIndex)
        {
            this.Name = name;
            this.PromptValue = value;

            this.VerifyCode = verifyCode;
            setVerifier(verifyCode);

            this.ComboBoxItemsString = comboString;
            this.Picture = picture;

            int ct;
            if (int.TryParse(controlType, out ct))
            {
                ControlType controlT = (ControlType)ct;

                if (controlT == ControlType.Invisabled)
                {
                    //如果是要改成不可见的，就不需要触发
                    oldControlType = this.controlType;//记录之
                    this.controlType = controlT;
                }
                else if (this.ControlType == ControlType.Invisabled)
                {
                    //如果原有的类型是不可见的，那就要比较一下了，如果前后两个类型并不一致的，就要触发，否则就不需要
                    if (this.oldControlType == controlT)
                        this.controlType = controlT;
                    else this.ControlType = controlT;
                }
                else
                {
                    //如果原有的类型是各种类型的TextBox
                    if (this.ControlType == ControlType.TextBox || this.ControlType == ControlType.UnEditabledTextBox)
                    {
                        //实际是一个类型的控件，只是属性不同，这里不触发修改
                        if (controlT == ControlType.TextBox || controlT == ControlType.UnEditabledTextBox)
                        {
                            this.controlType = controlT;
                        }
                        else this.ControlType = controlT;
                    }
                    else this.ControlType = controlT;
                }
            }
            else if (this.ControlType == ControlType.TextBox || this.ControlType == ControlType.UnEditabledTextBox)
                this.controlType = ControlType.TextBox;
            else this.ControlType = ControlType.TextBox;

            int colorId;
            if (int.TryParse(color, out colorId))
            {
                this.ColorIndex = colorId;
            }
            else this.ColorIndex = 0;

            int vis;
            if (int.TryParse(visible, out vis))
            {
                if (vis == 0) this.Visible = false;
                else this.Visible = true;
            }
            else this.Visible = true;

            int sir;
            if (int.TryParse(hideInReport, out sir))
            {
                if (sir == 1) this.IsHideInReport = true;
                else this.IsHideInReport = false;
            }
            else this.IsHideInReport = false;

            int index;
            if (int.TryParse(tabIndex, out index))
            {
                this.TabIndex = index;
            }
            else this.TabIndex = 0;

            string[] helps = helpMessage.Split('|');
            if (helps.Length > 1)
            {
                HelpMessage = helps[1];
                Tip = helps[0];
            }
            else
            {
                HelpMessage = helpMessage;
                Tip = helpMessage;
            }

            //由控制类型决定的是否可见
            if (this.controlType == ControlType.Invisabled)
            {
                this.Visible = false;
            }
            //由控制类型决定的是否可修改
            if (this.controlType == ControlType.UnEditabledTextBox)
            {
                this.IsEnabled = false;
            }
            else this.IsEnabled = true;

            int calIndex;
            if (int.TryParse(calculatorIndex, out calIndex))
            {
                this.CalculatorIndex = calIndex;
            }
            else this.CalculatorIndex = 99;//99默认值
        }

        private void setVerifier(string verifyCode)
        {
            string[] groups = verifyCode.Split('|');
            if (groups.Length != 2)
            {
                return;
            }

            double v1, v2;
            if (double.TryParse(groups[0], out v1) && double.TryParse(groups[1], out v2))
            {
                this.HasVerfier = true;
                this.LowerBound = (v1 < v2) ? v1 : v2;
                this.UpperBound = (v1 < v2) ? v2 : v1;
            }
            else this.HasVerfier = false;
        }

        private bool isHideInReport = false;
        public bool IsHideInReport
        {
            get { return isHideInReport; }
            set
            {
                isHideInReport = value;
                OnPropertyChanged("IsHideInReport");
            }
        }

        private string tip = string.Empty;
        public string Tip
        {
            get { return tip; }
            set
            {
                tip = value;
                OnPropertyChanged("Tip");
            }
        }

        private bool isEnabled = true;
        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                isEnabled = value;
                OnPropertyChanged("IsEnabled");
            }
        }

        private string errorMessage = string.Empty;

        public string ErrorMessage
        {
            get { return errorMessage; }
            set
            {
                errorMessage = value;
                OnPropertyChanged("ErrorMessage");
            }
        }


        private void OnPropertyChanged(string word)
        {
            if (this.PropertyChanged != null)
            {
                if (!IsPassive)
                {
                    Logger.GetLogger().Debug("OnPropertyChanged:" + word);
                    this.Manager.ReloadValues(this);
                }
                this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(word));
            }
        }


        public PromptsViewModel Manager { get; set; }

        public int RowNumber { get; set; }

        /// <summary>
        /// 是否有验证器
        /// </summary>
        public bool HasVerfier { get; private set; }

        public double LowerBound { get; private set; }
        public double UpperBound { get; private set; }

        /// <summary>
        /// 被动开关，用于防止反复更新
        /// </summary>
        public bool IsPassive { get; set; }

        private bool IsLoadFirstTime = true;

        public override string ToString()
        {
            return string.Format("{0}/{1}/{2}", Name, PromptValue, ControlType);
        }

        public string Error
        {
            get { return string.Empty; }
        }

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case "PromptValue":
                        {
                            if (!this.HasVerfier)
                            {
                                break;
                            }

                            double v;
                            if (double.TryParse(this.PromptValue, out v))
                            {
                                if (v < LowerBound || v > UpperBound)
                                {
                                    string x = string.Format("输入的数字必须在{0}和{1}之间", LowerBound, UpperBound);
                                    this.ErrorMessage = x;
                                    return x;
                                }
                            }
                            else
                            {
                                this.ErrorMessage = "请输入数字";
                                return "请输入数字";
                            }

                            break;
                        }
                }
                this.ErrorMessage = string.Empty;
                return string.Empty;
            }
        }
    }
}
