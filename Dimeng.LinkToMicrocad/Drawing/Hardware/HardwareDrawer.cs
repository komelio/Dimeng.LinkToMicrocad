using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Dimeng.LinkToMicrocad.Logging;
using Dimeng.WoodEngine.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Dimeng.LinkToMicrocad.Drawing
{
    public class HardwareDrawer
    {
        Database db;
        IMVLibrary library;
        string path;

        public HardwareDrawer(Database db, IMVLibrary library)
        {
            this.db = db;
            this.library = library;
            path = Path.Combine(library.MicrovellumData, "Graphics", "Hardware");
        }

        public void Draw(Hardware hw)
        {
            Logger.GetLogger().Debug(string.Format("Start drawing hardware:{0}/{1}/{2}/{3}",hw.Name,hw.TXOrigin,hw.TYOrigin,hw.TZOrigin));
            Logger.GetLogger().Debug(string.Format("- Associate Part:{0}",hw.AssociatedPart.PartName));

            string hwDWGFile = Path.Combine(path, string.Format("3D_{0}.dwg", hw.Name));
            if (!File.Exists(hwDWGFile))
            {
                Logger.GetLogger().Info(string.Format("File {0} not found!", hwDWGFile));
                return;
            }

            //CAD.CADHelper.AddDwgAsBlock(db, new FileInfo(hwDWGFile), Point3d.Origin, Vector3d.XAxis, Vector3d.YAxis, Vector3d.ZAxis, 0);
            CAD.CADHelper.AddDwgAsBlock(
                db,
                new FileInfo(hwDWGFile),
                new Point3d(hw.TXOrigin, hw.TYOrigin, hw.TZOrigin),
                hw.AssociatedPart.MovedOrginXAxis,
                hw.AssociatedPart.MovedOrginYAxis,
                (hw.OnTopFace) ? hw.AssociatedPart.MovedOrginZAxis : -hw.AssociatedPart.MovedOrginZAxis,
                (!hw.OnTopFace) ? -hw.TAssociatedRotation : hw.TAssociatedRotation
                );
        }


    }
}
