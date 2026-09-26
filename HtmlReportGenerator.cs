using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OneNoteDuplicatesRemover.Core;

namespace OneNoteDuplicatesRemover
{
    public class HtmlReportGenerator
    {
        public bool GenerateReportForRemovalOperation(List<RemovalResult> resultRemoval, out string reportFileName)
        {
            DateTime timestampNow = DateTime.Now;
            reportFileName = string.Format("report-{0}.html", timestampNow.ToString("yyyy-MM-dd-HH-mm-ss"));

            if (System.IO.File.Exists(reportFileName) == false)
            {
                System.IO.File.WriteAllText(reportFileName, RemovalReport.BuildHtml(resultRemoval, timestampNow));
                etc.LoggerHelper.LogInfo("Report Generated. {0}", reportFileName);
                return true;
            }
            else
            {
                etc.LoggerHelper.LogError("Report Generation Failed.");
                return false;
            }
        }
    }
}
