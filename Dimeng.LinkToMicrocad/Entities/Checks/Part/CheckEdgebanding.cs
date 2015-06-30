﻿using Dimeng.LinkToMicrocad.Logging;
using Dimeng.WoodEngine.Entities;
using SpreadsheetGear;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Entities.Checks
{
    public partial class PartChecker
    {
        public EdgeBanding EBW1(IWorkbook bookE, List<EdgeBanding> tempEdgeBandings)
        {
            return getPartEdgebanding(bookE, EdgebandingType.EBW1, tempEdgeBandings);
        }
        public EdgeBanding EBW2(IWorkbook bookE, List<EdgeBanding> tempEdgeBandings)
        {
            return getPartEdgebanding(bookE, EdgebandingType.EBW2, tempEdgeBandings);
        }
        public EdgeBanding EBL1(IWorkbook bookE, List<EdgeBanding> tempEdgeBandings)
        {
            return getPartEdgebanding(bookE, EdgebandingType.EBL1, tempEdgeBandings);
        }
        public EdgeBanding EBL2(IWorkbook bookE, List<EdgeBanding> tempEdgeBandings)
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
            Logger.GetLogger().Debug(edgeName);

            if (string.IsNullOrEmpty(edgeName.Trim()))
            {
                Logger.GetLogger().Debug("None edge");
                return EdgeBanding.Default();
            }

            EdgeBanding edge = tempEdgeBandings.Find(it => it.Name.Trim().ToUpper() == edgeName);
            if (!string.IsNullOrEmpty(edge.Name))
            {
                Logger.GetLogger().Debug(string.Format("Return edge {0}/{1}/{2}", edge.Name, edge.Thickness, edge.Code));
                return edge;
            }

            var sheet = bookE.Worksheets[1];
            for (int i = 0; i < sheet.Cells.RowCount; i++)
            {
                string _name = sheet.Cells[i, 0].Text;

                if (string.IsNullOrEmpty(_name.Trim()))
                {
                    break;
                }

                if (_name.ToUpper() == edgeName)
                {
                    double thick = GetDoubleValue(sheet.Cells[i, 1].Text, "Edgebanding thickness", true, errors);
                    string code = sheet.Cells[i, 3].Text;

                    if (thick > 0)
                    {
                        var edgeX = new EdgeBanding(edgeName, thick, code);
                        tempEdgeBandings.Add(edgeX);
                        Logger.GetLogger().Debug(string.Format("Return edge {0}/{1}/{2}", edgeName, thick, code));
                        return edgeX;
                    }
                }
            }

            //errors.Add()

            var e = new EdgeBanding(edgeName, 0, string.Empty);
            tempEdgeBandings.Add(e);

            errors.Add(
                new ModelError(string.Format("Edgeband {0} not found in edgx file!", edgeName))
                );

            return e;
        }
    }
}
