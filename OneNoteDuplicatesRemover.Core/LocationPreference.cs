using System.Collections.Generic;

namespace OneNoteDuplicatesRemover.Core
{
    // The user's keep order of locations, most preferred first.
    // Each move returns the item's new index, or the given index when it cannot move.
    public sealed class LocationPreference
    {
        private readonly List<string> locations;

        public LocationPreference(IEnumerable<string> locations)
        {
            this.locations = new List<string>(locations);
        }

        public IReadOnlyList<string> Locations => locations;

        public int MoveUp(int index)
        {
            return IsValid(index) && index > 0 ? Move(index, index - 1) : index;
        }

        public int MoveDown(int index)
        {
            return IsValid(index) && index < locations.Count - 1 ? Move(index, index + 1) : index;
        }

        public int MoveToTop(int index)
        {
            return IsValid(index) ? Move(index, 0) : index;
        }

        public int MoveToBottom(int index)
        {
            return IsValid(index) ? Move(index, locations.Count - 1) : index;
        }

        private bool IsValid(int index)
        {
            return index >= 0 && index < locations.Count;
        }

        private int Move(int from, int to)
        {
            string location = locations[from];
            locations.RemoveAt(from);
            locations.Insert(to, location);
            return to;
        }
    }
}
