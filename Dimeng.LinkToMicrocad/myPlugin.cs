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
            Dimeng.LinkToMicrocad.Logging.ConsoleHelper.Show();

            Logger.GetLogger().Debug("-----------");
            Logger.GetLogger().Debug("Program started");
            Logger.GetLogger().Debug("-----------");
        }

        void IExtensionApplication.Terminate()
        {
            // Do plug-in application clean up here
            Dimeng.LinkToMicrocad.Logging.ConsoleHelper.Hide();
        }

    }

}
