using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Entities.Checks
{
    public class MachineTokenChecker : Check
    {
        public List<ModelError> Errors { get; private set; }
        public MachineTokenChecker(List<ModelError> errors)
        {
            this.Errors = errors;
        }
        public int FaceNumber(string par, int startIndex, int[] valueRange)
        {
            if (valueRange.Length == 0)
            {
                throw new Exception("数据值范围不能为空");
            }

            if (par.Length >= startIndex + 1)
            {
                int value;
                if (int.TryParse(par.Substring(startIndex, 1), out value))
                {
                    if (valueRange.Contains(value))
                    {
                        return value;
                    }
                }
            }

            Errors.Add(new ModelError("Wrong face number!"));
            //TODO:需要报错信息
            return valueRange[0];
        }

        public List<double> PointPositions(string par)
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
                Errors.Add(new ModelError("xxxxxxxx"));
            }

            return points;
        }
    }
}
