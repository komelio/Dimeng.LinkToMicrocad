using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Business
{
    internal partial class PartChecker
    {
        internal double XRotation()
        {
            return this.getRotationValue(RotationType.X);
        }
        internal double YRotation()
        {
            return this.getRotationValue(RotationType.Y);
        }

        internal double ZRotation()
        {
            return this.getRotationValue(RotationType.Z);
        }

        private double getRotationValue(RotationType typeR)
        {
            string text = range[0, 32].Text;
            if (typeR == RotationType.Y)
            {
                text = range[0, 33].Text;
            }
            else if (typeR == RotationType.Z)
            {
                text = range[0, 34].Text;
            }

            if (string.IsNullOrEmpty(text.Trim()))
            {
                //this.writeError("未填写,默认为0", "板件绕" + typeR.ToString() + "旋转角度");
                return 0;
            }

            return GetDoubleValue(text, "板件绕" + typeR.ToString() + "旋转角度", false, errors);
        }

        private enum RotationType
        {
            X,
            Y,
            Z
        }

    }
}
