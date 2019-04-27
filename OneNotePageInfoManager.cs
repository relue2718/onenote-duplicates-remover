using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;

namespace OneNoteDuplicatesRemover
{
    class OneNotePageInfoManager
    {
        private class ComputeHashValuesStatistics
        {
            public int countReadPages_Success = 0;
            public int countReadPages_Failed = 0;
            public int countHashedPages_Success = 0;
            public int countHashedPages_Failed = 0;
            public int countTotalPages = 0;
            public string lastPageTitle = "";
            public bool workerThreadFinished = false;
        }

        private class ComputeHashValueTaskDetails
        {
            public String TargetPageId = null;
            public OneNotePageInfo TargetPageInfo = null;
        }

        private Dictionary<string, OneNotePageInfo> pageInfos = null;
        private OneNoteAccessor accessor = null;
        private System.Threading.Thread workerThread = null;
        private bool isScanCompleted = false;

        public OneNotePageInfoManager(OneNoteAccessor accessor, string rawXmlString)
        {
            this.accessor = accessor;
            pageInfos = new Dictionary<string, OneNotePageInfo>();
            System.Xml.XmlDocument hierarchyXml = new System.Xml.XmlDocument();
            try
            {
                hierarchyXml.LoadXml(rawXmlString);
                System.Xml.XmlNodeList pageNodeList = hierarchyXml.GetElementsByTagName("one:Page");
                foreach (System.Xml.XmlNode pageNode in pageNodeList)
                {
                    try
                    {
                        string pageUniqueId = pageNode.Attributes["ID"].Value;
                        string parentNodeName = pageNode.ParentNode.Name;

                        if (parentNodeName == "one:Section")
                        {
                            // We must check whether the pages are deleted. Otherwise, we may end up deleting the duplicated pages in the trash folder.
                            bool isDeletedPage = CheckIfDeletedPage(pageNode);
                            if (isDeletedPage == false)
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
                            }
                        }
                    }
                    catch (System.Exception exception)
                    {
                        etc.LoggerHelper.LogWarn("Ignoring the exception: {0}...", exception.ToString());
                    }
                }
                etc.LoggerHelper.LogInfo("Found {0} pages.", pageInfos.Count);
            }
            catch (System.Exception exception)
            {
                etc.LoggerHelper.LogUnexpectedException(exception);
            }
        }

