using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuoteExport.Dimensions
{
    public class VdrillSeq
    {
        public float VDrillX { get; set; }
        public float VDrillY { get; set; }
        public float VDrillZ { get; set; }
        public float VDrillX_Offset { get; set; }
        public float VDrillY_Offset { get; set; }
        public float VDrillDiameter { get; set; }
        public string VDrillToolName { get; set; }
        public float VDrillFeedSpeed { get; set; }
        public string VDrillEntrySpeed { get; set; }
        public int VDrillBitType { get; set; }
        public string VDrillFirstDrillDone { get; set; }
        public string VDrillPreviousToolName { get; set; }
        public string VDrillCounter { get; set; }
        public static VdrillSeq LoadSeq(string line)
        {
            VdrillSeq vds = new VdrillSeq();
            var pars = line.Split(',');
            vds.VDrillX = float.Parse(pars[1]);
            vds.VDrillY = float.Parse(pars[2]);
            vds.VDrillZ = float.Parse(pars[3]);
            vds.VDrillX_Offset = float.Parse(pars[4]);
            vds.VDrillY_Offset = float.Parse(pars[5]);
            vds.VDrillDiameter = float.Parse(pars[6]);
            vds.VDrillToolName = pars[7];
            //vds.VDrillFeedSpeed = float.Parse(pars[8]);
            //vds.VDrillEntrySpeed = pars[9];
            //vds.VDrillBitType = int.Parse(pars[10]);
            vds.VDrillFirstDrillDone = pars[11];
            vds.VDrillPreviousToolName = pars[12];
            vds.VDrillCounter = pars[13];
            return vds;
        }
    }
}
