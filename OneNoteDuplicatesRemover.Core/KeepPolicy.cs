using System;
using System.Collections.Generic;
using System.Linq;

namespace OneNoteDuplicatesRemover.Core
{
    // Decides which copy of each duplicate group to keep.
    public static class KeepPolicy
    {
        // Initial keep order: cloud notebooks first, the Recycle Bin last, otherwise alphabetical.
        public static List<string> DefaultLocationOrder(IEnumerable<PageGroup> groups)
        {
            List<string> locations = groups
                .Where(group => group.HasDuplicates)
                .SelectMany(group => group.Pages)
                .Select(page => page.Location)
                .ToList();
            locations.Sort(CompareLocations);
            return locations.Distinct().ToList();
        }

        // Selects every copy except the one in the most preferred location.
        public static HashSet<string> SelectExtraCopies(IEnumerable<PageGroup> groups, IReadOnlyList<string> locationOrder)
        {
            Dictionary<string, int> ranks = RankLocations(locationOrder);
            HashSet<string> selectedPageIds = new HashSet<string>();
            foreach (PageGroup group in groups)
            {
                PageRef? keep = PreferredCopy(group, ranks);
                foreach (PageRef page in group.Pages)
                {
                    if (!ReferenceEquals(page, keep))
                    {
                        selectedPageIds.Add(page.PageId);
                    }
                }
            }
            return selectedPageIds;
        }

        // Removing pages must leave at least one copy in every group.
        // Returns the first group whose copies would all be removed, or null.
        public static PageGroup? FindGroupWithoutKeptCopy(IEnumerable<PageGroup> groups, IReadOnlySet<string> selectedPageIds)
        {
            return groups.FirstOrDefault(group =>
                group.Pages.Count > 0 && group.Pages.All(page => selectedPageIds.Contains(page.PageId)));
        }

        // The copy in the highest-ranked location. Ties and unranked locations keep the earliest copy.
        private static PageRef? PreferredCopy(PageGroup group, Dictionary<string, int> ranks)
        {
            PageRef? best = null;
            int bestRank = int.MaxValue;
            foreach (PageRef page in group.Pages)
            {
                int rank = ranks.TryGetValue(page.Location, out int value) ? value : int.MaxValue;
                if (best == null || rank < bestRank)
                {
                    best = page;
                    bestRank = rank;
                }
            }
            return best;
        }

        private static Dictionary<string, int> RankLocations(IReadOnlyList<string> locationOrder)
        {
            Dictionary<string, int> ranks = new Dictionary<string, int>();
            for (int i = 0; i < locationOrder.Count; ++i)
            {
                ranks.TryAdd(locationOrder[i], i);
            }
            return ranks;
        }

        private static int CompareLocations(string left, string right)
        {
            int byCloud = IsCloud(right).CompareTo(IsCloud(left));
            if (byCloud != 0) return byCloud;
            int byRecycleBin = IsInRecycleBin(left).CompareTo(IsInRecycleBin(right));
            if (byRecycleBin != 0) return byRecycleBin;
            return string.Compare(left, right, StringComparison.CurrentCulture);
        }

        private static bool IsCloud(string location)
        {
            return location.StartsWith("https:", StringComparison.Ordinal);
        }

        // NOTE: Also matches a section folder literally named "OneNote_RecycleBin" (very unlikely).
        private static bool IsInRecycleBin(string location)
        {
            return location.Contains("\\OneNote_RecycleBin", StringComparison.Ordinal);
        }
    }
}
