using System.Collections.Generic;
using System.Linq;

namespace OneNoteDuplicatesRemover.Core.Tests
{
    public class KeepPolicyTests
    {
        private static PageRef Page(string pageId, string sectionPath)
        {
            return new PageRef(pageId, "Title " + pageId, sectionPath);
        }

        private static PageGroup Group(string contentHash, params PageRef[] pages)
        {
            return new PageGroup(contentHash, pages);
        }

        [Fact]
        public void DefaultLocationOrder_PutsCloudFirstRecycleBinLastAndSortsTheRest()
        {
            PageRef work = Page("work", @"C:\Notes\Work\A.one");
            PageRef archive = Page("archive", @"C:\Notes\Archive\A.one");
            PageRef recycled = Page("recycled", @"C:\Notes\OneNote_RecycleBin\A.one");
            PageRef cloud = Page("cloud", "https://d.docs.live.net/0123/Documents/Notes/A.one");
            PageGroup group = Group("H1", work, recycled, archive, cloud);

            List<string> order = KeepPolicy.DefaultLocationOrder(new[] { group });

            Assert.Equal(new[] { cloud.Location, archive.Location, work.Location, recycled.Location }, order);
        }

        [Fact]
        public void DefaultLocationOrder_IgnoresSinglePageGroupsAndListsEachLocationOnce()
        {
            PageGroup duplicates = Group("H1", Page("a", @"C:\Notes\Work\A.one"), Page("b", @"C:\Notes\Work\B.one"));
            PageGroup single = Group("H2", Page("c", @"C:\Notes\Other\C.one"));

            List<string> order = KeepPolicy.DefaultLocationOrder(new[] { duplicates, single });

            Assert.Equal(new[] { @"C:\Notes\Work" }, order);
        }

        [Fact]
        public void SelectExtraCopies_KeepsTheCopyInTheMostPreferredLocation()
        {
            PageGroup group = Group("H1",
                Page("a", @"C:\Notes\Work\A.one"),
                Page("b", @"C:\Notes\Home\A.one"),
                Page("c", @"C:\Notes\Archive\A.one"));
            string[] order = { @"C:\Notes\Home", @"C:\Notes\Archive", @"C:\Notes\Work" };

            HashSet<string> selected = KeepPolicy.SelectExtraCopies(new[] { group }, order);

            Assert.Equal(new[] { "a", "c" }, selected.OrderBy(id => id));
        }

        [Fact]
        public void SelectExtraCopies_KeepsTheFirstCopyWhenLocationsTie()
        {
            PageGroup group = Group("H1", Page("a", @"C:\Notes\Work\A.one"), Page("b", @"C:\Notes\Work\B.one"));

            HashSet<string> selected = KeepPolicy.SelectExtraCopies(new[] { group }, new[] { @"C:\Notes\Work" });

            Assert.Equal(new[] { "b" }, selected);
        }

        [Fact]
        public void SelectExtraCopies_PrefersRankedLocationsOverUnrankedOnes()
        {
            PageGroup group = Group("H1", Page("unranked", @"C:\Elsewhere\A.one"), Page("ranked", @"C:\Notes\Work\A.one"));

            HashSet<string> selected = KeepPolicy.SelectExtraCopies(new[] { group }, new[] { @"C:\Notes\Work" });

            Assert.Equal(new[] { "unranked" }, selected);
        }

        [Fact]
        public void SelectExtraCopies_KeepsOneCopyOfEveryGroupEvenWithoutAnyRanking()
        {
            PageGroup first = Group("H1", Page("a", @"C:\X\A.one"), Page("b", @"C:\Y\B.one"));
            PageGroup second = Group("H2", Page("c", @"C:\X\C.one"), Page("d", @"C:\Y\D.one"), Page("e", @"C:\Z\E.one"));

            HashSet<string> selected = KeepPolicy.SelectExtraCopies(new[] { first, second }, new string[0]);

            Assert.Equal(new[] { "b", "d", "e" }, selected.OrderBy(id => id));
            Assert.Null(KeepPolicy.FindGroupWithoutKeptCopy(new[] { first, second }, selected));
        }

        [Fact]
        public void FindGroupWithoutKeptCopy_ReturnsTheFirstGroupWhoseCopiesAreAllSelected()
        {
            PageGroup kept = Group("H1", Page("a", @"C:\X\A.one"), Page("b", @"C:\X\B.one"));
            PageGroup lost = Group("H2", Page("c", @"C:\X\C.one"), Page("d", @"C:\X\D.one"));
            HashSet<string> selected = new HashSet<string> { "a", "c", "d" };

            Assert.Same(lost, KeepPolicy.FindGroupWithoutKeptCopy(new[] { kept, lost }, selected));
        }

        [Fact]
        public void FindGroupWithoutKeptCopy_ReturnsNullWhenEveryGroupKeepsACopy()
        {
            PageGroup group = Group("H1", Page("a", @"C:\X\A.one"), Page("b", @"C:\X\B.one"));

            Assert.Null(KeepPolicy.FindGroupWithoutKeptCopy(new[] { group }, new HashSet<string> { "a" }));
            Assert.Null(KeepPolicy.FindGroupWithoutKeptCopy(new[] { group }, new HashSet<string>()));
        }
    }
}
