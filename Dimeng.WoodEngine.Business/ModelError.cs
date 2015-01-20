using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Business
{
    public struct ModelError
    {
        public ModelError(string _message)
        {
            message = _message;
            location = string.Empty;
        }

        public ModelError(string _location, string _message)
        {
            message = _message;
            location = _location;
        }

        private string message;
        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        private string location;
        public string Location
        {
            get { return location; }
            set { location = value; }
        }

       
    }
}
