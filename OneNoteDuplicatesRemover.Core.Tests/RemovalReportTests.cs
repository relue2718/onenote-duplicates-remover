using System;

namespace OneNoteDuplicatesRemover.Core.Tests
{
    public class RemovalReportTests
    {
        private static readonly DateTime Timestamp = new DateTime(2026, 9, 26, 10, 0, 0);

        [Fact]
        public void BuildHtml_ListsTitleLocationAndIdInSeparateColumns()
        {
            PageRef page = new PageRef("{ID-1}", "Meeting notes", @"C:\Notes\Work\A.one");

            string html = RemovalReport.BuildHtml(new[] { new RemovalResult(page, true) }, Timestamp);

            Assert.Contains("<th>Page title</th><th>Location</th><th>Page ID</th>", html);
            Assert.Contains(@"<tr><td>Meeting notes</td><td>C:\Notes\Work\A.one</td><td>{ID-1}</td></tr>", html);
        }

        [Fact]
        public void BuildHtml_EncodesValuesFromNotebooks()
        {
            PageRef page = new PageRef("id&1", "<script>alert(1)</script> Tom & Jerry", @"C:\A<B>\S.one");

            string html = RemovalReport.BuildHtml(new[] { new RemovalResult(page, false) }, Timestamp);

            Assert.DoesNotContain("<script>", html);
            Assert.Contains("&lt;script&gt;alert(1)&lt;/script&gt; Tom &amp; Jerry", html);
            Assert.Contains(@"C:\A&lt;B&gt;\S.one", html);
            Assert.Contains("id&amp;1", html);
        }

        [Fact]
        public void BuildHtml_SplitsRemovedAndFailedPagesAndDeclaresUtf8()
        {
            PageRef removed = new PageRef("p1", "회의록", @"C:\Notes\A.one");
            PageRef failed = new PageRef("p2", "Failed", @"C:\Notes\B.one");
            PageRef alsoRemoved = new PageRef("p3", "Other", @"C:\Notes\C.one");

            string html = RemovalReport.BuildHtml(new[]
            {
                new RemovalResult(removed, true), new RemovalResult(failed, false), new RemovalResult(alsoRemoved, true),
            }, Timestamp);

            Assert.Contains("<meta charset=\"utf-8\">", html);
            Assert.Contains("Removed pages (Count: 2)", html);
            Assert.Contains("Pages that could not be removed (Count: 1)", html);
            Assert.Contains("회의록", html);
            int failedSection = html.IndexOf("Pages that could not be removed", StringComparison.Ordinal);
            Assert.True(html.IndexOf("<td>Failed</td>", StringComparison.Ordinal) > failedSection);
            Assert.True(html.IndexOf("<td>Other</td>", StringComparison.Ordinal) < failedSection);
        }
    }
}
