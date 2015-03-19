using Autodesk.AutoCAD.Geometry;
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

        public int EdgeNumber(string par, int startIndex, int[] valueRange)
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

            Errors.Add(new ModelError("Wrong edge number!"));
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

        public string ToolName(string par, string tokenName)
        {
            if (string.IsNullOrEmpty(par.Trim()))
            {
                this.Errors.Add(new ModelError(tokenName + " could not be empty!"));
            }

            return par;
        }

        public ToolComp GetToolComp(string par, string tokenName)
        {
            if (par == "L")
            {
                return ToolComp.Left;
            }
            else if (par == "R")
            {
                return ToolComp.Right;
            }
            else
            {
                return ToolComp.None;
            }
        }

        internal List<double> GetBulges(string par)
        {
            List<Double> values = new List<double>();

            string[] valueStringArray = par.Split(';');
            foreach (var s in valueStringArray)
            {
                if(string.IsNullOrEmpty(s))
                {
                    values.Add(0);
                    continue;
                }

                values.Add(this.GetDoubleValue(s, "PLine/Bulges", false, this.Errors));
            }

            return values;
        }

        internal List<Point3d> GetPoints(string par)
        {
            List<Point3d> points = new List<Point3d>();

            if (string.IsNullOrEmpty(par))
            {
                this.Errors.Add(new ModelError("No points!"));
                return points;
            }

            string[] array = par.Split('|');
            foreach (var s in array)
            {
                string[] xyzArray = s.Split(';');
                if (xyzArray.Length != 3)
                {
                    this.Errors.Add(new ModelError("Error point data!"));
                    return points;
                }

                double x = this.GetDoubleValue(xyzArray[0], "PLine/PointX", false, this.Errors);
                double y = this.GetDoubleValue(xyzArray[1], "PLine/PointY", false, this.Errors);
                double z = this.GetDoubleValue(xyzArray[2], "PLine/PointZ", false, this.Errors);

                points.Add(new Point3d(x, y, z));
            }

            return points;
        }

        internal string TokenFileName(string par, string tokenName)
        {
            if (string.IsNullOrEmpty(par))
            {
                Errors.Add(new ModelError(tokenName + "文件名称不能为空"));
                return par;
            }

            return par;
        }
    }
}
