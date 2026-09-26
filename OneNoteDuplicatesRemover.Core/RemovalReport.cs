using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace OneNoteDuplicatesRemover.Core
{
    public sealed record RemovalResult(PageRef Page, bool Removed);

    // HTML summary of a removal run. Page titles and paths come from notebooks, so every value is encoded.
    public static class RemovalReport
    {
        public static string BuildHtml(IEnumerable<RemovalResult> results, DateTime timestamp)
        {
            List<RemovalResult> resultList = results.ToList();
            string reportTitle = "Report @ " + timestamp.ToString();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html><head><meta charset=\"utf-8\">");
            sb.AppendLine("<title>" + Encode(reportTitle) + "</title></head>");
            sb.AppendLine("<body>");
            sb.AppendLine("<h1>" + Encode(reportTitle) + "</h1>");
            AppendTable(sb, "Removed pages", resultList.Where(result => result.Removed));
            AppendTable(sb, "Pages that could not be removed", resultList.Where(result => !result.Removed));
            sb.AppendLine("</body></html>");
            return sb.ToString();
        }

        private static void AppendTable(StringBuilder sb, string heading, IEnumerable<RemovalResult> results)
        {
            List<PageRef> pages = results.Select(result => result.Page).ToList();
            sb.AppendLine(string.Format("<h3>{0} (Count: {1})</h3>", Encode(heading), pages.Count));
            sb.AppendLine("<table border=\"1\">");
            sb.AppendLine("<thead><tr><th>Page title</th><th>Location</th><th>Page ID</th></tr></thead>");
            sb.AppendLine("<tbody>");
            foreach (PageRef page in pages)
            {
                sb.AppendLine(string.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>",
                    Encode(page.Title), Encode(page.SectionPath), Encode(page.PageId)));
            }
            sb.AppendLine("</tbody></table>");
        }

        private static string Encode(string value)
        {
            return WebUtility.HtmlEncode(value ?? "");
        }
    }
}
