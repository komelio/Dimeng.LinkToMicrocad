using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Business
{
    public struct ModelError
    {
        public string Message;

        public ModelError(string message)
        {
            this.Message = message;
        }
    }
}
