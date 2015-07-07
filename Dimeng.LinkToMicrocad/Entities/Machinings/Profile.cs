using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.Geometry;

namespace Dimeng.WoodEngine.Entities.Machinings
{
    public class Profile
    {
        public int StartPointNumber { get; set; }
        public int EndPointNumber { get; set; }

        public string SharpFile { get; set; } //截面图形的文件名
        public bool IsSharpFromFile { get; set; }//标记从文件读取形状，这是对应一些不需要从文件读取形状的可能性，比如圆形、三角形等等

        public double Angle { get; set; }//斜切的角度
    }
}
