using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using ServiceStack.Logging;
using ServiceStack.Logging.NLogger;

namespace Yaqaap.Framework
{
    public static class LogHelper
    {
        private static string _name;

        public static void SetProperty(string property, string value)
        {
            GlobalDiagnosticsContext.Set(property, value);
        }

        /// <summary>
        /// Gets global exception logger
        /// </summary>
        public static ILog GetLogger()
        {
            return ServiceStack.Logging.LogManager.GetLogger(_name);
        }

        public static void InitFactory(string name)
        {
            _name = name;
            ServiceStack.Logging.LogManager.LogFactory = new NLogFactory();
        }

        public static void SendReport(bool sendErrorReport)
        {
            NLog.LogManager.GlobalThreshold = sendErrorReport ? LogLevel.Debug : LogLevel.Off;
        }
    }
}
