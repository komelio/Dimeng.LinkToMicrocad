using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Dimeng.WoodEngine.Entities;
using Dimeng.WoodEngine.Entities.Machinings;
using Dimeng.LinkToMicrocad.Logging;

namespace Dimeng.LinkToMicrocad.Drawing
{
    public class VDrillDrawer
    {
        Database db;
        public VDrillDrawer(Database db)
        {
            this.db = db;
        }
        public void Draw(ObjectId panelId, Part part)
        {
            if (part.VDrillings.Count > 0)
            {
                Logger.GetLogger().Debug(string.Format("开始绘制板件的垂直钻孔,板件名称[{0}],垂直孔数量[{1}]", part.PartName, part.VDrillings.Count));
            }

            foreach (var vdrill in part.VDrillings)
            {
                using (Transaction tran = db.TransactionManager.StartTransaction())
                {
                    Logger.GetLogger().Debug(string.Format("垂直孔信息,直径{0}/孔深{1}/所在面{2}/坐标X{3},Y{4}",
                                               vdrill.Diameter, vdrill.Depth, vdrill.FaceNumber, vdrill.DimX, vdrill.DimY));

                    if (vdrill.Diameter <= 0 || vdrill.Depth <= 0)
                    {
                        Logger.GetLogger().Warn("Vdrill`s diameter or depth could be zero!");
                        continue;
                    }

                    Solid3d panel = tran.GetObject(panelId, OpenMode.ForWrite) as Solid3d;

                    //Step1:创建三维实体
                    Solid3d vbore = new Solid3d();
                    vbore.CreateFrustum(vdrill.Depth * 2, vdrill.Diameter / 2, vdrill.Diameter / 2, vdrill.Diameter / 2);

                    //Step2:获取孔以MP为原点的坐标，并以此坐标进行旋转
                    Point3d vborePosition = GetBorePosition(vdrill);
                    vborePosition = vborePosition.TransformBy(Matrix3d.AlignCoordinateSystem(new Point3d(),
                                                                                             Vector3d.XAxis,
                                                                                             Vector3d.YAxis,
                                                                                             Vector3d.ZAxis,
                                                                                             vdrill.Part.MPPoint,
                                                                                             vdrill.Part.MPXAxis,
                                                                                             vdrill.Part.MPYAxis,
                                                                                             vdrill.Part.MPZAxis));
                    //Logger.GetLogger().Error(vborePosition.ToString());

                    vbore.TransformBy(Matrix3d.AlignCoordinateSystem(new Point3d(),
                                                                     Vector3d.XAxis,
                                                                     Vector3d.YAxis,
                                                                     Vector3d.ZAxis,
                                                                     vborePosition,
                                                                     vdrill.Part.MPXAxis,
                                                                     vdrill.Part.MPYAxis,
                                                                     vdrill.Part.MPZAxis));

                    panel.BooleanOperation(BooleanOperationType.BoolSubtract, vbore);
                    tran.Commit();
                }
            }
        }

        private Point3d GetBorePosition(VDrilling vdrill)
        {
            if (vdrill.FaceNumber == 5) return new Point3d(vdrill.DimX, vdrill.DimY, 0);
            else return new Point3d(vdrill.DimX, vdrill.DimY, vdrill.Part.Thickness);
        }
    }
}
