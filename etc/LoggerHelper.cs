using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace OneNoteDuplicatesRemover.etc
{
    public static class LoggerHelper
    {
        public class LogMessageCateogryCount
        {
            public int CountInfo = 0;
            public int CountWarning = 0;
            public int CountError = 0;
            public int CountException = 0;
        }

        public static LogMessageCateogryCount logMessageCateogryCount = new LogMessageCateogryCount();
        public delegate void CategoryCountChangedHandler(int countInfo, int countWarning, int countError, int countException);
        public static event CategoryCountChangedHandler EventCategoryCountChanged = null;

        private static string Message(string category, string current_timestamp, string format, params object[] args)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("[{0}][{1}]", category, current_timestamp));
            sb.Append(" ");
            sb.Append(string.Format(format, args));
            return sb.ToString();
        }

        public static void LogInfo(string format, params object[] args)
        {
            string currentTimestamp = DateTime.Now.ToString();
            string message = Message("INFO", currentTimestamp, format, args);
            etc.FileLogger.Instance.Print(message);
            etc.DebugLogger.Instance.Print(message);
            lock (logMessageCateogryCount)
            {
                logMessageCateogryCount.CountInfo += 1;
                EventCategoryCountChanged?.Invoke(logMessageCateogryCount.CountInfo, logMessageCateogryCount.CountWarning, logMessageCateogryCount.CountError, logMessageCateogryCount.CountException);
            }
        }

        public static void LogWarn(string format, params object[] args)
        {
            string currentTimestamp = DateTime.Now.ToString();
            string message = Message("WARNING", currentTimestamp, format, args);
            etc.FileLogger.Instance.Print(message);
            etc.DebugLogger.Instance.Print(message);
            lock (logMessageCateogryCount)
            {
                logMessageCateogryCount.CountWarning += 1;
                EventCategoryCountChanged?.Invoke(logMessageCateogryCount.CountInfo, logMessageCateogryCount.CountWarning, logMessageCateogryCount.CountError, logMessageCateogryCount.CountException);
            }
        }

        public static void LogError(string format, params object[] args)
        {
            string currentTimestamp = DateTime.Now.ToString();
            string message = Message("ERROR", currentTimestamp, format, args);
            etc.FileLogger.Instance.Print(message);
            etc.DebugLogger.Instance.Print(message);
            lock (logMessageCateogryCount)
            {
                logMessageCateogryCount.CountError += 1;
                EventCategoryCountChanged?.Invoke(logMessageCateogryCount.CountInfo, logMessageCateogryCount.CountWarning, logMessageCateogryCount.CountError, logMessageCateogryCount.CountException);
            }
        }

        public static void LogException(string format, params object[] args)
        {
            string currentTimestamp = DateTime.Now.ToString();
            string message = Message("EXCEPTION", currentTimestamp, format, args);
            etc.FileLogger.Instance.Print(message);
            etc.DebugLogger.Instance.Print(message);
            lock (logMessageCateogryCount)
            {
                logMessageCateogryCount.CountException += 1;
                EventCategoryCountChanged?.Invoke(logMessageCateogryCount.CountInfo, logMessageCateogryCount.CountWarning, logMessageCateogryCount.CountError, logMessageCateogryCount.CountException);
            }
        }

        public static void LogUnexpectedException(Exception e)
        {
            etc.LoggerHelper.LogException("Unexpected Exception: {0}", e.ToString());
        }
    }
}
