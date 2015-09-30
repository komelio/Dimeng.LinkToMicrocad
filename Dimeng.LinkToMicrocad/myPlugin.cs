using System;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using Dimeng.LinkToMicrocad.Logging;

[assembly: ExtensionApplication(typeof(Dimeng.LinkToMicrocad.MyPlugin))]

namespace Dimeng.LinkToMicrocad
{
    public class MyPlugin : IExtensionApplication
    {
        void IExtensionApplication.Initialize()
        {
#if DEBUG
            Logging.ConsoleHelper.Show();
#endif
            Logger.GetLogger().Debug("-----------");
            Logger.GetLogger().Debug("Program started");
            Logger.GetLogger().Debug("-----------");

            Context.GetContext().Init();
        }

        void IExtensionApplication.Terminate()
        {
        }
    }

}