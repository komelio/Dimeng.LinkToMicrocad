using Dimeng.WoodEngine.Entities;
using SpreadsheetGear;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Dimeng.LinkToMicrocad
{
    public class ExcelBuilder
    {
        PauchieProduct product;
        public ExcelBuilder(PauchieProduct product)
        {
            this.product = product;
        }

        public void Build(IWorkbook book)
        {
            buildPartsSheet(book);
            buildHardwareSheet(book);
        }

        private void buildPartsSheet(IWorkbook book)
        {
            var sheet1 = book.Worksheets.Add();
            sheet1.Name = "柜体";
            var cells = sheet1.Cells;
            cells[0, 0].Value = "产品种类";
            cells[0, 1].Value = "柜体";
            cells[0, 2].Value = "投影面积";
            cells[0, 4].Value = "标准产品?";
            cells[0, 5].Value = "N";
            cells[0, 6].Value = "订购套数";
            cells[0, 7].Value = product.Qty;
            cells[1, 0].Value = "产品颜色";
            cells[1, 1].Value = product.Color;
            cells[1, 2].Value = "成品尺寸";
            cells[1, 4].Value = "产品编号";
            cells[2, 0].Value = "板件信息";
            cells["A3:F3"].Merge();
            cells[2, 7].Value = "开料尺寸";
            cells["H3:K3"].Merge();
            cells[2, 11].Value = "成品尺寸";
            cells["L3:O3"].Merge();
            cells[2, 15].Value = "封边条信息";
            cells["P3:Q3"].Merge();
            cells[3, 0].Value = "序号";
            cells[3, 1].Value = "颜色";
            cells[3, 2].Value = "名称";
            cells[3, 3].Value = "板件类型";
            cells[3, 4].Value = "原材料SKU";
            cells[3, 5].Value = "数量";
            cells[3, 6].Value = "造型";
            cells[3, 7].Value = "高";
            cells[3, 8].Value = "宽";
            cells[3, 9].Value = "板厚";
            cells[3, 10].Value = "开料面积";
            cells[3, 11].Value = "高";
            cells[3, 12].Value = "宽";
            cells[3, 13].Value = "板厚";
            cells[3, 14].Value = "成品面积";
            cells[3, 15].Value = "封边条SKU";
            cells[3, 16].Value = "封边条颜色";
            cells[3, 17].Value = "加工工位";
            cells[3, 18].Value = "抽屉编号";
            cells[3, 19].Value = "正面机加工信息";
            cells[3, 20].Value = "反面机加工信息";
            int i = 4;
            foreach (var part in product.Parts)
            {
                cells[i, 0].Value = i - 3;//序号
                cells[i, 1].Value = part.Color;
                cells[i, 2].Value = part.PartName;
                cells[i, 3].Value = part.Category;
                cells[i, 4].Value = part.SKU;
                cells[i, 5].Value = part.Qty;
                cells[i, 6].Value = part.Model;
                cells[i, 7].Value = part.CutLength;
                cells[i, 8].Value = part.CutWidth;
                cells[i, 9].Value = part.CutThickness;
                cells[i, 10].Value = part.CutSquare;
                cells[i, 11].Value = part.Length;
                cells[i, 12].Value = part.Width;
                cells[i, 13].Value = part.Thickness;
                cells[i, 14].Value = part.Square;
                cells[i, 15].Value = part.EdgeSKU;
                cells[i, 16].Value = part.EdgeColor;
                cells[i, 17].Value = part.MachiningArea;
                cells[i, 18].Value = part.DrawerNumber;
                cells[i, 19].Value = part.FileName;
                cells[i, 20].Value = part.Face6FileName;
                i++;
            }
        }

        private void buildHardwareSheet(IWorkbook book)
        {
            var sheet1 = book.Worksheets[0];
            sheet1.Name = "五金";
            var cells = sheet1.Cells;
            cells[0, 0].Value = "订单号";
            cells[1, 0].Value = "行号";
            cells[2, 0].Value = "产品分类";
            cells[0, 1].Value = product.OrderNumber;
            cells[1, 1].Value = product.LineNumber;
            cells[1, 2].Value = "订单行描述";
            cells[1, 3].Value = product.Description;
            cells[3, 0].Value = "序号";
            cells[3, 1].Value = "SKU号";
            cells[3, 2].Value = "描述";
            cells[3, 3].Value = "数量";
            cells[3, 4].Value = "单位";

            int i = 4;
            foreach (var hw in product.Hardwares.OrderBy(it => it.Index))
            {
                cells[i, 0].Value = hw.Index;
                cells[i, 1].Value = hw.SKU;
                cells[i, 2].Value = hw.Description;
                cells[i, 3].Value = hw.Number;
                cells[i, 4].Value = hw.Unit;
                i++;
            }
        }
    }
}
