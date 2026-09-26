using System.Linq;

namespace OneNoteDuplicatesRemover.Core.Tests
{
    public class SectionFlatteningTests
    {
        // Shaped like GetHierarchy(null, hsSections) output: a Recycle Bin section group holds deleted
        // sections (isInRecycleBin only) and Deleted Pages sections (isInRecycleBin and isDeletedPages).
        private const string Hierarchy = @"<?xml version=""1.0""?>
<one:Notebooks xmlns:one=""http://schemas.microsoft.com/office/onenote/2013/onenote"">
  <one:Notebook name=""Notes"" ID=""{NB1}"" path=""C:\Notes\"">
    <one:Section name=""Inbox"" ID=""{S1}"" path=""C:\Notes\Inbox.one"" isCurrentlyViewed=""true"" />
    <one:Section name=""MERGED_ONE"" ID=""{TARGET}"" path=""C:\Notes\MERGED_ONE.one"" />
    <one:SectionGroup name=""Work"" ID=""{SG1}"" path=""C:\Notes\Work\"">
      <one:Section name=""Projects"" ID=""{S2}"" path=""C:\Notes\Work\Projects.one"" />
    </one:SectionGroup>
    <one:SectionGroup name=""OneNote_RecycleBin"" ID=""{RB}"" path=""C:\Notes\OneNote_RecycleBin\"" isRecycleBin=""true"">
      <one:Section name=""Old section"" ID=""{S3}"" path=""C:\Notes\OneNote_RecycleBin\Old section.one"" isInRecycleBin=""true"" />
      <one:Section name=""Deleted Pages"" ID=""{S4}"" path=""C:\Notes\OneNote_RecycleBin\OneNote_DeletedPages.one"" isInRecycleBin=""true"" isDeletedPages=""true"" />
    </one:SectionGroup>
  </one:Notebook>
  <one:Notebook name=""Other"" ID=""{NB2}"" path=""C:\Other\"">
    <one:Section name=""MERGED_ONE"" ID=""{TARGET2}"" path=""C:\Other\MERGED_ONE.one"" />
    <one:Section name=""Ideas"" ID=""{S5}"" path=""C:\Other\Ideas.one"" />
  </one:Notebook>
</one:Notebooks>";

        [Fact]
        public void Plan_MergesIntoTheFirstSectionWithTheTargetName()
        {
            SectionMergePlan plan = SectionFlattening.Plan(Hierarchy, "MERGED_ONE")!;

            Assert.Equal("{TARGET}", plan.DestinationSectionId);
            Assert.Equal(7, plan.TotalSections);
        }

        [Fact]
        public void Plan_SkipsRecycleBinAndDeletedPagesSections()
        {
            SectionMergePlan plan = SectionFlattening.Plan(Hierarchy, "MERGED_ONE")!;

            Assert.Equal(new[] { "{S1}", "{S2}", "{S5}" }, plan.Steps.Where(step => step.ShouldMerge).Select(step => step.SectionId));
            Assert.Equal(new[] { "{S3}", "{S4}" }, plan.Steps.Where(step => !step.ShouldMerge).Select(step => step.SectionId));
        }

        [Fact]
        public void Plan_LeavesOutEverySectionNamedLikeTheTarget()
        {
            SectionMergePlan plan = SectionFlattening.Plan(Hierarchy, "MERGED_ONE")!;

            Assert.DoesNotContain(plan.Steps, step => step.SectionName == "MERGED_ONE");
            Assert.Equal(new[] { "Inbox", "Projects", "Old section", "Deleted Pages", "Ideas" }, plan.Steps.Select(step => step.SectionName));
        }

        [Fact]
        public void Plan_ReturnsNullWithoutATargetSection()
        {
            Assert.Null(SectionFlattening.Plan(Hierarchy, "Does not exist"));
        }
    }
}
