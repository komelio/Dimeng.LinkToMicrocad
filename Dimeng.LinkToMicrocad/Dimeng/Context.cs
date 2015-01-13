using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.LinkToMicrocad
{
    public class Context//Singleton mode
    {
        private static readonly Context instance = new Context();
        public static Context GetContext()
        {
            return instance;
        }

        public void Init()
        {
            //Microcad产品信息
            AKInfo = AKInfo.GetInfo();

            //MV数据信息
            MVDataContext = MVDataContext.GetContext(AKInfo.Path);
        }

        public AKInfo AKInfo { get; private set; }
        public MVDataContext MVDataContext { get; private set; }
    }
}
