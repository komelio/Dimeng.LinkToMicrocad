using Dimeng.LinkToMicrocad.Web.Domain.Abstract;
using Dimeng.LinkToMicrocad.Web.Domain.Entities;
using SpreadsheetGear;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dimeng.LinkToMicrocad.Web.Business
{
    public class MVMaterialImporter
    {
        private IMaterialRepository repository;

        public MVMaterialImporter(IMaterialRepository repo)
        {
            this.repository = repo;
        }

        public void Import(string ctpxfilePath)
        {
            if (!File.Exists(ctpxfilePath))
            {
                throw new Exception("没有找到ctpx文件进行导入");
            }

            IWorkbook book = Factory.GetWorkbook(ctpxfilePath);
            for (int i = 1; i < 4; i++)//读取三个表
            {
                var sheet = book.Worksheets[i];
                var allCells = sheet.Cells;

                for(int x=0;x<allCells.Rows.RowCount;x++)
                {
                    string name = allCells[x, 0].Text;
                    if(string.IsNullOrEmpty(name.Trim()))
                    {
                        break;
                    }

                    Material material = new Material();
                    material.Name = name;

                    repository.Add(material);
                }
            }
        }
    }
}
