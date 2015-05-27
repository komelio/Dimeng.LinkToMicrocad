using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using PolylineGetter.Algorithm;
using PolylineGetter.Tools;

namespace PolylineGetter.Algorithm
{
    public class CADHelper
    {
        /// <summary> 
        /// </summary> 
        /// <param name="message">选择提示</param> 
        /// <returns>实体对象</returns> 
        public static Polyline GetPolyline(string message)
        {
            Database db = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database;
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            Polyline entity = null;
            PromptEntityResult ent = ed.GetEntity(message);
            if (ent.Status == PromptStatus.OK)
            {
                using (Transaction transaction = db.TransactionManager.StartTransaction())
                {
                    entity = (Polyline)transaction.GetObject(ent.ObjectId, OpenMode.ForRead, true);

                    transaction.Commit();
                }
            }
            return entity;
        }

        public static void PrintAllPolylineData(Polyline pline)
        {
            Database db = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database;
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;

            ed.WriteMessage("多段线的点数据包括:\n");
            for (int i = 0; i < pline.NumberOfVertices; i++)
            {
                ed.WriteMessage(pline.GetPoint2dAt(i).ToString() + "\n");
            }

            ed.WriteMessage("多段线的供高比数据包括:\n");
            for (int i = 0; i < pline.NumberOfVertices; i++)
            {
                ed.WriteMessage(pline.GetBulgeAt(i).ToString() + "\n");
            }

            ed.WriteMessage("FinishPrint......\n");
        }

        public static void PrintMyPolyline(MyPolyline mp, int colorIndex)
        {
            Polyline r = mytools.ConvertToPolyline(mp);
            r.ColorIndex = colorIndex;

            //打印坐标
            CADHelper.PrintAllPolylineData(r);

            //画
            Polyline temp = new Polyline();
            for (int ind = 0; ind < r.NumberOfVertices; ind++)
            {
                Point2d p2d = new Point2d(r.GetPoint2dAt(ind).X, r.GetPoint2dAt(ind).Y);
                temp.AddVertexAt(ind, p2d, r.GetBulgeAt(ind), 10, 10);
            }
            temp.Closed = true;
            CADHelper.AddLightweightPolyline(temp);
        }

        public static void AddLightweightPolyline(Polyline acPoly)
        {
            //获取当前图形文档和数据库
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            //开始一个事务
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // 以只读的方式打开块表
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId,
                                             OpenMode.ForRead) as BlockTable;

                //以写的方式打开块记录
                BlockTableRecord acBlkTblRec;
                acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                                                OpenMode.ForWrite) as BlockTableRecord;

                //把多段线对象添加到块记录和事务
                acBlkTblRec.AppendEntity(acPoly);
                acTrans.AddNewlyCreatedDBObject(acPoly, true);


                //提交事务,把点对象保存到数据库中
                acTrans.Commit();
            }
        }
    }
}
