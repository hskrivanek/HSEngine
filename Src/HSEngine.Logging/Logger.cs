using System;
using System.Collections.Generic;
using System.Text;

namespace HSEngine.Logging
{
    public class Logger
    {
        private readonly NLog.Logger logger;

        internal Logger(string name)
        {
            logger = NLog.LogManager.GetLogger(name);
        }

        public void Trace(string message) => logger.Trace(message);
        public void Debug(string message) => logger.Debug(message);
        public void Info(string message) => logger.Info(message);
        public void Warn(string message) => logger.Warn(message);
        public void Error(string message) => logger.Error(message);
        public void Fatal(string message) => logger.Fatal(message);
    }
}
