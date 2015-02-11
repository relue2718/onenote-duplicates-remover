using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OneNoteDuplicatesRemover.BugReport
{
  public class BugReportManager
  {
    private List<CaughtExceptionInfo> caughtExceptions = new List<CaughtExceptionInfo>();
    public void ReportCaughtException(Exception e)
    {
      CaughtExceptionInfo caughtExceptionInfo = new CaughtExceptionInfo();
      caughtExceptionInfo.Time = new DateTime().ToUniversalTime().ToString();
      caughtExceptionInfo.Exception = e.ToString();
      caughtExceptions.Add(caughtExceptionInfo);
    }

    public string CollectSystemInformation()
    {
      try
      {
        Dictionary<string, string> information = new Dictionary<string, string>();
        information.Add("Microsoft.Office.Interop.OneNote.Application.fullname", typeof(Microsoft.Office.Interop.OneNote.Application).Assembly.FullName);
        StringBuilder sb = new StringBuilder();
        foreach (KeyValuePair<string, string> e in information)
        {
          sb.Append(e.Key);
          sb.Append(":");
          sb.Append(e.Value);
          sb.Append("\r\n");
        }
        return sb.ToString();
      }
      catch (Exception e)
      {
        return e.ToString();
      }
    }
  }
}
