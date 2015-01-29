using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Entities
{
    public class MachinePoint
    {
        public MachinePoint(string mp)
        {
            mp = (string.IsNullOrEmpty(mp)) ? "1" : mp;//如果是空的，给个默认值

            if (mp == "1" || mp == "2M" || mp == "4" || mp == "3M" || mp == "5" || mp == "6M" || mp == "8" || mp == "7M")
            {
                IsRotated = false;
            }
            else
            {
                IsRotated = true;
            }

            MP = mp;
        }
        public bool IsRotated { get; private set; }
        public string MP { get; private set; }
    }
}
