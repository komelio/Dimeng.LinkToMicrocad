using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net.Config;
using System.IO;
using log4net;

namespace Dimeng.LinkToMicrocad.Logging
{
    public class Logger
    {
        //Singleton Mode
        private static readonly Logger instance = new Logger();
        public static Logger GetLogger()
        {
            return instance;
        }

        private Logger()
        {
            //Properties.Resources.Log4net
            //Set up the log4net
            XmlConfigurator.Configure(new MemoryStream(Properties.Resources.Log4net));
        }

        private readonly ILog logger = LogManager.GetLogger("Dimeng");

        public void Debug(string message)
        {
            if (logger.IsDebugEnabled)
            {
                logger.Debug(message);
            }
        }

        public void Info(string message)
        {
            if (logger.IsInfoEnabled)
            {
                logger.Info(message);
            }
        }

        public void Fatal(string message)
        {
            if (logger.IsFatalEnabled)
            {
                logger.Fatal(message);
            }
        }

        public void Warn(string message)
        {
            if (logger.IsWarnEnabled)
            {
                logger.Warn(message);
            }
        }

        public void Error(string message)
        {
            if (logger.IsErrorEnabled)
            {
                logger.Error(message);
            }
        }

        public void Error(Exception error)
        {
            if (logger.IsErrorEnabled)
            {
                logger.Error(string.Format("{0}\n {1}", error.Message, error.StackTrace));

                if (error.InnerException != null)
                {
                    errorInnerException(error.InnerException);
                }
            }
        }

        private void errorInnerException(Exception error)
        {
            logger.Error(string.Format("{0}\n {1}", error.Message, error.StackTrace));

            if (error.InnerException != null)
            {
                errorInnerException(error.InnerException);
            }
        }
    }
}
