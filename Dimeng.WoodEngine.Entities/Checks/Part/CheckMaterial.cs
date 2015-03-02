using Dimeng.WoodEngine.Entities;
using SpreadsheetGear;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Entities.Checks
{
    public partial class PartChecker
    {
        public double Thick(IWorkbook bookM, out Material material, List<Material> tempMaterials, List<ModelError> errors)
        {
            string materialText = range[0, 21].Text;
            string thickText = range[0, 20].Text;

            material = null;
            bool isFakeMaterial = false;

            //检查材料列是否有数据
            string name = materialText;
            if (string.IsNullOrEmpty(name.Trim()))
            {
                errors.Add(new ModelError("Part material should not be blank!"));
                material = null;//没有材料名称,赋予null
                return 0;
            }

            if (name.StartsWith("-"))
            {
                isFakeMaterial = true;
                name = name.Substring(0);
            }

            //从缓存中查找是否有数据
            var m = tempMaterials.Find(it => it.Name.ToUpper() == name.ToUpper());
            if (m != null)
            {
                double value = m.Thickness;
                if (value <= 0)
                {
                    errors.Add(new ModelError(string.Format("Material [{0}] not found", materialText)));
                }
                material = m;
            }
            //缓存中没有数据
            else
            {
                bool findit = false;
                //根据材料名称,再到M中查找材料
                for (int i = 1; i < 4; i++)
                {
                    if (findit)
                    {
                        break;
                    }

                    var sheet = bookM.Worksheets[i];

                    for (int r = 0; r < sheet.Cells.Rows.RowCount; r++)
                    {
                        string _name = sheet.Cells.Rows[r, 0].Text;

                        if (_name == "deleted")
                        {
                            continue;
                        }//标记被删除的材料

                        if (string.IsNullOrEmpty(_name.Trim()))
                        {
                            if (i == 3)//到最后一张表的时候,表示还没找到,那就说明这个材料在M中没有,就记录一下
                            {
                                material = new Material(name, 0);
                                tempMaterials.Add(material);
                            }
                            break;
                        }

                        if (_name.ToUpper() == name.ToUpper())
                        {
                            material = getMaterial(sheet.Cells.Rows[r, 1].EntireRow, errors);
                            tempMaterials.Add(material);
                            findit = true;
                            break;
                        }
                    }
                }
            }

            //根据以上逻辑,这里material不能为null
            material.IsFake = isFakeMaterial;
            if (material.Thickness <= 0)
            {
                return 0;
            }
            else
            {
                //判断厚度列是否有数据
                if (string.IsNullOrEmpty(thickText.Trim()))
                {
                    return material.Thickness;
                }
                else
                {
                    return GetDoubleValue(thickText, "Material Thickness(U)", true, errors);
                }
            }
        }

        private Material getMaterial(IRange entireRow, List<ModelError> errors)
        {
            string name = entireRow[0, 0].Text;
            double thick = GetDoubleValue(entireRow[0, 1].Text, "Material thickness", true, errors);

            Grain grain = Grain.None;
            if (string.IsNullOrEmpty(entireRow[0, 2].Text.Trim()))
            {
                grain = Grain.None;
            }
            else
            {
                grain = (Grain)GetIntValue(entireRow[0, 2].Text, "材料纹理", true, errors);
            }
            Material material = new Material(name, thick);
            material.Grain = grain;

            for (int i = 9; i < 10; i += 9)
            {
                if (string.IsNullOrEmpty(entireRow[0, i].Text.Trim()))
                {
                    break;
                }

                Stock stock = Stock.Default;
                stock.Qty = GetIntValue(entireRow[0, i].Text, "材料库存数量", true, errors);
                stock.Price = GetDoubleValue(entireRow[0, i + 3].Text, "材料单价", true, errors);
                stock.Width = GetDoubleValue(entireRow[0, i + 1].Text, "材料宽度", true, errors);
                stock.Length = GetDoubleValue(entireRow[0, i + 2].Text, "材料长度", true, errors);
                stock.TopTrimValue = GetDoubleValue(entireRow[0, i + 4].Text, "材料上修边", true, errors);
                stock.BottomTrimValue = GetDoubleValue(entireRow[0, i + 5].Text, "材料下修边", true, errors);
                stock.LeftTrimValue = GetDoubleValue(entireRow[0, i + 6].Text, "材料左修边", true, errors);
                stock.RightTrimValue = GetDoubleValue(entireRow[0, i + 7].Text, "材料友修边", true, errors);

                material.Stocks.Add(stock);
            }

            return material;
        }

        public void IsMaterialFitPartLengthAndWidth(double length, double width, Material material, List<ModelError> errors)
        {
            if (!material.HasFitStock(length, width))
            {
                this.PartError(
                    string.Format("材料[{0}]没有足够的尺寸容纳板件{1}/{2}", 
                        material.Name, length, width)
                    );
            }
        }
    }
}
