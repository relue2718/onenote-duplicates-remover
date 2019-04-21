using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OneNoteDuplicatesRemover
{
    public class OneNoteAccessor
    {
        private OneNoteApplicationWrapper onenoteApplication = null;
        private Dictionary<string, OneNotePageInfo> hierarchy = new Dictionary<string, OneNotePageInfo>();
        private string lastSelectedPageId = "";

        public delegate void ProgressEventHandler(int current, int max, string pageName);
        public event ProgressEventHandler OnUpdatedScanProgress = null;

        public void InitializeOneNoteWrapper()
        {
            onenoteApplication = new OneNoteApplicationWrapper();
            onenoteApplication.InitializeOneNoteTypeLibrary();
        }

        public bool UpdateHierarchy()
        {
            hierarchy = new Dictionary<string, OneNotePageInfo>();

            string hierarchyXmlString = "";
            bool success = onenoteApplication.TryGetHierarchyAsXML(out hierarchyXmlString);

            if (success)
            {
                System.Xml.XmlDocument hierarchyXml = new System.Xml.XmlDocument();

                try
                {
                    hierarchyXml.LoadXml(hierarchyXmlString);
                }
                catch (System.Exception exception)
                {
                    etc.LoggerHelper.LogException(exception);
                    return false;
                }

                bool successPageInfos = TryGetOneNotePageInfos(hierarchyXml, out hierarchy);
                if (successPageInfos)
                {
                    int totalCount = hierarchy.Count;
                    int i = 0;
                    foreach (KeyValuePair<string, OneNotePageInfo> pageInfo in hierarchy)
                    {
                        i++;
                        string pageId = pageInfo.Key;
                        string pageInnerTextHash = "";
                        bool successHash = TryGetHashOfOneNotePage(pageId, out pageInnerTextHash);
                        if (successHash)
                        {
                            pageInfo.Value.HashOfInnerText = pageInnerTextHash;
                            pageInfo.Value.IsContentRetrieved = true;

                            FireEventUpdatedScanProgress(i, totalCount, pageInfo.Value.PageName);
                        }
                        else
                        {
                            etc.LoggerHelper.LogWarn("Unable to get a hash, pageId:{0}", pageId);
                        }
                    }
                    return true;
                }
                else
                {
                    etc.LoggerHelper.LogWarn("Unable to parse the hierarchy");
                    return false;
                }
            }
            else
            {
                etc.LoggerHelper.LogWarn("Unable to get a hierarchy");
                return false;
            }
        }

        private void FireEventUpdatedScanProgress(int current, int max, string pageName)
        {
            if (OnUpdatedScanProgress != null)
            {
                OnUpdatedScanProgress(current, max, pageName);
            }
        }

        private bool TryGetOneNotePageInfos(System.Xml.XmlDocument xmlDocument, out Dictionary<string /*PageId*/, OneNotePageInfo> pageInfos)
        {
            pageInfos = new Dictionary<string, OneNotePageInfo>();

            if (xmlDocument != null)
            {
                System.Xml.XmlNodeList pageNodeList = xmlDocument.GetElementsByTagName("one:Page");
                foreach (System.Xml.XmlNode pageNode in pageNodeList)
                {
                    try
                    {
                        string pageUniqueId = pageNode.Attributes["ID"].Value;
                        string parentNodeName = pageNode.ParentNode.Name;

                        if (parentNodeName == "one:Section")
                        {
                            bool isDeletedPages = CheckIfDeleted(pageNode);
                            // To avoid the situation that it is going to delete the pages that shouldn't be deleted and to keep the pages in the 'trash' folder.
                            if (isDeletedPages == false)
                            {
                                if (pageInfos.ContainsKey(pageUniqueId) == false)
                                {
                                    // 'ID', 'path' and 'name' attributes are always existing.
                                    string sectionId = pageNode.ParentNode.Attributes["ID"].Value;
                                    string sectionPath = pageNode.ParentNode.Attributes["path"].Value;
                                    string sectionName = pageNode.ParentNode.Attributes["name"].Value;

                                    OneNotePageInfo pageInfo = new OneNotePageInfo();
                                    pageInfo.ParentSectionId = sectionId;
                                    pageInfo.ParentSectionFilePath = sectionPath;
                                    pageInfo.ParentSectionName = sectionName;
                                    pageInfo.PageName = pageNode.Attributes["name"].Value;
                                    pageInfos.Add(pageUniqueId, pageInfo);
                                }
                            }
                        }
                    }
                    catch (System.Exception exception)
                    {
                        etc.LoggerHelper.LogWarn("Ignore the exception: {0}", exception.ToString());
                    }
                }
                return true;
            }
            else
            {
                etc.LoggerHelper.LogWarn("xmlDocument is null");
                return false;
            }
        }

        private static bool CheckIfDeleted(System.Xml.XmlNode pageNode)
        {
            // The 'isDeletedPages' attribute is optional. If the attribute doesn't exist, we assume that the page isn't deleted.
            bool isDeletedPages = false;

            System.Xml.XmlAttribute IsDeletedPagesAttribute = pageNode.ParentNode.Attributes["isDeletedPages"];
            if (IsDeletedPagesAttribute != null)
            {
                isDeletedPages = bool.Parse(IsDeletedPagesAttribute.Value);
            }
            return isDeletedPages;
        }

        private bool TryGetHashOfOneNotePage(string pageId, out string hash)
        {
            hash = "";

            // The OneNote page consists of XML-like markups. 
            // Though the innerText is identical, it is common to have different 'objectID' and  'lastModifiedTime' attributes. 
            // These differences would cause a complete different hash value even if the contents are the same.
            // Therefore, I will ignore those attributes by extracting 'innerText' and calculate a hash value without those attributes.

            string pageContents = "";
            bool success = onenoteApplication.TryGetPageContent(pageId, out pageContents);
            if (success)
            {
                System.Xml.XmlDocument pageXmlContents = new System.Xml.XmlDocument();
                try
                {
                    pageXmlContents.LoadXml(pageContents);
                }
                catch (System.Exception exception)
                {
                    etc.LoggerHelper.LogException(exception);
                    return false;
                }

                bool successCalculateHash = TryCalculateHashOf(pageXmlContents.InnerText, out hash);
                if (successCalculateHash)
                {
                    return true;
                }
                else
                {
                    etc.LoggerHelper.LogWarn("Unable to calculate a hash, pageId:{0}", pageId);
                    return false;
                }
            }
            else
            {
                etc.LoggerHelper.LogWarn("Unable to get a page content, pageId:{0}", pageId);
                return false;
            }
        }

        private static bool TryCalculateHashOf(string plainText, out string hash)
        {
            hash = "";
            try
            {
                byte[] rawInnerText = Encoding.UTF8.GetBytes(plainText);
                byte[] computedHash = System.Security.Cryptography.SHA256.Create().ComputeHash(rawInnerText);
                hash = Utils.MakeHashString(computedHash);
                return true;
            }
            catch (System.Exception exception)
            {
                etc.LoggerHelper.LogException(exception);
                return false;
            }
        }

        public void Navigate(string pageId)
        {
            /*
                http://msdn.microsoft.com/en-us/library/gg649853(v=office.14).aspx
                bstrPageID: The OneNote ID of the page that contains the object to delete.
                bstrObjectID: The OneNote ID of the object that you want to delete. 
             */
            bool success = onenoteApplication.TryNavigateTo(pageId);
            if (!success)
            {
                etc.LoggerHelper.LogWarn("Navigate failed. pageId:{0}", pageId);
            }
        }

        public bool RemovePage(string pageId)
        {
            bool success = onenoteApplication.TryDeleteHierarchy(pageId);
            if (!success)
            {
                etc.LoggerHelper.LogWarn("Remove failed. pageId:{0}", pageId);
            }
            return success;
        }

        public string GetLastSelectedPageId()
        {
            return this.lastSelectedPageId;
        }

        public void SetLastSelectedPageId(string pageId)
        {
            this.lastSelectedPageId = pageId;
        }

        public bool HasPageId(string pageId)
        {
            if (hierarchy != null)
            {
                return hierarchy.ContainsKey(pageId);
            }
            else
            {
                etc.LoggerHelper.LogWarn("Unable to get the full hierarchy. pageId:{0}", pageId);
                return false;
            }
        }

        public Dictionary<string, List<Tuple<string /* pageId */, string /* pageName */ >>> GetDuplicatedGroups()
        {
            Dictionary<string /* innerTextHash */, List<Tuple<string, string>> /* Page Id List */ > duplicatedGroups = new Dictionary<string, List<Tuple<string, string>>>();
            foreach (KeyValuePair<string, OneNotePageInfo> pageInfo in hierarchy)
            {
                if (pageInfo.Value.IsContentRetrieved)
                {
                    string pageId = pageInfo.Key;
                    string pageInnerTextHash = pageInfo.Value.HashOfInnerText;

                    if (duplicatedGroups.ContainsKey(pageInnerTextHash) == false)
                    {
                        duplicatedGroups.Add(pageInnerTextHash, new List<Tuple<string, string>>());
                    }

                    duplicatedGroups[pageInnerTextHash].Add(new Tuple<string, string>(pageId, pageInfo.Value.PageName));
                }
            }
            return duplicatedGroups;
        }

        public bool TryGetSectionPath(string pageId, out string sectionPath)
        {
            sectionPath = "";

            if (hierarchy == null)
            {
                return false;
            }
            else
            {
                if (hierarchy.ContainsKey(pageId))
                {
                    sectionPath = hierarchy[pageId].ParentSectionFilePath;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
