//using Dimeng.LinkToMicrocad.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Entities.Checks
{
    public partial class PartChecker
    {
        public bool IsEQPart()
        {

            //全部组成大写,然后来弄哦
            string[] positions = new string[]{
                                                 range[0,29].Text.ToUpper(),
                                                 range[0,30].Text.ToUpper(),
                                                 range[0,31].Text.ToUpper()
                                             };

            return positions.Any(it => it.StartsWith("EQ"));
        }

        public double XPosition()
        {
            return GetDoubleValue(range[0, 29].Text, "板件X坐标", false, errors);
        }
        public double YPosition()
        {
            return GetDoubleValue(range[0, 30].Text, "板件Y坐标", false, errors);
        }

        public double ZPosition()
        {
            return GetDoubleValue(range[0, 31].Text, "板件Z坐标" + range[0, 31].Formula, false, errors);
        }


        public List<double[]> EQPositions(int qty, double thick, double productwidth)
        {
            //TODO:用专门的EQ解析器去完成

            string xo = range[0, 29].Text.ToUpper();
            string yo = range[0, 30].Text.ToUpper();
            string zo = range[0, 31].Text.ToUpper();

            //Logger.GetLogger().Debug(string.Format("Start analysising EQ functions:{0}/{1}/{2}", xo, yo, zo));

            List<double[]> doubles = new List<double[]>();

            if (xo.StartsWith("EQV"))
            {
                string[] r = xo.Replace("EQV ", "").Split(',');

                if (r.Length < 3)
                    throw new Exception("EQV函数参数不正确");

                int arrayqty = Int32.Parse(r[0]);

                //存在arrayqty大于板件qty的可能，如何处理？
                if (arrayqty > qty)
                    throw new Exception("EQV的arrayQty多于板件的Qty，无法处理");

                int stackqty = Int32.Parse(System.Math.Ceiling(Convert.ToDouble(qty) / Convert.ToDouble(arrayqty)).ToString());

                double leftthick = Convert.ToDouble(r[1]);
                double rightthick = Convert.ToDouble(r[2]);
                double panelthick = (r.Length > 3) ? Convert.ToDouble(r[3]) : thick;
                double startx = (r.Length > 4) ? Convert.ToDouble(r[4]) : 0;
                double starty = (r.Length > 5) ? Convert.ToDouble(r[5]) : productwidth;

                double yorigin = (string.IsNullOrEmpty(yo)) ? 0 : Convert.ToDouble(yo);
                double xorigin = 0;

                //如果z值包含eq，则需要考虑板件矩阵问题

                for (int a = 0; a < arrayqty; a++)
                {
                    xorigin = (starty - startx - leftthick - rightthick) / (arrayqty + 1) * (a + 1) + startx + panelthick / 2;

                    if (zo.ToUpper().StartsWith("EQ1"))
                    {
                        EQ1Parser(xorigin.ToString(), yo, zo, stackqty, thick, doubles);
                    }
                    else if (zo.ToUpper().StartsWith("EQ2"))
                    {
                        EQ2Parser(xorigin.ToString(), yo, zo, stackqty, doubles);
                    }
                    else
                    {
                        for (int s = 0; s < stackqty; s++)
                        {
                            doubles.Add(new double[] { xorigin, yorigin, Convert.ToDouble(zo) });
                        }
                    }
                }

            }
            else if (xo.StartsWith("EQH"))
            {
                string[] r = xo.Replace("EQH ", "").Split(',');

                if (r.Length < 3)
                    throw new Exception("EQH函数参数不正确");

                int arrayqty = Int32.Parse(r[0]);

                //存在arrayqty大于板件qty的可能，如何处理？
                if (arrayqty > qty)
                    throw new Exception("EQH的arrayQty多于板件的Qty，无法处理");

                int stackqty = Int32.Parse(System.Math.Ceiling(Convert.ToDouble(qty) / Convert.ToDouble(arrayqty)).ToString());

                double leftthick = Convert.ToDouble(r[1]);
                double rightthick = Convert.ToDouble(r[2]);
                double panelthick = (r.Length > 3) ? Convert.ToDouble(r[3]) : thick;

                double yorigin = (string.IsNullOrEmpty(yo)) ? 0 : Convert.ToDouble(yo);
                double xorigin = 0;

                //如果z值包含eq，则需要考虑板件矩阵问题

                for (int a = 0; a < arrayqty; a++)
                {
                    xorigin = (productwidth - leftthick - rightthick - panelthick) / arrayqty * (a) + leftthick + a * panelthick;

                    if (zo.StartsWith("EQ1"))
                    {
                        EQ1Parser(xorigin.ToString(), yo, zo, stackqty, thick, doubles);
                    }
                    else if (zo.ToUpper().StartsWith("EQ2"))
                    {
                        EQ2Parser(xorigin.ToString(), yo, zo, stackqty, doubles);
                    }
                    else
                    {
                        for (int s = 0; s < stackqty; s++)
                            doubles.Add(new double[] { xorigin, yorigin, Convert.ToDouble(zo) });
                    }
                }
            }
            else if (zo.StartsWith("EQ1"))
            {
                EQ1Parser(xo, yo, zo, qty, thick, doubles);
            }
            else if (zo.StartsWith("EQ2"))
            {
                EQ2Parser(xo, yo, zo, qty, doubles);
            }

            return doubles;
        }

        private void EQ2Parser(string xo, string yo, string zo, int qty, List<double[]> doubles)
        {
            string[] r = zo.Replace("EQ2 ", "").Split(',');

            double xorigin = (string.IsNullOrEmpty(xo)) ? 0 : Convert.ToDouble(xo);
            double yorigin = (string.IsNullOrEmpty(yo)) ? 0 : Convert.ToDouble(yo);
            double UpperBorder = Convert.ToDouble(r[1]);
            double DownBorder = Convert.ToDouble(r[0]);

            if (qty <= 1)
            {
                doubles.Add(new double[] { xorigin, yorigin, DownBorder });
            }
            else
            {
                doubles.Add(new double[] { xorigin, yorigin, DownBorder });
                doubles.Add(new double[] { xorigin, yorigin, UpperBorder });
                for (int i = 0; i < qty - 2; i++)
                {
                    doubles.Add(new double[] { xorigin, yorigin, DownBorder + (UpperBorder - DownBorder) / (qty - 1) * (i + 1) });
                }
            }
        }

        private void EQ1Parser(string xo, string yo, string zo, int qty, double thick, List<double[]> doubles)
        {
            string[] r = zo.Replace("EQ1 ", "").Split(',');

            double xorigin = (string.IsNullOrEmpty(xo)) ? 0 : Convert.ToDouble(xo);
            double yorigin = (string.IsNullOrEmpty(yo)) ? 0 : Convert.ToDouble(yo);
            double UpperBorder = Convert.ToDouble(r[1]);
            double DownBorder = Convert.ToDouble(r[0]);

            for (int i = 0; i < qty; i++)
            {
                doubles.Add(new double[] { xorigin, yorigin, DownBorder + (UpperBorder - DownBorder) / (qty + 1) * (i + 1) - thick / 2 });
            }
        }
    }
}
