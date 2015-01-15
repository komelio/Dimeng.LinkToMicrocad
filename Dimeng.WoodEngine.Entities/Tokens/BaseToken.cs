using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.Geometry;

namespace Dimeng.WoodEngine.Entities.MachineTokens
{
    public class BaseToken
    {
        public BaseToken(string token,
                         string par1,
                         string par2,
                         string par3,
                         string par4,
                         string par5,
                         string par6,
                         string par7,
                         string par8,
                         string par9,
                         int rowIndex,
                         int columnIndex,
                         Part p)
        {
            if (token.IndexOf("[") > -1)
            {
                //提取指令名中的说明部分
                Reference = token.Substring(token.LastIndexOf("[") + 1).Replace("]", "");
            }

            this.Token = token;
            this.Par1 = par1;
            this.Par2 = par2;
            this.Par3 = par3;
            this.Par4 = par4;
            this.Par5 = par5;
            this.Par6 = par6;
            this.Par7 = par7;
            this.Par8 = par8;
            this.Par9 = par9;
            this.Part = p;

            this.RowIndex = rowIndex;
            this.ColumnIndex = columnIndex;
        }

        //行号列号
        public int RowIndex { get; private set; }
        public int ColumnIndex { get; private set; }
        public string Reference { get; set; }//指令的备注名称,及中括号内的部分
        public string Token { get; set; }
        public int FaceNumber { get; set; }
        public int EdgeNumber { get; set; }
        public string Par1 { get; set; }
        public string Par2 { get; set; }
        public string Par3 { get; set; }
        public string Par4 { get; set; }
        public string Par5 { get; set; }
        public string Par6 { get; set; }
        public string Par7 { get; set; }
        public string Par8 { get; set; }
        public string Par9 { get; set; }

        private bool isValid = true;
        /// <summary>
        /// 返回指令初始化之后，是否可用
        /// </summary>
        /// <returns>如果可用则为true</returns>
        public virtual bool IsValid
        {
            get { return isValid; }
            protected set
            {
                isValid = value;
            }
        }

       // public virtual bool Valid(Logger logger) { return true; }

        //protected Logger logger { get; set; }

        protected Part Part { get; set; }

        public bool IsDrawOnly { get; protected set; }

        /// <summary>
        /// 将指令转换为机加工
        /// </summary>
        /// <param name="AssociatedDist">关联值，关联面之间的可容纳距离</param>
        public virtual void ToMachining(double AssociatedDist, ToolFile toolFile)
        {

        }

        /// <summary>
        /// 用来查找关联面的函数
        /// </summary>
        /// <param name="associateDist"></param>
        public void FindAssociatedFaces(double associateDist)
        {
            //PartFace pf = this.Part.GetPartFaceByNumber(this.FaceNumber);

            //if (pf.AssociatedPartFaces == null)//如果这个面的相关联板件是null，表明还没有算过，就算一次
            //{
            //    pf.AssociatedPartFaces = new List<PartFace>();

            //    foreach (Part part in Part.Product.CombinedParts.Where(it => it.IsBend == false))
            //    {
            //        if (part == Part)//跳过自己
            //        {
            //            continue;
            //        }

            //        foreach (PartFace face in part.Faces)
            //        {
            //            if (pf.IsAssocaitedWithAnotherFace(face, associateDist))
            //            {
            //                pf.AssociatedPartFaces.Add(face);//添加关联板件
            //            }
            //        }
            //    }
            //}
        }

        public List<PartFace> FindAssociatedFacesOneTime(double tolerateValue, double associateDist)
        {
            List<PartFace> Faces = new List<PartFace>();

            ////获取当前的Face
            //PartFace pf = this.Part.GetPartFaceByNumber(this.FaceNumber);

            //var NonBendParts = Part.Product.CombinedParts.Where(it => it.IsBend == false && it != this.Part).ToList();

            //foreach (Part part in NonBendParts)
            //{
            //    foreach (PartFace face in part.Faces)
            //    {
            //        if (pf.IsAssocaitedWithAnotherFace(face, tolerateValue, associateDist))
            //        {
            //            Faces.Add(face);//添加关联板件
            //        }
            //    }
            //}

            return Faces;
        }



        protected Vector3d GetPointXAxis(int num)
        {
            if (num > 0 && num < 5)
                return Vector3d.XAxis;
            else if (num > 4 && num <= 8)
                return -Vector3d.XAxis;
            throw new Exception("Error num" + num.ToString());
        }
        protected Vector3d GetPointYAxis(int num)
        {
            if (num == 1 || num == 2 || num == 7 || num == 8)
                return Vector3d.YAxis;
            else if (num == 3 || num == 4 || num == 5 || num == 6)
                return -Vector3d.YAxis;
            throw new Exception("Error num" + num.ToString());
        }


