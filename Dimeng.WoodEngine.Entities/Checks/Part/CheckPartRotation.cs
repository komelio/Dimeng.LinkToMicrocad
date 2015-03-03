﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Entities.Checks
{
    public partial class PartChecker
    {
        public double XRotation()
        {
            return this.getRotationValue(RotationType.X);
        }
        public double YRotation()
        {
            return this.getRotationValue(RotationType.Y);
        }

        public double ZRotation()
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
                this.PartWarn("板件绕" + typeR.ToString() + "旋转角度未填写,默认为0");
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