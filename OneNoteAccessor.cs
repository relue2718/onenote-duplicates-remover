using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace OneNoteDuplicatesRemover
{
    public class OneNoteAccessor
    {
        // Member Variables
        private OneNoteApplicationWrapper onenoteApplication = null;
        private Dictionary<string, OneNotePageInfo> pageInfos = null;
        private string lastSelectedPageId = null;

        public delegate void CancelledEventHandler();
        public event CancelledEventHandler OnCancelled = null;

        public Tuple<bool, string> InitializeOneNoteWrapper()
        {
            onenoteApplication = new OneNoteApplicationWrapper();
            if (onenoteApplication.InitializeOneNoteTypeLibrary())
            {
                return Tuple.Create(true, "");
            }
            else
            {
                return Tuple.Create(false, "Unable to initialize OneNote type library.");
            }
        }

        public Type GetApplicationType()
        {
            return onenoteApplication.GetApplicationType();
        }

        private Tuple<bool, string> UpdatePageInfos()
        {
            pageInfos = new Dictionary<string, OneNotePageInfo>();

            string rawXmlString = "";
            if (onenoteApplication.TryGetPageHierarchyAsXML(out rawXmlString) == false)
            {
                return Tuple.Create(false, "Unable to retrieve page hierarchy.");
            }
            etc.LoggerHelper.LogInfo("Retrieved page hierarchy: {0} bytes.", rawXmlString.Length);

            System.Xml.XmlDocument hierarchyXml = new System.Xml.XmlDocument();
            try
            {
                hierarchyXml.LoadXml(rawXmlString);
            }
            catch (System.Exception exception)
            {
                etc.LoggerHelper.LogUnexpectedException(exception);
                return Tuple.Create(false, "Unable to parse page hierarchy.");
            }

            System.Xml.XmlNodeList pageNodeList = hierarchyXml.GetElementsByTagName("one:Page");
            foreach (System.Xml.XmlNode pageNode in pageNodeList)
            {
                try
                {
                    string pageUniqueId = pageNode.Attributes["ID"].Value;
                    string parentNodeName = pageNode.ParentNode.Name;

                    if (parentNodeName == "one:Section")
                    {
                        // We must check whether the pages are deleted. Otherwise, we may end up deleting the duplicates in the trash folder.
                        if (IsPageDeleted(pageNode) == false)
                        {
                            if (pageInfos.ContainsKey(pageUniqueId) == false)
                            {
                                // 'ID', 'path' and 'name' attributes always exist.
                                string sectionId = pageNode.ParentNode.Attributes["ID"].Value;
                                string sectionPath = pageNode.ParentNode.Attributes["path"].Value;
                                string sectionName = pageNode.ParentNode.Attributes["name"].Value;

                                OneNotePageInfo newPageInfo = new OneNotePageInfo();
                                newPageInfo.ParentSectionId = sectionId;
                                newPageInfo.ParentSectionFilePath = sectionPath;
                                newPageInfo.ParentSectionName = sectionName;
                                newPageInfo.PageTitle = pageNode.Attributes["name"].Value;
                                pageInfos.Add(pageUniqueId, newPageInfo);
                            }
                            else
                            {
                                return Tuple.Create(false, string.Format("The page id ({0}) is not unique.", pageUniqueId));
                            }
                        }
                    }
                }
                catch (System.Exception exception)
                {
                    etc.LoggerHelper.LogUnexpectedException(exception);
                    return Tuple.Create(false, "The page hierarchy is corrupted.");
                }
            }
            return Tuple.Create(true, "");
        }

        public Tuple<bool, string> ScanOneNotePages(IProgress<Tuple<int, int, int, string>> progress, System.Threading.CancellationToken cancellationToken)
        {
            Tuple<bool, string> resultUpdatePageInfos = UpdatePageInfos();
            if (resultUpdatePageInfos.Item1 == false)
            {
                return resultUpdatePageInfos;
            }
            else
            {
                etc.LoggerHelper.LogInfo("Found {0} pages.", pageInfos.Count);
                int statCountReadSuccess = 0;
                int statCountReadFailed = 0;
                int statCountTotal = pageInfos.Count;
                string statPageTitle = null;
                progress.Report(Tuple.Create(statCountReadSuccess, statCountReadFailed, statCountTotal, statPageTitle));
                foreach (KeyValuePair<string, OneNotePageInfo> elem in pageInfos)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        OnCancelled.Invoke();
                        break;
                    }
                    string pageId = elem.Key;
                    OneNotePageInfo pageInfo = elem.Value;
                    string pageContent = "";
                    bool successHash = false;
                    pageInfo.HashValueForInnerText = null;
                    if (onenoteApplication.TryGetPageContent(elem.Key, out pageContent))
                    {
                        try
                        {
                            System.Xml.XmlDocument pageContentXml = new System.Xml.XmlDocument();
                            pageContentXml.LoadXml(pageContent);
                            /*
                            * Though the page contents are identical, it is quite common to see different 'objectID' and 'lastModified' attributes.
                            * This difference results in a completely different hash value, which cannot be detected by other duplicate remove software.
                            * By simply taking 'innerText' of the internal XML-like format, those attributes will be ignored.
                            * [!] The underlying assumption is that a user cannot modify the attributes directly.
                            */
                            if (TryCalculateHashValue(pageContentXml.InnerText, out string hashValue))
                            {
                                successHash = true;
                                pageInfo.HashValueForInnerText = hashValue;
                            }
                            else
                            {
                                etc.LoggerHelper.LogWarn("Failed to calculate hash for the page ({0}).", pageId);
                            }
                        }
                        catch (System.Exception exception)
                        {
                            etc.LoggerHelper.LogUnexpectedException(exception);
                        }
                    }

                    statPageTitle = pageInfo.PageTitle;
                    if (successHash)
                    {
                        statCountReadSuccess += 1;
                    }
                    else
                    {
                        statCountReadFailed += 1;
                        etc.LoggerHelper.LogWarn("Failed to retrieve the content of the page ({0}).", pageId);
                    }
                    progress.Report(Tuple.Create(statCountReadSuccess, statCountReadFailed, statCountTotal, statPageTitle));
                }
            }
            return Tuple.Create(true, "");
        }

        public List<Tuple<string, string, bool>> RemovePages(List<Tuple<string, string>> pagesBeingRemoved, IProgress<Tuple<int, int, int, string>> progress, System.Threading.CancellationToken cancellationToken)
        {
            int countRemoved = 0;
            int countFailedToRemove = 0;
            int countTotal = pagesBeingRemoved.Count;
            List<Tuple<string, string, bool>> ret = new List<Tuple<string, string, bool>>();
            foreach (Tuple<string, string> elem in pagesBeingRemoved)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    OnCancelled.Invoke();
                    break;
                }
                if (onenoteApplication.TryDeleteHierarchy(elem.Item1))
                {
                    countRemoved += 1;
                    ret.Add(Tuple.Create(elem.Item1, elem.Item2, true));
                }
                else
                {
                    countFailedToRemove += 1;
                    ret.Add(Tuple.Create(elem.Item1, elem.Item2, false));
                }
                progress.Report(Tuple.Create(countRemoved, countFailedToRemove, countTotal, elem.Item2));
            }
            return ret;
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
                    // NOTE: Might be unsafe if the section name contains "OneNote_RecycleBin" (very unlikely)
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

        public List<string> GetSectionPathList(Dictionary<string, List<Tuple<string, string>>> duplicatesGroups)
        {
            List<string> sectionPathList = new List<string>();
            foreach (KeyValuePair<string, List<Tuple<string, string>>> groupInfo in duplicatesGroups)
            {
                if (groupInfo.Value.Count > 1)
                {
                    for (int i = 0; i < groupInfo.Value.Count; ++i)
                    {
                        string pageId = groupInfo.Value[i].Item1;
                        string sectionPath = "";
                        if (TryGetSectionPath(pageId, out sectionPath))
                        {
                            sectionPathList.Add(System.IO.Path.GetDirectoryName(sectionPath));
                        }
                    }
                }
            }
            return SortSectionPathList(sectionPathList);
        }

        public bool TryNavigate(string pageId)
        {
            /*
                http://msdn.microsoft.com/en-us/library/gg649853(v=office.14).aspx
                bstrPageID: The OneNote ID of the page that contains the object to delete.
                bstrObjectID: The OneNote ID of the object that you want to delete. 
             */
            if (CheckIfPageExists(pageId))
            {
                if (lastSelectedPageId == null || lastSelectedPageId != pageId)
                {
                    lastSelectedPageId = pageId;
                    if (onenoteApplication.TryNavigateTo(pageId))
                    {
                        return true;
                    }
                    else
                    {
                        etc.LoggerHelper.LogWarn("Navigate failed. pageId:{0}", pageId); // not critical
                    }
                }
            }
            return false;
        }

        public bool TryFlattenSections(string targetSectionName, IProgress<Tuple<int, int, int, string>> progress, System.Threading.CancellationToken cancellationToken)
        {
            onenoteApplication.TryGetSectionHierarchyAsXML(out string rawXmlString);
            System.Xml.XmlDocument sectionHierarchyXml = new System.Xml.XmlDocument();
            try
            {
                sectionHierarchyXml.LoadXml(rawXmlString);
                string destinationSectionId = null;
                System.Xml.XmlNodeList sectionNodeList = sectionHierarchyXml.GetElementsByTagName("one:Section");
                int countTotalSections = sectionNodeList.Count;
                int countFlattenedSections = 0;
                int countNotFlattenedSections = 0;
                foreach (System.Xml.XmlNode sectionNode in sectionNodeList)
                {
                    if (sectionNode.Attributes["name"].Value == targetSectionName)
                    {
                        destinationSectionId = sectionNode.Attributes["ID"].Value;
                        break;
                    }
                }
                if (destinationSectionId != null)
                {
                    foreach (System.Xml.XmlNode sectionNode in sectionNodeList)
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            OnCancelled.Invoke();
                            break;
                        }
                        string sourceSectionName = sectionNode.Attributes["name"].Value;
                        if (sourceSectionName != targetSectionName)
                        {
                            string sourceSectionId = sectionNode.Attributes["ID"].Value;

                            if ((sectionNode.Attributes["isInRecycleBin"] == null) || (sectionNode.Attributes["isInRecycleBin"].Value != "true") &&
                               (sectionNode.Attributes["isDeletedPages"] == null) || (sectionNode.Attributes["isDeletedPages"].Value != "true"))
                            {
                                if (onenoteApplication.TryMergeSection(sourceSectionId, destinationSectionId))
                                {
                                    countFlattenedSections += 1;
                                }
                                else
                                {
                                    countNotFlattenedSections += 1;
                                }
                            }
                        }
                        progress.Report(Tuple.Create(countFlattenedSections, countNotFlattenedSections, countTotalSections, sourceSectionName));
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (System.Exception exception)
            {
                etc.LoggerHelper.LogUnexpectedException(exception);
                return false;
            }
        }

        public bool CheckIfPageExists(string pageId)
        {
            if (pageInfos != null)
            {
                return pageInfos.ContainsKey(pageId);
            }
            else
            {
                etc.LoggerHelper.LogError("Pages are not loaded");
                return false;
            }
        }

        public bool TryGetSectionPath(string pageId, out string sectionPath)
        {
            sectionPath = "";

            if (pageInfos != null)
            {
                if (pageInfos.ContainsKey(pageId))
                {
                    sectionPath = pageInfos[pageId].ParentSectionFilePath;
                    return true;
                }
                else
                {
                    etc.LoggerHelper.LogError("Page ({0}) not found!", pageId);
                    return false;
                }
            }
            else
            {
                etc.LoggerHelper.LogError("Pages are not loaded");
                return false;
            }
        }

        public Dictionary<string, List<Tuple<string /* pageId */, string /* pageName */ >>> GetDuplicatesGroups()
        {
            if (pageInfos == null) { return null; }
            else
            {
                Dictionary<string, List<Tuple<string, string>>> duplicatesGroups = new Dictionary<string, List<Tuple<string, string>>>();
                foreach (KeyValuePair<string, OneNotePageInfo> elem in pageInfos)
                {
                    string pageId = elem.Key;
                    OneNotePageInfo pageInfo = elem.Value;

                    string hashValueForInnerText = pageInfo.HashValueForInnerText;
                    if (hashValueForInnerText != null)
                    {
                        if (duplicatesGroups.ContainsKey(hashValueForInnerText) == false)
                        {
                            duplicatesGroups.Add(hashValueForInnerText, new List<Tuple<string, string>>());
                        }
                        duplicatesGroups[hashValueForInnerText].Add(Tuple.Create(pageId, pageInfo.PageTitle));
                    }
                }
                return duplicatesGroups;
            }
        }

        private static bool IsPageDeleted(System.Xml.XmlNode pageNode)
        {
            // The 'isDeletedPages' attribute is optional. If the attribute doesn't exist, we assume that the page isn't deleted.
            System.Xml.XmlAttribute isDeletedPageAttr = pageNode.ParentNode.Attributes["isDeletedPages"];
            if (isDeletedPageAttr != null)
            {
                return bool.Parse(isDeletedPageAttr.Value);
            }
            else
            {
                return false;
            }
        }

        private static bool TryCalculateHashValue(string rawString, out string hashValue)
        {
            hashValue = "";
            try
            {
                byte[] content = Encoding.UTF8.GetBytes(rawString);
                byte[] computedHashValue = System.Security.Cryptography.SHA256.Create().ComputeHash(content);
                hashValue = Utils.ConvertToHexString(computedHashValue);
                return true;
            }
            catch (System.Exception exception)
            {
                etc.LoggerHelper.LogUnexpectedException(exception);
                return false;
            }
        }
    }
}
