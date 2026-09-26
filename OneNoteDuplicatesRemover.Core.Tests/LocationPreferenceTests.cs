namespace OneNoteDuplicatesRemover.Core.Tests
{
    public class LocationPreferenceTests
    {
        private static LocationPreference Preference()
        {
            return new LocationPreference(new[] { "A", "B", "C", "D" });
        }

        [Fact]
        public void MoveUp_SwapsWithThePreviousLocation()
        {
            LocationPreference preference = Preference();

            Assert.Equal(1, preference.MoveUp(2));
            Assert.Equal(new[] { "A", "C", "B", "D" }, preference.Locations);
        }

        [Fact]
        public void MoveDown_SwapsWithTheNextLocation()
        {
            LocationPreference preference = Preference();

            Assert.Equal(2, preference.MoveDown(1));
            Assert.Equal(new[] { "A", "C", "B", "D" }, preference.Locations);
        }

        [Fact]
        public void MoveToTop_AndMoveToBottom_KeepTheOrderOfTheOthers()
        {
            LocationPreference preference = Preference();

            Assert.Equal(0, preference.MoveToTop(2));
            Assert.Equal(new[] { "C", "A", "B", "D" }, preference.Locations);
            Assert.Equal(3, preference.MoveToBottom(1));
            Assert.Equal(new[] { "C", "B", "D", "A" }, preference.Locations);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(4)]
        public void Moves_IgnoreIndexesOutsideTheList(int index)
        {
            LocationPreference preference = Preference();

            Assert.Equal(index, preference.MoveUp(index));
            Assert.Equal(index, preference.MoveDown(index));
            Assert.Equal(index, preference.MoveToTop(index));
            Assert.Equal(index, preference.MoveToBottom(index));
            Assert.Equal(new[] { "A", "B", "C", "D" }, preference.Locations);
        }

        [Fact]
        public void Moves_PastTheEndsLeaveTheListUnchanged()
        {
            LocationPreference preference = Preference();

            Assert.Equal(0, preference.MoveUp(0));
            Assert.Equal(3, preference.MoveDown(3));
            Assert.Equal(new[] { "A", "B", "C", "D" }, preference.Locations);
        }
    }
}
