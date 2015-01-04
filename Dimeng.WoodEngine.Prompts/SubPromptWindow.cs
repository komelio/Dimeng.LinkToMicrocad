using Dimeng.WoodEngine.Entities;
using SpreadsheetGear;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Prompts
{
    public class SubPromptWindow : PromptWindow
    {
        protected SubPromptWindow(Logger logger, App app)
            : base(logger, app)
        {
            base.gridProductInfo.Height = 0;
            base.SubassemblyMenuItem.Visibility = System.Windows.Visibility.Collapsed;
        }

        public SubPromptWindow(Logger logger, App app, string subCutxFilePath, IWorkbookSet books, SubassemblyItem item)
            : this(logger, app)
        {
            this.Manager = new PromptsViewModel(subCutxFilePath, item.Width.PropertyValue, item.Height.PropertyValue, item.Depth.PropertyValue, books, logger);

            Init();
        }
    }
}
