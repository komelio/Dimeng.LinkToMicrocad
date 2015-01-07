using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.LinkToMicrocad
{
    public class MVDataContext
    {
        /// <summary>
        /// Maintain Microvellum data information
        /// </summary>
        /// <returns></returns>
        public static MVDataContext GetContext()
        {


            return new MVDataContext();
        }
    }
}
