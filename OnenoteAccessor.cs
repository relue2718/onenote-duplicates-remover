using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace OneNoteDuplicatesRemover
{
    public class OneNoteAccessor
    {
        private OneNoteApplicationWrapper onenoteApplication = null;
        private OneNotePageInfoManager onenotePageInfoManager = null;
        private string lastSelectedPageId = "";

        public delegate void ProgressEventHandler(int countReadPages_Success, int countReadPages_Failed, int countHashedPages_Success, int countHashedPages_Failed, int countTotalPages, string lastPageTitle);
        public event ProgressEventHandler EventProgressEvent = null;
        public delegate void ScanCompletedEventHandler();
        public event ScanCompletedEventHandler EventScanCompleted = null;

        public void InitializeOneNoteWrapper()
        {
            onenoteApplication = new OneNoteApplicationWrapper();
            onenoteApplication.InitializeOneNoteTypeLibrary();
            
        }

        public bool InvokeScanPages()
        {
            string rawXmlString = "";
            if (onenoteApplication.TryGetPageHierarchyAsXML(out rawXmlString))
            {
                onenotePageInfoManager = new OneNotePageInfoManager(this, rawXmlString);
                onenotePageInfoManager.AsyncComputeHashValues();
                return true;
            }
            else
            {
                return false;
            }
        }

        internal OneNoteApplicationWrapper GetOneNoteApplication()
        {
            return onenoteApplication;
        }

        internal void FireEventUpdateProgress(int countReadPages_Success, int countReadPages_Failed, int countHashedPages_Success, int countHashedPages_Failed, int countTotalPages, string lastPageTitle)
        {
            EventProgressEvent?.Invoke(countReadPages_Success, countReadPages_Failed, countHashedPages_Success, countHashedPages_Failed, countTotalPages, lastPageTitle);
        }

        internal void FireEventScanComplete()
        {
            EventScanCompleted?.Invoke();
        }

        public Dictionary<string, List<Tuple<string, string>>> GetDuplicatesGroups()
        {
            if (onenotePageInfoManager != null)
            {
                return onenotePageInfoManager.GetDuplicatesGroups();
            }
            return null;
            
        }
        
        public bool TryGetSectionPath(string pageId, out string sectionPath)
        {
            if (onenotePageInfoManager != null)
            {
                return onenotePageInfoManager.TryGetSectionPath(pageId, out sectionPath);
            }
            else
            {
                sectionPath = "";
                return false;
            }
            
        }

        public bool CheckIfPageExists(string pageId)
        {
            if (onenotePageInfoManager != null)
            {
                return onenotePageInfoManager.CheckIfPageExists(pageId);
            }
            else
            {
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
                etc.LoggerHelper.LogWarn("Navigate failed. pageId:{0}", pageId); // not critical
            }
        }

        public bool RemovePage(string pageId)
        {
            bool success = onenoteApplication.TryDeleteHierarchy(pageId);
            if (!success)
            {
                etc.LoggerHelper.LogError("Remove failed. pageId:{0}", pageId);
            }
            return success;
        }

        public string GetLastSelectedPageId()
        {
            return lastSelectedPageId;
        }

        public void SetLastSelectedPageId(string pageId)
        {
            lastSelectedPageId = pageId;
        }

        public bool TryFlattenSections(string targetSectionName)
        {
            string rawXmlString = "";
            onenoteApplication.TryGetSectionHierarchyAsXML(out rawXmlString);
            System.Xml.XmlDocument sectionHierarchyXml = new System.Xml.XmlDocument();
            try
            {
                sectionHierarchyXml.LoadXml(rawXmlString);
                string destinationSectionId = null;
                System.Xml.XmlNodeList sectionNodeList = sectionHierarchyXml.GetElementsByTagName("one:Section");
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
                        string sourceSectionName = sectionNode.Attributes["name"].Value;
                        if (sourceSectionName != targetSectionName)
                        {
                            string sourceSectionId = sectionNode.Attributes["ID"].Value;
                            onenoteApplication.TryMergeSection(sourceSectionId, destinationSectionId);
                        }
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
    }
}
