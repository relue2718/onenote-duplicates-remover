using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace OneNoteDuplicatesRemover.etc
{
    public static class LoggerHelper
    {
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
            string current_timestamp = DateTime.Now.ToString();
            string message = Message("INFO", current_timestamp, format, args);
            etc.FileLogger.Instance.Print(message);
            Debug.WriteLine(message);
        }

        public static void LogWarn(string format, params object[] args)
        {
            string current_timestamp = DateTime.Now.ToString();
            string message = Message("WARNING", current_timestamp, format, args);
            etc.FileLogger.Instance.Print(message);
            Debug.WriteLine(message);
        }

        public static void LogError(string format, params object[] args)
        {
            string current_timestamp = DateTime.Now.ToString();
            string message = Message("ERROR", current_timestamp, format, args);
            etc.FileLogger.Instance.Print(message);
            Debug.WriteLine(message);
        }

        public static void LogException(string format, params object[] args)
        {
            string current_timestamp = DateTime.Now.ToString();
            string message = Message("EXCEPTION", current_timestamp, format, args);
            etc.FileLogger.Instance.Print(message);
            Debug.WriteLine(message);
        }

        public static void LogUnexpectedException(Exception e)
        {
            etc.LoggerHelper.LogException("Unexpected Exception: {0}", e.ToString());
        }
    }
}
