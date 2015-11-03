using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuoteExport.Dimensions
{
    public class RouteSeq
    {
        public RouteOneSeq SetMillSeq { get; private set; }
        public List<RouteOneSeq> RouteSeqs { get; private set; }
        public RouteSeq(string lineSetMill)
        {
            this.SetMillSeq = RouteOneSeq.LoadSeq(lineSetMill);
            this.RouteSeqs = new List<RouteOneSeq>();
        }
        public void AddRoute(string line)
        {
            RouteSeqs.Add(RouteOneSeq.LoadSeq(line));
        }
    }
    public class RouteOneSeq
    {
        public float RouteSetMillX { get; set; }
        public float RouteSetMillY { get; set; }
        public float RouteSetMillZ { get; set; }
        public float RouteStartOffsetX { get; set; }
        public float RouteStartOffsetY { get; set; }
        public float RouteDiameter { get; set; }
        public string RouteToolName { get; set; }
        public string RoutePreviousToolName { get; set; }
        public string RouteNextToolName { get; set; }
        public string RouteFeedSpeed { get; set; }
        public string RouteEntrySpeed { get; set; }
        public string RouteBitType { get; set; }
        public string RouteRotation { get; set; }
        public string RouteToolComp { get; set; }
        public float RouteX { get; set; }
        public float RouteY { get; set; }
        public float RouteZ { get; set; }
        public string RouteEndOffsetX { get; set; }
        public string RouteEndOffsetY { get; set; }
        public float RouteBulge { get; set; }
        public float RouteRadius { get; set; }
        public float RouteCenterX { get; set; }
        public float RouteCenterY { get; set; }
        public string RouteNextX { get; set; }
        public string RouteNextY { get; set; }
        public string RoutePreviousX { get; set; }
        public string RoutePreviousY { get; set; }
        public string RoutePreviousZ { get; set; }
        public string RouteBulgeNext { get; set; }
        public string RouteSetMillCounter { get; set; }
        public string RouteVectorCounter { get; set; }
        public string RouteVectorCount { get; set; }
        public string RouteAngle { get; set; }
        public string RoutePreviousFeedSpeed { get; set; }
        public string A35 { get; set; }
        public static RouteOneSeq LoadSeq(string line)
        {
            RouteOneSeq rs = new RouteOneSeq();
            var pars = line.Split(',');
            rs.RouteSetMillX = string.IsNullOrEmpty(pars[1]) ? 0 : float.Parse(pars[1]);
            rs.RouteSetMillY = string.IsNullOrEmpty(pars[2]) ? 0 : float.Parse(pars[2]);
            rs.RouteSetMillZ = string.IsNullOrEmpty(pars[3]) ? 0 : float.Parse(pars[3]);
            rs.RouteStartOffsetX = string.IsNullOrEmpty(pars[4]) ? 0 : float.Parse(pars[4]);
            rs.RouteStartOffsetY = string.IsNullOrEmpty(pars[5]) ? 0 : float.Parse(pars[5]);
            rs.RouteDiameter = string.IsNullOrEmpty(pars[6]) ? 0 : float.Parse(pars[6]);
            rs.RouteToolName = pars[7];
            rs.RoutePreviousToolName = pars[8];
            rs.RouteNextToolName = pars[9];
            rs.RouteFeedSpeed = pars[10];
            rs.RouteEntrySpeed = pars[11];
            rs.RouteBitType = pars[12];
            rs.RouteRotation = pars[13];
            rs.RouteToolComp = pars[14];
            rs.RouteX = string.IsNullOrEmpty(pars[15]) ? 0 : float.Parse(pars[15]);
            rs.RouteY = string.IsNullOrEmpty(pars[16]) ? 0 : float.Parse(pars[16]);
            rs.RouteZ = string.IsNullOrEmpty(pars[17]) ? 0 : float.Parse(pars[17]);
            rs.RouteEndOffsetX = pars[18];
            rs.RouteEndOffsetY = pars[19];
            rs.RouteBulge = string.IsNullOrEmpty(pars[20]) ? 0 : float.Parse(pars[20]);
            rs.RouteRadius = string.IsNullOrEmpty(pars[21]) ? 0 : float.Parse(pars[21]);
            rs.RouteCenterX = string.IsNullOrEmpty(pars[22]) ? 0 : float.Parse(pars[22]);
            rs.RouteCenterY = string.IsNullOrEmpty(pars[23]) ? 0 : float.Parse(pars[23]);
            rs.RouteNextX = pars[24];
            rs.RouteNextY = pars[25];
            rs.RoutePreviousX = pars[26];
            rs.RoutePreviousY = pars[27];
            rs.RoutePreviousZ = pars[28];
            rs.RouteBulgeNext = pars[29];
            rs.RouteSetMillCounter = pars[30];
            rs.RouteVectorCounter = pars[31];
            rs.RouteVectorCount = pars[32];
            rs.RouteAngle = pars[33];
            rs.RoutePreviousFeedSpeed = pars[34];
            rs.A35 = pars[35];
            return rs;
        }
    }
}
