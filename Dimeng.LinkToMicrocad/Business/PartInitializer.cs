using Dimeng.LinkToMicrocad.Logging;
using Dimeng.WoodEngine.Entities;
using Dimeng.WoodEngine.Entities.MachineTokens;
using Dimeng.WoodEngine.Entities.Checks;
using SpreadsheetGear;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Dimeng.WoodEngine.Business
{
    public class PartInitializer
    {
        private List<ModelError> errors = new List<ModelError>();

        public List<ModelError> GetPartsFromOneLine(IRange partRange, IRange machineRange, IProduct product, List<Part> parts,
            IWorkbookSet books, List<Material> tempMaterials, List<EdgeBanding> tempEdgebandings)
        {
            PartChecker check = new PartChecker(partRange, getLocation(product), errors);

            string partName = check.PartName();
            int qty = check.Qty();
            if (qty <= 0)
            {
                return errors;
            }

            double width = check.Width();
            double length = check.Length();

            Material material;
            double thick = check.Thick(books.Workbooks["M"], out material, tempMaterials, errors);

            EdgeBanding ebw1 = check.EBW1(books.Workbooks["E"], tempEdgebandings);
            EdgeBanding ebw2 = check.EBW2(books.Workbooks["E"], tempEdgebandings);
            EdgeBanding ebl1 = check.EBL1(books.Workbooks["E"], tempEdgebandings);
            EdgeBanding ebl2 = check.EBL2(books.Workbooks["E"], tempEdgebandings);

            string[] comments = partRange[0, 26].Text.Split('|');//Column26
            string comment = comments[0];
            string comment2 = (comments.Length > 1) ? comments[1] : string.Empty;
            string comment3 = (comments.Length > 2) ? comments[2] : string.Empty;

            int basepoint = check.BasePoint();
            MachinePoint machinePoint = check.MachinePoint();

            double xRotation = check.XRotation();
            double yRotation = check.YRotation();
            double zRotation = check.ZRotation();

            bool isDraw3d;
            string layname3d = check.Layname3d(out isDraw3d);
            string layname2d = partRange[0, 36].Text;

            if (width <= 0 || length <= 0 || thick <= 0)
            {
                Logger.GetLogger().Info("Part got errors !");
                return errors;
            }

            check.IsMaterialFitPartLengthAndWidth(length, width, material, errors);

            List<BaseToken> tokens = getTokens(partRange, machineRange);

            bool isEQPart = check.IsEQPart();
            if (isEQPart)
            {
                List<double[]> origins = check.EQPositions(qty, thick, product.Width);

                if (errors.Count > 0)
                {
                    Logger.GetLogger().Info("Part got errors !");
                    //return errors; //有问题的板件还是画
                }

                foreach (var d in origins)
                {
                    Part part = new Part(partName, 1, width, length, thick, material,
                        ebw1, ebw2, ebl1, ebl2,
                        comment, comment2, comment3,
                        basepoint, machinePoint,
                        d[0], d[1], d[2], xRotation, yRotation, zRotation,
                        layname3d, layname2d, isDraw3d,
                        product);
                    parts.Add(part);
                    Logger.GetLogger().Debug(string.Format("{0}/{1}/{2}/{3}/{4}", partName, 1, width, length, isDraw3d));
                }
            }
            else
            {
                double xPos = check.XPosition();
                double yPos = check.YPosition();
                double zPos = check.ZPosition();

                if (errors.Count > 0)
                {
                    Logger.GetLogger().Info("Part got errors !");
                    //return errors; //有问题的板件还是画
                }

                Part part = new Part(partName, qty, width, length, thick, material,
                    ebw1, ebw2, ebl1, ebl2,
                    comment, comment2, comment3,
                    basepoint, machinePoint,
                    xPos, yPos, zPos, xRotation, yRotation, zRotation,
                    layname3d, layname2d, isDraw3d,
                    product);

                parts.Add(part);
                Logger.GetLogger().Debug(string.Format("{0}/{1}/{2}/{3}/{4}", partName, 1, width, length, isDraw3d));
            }

            tokens.ForEach(t => t.Part = parts[0]);
            MachineTokenChecker mChecker = new MachineTokenChecker(errors);
            parts.ForEach(it =>
            {
                var tempTokens = new List<BaseToken>();
                foreach (var t in tokens)
                {
                    if (t.Valid(mChecker))
                    {
                        var tempToken = t.Clone();
                        tempToken.Part = it;
                        tempTokens.Add(tempToken);
                    }
                }
                it.MachineTokens = tempTokens;
            });

            return errors;
        }

        private List<BaseToken> getTokens(IRange partRow, IRange machineColumn)
        {
            List<BaseToken> tokens = new List<BaseToken>();

            for (int i = 0; i <= partRow.Columns.ColumnCount; i += 10)
            {
                string fullToken = partRow[0, 40 + i].Text;

                if (String.IsNullOrEmpty(fullToken))
                {
                    break;
                }

                if (i == 90)
                {
                    for (int x = 0; x <= machineColumn.Rows.RowCount; x += 10)
                    {
                        string xfullToken = machineColumn[x, 0].Text;

                        if (String.IsNullOrEmpty(xfullToken))
                        {
                            break;
                        }

                        double xv;
                        if (double.TryParse(xfullToken, out xv) && xv == 0)
                        {
                            continue;
                        }

                        string xtokenName = GetTokenFromMachiningTokenString(xfullToken);

                        string xtypeFullName = "Dimeng.WoodEngine.Entities.MachineTokens." + xtokenName + "MachiningToken";

                        //提前检查是否含有这个指令
                        if (ModelAssemblyLoader.GetInstance().Types.Find(it => it.FullName == xtypeFullName) == null)
                        {
                            continue;
                        }

                        try
                        {
                            object[] parms = new object[] { 
                                                                                machineColumn[x,0].Text, 
                                                                                machineColumn[x+1,0].Text, 
                                                                                machineColumn[x+2,0].Text, 
                                                                                machineColumn[x+3,0].Text, 
                                                                                machineColumn[x+4,0].Text, 
                                                                                machineColumn[x+5,0].Text, 
                                                                                machineColumn[x+6,0].Text, 
                                                                                machineColumn[x+7,0].Text, 
                                                                                machineColumn[x+8,0].Text, 
                                                                                machineColumn[x+9,0].Text, 
                                                                      };

                            BaseToken token = (BaseToken)Activator.CreateInstance(
                                ModelAssemblyLoader.GetInstance().Assembly.GetType(
                                    xtypeFullName,
                                    true,
                                    true
                                    )
                                , parms);

                            tokens.Add(token);
                        }
                        catch (Exception error)
                        {
                            throw error;
                        }
                    }
                }

                double v;
                if (double.TryParse(fullToken, out v) && v == 0)
                {
                    continue;
                }

                string tokenName = GetTokenFromMachiningTokenString(fullToken);

                string typeFullName = "Dimeng.WoodEngine.Entities.MachineTokens." + tokenName + "MachiningToken";

                //提前检查是否含有这个指令
                if (ModelAssemblyLoader.GetInstance().Types.Find(it => it.FullName == typeFullName) == null)
                {
                    continue;
                }

                try
                {
                    object[] parms = new object[] { 
                                                                                partRow[0, 40 + i].Text, 
                                                                                partRow[0, 41 + i].Text, 
                                                                                partRow[0, 42 + i].Text, 
                                                                                partRow[0, 43 + i].Text, 
                                                                                partRow[0, 44 + i].Text, 
                                                                                partRow[0, 45 + i].Text, 
                                                                                partRow[0, 46 + i].Text, 
                                                                                partRow[0, 47 + i].Text, 
                                                                                partRow[0, 48 + i].Text, 
                                                                                partRow[0, 49 + i].Text, 
                                                                      };

                    BaseToken token = (BaseToken)Activator.CreateInstance(
                        ModelAssemblyLoader.GetInstance().Assembly.GetType(
                            typeFullName,
                            true,
                            true
                            )
                        , parms);

                    tokens.Add(token);
                }
                catch (Exception error)
                {
                    throw error;
                }
            }

            return tokens;
        }

        private string getLocation(IProduct product)
        {
            if (product is Product)
            {
                var _product = product as Product;
                return string.Format("{0} {1}", _product.Handle, _product.Description);
            }

            if (product is Subassembly)
            {
                var _sub = product as Subassembly;
                return string.Format("{0}", _sub.Description);//TODO
            }

            throw new Exception("Unknown type of product");
        }

        private string GetTokenFromMachiningTokenString(string token)
        {
            token = token.ToUpper();//先变成大写

            string temp = "";
            if (token.IndexOf("[") > -1)//如果指令含有中括号，就去掉括号内的内容
            {
                temp = token.Substring(0, token.IndexOf("["));
            }
            else temp = token;

            //过滤掉空格
            temp = temp.Replace(" ", "");

            if (temp.StartsWith("3"))//如果指令名以数字开头，应加上一个下划线，将来应采用正则表达式
            {
                temp = "_" + temp;
            }

            //////对MITER/PROFILE这些包含两个空格的指令，要进行特殊处理？
            ////未完成
            if (temp.StartsWith("MITER")) return "MITER";
            if (temp.StartsWith("PROFILE")) return "PROFILE";
            if (temp.StartsWith("BENDING")) return "BENDING";

            //正则匹配，最后两位是数字的，就去掉最后两个返回
            //只有一位是数字的，就去掉最后一个
            if (isLastTwoDigital(temp))
            {
                return temp.Substring(0, temp.Length - 2);
            }
            else if (isLastOneDigital(temp))
            {
                return temp.Substring(0, temp.Length - 1);
            }

            return temp;//试试运气了
        }

        private bool isLastOneDigital(string trimTokenString)
        {
            Regex regex = new Regex("\\d$");
            return regex.IsMatch(trimTokenString);
        }
        private bool isLastTwoDigital(string trimTokenString)
        {
            Regex regex = new Regex("\\d\\d$");
            return regex.IsMatch(trimTokenString);
        }
    }
}
