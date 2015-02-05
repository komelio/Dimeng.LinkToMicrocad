using Dimeng.LinkToMicrocad.Logging;
using Dimeng.WoodEngine.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Business
{
    public class EQParaser
    {
        private string xString;
        private string yString;
        private string zString;

        public EQParaser(string xStr, string yStr, string zStr)
        {
            this.xString = xStr.ToUpper().TrimStart().TrimEnd();
            this.yString = yStr.ToUpper().TrimStart().TrimEnd();
            this.zString = zStr.ToUpper().TrimStart().TrimEnd();
        }

        public IEnumerable<double> GetPositions(List<ModelError> errors, int qty, double panelThick, double productWidth)
        {
            Logger.GetLogger().Debug(string.Format("Start parasing EQ strings:{0}/{1}/{2}", xString, yString, zString));

            if (!(xString.StartsWith("EQ") || yString.StartsWith("EQ") || zString.StartsWith("EQ")))
            {
                throw new Exception("String value must start with 'EQ'!");
            }

            List<double> doubles = new List<double>();



            return doubles;
        }
    }
}
