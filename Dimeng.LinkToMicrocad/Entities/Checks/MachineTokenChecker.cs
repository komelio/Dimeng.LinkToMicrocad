using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Entities.Checks
{
    public class MachineTokenChecker : Check
    {
        public int FaceNumber(string par, int startIndex, int[] valueRange,List<ModelError> errors)
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

            //todo
            //Errors.Add(new ModelError(string.Format("Wrong face number!{0}/{1}", par, startIndex)));
            return valueRange[0];
        }

        public int EdgeNumber(string par, int startIndex, int[] valueRange, List<ModelError> errors)
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

            errors.Add(new ModelError("Wrong edge number!"));
            //TODO:需要报错信息
            return valueRange[0];
        }

        public List<double> PointPositions(string par, List<ModelError> errors)
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
                errors.Add(
                        new ModelError(
                            string.Format("Wrong point positions!Data: {0}", par)
                                      )
                                      );
            }

            return points;
        }

        public string ToolName(string par, string tokenName, List<ModelError> errors)
        {
            if (string.IsNullOrEmpty(par.Trim()))
            {
                errors.Add(new ModelError(tokenName + " could not be empty!"));
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

        internal List<double> GetBulges(string par, List<ModelError> errors)
        {
            List<Double> values = new List<double>();

            string[] valueStringArray = par.Split(';');
            foreach (var s in valueStringArray)
            {
                if (string.IsNullOrEmpty(s))
                {
                    values.Add(0);
                    continue;
                }

                values.Add(this.GetDoubleValue(s, "PLine/Bulges", false, errors));
            }

            return values;
        }

        internal List<Point3d> GetPoints(string par, List<ModelError> errors)
        {
            List<Point3d> points = new List<Point3d>();

            if (string.IsNullOrEmpty(par))
            {
                errors.Add(new ModelError("No points!"));
                return points;
            }

            string[] array = par.Split('|');
            foreach (var s in array)
            {
                string[] xyzArray = s.Split(';');
                if (xyzArray.Length != 3)
                {
                    errors.Add(new ModelError("Error point data!"));
                    return points;
                }

                double x = this.GetDoubleValue(xyzArray[0], "PLine/PointX", false, errors);
                double y = this.GetDoubleValue(xyzArray[1], "PLine/PointY", false, errors);
                double z = this.GetDoubleValue(xyzArray[2], "PLine/PointZ", false, errors);

                points.Add(new Point3d(x, y, z));
            }

            return points;
        }

        internal string TokenFileName(string par, string tokenName, List<ModelError> errors)
        {
            if (string.IsNullOrEmpty(par))
            {
                errors.Add(new ModelError(tokenName + "文件名称不能为空"));
                return par;
            }

            return par;
        }
    }
}
