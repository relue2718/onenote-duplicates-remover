using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OneNoteDuplicatesRemover.etc
{
  public static class LoggerHelper
  {
    public static void LogInfo(string format, params object[] args)
    {
      etc.FileLogger.Instance.Print("INFO", format, args);
    }

    public static void LogWarn(string format, params object[] args)
    {
      etc.FileLogger.Instance.Print("WARNING", format, args);
    }

    public static void LogError(string format, params object[] args)
    {
      etc.FileLogger.Instance.Print("ERROR", format, args);
    }

    public static void LogException(string format, params object[] args)
    {
      etc.FileLogger.Instance.Print("EXCEPTION", format, args);
    }

    public static void LogException(Exception e)
    {
      etc.LoggerHelper.LogException("Unexpected Exception: {0}", e.ToString());
    }
  }
}
