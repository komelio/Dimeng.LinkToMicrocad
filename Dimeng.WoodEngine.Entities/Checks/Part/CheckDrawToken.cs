using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Entities.Checks
{
    public partial class PartChecker
    {
        public string Layname3d(out bool IsDraw3d)
        {
            string text = range[0, 35].Text.ToUpper();

            if (text.StartsWith("DRAW3DBOX"))
            {
                IsDraw3d = true;

                string[] words = text.Split(' ');
                if (words.Length > 1)
                {
                    return "3D_" + words[1];
                }
                else
                {
                    return "0";
                }
            }

            IsDraw3d = false;
            return "0";
        }
    }
}
