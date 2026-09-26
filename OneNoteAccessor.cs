using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using OneNoteDuplicatesRemover.Core;

namespace OneNoteDuplicatesRemover
{
    public class OneNoteAccessor : IDisposable
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
            return onenoteApplication?.GetApplicationType();
        }

        public void Dispose()
        {
            // Keep the field set: a worker thread may still be running and will get 'false' from the disposed wrapper.
            onenoteApplication?.Dispose();
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

        public List<RemovalResult> RemovePages(IReadOnlyList<PageRef> pagesBeingRemoved, IProgress<Tuple<int, int, int, string>> progress, System.Threading.CancellationToken cancellationToken)
        {
            int countRemoved = 0;
            int countFailedToRemove = 0;
            int countTotal = pagesBeingRemoved.Count;
            List<RemovalResult> ret = new List<RemovalResult>();
            foreach (PageRef page in pagesBeingRemoved)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    OnCancelled.Invoke();
                    break;
                }
                if (onenoteApplication.TryDeleteHierarchy(page.PageId))
                {
                    countRemoved += 1;
                    ret.Add(new RemovalResult(page, true));
                }
                else
                {
                    countFailedToRemove += 1;
                    ret.Add(new RemovalResult(page, false));
                }
                progress.Report(Tuple.Create(countRemoved, countFailedToRemove, countTotal, page.Title));
            }
            return ret;
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

                            bool isInRecycleBin = sectionNode.Attributes["isInRecycleBin"]?.Value == "true";
                            bool isDeletedPages = sectionNode.Attributes["isDeletedPages"]?.Value == "true";
                            if (!isInRecycleBin && !isDeletedPages)
                            {
                                // TODO: Can a section node have the attribute 'isDeletedPages'?
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

        private bool isRecycled(System.Xml.XmlNode xmlNode)
        {
            if (xmlNode.Attributes["isInRecycleBin"] == null)
            {
                return false;
            }
            else
            {
                return bool.Parse(xmlNode.Attributes["isInRecycleBin"].Value);
            }
        }

        public bool TryExportSectionHierarchyAsXML(string path)
        {
            bool result = onenoteApplication.TryGetSectionHierarchyAsXML(out string rawXmlString);
            if (result)
            {
                System.Xml.XmlDocument xml = new System.Xml.XmlDocument();
                xml.LoadXml(rawXmlString);
                xml.Save(path);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool TryExportPageHierarchyAsXML(string path)
        {
            bool result = onenoteApplication.TryGetPageHierarchyAsXML(out string rawXmlString);
            if (result)
            {
                System.Xml.XmlDocument xml = new System.Xml.XmlDocument();
                xml.LoadXml(rawXmlString);
                xml.Save(path);
                return true;
            }
            else
            {
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

        // Pages grouped by content hash, in scan order. Includes groups with a single page. Null before the first scan.
        public List<PageGroup> GetPageGroups()
        {
            if (pageInfos == null) { return null; }
            Dictionary<string, List<PageRef>> pagesByHash = new Dictionary<string, List<PageRef>>();
            List<PageGroup> groups = new List<PageGroup>();
            foreach (KeyValuePair<string, OneNotePageInfo> elem in pageInfos)
            {
                string hashValueForInnerText = elem.Value.HashValueForInnerText;
                if (hashValueForInnerText != null)
                {
                    if (pagesByHash.TryGetValue(hashValueForInnerText, out List<PageRef> pages) == false)
                    {
                        pages = new List<PageRef>();
                        pagesByHash.Add(hashValueForInnerText, pages);
                        groups.Add(new PageGroup(hashValueForInnerText, pages));
                    }
                    pages.Add(new PageRef(elem.Key, elem.Value.PageTitle, elem.Value.ParentSectionFilePath));
                }
            }
            return groups;
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
