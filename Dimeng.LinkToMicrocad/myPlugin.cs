// (C) Copyright 2014 by  
//
using System;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;

// This line is not mandatory, but improves loading performances
[assembly: ExtensionApplication(typeof(Dimeng.LinkToMicrocad.MyPlugin))]

namespace Dimeng.LinkToMicrocad
{
    public class MyPlugin : IExtensionApplication
    {
        void IExtensionApplication.Initialize()
        {
            Dimeng.LinkToMicrocad.Logging.ConsoleHelper.Show();
        }

        void IExtensionApplication.Terminate()
        {
            // Do plug-in application clean up here
            Dimeng.LinkToMicrocad.Logging.ConsoleHelper.Hide();
        }

    }

}
