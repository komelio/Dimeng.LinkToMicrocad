using Dimeng.WoodEngine.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Entities.Checks
{
    public class Check
    {
        public double GetDoubleValue(string rangeText, string errorPrefix, bool needPositive, List<ModelError> errors)
        {
            double value;
            if (double.TryParse(rangeText, out value))
            {
                if (needPositive && value < 0)
                {
                    errors.Add(
                        new ModelError(
                            string.Format("{0} should not below zero!Original data:{1}", errorPrefix, value)
                                      )
                              );
                    return 0;
                }

                return value;
            }
            else
            {
                errors.Add(
                        new ModelError(
                            string.Format("{0} can not be converted to double type!Original data:{1}", errorPrefix, value)
                                      )
                              );
                return 0;
            }
        }

        public int GetIntValue(string rangeText, string errorPrefix, bool needPositive, List<ModelError> errors)
        {
            int value;
            if (int.TryParse(rangeText, out value))
            {
                if (needPositive && value < 0)
                {
                    errors.Add(
                        new ModelError(
                            string.Format("{0} should not below zero!Original data:{1}", errorPrefix, value)
                            ));
                    return 0;
                }

                return value;
            }
            else
            {
                errors.Add(
                        new ModelError(
                            string.Format("{0} can not be converted to double type!Original data:{1}", errorPrefix, value)
                                      )
                              );
                return 0;
            }
        }

        public int GetIntValue(string rangeText, string errorPrefix, bool needPositive, int[] ranges, List<ModelError> errors)
        {
            int value;
            if (int.TryParse(rangeText, out value))
            {
                if (needPositive && value < 0)
                {
                    errors.Add(
                        new ModelError(
                            string.Format("{0} can not below zero!Original data:{1}", errorPrefix, value)
                            ));
                    return 0;
                }

                if (!ranges.Contains(value))
                {
                    errors.Add(
                        new ModelError(
                            string.Format("{0} should be in range {2}!Original data:{1}", errorPrefix, value, ranges)
                            ));
                    return 0;
                }

                return value;
            }
            else
            {
                errors.Add(
                        new ModelError(
                            string.Format("{0} can not be converted to double type!Original data:{1}", errorPrefix, value)
                                      )
                              );
                return 0;
            }
        }

        public bool GetBoolValue(string text, string errorPrefix, bool needNotEmpty, bool defaultValue, List<ModelError> errors)
        {
            if (string.IsNullOrEmpty(text))
            {
                if (!needNotEmpty)
                {
                    return defaultValue;
                }
                else
                {
                    errors.Add(new ModelError("xxxxx"));
                    return defaultValue;
                }
            }
            else
            {
                double value;
                if (double.TryParse(text, out value))
                {
                    if (value == 1)
                    {return true;}
                    else { return false; }
                }
                else
                {
                    errors.Add(new ModelError("xxxxx"));
                    return defaultValue;
                }
            }
        }
    }
}
