using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.SpreadCustomFunctions
{
    public class PartThicknessM : SpreadsheetGear.CustomFunctions.Function
    {
        public static readonly PartThicknessM PTM = new PartThicknessM();

        public override void Evaluate(SpreadsheetGear.CustomFunctions.IArguments arguments, SpreadsheetGear.CustomFunctions.IValue result)
        {
            if (arguments.Count == 5 || arguments.Count == 4)
            {
                string partname = "";
                int rowCount, columnCount;
                arguments.GetArrayDimensions(0, out rowCount, out columnCount);

                if (arguments.Count == 5)
                    partname = arguments.GetText(4);
                else
                {
                    for (int i = 0; i < rowCount; i++)
                    {
                        SpreadsheetGear.CustomFunctions.IValue value = arguments.GetArrayValue(0, i, 0);
                        if (arguments.CurrentRow == i)
                        {
                            partname = value.Text;
                            break;
                        }
                    }

                }


                string MaterialName = "";
                for (int i = 0; i < rowCount; i++)
                {
                    SpreadsheetGear.CustomFunctions.IValue value = arguments.GetArrayValue(0, i, 0);
                    if (value.Text == partname)
                    {
                        MaterialName = arguments.GetArrayValue(0, i, 5).Text;
                        break;
                    }
                }

                if (MaterialName != "")
                {
                    bool findit = false;

                    if (!findit)
                    {
                        arguments.GetArrayDimensions(1, out rowCount, out columnCount);
                        for (int i = 0; i < rowCount; i++)
                        {
                            SpreadsheetGear.CustomFunctions.IValue value = arguments.GetArrayValue(1, i, 0);
                            if (value.Text == MaterialName)
                            {
                                result.Number = arguments.GetArrayValue(1, i, 1).Number;
                                findit = true;
                                break;
                            }
                        }
                    }
                    if (!findit)
                    {
                        arguments.GetArrayDimensions(2, out rowCount, out columnCount);
                        for (int i = 0; i < rowCount; i++)
                        {
                            SpreadsheetGear.CustomFunctions.IValue value = arguments.GetArrayValue(2, i, 0);
                            if (value.Text == MaterialName)
                            {
                                result.Number = arguments.GetArrayValue(2, i, 1).Number;
                                findit = true;
                                break;
                            }
                        }
                    }
                    if (!findit)
                    {
                        arguments.GetArrayDimensions(3, out rowCount, out columnCount);
                        for (int i = 0; i < rowCount; i++)
                        {
                            SpreadsheetGear.CustomFunctions.IValue value = arguments.GetArrayValue(3, i, 0);
                            if (value.Text == MaterialName)
                            {
                                result.Number = arguments.GetArrayValue(3, i, 1).Number;
                                findit = true;
                                break;
                            }
                        }
                    }
                    if (!findit)
                        result.Error = SpreadsheetGear.ValueError.Value;
                }

            }
            else
            {
                result.Error = SpreadsheetGear.ValueError.Value;
            }
        }

        private PartThicknessM()
            : base("PartThicknessM", SpreadsheetGear.CustomFunctions.Volatility.Invariant, SpreadsheetGear.CustomFunctions.ValueType.Number)
        { }
    }
}