        public void AsyncComputeHashValues()
        {
            if (pageInfos != null && workerThread == null && isScanCompleted == false)
            {
                // Assumption: the dictionary page_infos is not modified.
                // 1. WorkerThread: Obtain a page content (single-threaded)
                // 2. ThreadPool: Computing SHA256sum (and do not keep the page content)

                workerThread = new System.Threading.Thread((object state) =>
                {
                    Dictionary<string, OneNotePageInfo> currentPageInfos = state as Dictionary<string, OneNotePageInfo>;
                    ComputeHashValuesStatistics stat = new ComputeHashValuesStatistics();
                    stat.countTotalPages = currentPageInfos.Count;
                    foreach (KeyValuePair<string, OneNotePageInfo> pageInfo in currentPageInfos)
                    {
                        string pageId = pageInfo.Key;
                        if (accessor.GetOneNoteApplication().TryGetPageContent(pageId, out pageInfo.Value.PageContent))
                        {
                            lock (stat)
                            {
                                stat.countReadPages_Success += 1;
                                accessor.FireEventUpdateProgress(stat.countReadPages_Success, stat.countReadPages_Failed, stat.countHashedPages_Success, stat.countHashedPages_Failed, stat.countTotalPages, stat.lastPageTitle);
                            }

                            ComputeHashValueTaskDetails computeHashValueTask = new ComputeHashValueTaskDetails();
                            computeHashValueTask.TargetPageId = pageId;
                            computeHashValueTask.TargetPageInfo = pageInfo.Value;

                            System.Threading.ThreadPool.QueueUserWorkItem((object state2) =>
                            {
                                ComputeHashValueTaskDetails currentComputeHashValueTask = (ComputeHashValueTaskDetails)state2;
                                OneNotePageInfo targetPageInfo = currentComputeHashValueTask.TargetPageInfo;
                                try
                                {
                                    targetPageInfo.PageContentXml = new System.Xml.XmlDocument();
                                    targetPageInfo.PageContentXml.LoadXml(targetPageInfo.PageContent);
                                    string hashValue = "";
                                    /*
                                     * Though the page contents are identical, it is quite common to see different 'objectID' and 'lastModified' attributes.
                                     * This difference results in a completely different hash value, which cannot be detected by other duplicate remove software.
                                     * By simply taking 'innerText' of the internal XML-like format, those attributes will be ignored.
                                     * [!] The underlying assumption is that a user cannot modify the attributes directly.
                                     */
                                    if (TryCalculateHashValue(targetPageInfo.PageContentXml.InnerText, out hashValue))
                                    {
                                        targetPageInfo.PageContent = null;
                                        targetPageInfo.PageContentXml = null;
                                        targetPageInfo.HashValueForInnerText = hashValue;
                                        targetPageInfo.IsContentRetrieved = true;
                                    }
                                }
                                catch (System.Exception)
                                {
                                    // Ignore an exception
                                }
                                finally
                                {
                                    lock (stat)
                                    {
                                        if (targetPageInfo.IsContentRetrieved)
                                        {
                                            stat.countHashedPages_Success += 1;
                                        }
                                        else
                                        {
                                            stat.countHashedPages_Failed += 1;
                                        }
                                        stat.lastPageTitle = targetPageInfo.PageTitle;
                                        accessor.FireEventUpdateProgress(stat.countReadPages_Success, stat.countReadPages_Failed, stat.countHashedPages_Success, stat.countHashedPages_Failed, stat.countTotalPages, stat.lastPageTitle);
                                        if (stat.workerThreadFinished && stat.countHashedPages_Failed + stat.countHashedPages_Success == stat.countReadPages_Success)
                                        {
                                            this.isScanCompleted = true;
                                            accessor.FireEventScanComplete();
                                        }
                                    }
                                }
                            }, (object)computeHashValueTask);
                        }
                        else
                        {
                            lock (stat)
                            {
                                stat.countReadPages_Failed += 1;
                                accessor.FireEventUpdateProgress(stat.countReadPages_Success, stat.countReadPages_Failed, stat.countHashedPages_Success, stat.countHashedPages_Failed, stat.countTotalPages, stat.lastPageTitle);
                            }
                        }
                    }

                    lock (stat)
                    {
                        stat.workerThreadFinished = true;
                    }
                });
                workerThread.Start(pageInfos);
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

            if (pageInfos == null)
            {
                return false;
            }
            else
            {
                if (pageInfos.ContainsKey(pageId))
                {
                    sectionPath = pageInfos[pageId].ParentSectionFilePath;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public Dictionary<string, List<Tuple<string /* pageId */, string /* pageName */ >>> GetDuplicatesGroups()
        {
            Dictionary<string /* innerTextHash */, List<Tuple<string, string>> /* Page Id List */ > duplicatesGroups = new Dictionary<string, List<Tuple<string, string>>>();
            if (isScanCompleted)
            {
                foreach (KeyValuePair<string, OneNotePageInfo> pageInfo in pageInfos)
                {
                    if (pageInfo.Value.IsContentRetrieved)
                    {
                        string pageId = pageInfo.Key;
                        string hashValueForInnerText = pageInfo.Value.HashValueForInnerText;

                        if (duplicatesGroups.ContainsKey(hashValueForInnerText) == false)
                        {
                            duplicatesGroups.Add(hashValueForInnerText, new List<Tuple<string, string>>());
                        }

                        duplicatesGroups[hashValueForInnerText].Add(new Tuple<string, string>(pageId, pageInfo.Value.PageTitle));
                    }
                }
            }
            else
            {
                etc.LoggerHelper.LogError("Scan is not finished.");
            }
            return duplicatesGroups;
        }
        
        private static bool CheckIfDeletedPage(System.Xml.XmlNode pageNode)
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
