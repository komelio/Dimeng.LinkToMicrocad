using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Entities.MachineTokens
{
    public class AssociativeToken : BaseToken
    {
        public AssociativeToken(string token, string par1, string par2, string par3, string par4, string par5, string par6, string par7, string par8, string par9)
            : base(token, par1, par2, par3, par4, par5, par6, par7, par8, par9)
        {

        }

        protected override void getReferenceString()
        {
            base.getReferenceString();

            //获取关联板件清单名称
            int index = this.Token.LastIndexOf("*");
            if (index > -1)
            {
                string parts = this.Token.Substring(index + 1);
                CanAssociativePartsList = parts.Split(',');
            }
        }

        public string[] CanAssociativePartsList { get; private set; }

        /// <summary>
        /// 保存这个指令的关联面
        /// </summary>
        public List<PartFace> AssociatedPartFaces { get; private set; }

        /// <summary>
        /// 用来查找关联面的函数
        /// </summary>
        /// <param name="associateDist"></param>
        public void FindAssociatedFaces(double associateDist)
        {
            PartFace pf = this.Part.GetPartFaceByNumber(this.FaceNumber);

            if (this.AssociatedPartFaces == null)//如果这个面的相关联板件是null，表明还没有算过，就算一次
            {
                this.AssociatedPartFaces = new List<PartFace>();

                foreach (Part part in Part.Product.Parts.Where(it => it.IsBend == false))
                {
                    if (part == Part)//跳过自己
                    {
                        continue;
                    }

                    foreach (PartFace face in part.Faces)
                    {
                        if (pf.IsAssocaitedWithAnotherFace(face, associateDist))
                        {
                            this.AssociatedPartFaces.Add(face);//添加关联板件
                        }
                    }
                }

                foreach (Subassembly sub in Part.Product.Subassemblies)
                {
                    foreach (Part part in sub.Parts.Where(it => it.IsBend == false))
                    {
                        if (part == Part)//跳过自己
                        {
                            continue;
                        }

                        foreach (PartFace face in part.Faces)
                        {
                            if (pf.IsAssocaitedWithAnotherFace(face, associateDist))
                            {
                                this.AssociatedPartFaces.Add(face);//添加关联板件
                            }
                        }
                    }
                }
            }
        }

    }
}
