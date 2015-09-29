/* Prompt 数据提示屏的展示窗口
 * Author:谢少鹏
 * 
 * PromptWindow：不放置逻辑，仅仅呈现数据
 * PromptsViewModel：SpreadSheet数据读取/产品数据维护
 * 
 * 逻辑：
 * 1、PromptViewModel读取Prompts表，获取可显变量列表
 * 2、Window负责渲染界面，并进行相应的数值绑定
 * 3、如果出现变量的类型改变（例如从textbox变成combobox），则进行重新渲染
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SpreadsheetGear;
using Dimeng.LinkToMicrocad.Logging;
using Dimeng.LinkToMicrocad.Prompts.Subassemblies;
using Dimeng.LinkToMicrocad;

namespace Dimeng.WoodEngine.Prompts
{
    /// <summary>
    /// PromptWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PromptWindow : Window
    {
        private int tabLength = 0;//作为一个记录，记录一共用多少个tab

        public PromptWindow()
        {
            InitializeComponent();
        }

        #region Protected Method
        private void init()
        {
            ViewModel.ControlTypeChangedAction = new Action<List<PromptItem>>(viewModel_ControlTypeChanged);

            tabLength = ViewModel.PromptTabs.Count;

            for (int i = 0; i < viewModel.PromptTabs.Count; i++)
            {
                string t = viewModel.PromptTabs[i];
                TabItem tab = new TabItem();
                tab.Header = t;
                tab.Tag = i;

                tabAll.Items.Add(tab);
            }
        }
        #endregion

        #region Private Method
        /// <summary>
        /// 调用计算器的方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void item_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is MenuItem))
                return;

            var menuItem = sender as MenuItem;
            var calItem = menuItem.Tag as CalculatorItem;

            if (calItem == null)
                return;

            //logger.Debug(string.Format("选中计算器:{0}/{2}/{1}",
            //                           calItem.Name,
            //                           calItem.UpperBound,
            //                           calItem.Gap));

            var prompts = viewModel.Prompts.Where(it => it.CalculatorIndex == calItem.Index);

            if (prompts.Count() == 0)
            {
                MessageBox.Show("没有变量适用于当前计算器:" + calItem.Name);
                return;
            }

            Calculator cal = new Calculator(calItem, prompts);
            if (cal.ShowDialog() == true)
            {
                foreach (var item in prompts)
                {
                    viewModel.PromptCells[item.RowNumber, 1].Value = item.PromptValue;
                }
                TabItem tab = (TabItem)tabAll.SelectedItem;
                RefreshTabContent(tab);
            }
        }

        /// <summary>
        /// 当出现控件类型变化时，刷新整个Tab
        /// </summary>
        /// <param name="prompts"></param>
        private void viewModel_ControlTypeChanged(List<PromptItem> prompts)
        {
            TabItem tab = tabAll.SelectedItem as TabItem;

            if (tab == null)
            {
                return;
            }

            //如果有prompt的ControlType发生了变化，那么就刷新页面
            int index = (int)tab.Tag;
            var numbers = prompts.Where(it => it.TabIndex == index);
            if (prompts.Where(it => it.TabIndex == index).Count() > 0)
            {
                RefreshTabContent(tab);
            }
        }

        /// <summary>
        /// 切换Tab时刷新Tab的内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabAll_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //如果没有数量就跳出？
            //貌似是个bug
            if (e.AddedItems.Count == 0)
                return;

            var tab = e.AddedItems[0] as TabItem;

            if (tab == null)
            {
                e.Handled = true;
                return;
            }

            RefreshTabContent(tab);
            e.Handled = true;
        }

        /// <summary>
        /// 具体刷新Tab内容的方法
        /// </summary>
        /// <param name="tab"></param>
        private void RefreshTabContent(TabItem tab)
        {
            //logger.Debug("刷新控件ing");

            if (tab.Tag == null)
                return;

            int index = (int)tab.Tag;

            var prompts = viewModel.Prompts.Where(it => it.TabIndex == index)
                                           .ToList();

            Grid grid = new Grid();

            ScrollViewer viewer = new ScrollViewer();
            viewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            viewer.Content = grid;

            tab.Content = viewer;

            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(10) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength() });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(20) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(230) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(220) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(250, GridUnitType.Star) });
            StackPanel leftSP = new StackPanel();
            leftSP.Margin = new Thickness(5, 0, 30, 0);
            leftSP.SetValue(Grid.RowProperty, 1);
            leftSP.SetValue(Grid.ColumnProperty, 1);
            StackPanel middleSP = new StackPanel();
            middleSP.Margin = new Thickness(30, 0, 30, 0);
            middleSP.SetValue(Grid.RowProperty, 1);
            middleSP.SetValue(Grid.ColumnProperty, 2);
            StackPanel rightSP = new StackPanel();
            rightSP.Margin = new Thickness(30, 0, 0, 0);
            rightSP.SetValue(Grid.RowProperty, 1);
            rightSP.SetValue(Grid.ColumnProperty, 3);

            grid.Children.Add(leftSP);
            grid.Children.Add(middleSP);
            grid.Children.Add(rightSP);

            for (int i = 0; i < prompts.Count(); i++)
            {
                var prompt = prompts[i];

                if (prompt.IsBelowBlankRow)
                {
                    continue;
                }

                if (prompt.ControlType == ControlType.TextBox
                    || prompt.ControlType == ControlType.UnEditabledTextBox
                    || prompt.ControlType == ControlType.Invisabled)
                {
                    Label label = new Label();
                    label.Foreground = getBrushById(prompt.ColorIndex);
                    label.DataContext = prompt;
                    label.SetBinding(Label.ContentProperty, new Binding("Name"));
                    label.SetBinding(Label.VisibilityProperty, new Binding("Visible") { Converter = new PromptValueToVisibilityConverter() });
                    label.SetBinding(Label.ToolTipProperty, new Binding("HelpMessage"));
                    leftSP.Children.Add(label);

                    TextBox tb = new TextBox();
                    //tb.MouseDoubleClick += tb_MouseDoubleClick;
                    //tb.AddHandler(UIElement.MouseEnterEvent, new RoutedEventHandler(tb_MouseLeftButtonDown), true);
                    tb.DataContext = prompt;
                    tb.SetBinding(TextBox.TextProperty, new Binding("PromptValue"));
                    tb.SetBinding(TextBox.VisibilityProperty, new Binding("Visible") { Converter = new PromptValueToVisibilityConverter() });
                    tb.SetBinding(TextBox.IsEnabledProperty, new Binding("IsEnabled"));
                    tb.SetBinding(TextBox.ToolTipProperty, new Binding("HelpMessage"));
                    tb.MouseEnter += mouseEnter;
                    leftSP.Children.Add(tb);

                    TextBlock error = new TextBlock();
                    error.Foreground = Brushes.Red;
                    error.DataContext = prompt;
                    error.SetBinding(TextBlock.TextProperty, new Binding("ErrorMessage"));
                    error.SetBinding(Label.VisibilityProperty, new Binding("ErrorMessage") { Converter = new PromptValueToVisibilityConverter() });
                    leftSP.Children.Add(error);
                }
                else if (prompt.ControlType == ControlType.CheckBox)
                {
                    CheckBox cb = new CheckBox();
                    cb.Foreground = getBrushById(prompt.ColorIndex);
                    cb.Margin = new Thickness(0, 3, 0, 3);
                    cb.DataContext = prompt;
                    //cb.AddHandler(UIElement.MouseEnterEvent, new RoutedEventHandler(tb_MouseLeftButtonDown), true);
                    cb.SetBinding(CheckBox.IsCheckedProperty, new Binding("PromptValue") { Converter = new PromptValueToCheckedConverter() });
                    cb.SetBinding(CheckBox.ContentProperty, new Binding("Name"));
                    cb.SetBinding(CheckBox.VisibilityProperty, new Binding("Visible") { Converter = new PromptValueToVisibilityConverter() });
                    cb.SetBinding(CheckBox.ToolTipProperty, new Binding("HelpMessage"));
                    cb.MouseEnter += mouseEnter;
                    rightSP.Children.Add(cb);
                }
                else if (prompt.ControlType == ControlType.ComboBox)
                {
                    Label label = new Label();
                    label.Foreground = getBrushById(prompt.ColorIndex);
                    label.DataContext = prompt;
                    label.SetBinding(Label.ContentProperty, new Binding("Name"));
                    label.SetBinding(Label.VisibilityProperty, new Binding("Visible") { Converter = new PromptValueToVisibilityConverter() });
                    label.SetBinding(Label.ToolTipProperty, new Binding("HelpMessage"));
                    leftSP.Children.Add(label);

                    ComboBox combo = new ComboBox();
                    combo.Foreground = getBrushById(prompt.ColorIndex);
                    combo.DataContext = prompt;
                    //combo.AddHandler(UIElement.MouseEnterEvent, new RoutedEventHandler(tb_MouseLeftButtonDown), true);
                    combo.SetBinding(ComboBox.SelectedValueProperty, new Binding("PromptValue") { Converter = new PromptValueToIntConverter() });//
                    combo.SetBinding(ComboBox.VisibilityProperty, new Binding("Visible") { Converter = new PromptValueToVisibilityConverter() });
                    combo.SetBinding(ComboBox.ToolTipProperty, new Binding("HelpMessage"));
                    combo.MouseEnter += mouseEnter;
                    combo.ItemsSource = prompt.ComboBoxItemsString.Split('|');

                    //DataTemplate dt = new DataTemplate();
                    //dt.DataType = typeof(string);
                    //FrameworkElementFactory spFactory = new FrameworkElementFactory(typeof(TextBlock));

                    leftSP.Children.Add(combo);
                }
                else if (prompt.ControlType == ControlType.RadioButton)
                {
                    GroupBox gb = new GroupBox();
                    gb.Foreground = getBrushById(prompt.ColorIndex);
                    gb.DataContext = prompt;
                    gb.SetBinding(GroupBox.HeaderProperty, new Binding("Tip"));
                    gb.SetBinding(GroupBox.VisibilityProperty, new Binding("Visible") { Converter = new PromptValueToVisibilityConverter() });
                    middleSP.Children.Add(gb);

                    StackPanel spinsder = new StackPanel();
                    gb.Content = spinsder;

                    RadioButton rb = new RadioButton();
                    rb.Foreground = getBrushById(prompt.ColorIndex);
                    rb.Margin = new Thickness(0, 5, 0, 5);
                    rb.DataContext = prompt;
                    //rb.AddHandler(UIElement.MouseEnterEvent, new RoutedEventHandler(tb_MouseLeftButtonDown), true);
                    rb.SetBinding(RadioButton.ContentProperty, new Binding("Name"));
                    rb.SetBinding(RadioButton.IsCheckedProperty, new Binding("PromptValue") { Converter = new PromptValueToCheckedConverter() });
                    rb.SetBinding(RadioButton.VisibilityProperty, new Binding("Visible") { Converter = new PromptValueToVisibilityConverter() });
                    rb.MouseEnter += mouseEnter;
                    spinsder.Children.Add(rb);

                    for (int x = i + 1; x < prompts.Count; x++)
                    {
                        var nextPrompt = prompts[x];
                        if (nextPrompt.ControlType != ControlType.OtherRadioButton)
                        {
                            break;
                        }

                        RadioButton rb2 = new RadioButton();
                        rb2.Foreground = getBrushById(prompt.ColorIndex);
                        rb2.Margin = new Thickness(0, 5, 0, 5);
                        rb2.DataContext = nextPrompt;
                        //rb2.AddHandler(UIElement.MouseEnterEvent, new RoutedEventHandler(tb_MouseLeftButtonDown), true);
                        rb2.SetBinding(RadioButton.ContentProperty, new Binding("Name"));
                        rb2.SetBinding(RadioButton.IsCheckedProperty, new Binding("PromptValue") { Converter = new PromptValueToCheckedConverter() });
                        rb2.SetBinding(RadioButton.VisibilityProperty, new Binding("Visible") { Converter = new PromptValueToVisibilityConverter() });
                        rb2.MouseEnter += mouseEnter;
                        spinsder.Children.Add(rb2);

                    }
                }
            }
        }

        void mouseEnter(object sender, MouseEventArgs e)
        {
            PromptItem p = ((Control)e.OriginalSource).DataContext as PromptItem;
            if (p == null)
            {
                Logger.GetLogger().Debug("promptItem is null");
            }
            //if(string.IsNullOrEmpty(p.Picture))
            //{
            //    return;
            //}

            string imagePath = getImagePath(p);

            if (System.IO.File.Exists(imagePath))
            {
                this.image.Source = new BitmapImage(new Uri(imagePath));
            }
            else
            {
                this.image.Source = null;
            }
        }

        void tb_GotFocus(object sender, RoutedEventArgs e)
        {
            PromptItem p = ((TextBox)e.OriginalSource).DataContext as PromptItem;
            if (p == null)
            {
                Logger.GetLogger().Debug("promptItem is null");
            }
            //if(string.IsNullOrEmpty(p.Picture))
            //{
            //    return;
            //}

            string imagePath = getImagePath(p);

            if (System.IO.File.Exists(imagePath))
            {
                this.image.Source = new BitmapImage(new Uri(imagePath));
            }
            else
            {
                this.image.Source = null;
            }
        }

        private static string getImagePath(PromptItem p)
        {
            string imagePath = System.IO.Path.Combine(Context.GetContext().MVDataContext.GetLatestRelease().MicrovellumData,
                "Graphics", "Product Prompt Pictures", p.Picture);
            return imagePath;
        }

        /// <summary>
        /// 点击刷新按钮的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshMenuItem_Click(object sender, RoutedEventArgs e)
        {
            TabItem tab = (TabItem)tabAll.SelectedItem;
            RefreshTabContent(tab);
        }

        /// <summary>
        /// 点击OK
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        /// <summary>
        /// 点击Cancel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        /// <summary>
        /// 控制控件颜色的转换
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private SolidColorBrush getBrushById(int id)
        {
            switch (id)
            {
                case 1:
                    return Brushes.Red;
                case 2:
                    return Brushes.Yellow;
                case 3:
                    return Brushes.Green;
                case 4:
                    return Brushes.DarkBlue;
                case 5:
                    return Brushes.Blue;
                case 6:
                    return Brushes.OrangeRed;
                case 9:
                    return Brushes.Gray;
            }


            return Brushes.Black;
        }

        #endregion

        #region Properties
        private PromptsViewModel viewModel;
        public PromptsViewModel ViewModel
        {
            get
            {
                return viewModel;
            }
            set
            {
                viewModel = value;

                this.DataContext = viewModel;

                init();
            }
        }
        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SpreadExplorer se = new SpreadExplorer(this.viewModel.Book);
            se.ShowDialog();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }

        private void SubassemblyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SubassembliesViewModel viewmodel =
                new SubassembliesViewModel(this.ViewModel.Book.Worksheets["Subassemblies"].Cells);
            viewmodel.BookSet = this.ViewModel.BookSet;
            viewmodel.Library = this.ViewModel.Library;
            viewmodel.ProjectPath = this.ViewModel.ProjectPath;
            viewmodel.Handle = this.ViewModel.Handle;
            Subassemblies window = new Subassemblies(viewmodel);
            window.ShowDialog();
        }

        private void ContextMenu_Loaded(object sender, RoutedEventArgs e)
        {
            //刷新页面，更新计算器
            ViewModel.ReloadCalculators();

            this.CalculatorMenuItem.Items.Clear();

            for (int i = 0; i < viewModel.Calcuators.Count; i++)
            {
                MenuItem item = new MenuItem();
                item.Click += item_Click;
                item.Tag = viewModel.Calcuators[i];
                item.Header = viewModel.Calcuators[i].Name;
                this.CalculatorMenuItem.Items.Add(item);
                this.CalculatorMenuItem.IsEnabled = true;//改为可用
            }
        }

        /// <summary>
        /// 禁止在控件上右键弹出菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabAll_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            DependencyObject depObj = (DependencyObject)e.OriginalSource;
            Logger.GetLogger().Debug("depObj is " + depObj.GetType());
            while ((depObj != null) && !(depObj is ComboBox))
            {
                depObj = VisualTreeHelper.GetParent(depObj);
            }

            if (depObj == null)
            {
                return;
            }

            if (!(depObj is ComboBox))
            {
                Logger.GetLogger().Debug("depObj is not combox!");
                tabAll.ContextMenu.Visibility = Visibility.Visible;
            }
            else
            {
                Logger.GetLogger().Debug("depOjb is combox!");
                tabAll.ContextMenu.Visibility = Visibility.Collapsed;
            }
        }
    }
}

