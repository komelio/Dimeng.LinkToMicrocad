using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.Geometry;
using Dimeng.WoodEngine.Entities.Checks;

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
                         string par9)
        {
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

            getReferenceString();

            this.IsDrawOnly = false;

            Errors = new List<ModelError>();
        }

        protected virtual void getReferenceString()
        {
            int index1 = this.Token.LastIndexOf("[");
            int index2 = this.Token.LastIndexOf("]");

            if (index1 > -1)
            {
                int length = index2 - index1 - 1;
                if (index2 < index1)
                {
                    length = Token.Length - index1 - 1;
                }

                //提取指令名中的说明部分
                Reference = this.Token.Substring(index1 + 1, length);
            }
        }

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

        public Part Part { get; set; }

        public bool IsDrawOnly { get; protected set; }

        /// <summary>
        /// 将指令转换为机加工
        /// </summary>
        /// <param name="AssociatedDist">关联值，关联面之间的可容纳距离</param>
        public virtual void ToMachining(double AssociatedDist, ToolFile toolFile)
        {

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

        public virtual bool Valid(MachineTokenChecker chekcer)
        {
            return true;
        }

        public BaseToken Clone()
        {
            return this.MemberwiseClone() as BaseToken;
        }

        public List<ModelError> Errors { get; private set; }
    }
}
