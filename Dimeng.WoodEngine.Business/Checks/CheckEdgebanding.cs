using Dimeng.WoodEngine.Entities;
using SpreadsheetGear;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Business
{
    internal partial class PartChecker
    {
        internal EdgeBanding EBW1(IWorkbook bookE, List<EdgeBanding> tempEdgeBandings)
        {
            return getPartEdgebanding(bookE, EdgebandingType.EBW1, tempEdgeBandings);
        }
        internal EdgeBanding EBW2(IWorkbook bookE, List<EdgeBanding> tempEdgeBandings)
        {
            return getPartEdgebanding(bookE, EdgebandingType.EBW2, tempEdgeBandings);
        }
        internal EdgeBanding EBL1(IWorkbook bookE, List<EdgeBanding> tempEdgeBandings)
        {
            return getPartEdgebanding(bookE, EdgebandingType.EBL1, tempEdgeBandings);
        }
        internal EdgeBanding EBL2(IWorkbook bookE, List<EdgeBanding> tempEdgeBandings)
        {
            return getPartEdgebanding(bookE, EdgebandingType.EBL2, tempEdgeBandings);
        }

        private EdgeBanding getPartEdgebanding(IWorkbook bookE, EdgebandingType type, List<EdgeBanding> tempEdgeBandings)
        {
            int index = 0;
            switch (type)
            {
                case EdgebandingType.EBL1:
                    index = 24;
                    break;
                case EdgebandingType.EBL2:
                    index = 25;
                    break;
                case EdgebandingType.EBW1:
                    index = 22;
                    break;
                case EdgebandingType.EBW2:
                    index = 23;
                    break;
                default:
                    throw new Exception("Unknow type of edgebanding");
            }

            string edgeName = range[0, index].Text.ToUpper();
            if (string.IsNullOrEmpty(edgeName.Trim()))
            {
                return EdgeBanding.Default();
            }

            EdgeBanding edge = tempEdgeBandings.Find(it => it.Name.Trim().ToUpper() == edgeName);
            if (edge.Name != string.Empty)
            {
                return edge;
            }

            var sheet = bookE.Worksheets[1];
            for(int i=0 ; i<sheet.Cells.RowCount;i++)
            {
                string _name = sheet.Cells[i, 0].Text;

                if(string.IsNullOrEmpty(_name.Trim()))
                {
                    break;
                }

                if(_name.ToUpper() == edgeName)
                {
                    double thick = GetDoubleValue(sheet.Cells[i, 1].Text, "Edgebanding thickness", true, errors);

                    if(thick>0)
                    {
                        tempEdgeBandings.Add(new EdgeBanding(edgeName, thick));
                    }
                }
            }

            //errors.Add()

            tempEdgeBandings.Add(EdgeBanding.Default());
            return EdgeBanding.Default();
        }
    }
}
