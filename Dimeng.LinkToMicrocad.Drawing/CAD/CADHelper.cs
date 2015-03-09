using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Dimeng.LinkToMicrocad.Drawing.CAD
{
    public class CADHelper
    {
        public static Entity AddDwgEntities(string path, Transaction Trans)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database CurDb = doc.Database;
            ObjectIdCollection collection = new ObjectIdCollection();

            //看来不能直接新创建一个Transaction来获取结果，因为返回之后这个Trans就结束了   
            //using (Transaction Trans = CurDb.TransactionManager.StartTransaction())     
            using (Database OpenDb = new Database(false, true))
            {
                OpenDb.ReadDwgFile(path, FileShare.ReadWrite, true, "");
                using (Transaction openTrans = OpenDb.TransactionManager.StartTransaction())
                {
                    BlockTable openBt = (BlockTable)openTrans.GetObject(OpenDb.BlockTableId, OpenMode.ForWrite);
                    BlockTableRecord btr = (BlockTableRecord)openTrans.GetObject(openBt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                    foreach (var a in btr)
                    {
                        Entity entity = (Entity)openTrans.GetObject(a, OpenMode.ForWrite);
                        collection.Add(a);
                    }
                    openTrans.Commit();
                }

                BlockTable Bt = (BlockTable)Trans.GetObject(CurDb.BlockTableId, OpenMode.ForWrite);
                BlockTableRecord btr2 = (BlockTableRecord)Trans.GetObject(Bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                IdMapping map = new IdMapping();
                CurDb.WblockCloneObjects(collection, btr2.ObjectId, map, DuplicateRecordCloning.Replace, false);//把dwg文件里的文件拷贝到当前的Database       

                foreach (IdPair id in map)
                {
                    //跳过那些被删掉的东西，我也不知道为什么会被拷贝近来
                    if (id.Value.IsErased)
                    {
                        continue;
                    }

                    BlockTableRecord btr = Trans.GetObject(id.Value, OpenMode.ForWrite) as BlockTableRecord;
                    if (btr != null)
                    {
                        foreach (var a in btr)
                        {
                            Entity entity = Trans.GetObject(a, OpenMode.ForWrite) as Entity;
                            if (entity is Polyline || entity is Polyline2d)
                            {
                                return entity;
                            }
                        }
                    }

                }
                throw new System.Exception("未找到属于该dwg的多段线截面数据" + path);
            }
        }
    }
}
