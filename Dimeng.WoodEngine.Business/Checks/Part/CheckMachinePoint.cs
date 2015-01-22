using Dimeng.WoodEngine.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Business
{
    internal partial class PartChecker
    {
        internal MachinePoint MachinePoint()
        {
            string mp = range[0, 28].Text;

            if (string.IsNullOrEmpty(mp.Trim()))
            {
                mp = "1";//default point
            }

            mp = mp.ToUpper();

            string[] rangeMP = new string[] 
            {
                "1", "1M", "2", "2M", "3", "3M", "4", "4M", "5", "5M", "6", "6M", "7", "7M", "8", "8M" 
            };

            if (!rangeMP.Contains(mp))
            {
                //this.writeError("不在范围1-8/1M-8M之内", "机加工原点");
                mp = "1";
            }

            return new MachinePoint(mp);

        }
    }
}
