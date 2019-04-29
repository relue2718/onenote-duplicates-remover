using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OneNoteDuplicatesRemover
{
    public class HtmlReportGenerator
    {
        public bool GenerateReportForRemovalOperation(List<Tuple<string, string, bool>> resultRemoval, out string reportFileName)
        {
            DateTime timestampNow = DateTime.Now;
            reportFileName = string.Format("report-{0}.html", timestampNow.ToString("yyyy-MM-dd-HH-mm-ss"));
            string reportTitle = string.Format("Report @ {0}", timestampNow.ToString());

            List<Tuple<string, string>> pagesRemoved = new List<Tuple<string, string>>();
            List<Tuple<string, string>> pagesNotRemoved = new List<Tuple<string, string>>();

            foreach (Tuple<string, string, bool> elem in resultRemoval)
            {
                if (elem.Item3)
                {
                    pagesRemoved.Add(Tuple.Create(elem.Item1, elem.Item2));
                }
                else
                {
                    pagesNotRemoved.Add(Tuple.Create(elem.Item1, elem.Item2));
                }
            }

            if (System.IO.File.Exists(reportFileName) == false)
            {
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(reportFileName))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(string.Format("<html><head><title>{0}</title></head>", reportTitle));
                    sb.AppendLine(string.Format("<body>"));
                    sb.AppendLine(string.Format("<h1>{0}</h1>", reportTitle));
                    ////////////////////////////////////////////////////////////////////
                    sb.AppendLine(string.Format("<h3>Removed Pages (Count: {0})</h3>", pagesRemoved.Count));
                    sb.AppendLine(string.Format("<table border=1>"));
                    sb.AppendLine(string.Format("<thead><td>Page ID</td><td>Section Title</td></thead>"));
                    foreach (Tuple<string, string> elem in pagesRemoved)
                    {
                        sb.AppendLine(string.Format("<tr><td>{0}</td><td>{1}</td></tr>", elem.Item1, elem.Item2));
                    }
                    sb.AppendLine(string.Format("</table>"));
                    ////////////////////////////////////////////////////////////////////
                    sb.AppendLine(string.Format("<h3>Pages that could not be removed (Count:{0})</h3>", pagesNotRemoved.Count));
                    sb.AppendLine(string.Format("<table border=1>"));
                    sb.AppendLine(string.Format("<thead><td>Page ID</td><td>Section Title</td></thead>"));
                    foreach (Tuple<string, string> elem in pagesNotRemoved)
                    {
                        sb.AppendLine(string.Format("<tr><td>{0}</td><td>{1}</td></tr>", elem.Item1, elem.Item2));
                    }
                    sb.AppendLine(string.Format("</table>"));
                    sb.AppendLine(string.Format("</body></html>"));
                    sw.Write(sb.ToString());
                }
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
