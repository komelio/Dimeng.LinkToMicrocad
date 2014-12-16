// (C) Copyright 2014 by  
//
using System;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;

// This line is not mandatory, but improves loading performances
[assembly: ExtensionApplication(typeof(Dimeng.TestMicrocad.MyPlugin))]

namespace Dimeng.TestMicrocad
{
    public class MyPlugin : IExtensionApplication
    {
        void IExtensionApplication.Initialize()
        {
            
        }

        void IExtensionApplication.Terminate()
        {
            // Do plug-in application clean up here
        }

    }

}
