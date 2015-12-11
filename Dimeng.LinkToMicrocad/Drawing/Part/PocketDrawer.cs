using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Dimeng.LinkToMicrocad.Logging;
using Dimeng.WoodEngine.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.LinkToMicrocad.Drawing
{
    public class PocketDrawer
    {
        Database db;
        public PocketDrawer(Database db)
        {
            this.db = db;
        }
        public void Draw(ObjectId panelId, Part part)
        {
            using (Transaction tran = db.TransactionManager.StartTransaction())
            {
                Solid3d panel = tran.GetObject(panelId, OpenMode.ForWrite) as Solid3d;

                if (part.Pockets.Count > 0)
                {
                    Logger.GetLogger().Debug(string.Format("开始绘制板件的Pocket,板件名称[{0}],数量[{1}]", part.PartName, part.Pockets.Count));
                }

                foreach (var pocket in part.Pockets)
                {
                    Matrix3d matrix = Matrix3d.AlignCoordinateSystem(new Point3d(),
                                                                     Vector3d.XAxis,
                                                                     Vector3d.YAxis,
                                                                     Vector3d.ZAxis,
                                                                     part.MPPoint,
                                                                     part.MPXAxis,
                                                                     part.MPYAxis,
                                                                     part.MPZAxis);

                    var points = pocket.Points.Select(it => it.TransformBy(matrix)).ToList();//转到wcs中

                    using (Polyline pl = new Polyline())
                    {
                        for (int i = 0; i < pocket.Points.Count; i++)
                        {
                            pl.AddVertexAt(i, new Point2d(points[i].X, points[i].Y), -pocket.Bulges[i], 0, 0);
                        }
                        pl.Closed = true;

                        if (pocket.OnTopFace)
                        {
                            double startZ = pocket.Part.Thickness / 2 - pocket.Depth;
                            pl.TransformBy(Matrix3d.Displacement(new Vector3d(0, 0, startZ)));//移动到指定深度
                            Solid3d pocketSolid = new Solid3d();
                            pocketSolid.CreateExtrudedSolid(pl, new Vector3d(0, 0, 100), new SweepOptions());

                            panel.BooleanOperation(BooleanOperationType.BoolSubtract, pocketSolid);
                            pocketSolid.Dispose();
                        }
                        else
                        {
                            double startZ = -pocket.Part.Thickness / 2 + pocket.Depth;
                            pl.TransformBy(Matrix3d.Displacement(new Vector3d(0, 0, startZ)));//移动到指定深度
                            Solid3d pocketSolid = new Solid3d();
                            pocketSolid.CreateExtrudedSolid(pl, new Vector3d(0, 0, -100), new SweepOptions());

                            panel.BooleanOperation(BooleanOperationType.BoolSubtract, pocketSolid);
                            pocketSolid.Dispose();
                        }
                    }
                }

                tran.Commit();
            }
        }
    }
}
