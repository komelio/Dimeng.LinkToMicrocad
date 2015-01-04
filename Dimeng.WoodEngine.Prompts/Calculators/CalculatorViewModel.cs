using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Prompts
{
    public class CalculatorViewModel : ViewModelBase
    {
        private CalculatorItem calItem;

        private CalculatorViewModel()
        {
            Items = new List<OneCalculatorViewModel>();

            CalculateCommand = new RelayCommand(this.Calculate);

            MakePromptValueCommand = new RelayCommand(this.SaveValue);
        }

        private void SaveValue()
        {
            foreach(var one in this.Items)
            {
                one.PromptItem.PromptValue = one.Value;
            }
        }

        public CalculatorViewModel(CalculatorItem calItem, IEnumerable<PromptItem> prompts)
            : this()
        {
            this.calItem = calItem;

            foreach (var prompt in prompts)
            {
                OneCalculatorViewModel one = new OneCalculatorViewModel();
                one.Name = prompt.Name;
                one.Value = prompt.PromptValue;
                one.IsEqual = false;
                one.PromptItem = prompt;

                Items.Add(one);
            }
        }

        private void Calculate()
        {
            double total = 0;
            foreach (var i in Items)
            {
                if (!i.IsEqual)
                {
                    double v;
                    if (double.TryParse(i.Value, out v))
                    {
                        total += v;
                        total += calItem.Gap;
                    }
                }
            }

            int count = Items.Count(it => it.IsEqual);
            if (count == 0)
                return;

            double rest = (calItem.UpperBound - total) / count;

            foreach (var i in Items.Where(it => it.IsEqual))
            {
                //TODO:这里强制取了两位小数
                //出现了硬值，将来应该怎么搞定？
                i.Value = Math.Round(rest, 2).ToString();
            }
        }

        public List<OneCalculatorViewModel> Items { get; private set; }

        public RelayCommand CalculateCommand { get; private set; }
        public RelayCommand MakePromptValueCommand { get; private set; }
    }
}
