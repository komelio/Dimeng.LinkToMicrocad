using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuoteExport.Dimensions
{
    public class HdrillSeq
    {
        public string CurrentFace { get; set; }
        public string PreviousFace { get; set; }
        public float HDrillX { get; set; }
        public float HDrillY { get; set; }
        public float HDrillZ { get; set; }
        public float HDrillDiameter { get; set; }
        public string HDrillToolName { get; set; }
        public float HDrillFeedSpeed { get; set; }
        public float HDrillEntrySpeed { get; set; }
        public int HDrillBitType { get; set; }
        public string HDrillFirstDrillDone { get; set; }
        public string HDrillPreviousToolName { get; set; }
        public string HDrillNextToolName { get; set; }
        public string HDrillCounter { get; set; }

        public static HdrillSeq LoadSeq(string line)
        {
            HdrillSeq hds = new HdrillSeq();
            var pars = line.Split(',');
            hds.CurrentFace = pars[1];
            hds.PreviousFace = pars[2];
            hds.HDrillX = float.Parse(pars[3]);
            hds.HDrillY = float.Parse(pars[4]);
            hds.HDrillZ = float.Parse(pars[5]);
            hds.HDrillDiameter = float.Parse(pars[6]);
            hds.HDrillToolName = pars[7];
            //hds.HDrillFeedSpeed = float.Parse(pars[8]);
            //hds.HDrillEntrySpeed = float.Parse(pars[9]);
            //hds.HDrillBitType = int.Parse(pars[10]);
            hds.HDrillFirstDrillDone = pars[11];
            hds.HDrillPreviousToolName = pars[12];
            hds.HDrillNextToolName = pars[13];
            //hds.HDrillCounter = pars[14];
            return hds;
        }
    }
}
