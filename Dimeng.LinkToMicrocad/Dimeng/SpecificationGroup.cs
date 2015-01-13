using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.LinkToMicrocad
{
    internal class SpecificationGroup
    {
        public string Name { get; set; }
        public string GlobalFileName { get; set; }
        public string CutPartsFileName { get; set; }
        public string HardwareFileName { get; set; }
        public string EdgeBandFileName { get; set; }
        public string DoorWizardFileName { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
