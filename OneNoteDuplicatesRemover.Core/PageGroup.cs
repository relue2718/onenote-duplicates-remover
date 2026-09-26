using System.Collections.Generic;

namespace OneNoteDuplicatesRemover.Core
{
    // Pages whose text content has the same hash.
    public sealed record PageGroup(string ContentHash, IReadOnlyList<PageRef> Pages)
    {
        // SHA-256 of an empty string, i.e. pages without any text.
        public const string EmptyContentHash = "E3B0C44298FC1C149AFBF4C8996FB92427AE41E4649B934CA495991B7852B855";

        public bool HasDuplicates => Pages.Count > 1;

        public bool IsEmptyContent => ContentHash == EmptyContentHash;
    }
}
