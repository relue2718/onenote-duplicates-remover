using System;
using System.Collections.Generic;
using System.Linq;

namespace OneNoteDuplicatesRemover.Core.Tests
{
    // Randomized comparison against the pre-refactor logic, plus invariants that must hold for any scan.
    public class KeepPolicyEquivalenceTests
    {
        private const int Scenarios = 1000;

        // Local, cloud, Recycle Bin, non-ASCII, and case-only variants, as OneNote reports section folders.
        private static readonly string[] Folders =
        {
            @"C:\Notes\Work", @"C:\Notes\Archive", @"C:\Notes\archive", @"C:\Notes\회의", @"D:\Backup\Notes\Work",
            @"C:\Notes\OneNote_RecycleBin", @"C:\Notes\Work\OneNote_RecycleBin",
            "https://d.docs.live.net/2B5703B7A65ED749/Documents/Notes",
            "https://d.docs.live.net/2B5703B7A65ED749/Documents/Notes/OneNote_RecycleBin",
            "https://d.docs.live.net/2B5703B7A65ED749/Documents/회의록",
            "https://contoso.sharepoint.com/sites/team/Shared Documents/Notes",
        };

        private static List<PageGroup> RandomScan(Random random)
        {
            List<PageGroup> groups = new List<PageGroup>();
            int groupCount = random.Next(1, 30);
            for (int g = 0; g < groupCount; ++g)
            {
                List<PageRef> pages = new List<PageRef>();
                int pageCount = random.Next(1, 7);
                for (int p = 0; p < pageCount; ++p)
                {
                    string folder = Folders[random.Next(Folders.Length)];
                    string separator = folder.StartsWith("https:", StringComparison.Ordinal) ? "/" : "\\";
                    string sectionPath = folder + separator + "Section " + random.Next(3) + ".one";
                    pages.Add(new PageRef($"{{{g}-{p}}}", "Page " + g, sectionPath));
                }
                groups.Add(new PageGroup("H" + g, pages));
            }
            return groups;
        }

        private static IEnumerable<(int Scenario, List<PageGroup> Scan, List<string> Order)> Cases()
        {
            Random random = new Random(20260926);
            for (int scenario = 0; scenario < Scenarios; ++scenario)
            {
                List<PageGroup> scan = RandomScan(random);
                List<string> defaultOrder = LegacyKeepPolicy.GetSectionPathList(scan);
                yield return (scenario, scan, defaultOrder);
                // The user may reorder the list in any way.
                yield return (scenario, scan, defaultOrder.OrderBy(_ => random.Next()).ToList());
            }
        }

        [Fact]
        public void DefaultLocationOrder_MatchesThePreviousImplementation()
        {
            foreach ((int scenario, List<PageGroup> scan, _) in Cases())
            {
                List<string> expected = LegacyKeepPolicy.GetSectionPathList(scan);
                List<string> actual = KeepPolicy.DefaultLocationOrder(scan);
                Assert.True(expected.SequenceEqual(actual), $"scenario {scenario}: [{string.Join(" | ", actual)}]");
            }
        }

        [Fact]
        public void SelectExtraCopies_MatchesThePreviousImplementation()
        {
            foreach ((int scenario, List<PageGroup> scan, List<string> order) in Cases())
            {
                List<PageGroup> duplicates = scan.Where(group => group.HasDuplicates).ToList();
                HashSet<string> expected = LegacyKeepPolicy.SelectAllExceptOne(duplicates, order);
                HashSet<string> actual = KeepPolicy.SelectExtraCopies(duplicates, order);
                Assert.True(expected.SetEquals(actual), $"scenario {scenario}");
            }
        }

        [Fact]
        public void SelectExtraCopies_KeepsExactlyOneCopyPerGroup()
        {
            foreach ((int scenario, List<PageGroup> scan, List<string> order) in Cases())
            {
                List<PageGroup> duplicates = scan.Where(group => group.HasDuplicates).ToList();
                HashSet<string> selected = KeepPolicy.SelectExtraCopies(duplicates, order);
                foreach (PageGroup group in duplicates)
                {
                    Assert.Equal(1, group.Pages.Count(page => !selected.Contains(page.PageId)));
                }
                Assert.Equal(duplicates.Sum(group => group.Pages.Count - 1), selected.Count);
                Assert.Null(KeepPolicy.FindGroupWithoutKeptCopy(duplicates, selected));
            }
        }

        [Fact]
        public void FindGroupWithoutKeptCopy_FindsTheFirstFullySelectedGroup()
        {
            Random random = new Random(7);
            for (int scenario = 0; scenario < Scenarios; ++scenario)
            {
                List<PageGroup> duplicates = RandomScan(random).Where(group => group.HasDuplicates).ToList();
                if (duplicates.Count == 0) continue;
                HashSet<string> selected = KeepPolicy.SelectExtraCopies(duplicates, new string[0]);
                PageGroup fullySelected = duplicates[random.Next(duplicates.Count)];
                selected.UnionWith(fullySelected.Pages.Select(page => page.PageId));

                Assert.Same(fullySelected, KeepPolicy.FindGroupWithoutKeptCopy(duplicates, selected));
            }
        }

        [Fact]
        public void DefaultLocationOrder_RanksCloudRecycleBinBetweenCloudAndLocal()
        {
            // OneNote reports OneDrive sections as URLs; Path.GetDirectoryName turns them into "https:\..." folders.
            PageRef cloud = new PageRef("1", "T", "https://d.docs.live.net/2B5703B7A65ED749/Documents/scratch/Quick Notes.one");
            PageRef cloudRecycled = new PageRef("2", "T", "https://d.docs.live.net/2B5703B7A65ED749/Documents/scratch/OneNote_RecycleBin/Quick Notes.one");
            PageRef local = new PageRef("3", "T", @"C:\Notes\Quick Notes.one");
            PageRef localRecycled = new PageRef("4", "T", @"C:\Notes\OneNote_RecycleBin\Quick Notes.one");

            List<string> order = KeepPolicy.DefaultLocationOrder(new[] { new PageGroup("H", new[] { localRecycled, local, cloudRecycled, cloud }) });

            Assert.Equal(@"https:\d.docs.live.net\2B5703B7A65ED749\Documents\scratch\OneNote_RecycleBin", cloudRecycled.Location);
            Assert.Equal(new[] { cloud.Location, cloudRecycled.Location, local.Location, localRecycled.Location }, order);
        }
    }
}
