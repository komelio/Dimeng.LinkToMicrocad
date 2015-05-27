using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimeng.WoodEngine.Entities
{
    public struct ModelError
    {
        public ModelError(string _message)
        {
            message = _message;
            location = string.Empty;
            errorLevel = ErrorLevel.Warn;
        }

        public ModelError(string _location, string _message, ErrorLevel level)
        {
            message = _message;
            location = _location;
            errorLevel = level;
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

        private ErrorLevel errorLevel;
        public ErrorLevel ErrorLevel
        {
            get { return errorLevel; }
            set { errorLevel = value; }
        }
    }
}
