using System.Collections.Generic;
using System.Linq;

namespace OneNoteDuplicatesRemover.Core.Tests
{
    public class PageGroupDumpTests
    {
        private static readonly PageGroup Duplicates = new PageGroup("H1", new[]
        {
            new PageRef("p1", "Meeting", @"C:\Notes\Work\A.one"),
            new PageRef("p2", "Meeting", @"C:\Notes\Home\A.one"),
        });

        private static readonly PageGroup Single = new PageGroup("H2", new[]
        {
            new PageRef("p3", "Recipe", @"C:\Notes\Home\B.one"),
        });

        [Fact]
        public void Serialize_WritesTheTupleShapeOfOlderVersions()
        {
            string json = PageGroupDump.Serialize(new[] { Duplicates, Single });

            Assert.Equal(
                "{\"H1\":[{\"Item1\":\"p1\",\"Item2\":\"Meeting\"},{\"Item1\":\"p2\",\"Item2\":\"Meeting\"}]," +
                "\"H2\":[{\"Item1\":\"p3\",\"Item2\":\"Recipe\"}]}",
                json);
        }

        [Fact]
        public void SelectPagesWithDumpedContent_ReturnsEveryCopyOfTheMatchingGroups()
        {
            // Indented like a hand-edited or Newtonsoft.Json-formatted dump; only the hashes are used.
            string dump = "{\n  \"H1\": [ { \"Item1\": \"old-id\", \"Item2\": \"Old title\" } ],\n  \"H9\": []\n}";

            List<PageRef> pages = PageGroupDump.SelectPagesWithDumpedContent(new[] { Duplicates, Single }, dump);

            Assert.Equal(new[] { "p1", "p2" }, pages.Select(page => page.PageId));
        }

        [Fact]
        public void SelectPagesWithDumpedContent_ReadsWhatSerializeWrites()
        {
            string dump = PageGroupDump.Serialize(new[] { Single });

            List<PageRef> pages = PageGroupDump.SelectPagesWithDumpedContent(new[] { Duplicates, Single }, dump);

            Assert.Equal(new[] { "p3" }, pages.Select(page => page.PageId));
        }
    }
}
