using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuoteExport.Dimensions
{
    public class BorderSeq
    {
        public float PanelWidth { get; set; }
        public float PanelLength { get; set; }
        public float PanelThickness { get; set; }
        public string RunField { get; set; }
        public string CurrentFace { get; set; }
        public string PreviousFace { get; set; }
        public string CurrentZoneName { get; set; }
        public string FieldOffsetX { get; set; }
        public string FieldOffsetY { get; set; }
        public string FieldOffsetZ { get; set; }
        public string JobName { get; set; }
        public string ItemNumber { get; set; }
        public string FileName { get; set; }
        public string Face6FileName { get; set; }
        public string Description { get; set; }
        public int PartQty { get; set; }
        public float CutPartWidth { get; set; }
        public float CutPartLength { get; set; }
        public string MaterialName { get; set; }
        public string MaterialCode { get; set; }
        public string Edge1 { get; set; }
        public string Edge2 { get; set; }
        public string Edge3 { get; set; }
        public string Edge4 { get; set; }
        public string PartComments { get; set; }
        public string ProductDescription { get; set; }
        public string ProductQty { get; set; }
        public string ProductWidth { get; set; }
        public string ProductHeight { get; set; }
        public string ProductDepth { get; set; }
        public string ProductComments { get; set; }
        public string PerfectGrain { get; set; }
        public string GrainFlag { get; set; }
        public string PartCounter { get; set; }
        public bool FoundHdrill { get; set; }
        public bool FoundVdrill { get; set; }
        public bool FoundVdrillFace6 { get; set; }
        public bool FoundRouting { get; set; }
        public bool FoundRoutingFace6 { get; set; }
        public bool FoundSawing { get; set; }
        public bool FoundSawingFace6 { get; set; }
        public bool FoundFace6Program { get; set; }
        public bool FoundNesting { get; set; }
        public string FirstPassDepth { get; set; }
        public string SpoilBoardPenetration { get; set; }
        public string BasePoint { get; set; }
        public string MachinePoint { get; set; }
        public string MfgDataPath { get; set; }
        public string MaterialType { get; set; }
        public string EdgeFileNames1 { get; set; }
        public string EdgeFileNames2 { get; set; }
        public string EdgeFileNames3 { get; set; }
        public string EdgeFileNames4 { get; set; }
        public string EdgeBarCodes1 { get; set; }
        public string EdgeBarCodes2 { get; set; }
        public string EdgeBarCodes3 { get; set; }
        public string EdgeBarCodes4 { get; set; }
        public string A58 { get; set; }
        public string A59 { get; set; }
        public string A60 { get; set; }
        public string A61 { get; set; }
        public string A62 { get; set; }
        public string A63 { get; set; }
        public string A64 { get; set; }
        public string A65 { get; set; }
        public string A66 { get; set; }
        public string A67 { get; set; }
        public string A68 { get; set; }
        public bool Face6Only { get; set; }
        public bool Face5Only { get; set; }
        public bool DoubleFace5 { get; set; }
        public bool DoubleFace6 { get; set; }
        public static BorderSeq LoadSeq(string Line)
        {
            BorderSeq border = new BorderSeq();
            var pars = Line.Split(',');
            border.PanelWidth = float.Parse(pars[1]);
            border.PanelLength = float.Parse(pars[2]);
            border.PanelThickness = float.Parse(pars[3]);
            border.RunField = pars[4];
            border.CurrentFace = pars[5];
            border.PreviousFace = pars[6];
            border.CurrentZoneName = pars[7];
            border.FieldOffsetX = pars[8];
            border.FieldOffsetY = pars[9];
            border.FieldOffsetZ = pars[10];
            border.JobName = pars[11];
            border.ItemNumber = pars[12];
            border.FileName = pars[13];
            border.Face6FileName = pars[14];
            border.Description = pars[15];
            border.PartQty = int.Parse(pars[16]);
            border.CutPartWidth = float.Parse(pars[17]);
            border.CutPartLength = float.Parse(pars[18]);
            border.MaterialName = pars[19];
            border.MaterialCode = pars[20];
            border.Edge1 = pars[21];
            border.Edge2 = pars[23];
            border.Edge3 = pars[22];
            border.Edge4 = pars[24];
            border.PartComments = pars[25];
            border.ProductDescription = pars[26];
            border.ProductQty = pars[27];
            border.ProductWidth = pars[28];
            border.ProductHeight = pars[29];
            border.ProductDescription = pars[30];
            border.ProductComments = pars[31];
            border.PerfectGrain = pars[32];
            border.GrainFlag = pars[33];
            border.PartCounter = pars[34];
            border.FoundHdrill = bool.Parse(pars[35]);
            border.FoundVdrill = bool.Parse(pars[36]);
            border.FoundVdrillFace6 = bool.Parse(pars[37]);
            border.FoundRouting = bool.Parse(pars[38]);
            border.FoundRoutingFace6 = bool.Parse(pars[39]);
            border.FoundSawing = bool.Parse(pars[40]);
            border.FoundSawingFace6 = bool.Parse(pars[41]);
            border.FoundFace6Program = bool.Parse(pars[42]);
            border.FoundNesting = bool.Parse(pars[43]);
            border.FirstPassDepth = pars[44];
            border.SpoilBoardPenetration = pars[45];
            border.BasePoint = pars[46];
            border.MachinePoint = string.IsNullOrEmpty(pars[47]) ? "1" : pars[47];
            //border.MfgDataPath = pars[48];
            //border.MaterialType = pars[49];
            //border.EdgeFileNames1 = pars[50];
            //border.EdgeFileNames2 = pars[51];
            //border.EdgeFileNames3 = pars[52];
            //border.EdgeFileNames4 = pars[53];
            //border.EdgeBarCodes1 = pars[54];
            //border.EdgeBarCodes2 = pars[55];
            //border.EdgeBarCodes3 = pars[56];
            //border.EdgeBarCodes4 = pars[57];
            //border.A58 = pars[58];
            //border.A59 = pars[59];
            //border.A60 = pars[60];
            //border.A61 = pars[61];
            //border.A62 = pars[62];
            //border.A63 = pars[63];
            //border.A64 = pars[64];
            //border.A65 = pars[65];
            //border.A66 = pars[66];
            //border.A67 = pars[67];
            //border.A68 = pars[68];
            border.Face5Only = border.FileName != "" & border.Face6FileName == "";
            border.Face6Only = border.FileName == "" & !border.FoundRouting & !border.FoundSawing & !border.FoundVdrill;
            border.DoubleFace5 = border.FileName != "" & border.Face6FileName != "";
            border.DoubleFace6 = border.FileName == "" & border.Face6FileName != "" & (border.FoundVdrill | border.FoundRouting | border.FoundSawing);
            return border;
        }
    }
}