        public override string ToString()
        {
            return Token;
        }

        #region

        protected void faceNumberChecker(string par, int startIndex, int[] valueRange)
        {
            if (par.Length >= startIndex + 1)
            {
                int value;
                if (int.TryParse(par.Substring(startIndex, 1), out value))
                {
                    if (valueRange.Contains(value))
                    {
                        FaceNumber = value;

                        return;
                    }
                }
            }

            this.IsValid = false;
            this.writeError(string.Format("机加工指令{0}参数所在面不正确,错误值：{1}", this.Token, par), false);
        }

        protected void edgeNumberChecker(string par, int startIndex, int[] valueRange)
        {
            if (par.Length >= startIndex + 1)
            {
                int value;
                if (int.TryParse(par.Substring(startIndex, 1), out value))
                {
                    if (valueRange.Contains(value))
                    {
                        EdgeNumber = value;

                        return;
                    }
                }
            }

            this.IsValid = false;
            this.writeError(string.Format("机加工指令{0}参数所在边不正确,错误值：{1}", this.Token, par), false);
        }

        protected List<double> pointsChecker(string par)
        {
            List<double> points = new List<double>();
            string[] strValues = par.Split('|');

            bool hasError = false;

            foreach (var s in strValues)
            {
                double value;
                if (double.TryParse(s, out value))
                {
                    points.Add(value);
                }
                else
                {
                    hasError = true;
                    break;
                }
            }

            if (hasError)
            {
                this.IsValid = false;
                this.writeError(string.Format("机加工指令{0},参数的点阵数据不正确,错误值：{1}", this.Token, par), false);
            }

            return points;
        }

        protected double DoubleChecker(string par, string tokenName, bool needPositive)
        {
            double value;
            if (double.TryParse(par, out value))
            {
                if (needPositive)
                {
                    if (value < 0)
                    {
                        this.IsValid = false;
                        this.writeError(string.Format("机加工指令{0}的参数值必须大于等于0，错误值:{1}",
                                        tokenName,
                                        par
                                        ), false);
                        return 0;
                    }
                }
                return value;
            }
            else
            {
                this.IsValid = false;
                this.writeError(string.Format("机加工指令{0}的参数值必须为数字,错误值:{1}",
                                  tokenName,
                                  par
                                  ), false);

                return 0;
            }
        }

        protected bool BoolChecker(string par, string tokenName, bool needNotEmpty, bool defaultValue)
        {
            if (!needNotEmpty)
            {
                if (string.IsNullOrEmpty(par))
                    return defaultValue;
            }

            double value;
            if (double.TryParse(par, out value))
            {
                if (value == 1)
                {
                    return true;
                }
                else if (value == 0)
                {
                    return false;
                }
            }

            this.IsValid = false;
            this.writeError(string.Format("机指令{0}参数错误，错误值：{1}", tokenName, par), false);
            return false;
        }

        protected string notEmptyStringChecker(string par, string tokenInfo)
        {
            if (string.IsNullOrEmpty(par))
            {
                this.isValid = false;
                this.writeError(string.Format("机加工指令{0}不能为空", tokenInfo), false);
            }

            return par;
        }

        protected ToolComp toolCompChecker(string par, string tokenInfo)
        {
            ToolComp comp = ToolComp.None;

            if (string.IsNullOrEmpty(par))
            {
                return comp;
            }

            if (par.ToUpper() == "R")
            {
                comp = ToolComp.Right;
            }
            else if (par.ToUpper() == "L")
            {
                comp = ToolComp.Left;
            }
            else
            {
                this.isValid = false;
                this.writeError(string.Format("机加工指令{0}参数错误,错误值：{1}", tokenInfo, par), false);
            }

            return comp;
        }

        protected void writeError(string message, bool isWarning)
        {
            //string productName = Part.Product.Description;
            //if (Part.Subassembly != null)
            //    productName += "//" + Part.Subassembly.Name;
            //if (Part.NestSubassembly != null)
            //    productName += "//" + Part.NestSubassembly.Name;

            //string mes = string.Format("{0}|{1}|{2}|{3}|{4}|",
            //                  Part.Product.ItemNumber,
            //                  productName.Replace("|", ""),
            //                  Part.PartName.Replace("|", ""),
            //                  this.RowIndex + 1,
            //                  this.ColumnIndex + 1)
            //                    + message;

            //if (isWarning)
            //    logger.Warn(mes);
            //else logger.Error(mes);
        }

        #endregion
    }
}
