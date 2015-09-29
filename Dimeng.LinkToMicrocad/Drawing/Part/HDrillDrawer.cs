using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Dimeng.LinkToMicrocad.Logging;
using Dimeng.WoodEngine.Entities;
using Dimeng.WoodEngine.Entities.Machinings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.LinkToMicrocad.Drawing
{
    public class HDrillDrawer
    {
        private Database db;

        public HDrillDrawer(Database db)
        {
            this.db = db;
        }

        public void Draw(ObjectId panelId, Part p)
        {
            if (p.HDrillings.Count > 0)
            {
                Logger.GetLogger().Debug(string.Format("开始绘制板件的水平钻孔,板件名称[{0}],水平孔数量[{1}]", p.PartName, p.HDrillings.Count));
            }

            foreach (var hdrill in p.HDrillings)
            {
                Logger.GetLogger().Debug(string.Format("水平孔信息,直径{0}/孔深{1}/所在面{2}/坐标{3}",
                                           hdrill.Diameter, hdrill.Depth, hdrill.FaceNumber, hdrill.Position));

                if (hdrill.Diameter <= 0 || hdrill.Depth <= 0)
                {
                    Logger.GetLogger().Warn("hdrill`s diameter or depth could be zero!");
                    continue;
                }

                using (Transaction tran = db.TransactionManager.StartTransaction())
                {
                    Solid3d panel = tran.GetObject(panelId, OpenMode.ForWrite) as Solid3d;

                    //Step1:创建三维实体
                    Solid3d hole = new Solid3d();
                    hole.CreateFrustum(hdrill.Depth * 2, hdrill.Diameter / 2, hdrill.Diameter / 2, hdrill.Diameter / 2);

                    //Step2：在原点先进行旋转
                    hole.TransformBy(Matrix3d.Rotation(System.Math.PI / 2, GetBoreDirection(hdrill), new Point3d()));//将水平孔的实体根据孔是左右方向或水平方向进行旋转

                    //Step3:获取孔以MP为原点的坐标,并以此坐标进行旋转
                    Point3d holePosition = GetBorePosition(hdrill);
                    holePosition = holePosition.TransformBy(Matrix3d.AlignCoordinateSystem(new Point3d(),
                                                                                           Vector3d.XAxis,
                                                                                           Vector3d.YAxis,
                                                                                           Vector3d.ZAxis,
                                                                                           hdrill.Part.MPPoint,
                                                                                           hdrill.Part.MPXAxis,
                                                                                           hdrill.Part.MPYAxis,
                                                                                           hdrill.Part.MPZAxis));

                    hole.TransformBy(Matrix3d.AlignCoordinateSystem(new Point3d(),
                                                                    Vector3d.XAxis,
                                                                    Vector3d.YAxis,
                                                                    Vector3d.ZAxis,
                                                                    holePosition,
                                                                    hdrill.Part.MPXAxis,
                                                                    hdrill.Part.MPYAxis,
                                                                    hdrill.Part.MPZAxis));
                    panel.BooleanOperation(BooleanOperationType.BoolSubtract, hole);//对板件的实体做差集，形成孔位
                    tran.Commit();
                }
            }
        }

        private Vector3d GetBoreDirection(HDrilling hdrill)
        {
            Vector3d holeDirection = new Vector3d();
            if (hdrill.Part.MachinePoint.IsRotated)
            {
                if (hdrill.FaceNumber == 3 || hdrill.FaceNumber == 4)
                    holeDirection = Vector3d.XAxis;
                else holeDirection = Vector3d.YAxis;
            }
            else
            {
                if (hdrill.FaceNumber == 1 || hdrill.FaceNumber == 2)
                    holeDirection = Vector3d.XAxis;
                else holeDirection = Vector3d.YAxis;
            }
            return holeDirection;
        }

        private Point3d GetBorePosition(HDrilling hdrill)
        {
            Point3d holePosition = new Point3d();
            if (hdrill.FaceNumber == 1)
                holePosition = new Point3d(hdrill.Position, 0, hdrill.ZValue);
            else if (hdrill.FaceNumber == 2)
                holePosition = new Point3d(hdrill.Position, hdrill.Part.Width, hdrill.ZValue);
            else if (hdrill.FaceNumber == 3)
                holePosition = new Point3d(0, hdrill.Position, hdrill.ZValue);
            else if (hdrill.FaceNumber == 4)
                holePosition = new Point3d(hdrill.Part.Length, hdrill.Position, hdrill.ZValue);
            return holePosition;
        }
    }
}
