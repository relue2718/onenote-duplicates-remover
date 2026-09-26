using System;
using System.Security.Cryptography;

namespace OneNoteDuplicatesRemover.Core.Tests
{
    public class PageGroupTests
    {
        [Fact]
        public void EmptyContentHash_IsTheSha256OfAnEmptyString()
        {
            Assert.Equal(PageGroup.EmptyContentHash, Convert.ToHexString(SHA256.HashData(Array.Empty<byte>())));
        }

        [Fact]
        public void HasDuplicates_NeedsMoreThanOnePage()
        {
            PageRef page = new PageRef("p1", "Title", @"C:\Notes\A.one");

            Assert.False(new PageGroup("H1", new[] { page }).HasDuplicates);
            Assert.True(new PageGroup("H1", new[] { page, page with { PageId = "p2" } }).HasDuplicates);
        }

        [Fact]
        public void Location_IsTheFolderOfTheSectionFile()
        {
            Assert.Equal(@"C:\Notes\Work", new PageRef("p1", "Title", @"C:\Notes\Work\A.one").Location);
        }
    }
}
