using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace OneNoteDuplicatesRemover.Core
{
    // JSON dump of scanned page groups: { "<content hash>": [ { "Item1": "<page id>", "Item2": "<title>" } ] }.
    // The shape comes from serializing Tuple<string, string> and is kept so older dump files still load.
    public static class PageGroupDump
    {
        public static string Serialize(IEnumerable<PageGroup> groups)
        {
            Dictionary<string, List<Tuple<string, string>>> dump = new Dictionary<string, List<Tuple<string, string>>>();
            foreach (PageGroup group in groups)
            {
                dump[group.ContentHash] = group.Pages.Select(page => Tuple.Create(page.PageId, page.Title)).ToList();
            }
            return JsonSerializer.Serialize(dump);
        }

        // Every page whose content hash appears in the dump, including all copies of a group.
        public static List<PageRef> SelectPagesWithDumpedContent(IEnumerable<PageGroup> groups, string dumpJson)
        {
            // Only the content hashes matter, so the page entries are not parsed.
            Dictionary<string, JsonElement> dump = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(dumpJson)
                ?? throw new JsonException("The dump file does not contain any page groups.");
            return groups
                .Where(group => dump.ContainsKey(group.ContentHash))
                .SelectMany(group => group.Pages)
                .ToList();
        }
    }
}
