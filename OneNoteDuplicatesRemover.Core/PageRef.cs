using System.IO;

namespace OneNoteDuplicatesRemover.Core
{
    // A page found by a scan.
    public sealed record PageRef(string PageId, string Title, string SectionPath)
    {
        // The folder that holds the section file. Keep preferences are ranked by location.
        public string Location => Path.GetDirectoryName(SectionPath) ?? "";
    }
}
