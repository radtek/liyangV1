using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WCAPP
{
    public class Log
    {
        public static void Info(string info)
        {
            ILog log = LogManager.GetLogger("log4netlogger");
            log.Info(info);
        }
        public static void Error(string error)
        {
            ILog log = LogManager.GetLogger("log4netlogger");
            log.Error(error);
        }
    }
}