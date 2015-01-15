using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Entities.MachineTokens
{
    public class CONSTMachiningToken : BaseToken
    {
        public CONSTMachiningToken(string token, string par1, string par2, string par3, string par4, string par5, string par6, string par7, string par8, string par9, int row, int column, Part p)
            : base(token, par1, par2, par3, par4, par5, par6, par7, par8, par9, row, column, p)
        {
            this.IsDrawOnly = false;
        }

        public override bool Valid(Logger logger)
        {
            this.logger = logger;

            base.faceNumberChecker(this.Token, 5, new int[] { 1, 2, 3, 4 });
            DimToFirstHole = base.DoubleChecker(this.Par1, "关联的木榫孔机加工类型1/到第一个孔的距离", false);
            DimToLastHole = base.DoubleChecker(this.Par2, "关联的木榫孔机加工类型1/到最后一个孔的距离", false);
            EdgeHoleDepth = base.DoubleChecker(this.Par3, "关联的木榫孔机加工类型1/边孔深度", false);
            EdgeHoleDiameter = base.DoubleChecker(this.Par4, "关联的木榫孔机加工类型1/边孔直径", false);
            FaceHoleDepth = base.DoubleChecker(this.Par5, "关联的木榫孔机加工类型1/面孔深度", false);
            FaceHoleDiameter = base.DoubleChecker(this.Par6, "关联的木榫孔机加工类型1/面孔直径", false);
            IsDrillFromOppositeFace = base.BoolChecker(this.Par7, "关联的木榫孔机加工类型1/是否反面钻孔", false, false);
            DrillGap = string.IsNullOrEmpty(this.Par9) ? 32 : base.DoubleChecker(this.Par9, "关联的木榫孔机加工类型1/关联孔间距", false);
            //TODO：有一个默认32，似乎以后不能这么搞
            
            return this.IsValid;
        }

        public double DimToFirstHole { get; private set; }
        public double DimToLastHole { get; private set; }
        public double EdgeHoleDepth { get; private set; }
        public double EdgeHoleDiameter { get; private set; }
        public double FaceHoleDepth { get; private set; }
        public double FaceHoleDiameter { get; private set; }
        public bool IsDrillFromOppositeFace { get; private set; }
        public double DrillGap { get; private set; }
    }
}
