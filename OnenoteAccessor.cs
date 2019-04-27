using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace OneNoteDuplicatesRemover
{
    public class OneNoteAccessor
    {
        private OneNoteApplicationWrapper onenote_application = null;
        private OneNotePageInfoManager page_info_manager;
        private string last_selected_page_id = "";

        public delegate void ProgressEventHandler(int count_read_pages_success, int count_read_pages_failed, int count_hashed_pages_success, int count_hashed_pages_failed, int total_pages, string page_title);
        public event ProgressEventHandler EventProgressEvent = null;
        public delegate void ScanCompleteEventHandler();
        public event ScanCompleteEventHandler EventScanCompleted = null;

        public void InitializeOneNoteWrapper()
        {
            onenote_application = new OneNoteApplicationWrapper();
            onenote_application.InitializeOneNoteTypeLibrary();
            page_info_manager = new OneNotePageInfoManager(this);
        }

        public bool InvokeScanPages()
        {
            string raw_xml_string = "";
            if (onenote_application.TryGetPageHierarchyAsXML(out raw_xml_string))
            {
                if (page_info_manager.TryLoadFromXmlString(raw_xml_string))
                {
                    page_info_manager.AsyncComputeHashValues();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        internal OneNoteApplicationWrapper GetOneNoteApplication()
        {
            return onenote_application;
        }

        internal Dictionary<string, List<Tuple<string, string>>> GetDuplicatesGroups()
        {
            return page_info_manager.GetDuplicatedGroups();
        }

        internal void FireEventUpdateProgress(int count_read_pages_success, int count_read_pages_failed, int count_hashed_pages_success, int count_hashed_pages_failed, int total_pages, string page_title)
        {
            EventProgressEvent?.Invoke(count_read_pages_success, count_read_pages_failed, count_hashed_pages_success, count_hashed_pages_failed, total_pages, page_title);
        }

        internal void FireEventScanComplete()
        {
            EventScanCompleted?.Invoke();
        }

        public void Navigate(string pageId)
        {
            /*
                http://msdn.microsoft.com/en-us/library/gg649853(v=office.14).aspx
                bstrPageID: The OneNote ID of the page that contains the object to delete.
                bstrObjectID: The OneNote ID of the object that you want to delete. 
             */
            bool success = onenote_application.TryNavigateTo(pageId);
            if (!success)
            {
                etc.LoggerHelper.LogWarn("Navigate failed. pageId:{0}", pageId);
            }
        }

        public bool RemovePage(string pageId)
        {
            bool success = onenote_application.TryDeleteHierarchy(pageId);
            if (!success)
            {
                etc.LoggerHelper.LogWarn("Remove failed. pageId:{0}", pageId);
            }
            return success;
        }

        public string GetLastSelectedPageId()
        {
            return last_selected_page_id;
        }

        public void SetLastSelectedPageId(string page_id)
        {
            last_selected_page_id = page_id;
        }

        public bool TryFlattenSections(string target_section_name)
        {
            string section_hierarchy_xml_string = "";
            onenote_application.TryGetSectionHierarchyAsXML(out section_hierarchy_xml_string);
            System.Xml.XmlDocument section_hierarchy = new System.Xml.XmlDocument();
            try
            {
                section_hierarchy.LoadXml(section_hierarchy_xml_string);
                string target_section_id = null;
                System.Xml.XmlNodeList section_nodes = section_hierarchy.GetElementsByTagName("one:Section");
                foreach (System.Xml.XmlNode section_node in section_nodes)
                {
                    if (section_node.Attributes["name"].Value == target_section_name)
                    {
                        target_section_id = section_node.Attributes["ID"].Value;
                        break;
                    }
                }
                if (target_section_id != null)
                {
                    foreach (System.Xml.XmlNode section_node in section_nodes)
                    {
                        string source_section_name = section_node.Attributes["name"].Value;
                        if (source_section_name != target_section_name)
                        {
                            string source_section_id = section_node.Attributes["ID"].Value;
                            onenote_application.TryMergeSection(source_section_id, target_section_id);
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

        internal bool TryGetSectionPath(string pageId, out string sectionPath)
        {
            return page_info_manager.TryGetSectionPath(pageId, out sectionPath);
        }

        internal bool CheckIfPageExists(string page_id)
        {
            return page_info_manager.CheckIfPageExists(page_id);
        }
    }
}
