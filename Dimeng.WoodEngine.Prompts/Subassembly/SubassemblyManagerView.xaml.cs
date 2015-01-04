using System.Windows;
using Dimeng.WoodEngine.Entities;
using SpreadsheetGear;
using Microsoft.Practices.Unity;
using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;
using Dimeng.WoodEngine.Entities.Events;

namespace Dimeng.WoodEngine.Prompts
{
    /// <summary>
    /// SubassemblyManager.xaml 的交互逻辑
    /// </summary>
    public partial class SubassemblyManagerView : Window
    {
        SubassemblyManagerViewModel viewmodel;
        public SubassemblyManagerView(App app, Logger logger)
        {
            InitializeComponent();
        }

        [Dependency]
        public SubassemblyManagerViewModel ViewModel
        {
            get
            {
                return this.viewmodel;
            }
            set
            {
                this.DataContext = value;
                this.viewmodel = value;
            }
        }

        private void DataGridTextColumn_MouseClick(object sender, MouseEventArgs e)
        {
            var ds = sender as System.Windows.Controls.DataGridCell;
            if(ds!=null)
            {
                SubassemblyItem si = ds.DataContext as SubassemblyItem;

                int index = ds.Column.DisplayIndex;
                switch (index)
                {
                    case 1:
                        Messenger.Default.Send<ToModifyCellFormulaEvent>(new ToModifyCellFormulaEvent() { Element = si.Name });
                        break;
                    case 2:
                        Messenger.Default.Send<ToModifyCellFormulaEvent>(new ToModifyCellFormulaEvent() { Element = si.Qty });
                        break;
                    case 3:
                        Messenger.Default.Send<ToModifyCellFormulaEvent>(new ToModifyCellFormulaEvent() { Element = si.Width });
                        break;
                    case 4:
                        Messenger.Default.Send<ToModifyCellFormulaEvent>(new ToModifyCellFormulaEvent() { Element = si.Height });
                        break;
                    case 5:
                        Messenger.Default.Send<ToModifyCellFormulaEvent>(new ToModifyCellFormulaEvent() { Element = si.Depth });
                        break;
                    case 6:
                        Messenger.Default.Send<ToModifyCellFormulaEvent>(new ToModifyCellFormulaEvent() { Element = si.XOrigin });
                        break;
                    case 7:
                        Messenger.Default.Send<ToModifyCellFormulaEvent>(new ToModifyCellFormulaEvent() { Element = si.YOrigin });
                        break;
                    case 8:
                        Messenger.Default.Send<ToModifyCellFormulaEvent>(new ToModifyCellFormulaEvent() { Element = si.ZOrigin });
                        break;
                    case 9:
                        Messenger.Default.Send<ToModifyCellFormulaEvent>(new ToModifyCellFormulaEvent() { Element = si.ZRotation });
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
