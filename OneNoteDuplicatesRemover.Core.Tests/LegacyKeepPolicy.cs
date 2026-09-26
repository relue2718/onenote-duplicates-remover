#nullable disable
using System.Collections.Generic;
using System.Linq;

namespace OneNoteDuplicatesRemover.Core.Tests
{
    // The keep-copy logic as it was before the Core refactor (68db101): FormMain.buttonSelectAllExceptOne_Click
    // and OneNoteAccessor.GetSectionPathList/SortSectionPathList. TreeView and ListBox access is replaced by
    // lists; everything else is unchanged so it can serve as a reference for KeepPolicy.
    internal static class LegacyKeepPolicy
    {
        public static List<string> GetSectionPathList(IEnumerable<PageGroup> groups)
        {
            List<string> sectionPathList = new List<string>();
            foreach (PageGroup group in groups)
            {
                if (group.Pages.Count > 1)
                {
                    foreach (PageRef page in group.Pages)
                    {
                        sectionPathList.Add(System.IO.Path.GetDirectoryName(page.SectionPath));
                    }
                }
            }
            return SortSectionPathList(sectionPathList);
        }

        private static List<string> SortSectionPathList(List<string> sectionPathList)
        {
            sectionPathList.Sort((string left, string right) =>
            {
                bool isLeftCloud = (left.IndexOf("https:") == 0);
                bool isRightCloud = (right.IndexOf("https:") == 0);
                if (isLeftCloud && !isRightCloud)
                {
                    return -1;
                }
                else if (!isLeftCloud && isRightCloud)
                {
                    return 1;
                }
                else
                {
                    bool isLeftRecycleBin = left.Contains("\\OneNote_RecycleBin");
                    bool isRightRecycleBin = right.Contains("\\OneNote_RecycleBin");
                    if (isLeftRecycleBin && !isRightRecycleBin)
                    {
                        return 1;
                    }
                    else if (!isLeftRecycleBin && isRightRecycleBin)
                    {
                        return -1;
                    }
                    else
                    {
                        return left.CompareTo(right);
                    }
                }
            });
            sectionPathList = sectionPathList.Distinct().ToList();
            return sectionPathList;
        }

        // groupsInTree: the groups the tree showed, i.e. those with more than one page.
        public static HashSet<string> SelectAllExceptOne(IEnumerable<PageGroup> groupsInTree, List<string> preference)
        {
            HashSet<string> checkedPageIds = new HashSet<string>();
            foreach (PageGroup treeNode in groupsInTree)
            {
                int childCount = treeNode.Pages.Count;
                int whereMin = int.MaxValue;
                int wherePos = -1;
                for (int i = 0; i < childCount; ++i)
                {
                    string sectionPath = treeNode.Pages[i].SectionPath;
                    string sectionDir = System.IO.Path.GetDirectoryName(sectionPath);
                    int where = preference.IndexOf(sectionDir);
                    if (whereMin > where)
                    {
                        whereMin = where;
                        wherePos = i;
                    }
                }
                for (int i = 0; i < childCount; ++i)
                {
                    if (i != wherePos)
                    {
                        checkedPageIds.Add(treeNode.Pages[i].PageId);
                    }
                }
            }
            return checkedPageIds;
        }
    }
}
