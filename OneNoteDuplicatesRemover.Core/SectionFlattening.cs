using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace OneNoteDuplicatesRemover.Core
{
    public sealed record SectionMergeStep(string SectionId, string SectionName, bool ShouldMerge);

    public sealed record SectionMergePlan(string DestinationSectionId, int TotalSections, IReadOnlyList<SectionMergeStep> Steps);

    // "Flatten sections" merges every section into the first section with the target name.
    public static class SectionFlattening
    {
        // Plans the merge from a section hierarchy (GetHierarchy with hsSections). Sections named like the
        // target are left out; Recycle Bin and Deleted Pages sections are listed but not merged.
        // Returns null when no section has the target name.
        public static SectionMergePlan? Plan(string sectionHierarchyXml, string targetSectionName)
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(sectionHierarchyXml);
            List<XmlNode> sections = document.GetElementsByTagName("one:Section").Cast<XmlNode>().ToList();

            XmlNode? destination = sections.FirstOrDefault(section => Attribute(section, "name") == targetSectionName);
            if (destination == null)
            {
                return null;
            }

            List<SectionMergeStep> steps = sections
                .Where(section => Attribute(section, "name") != targetSectionName)
                .Select(section => new SectionMergeStep(
                    Attribute(section, "ID") ?? "",
                    Attribute(section, "name") ?? "",
                    Attribute(section, "isInRecycleBin") != "true" && Attribute(section, "isDeletedPages") != "true"))
                .ToList();
            return new SectionMergePlan(Attribute(destination, "ID") ?? "", sections.Count, steps);
        }

        private static string? Attribute(XmlNode node, string name)
        {
            return node.Attributes?[name]?.Value;
        }
    }
}
