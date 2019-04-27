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

        public delegate void ProgressEventHandler(int current, int max, string page_name);
        public event ProgressEventHandler OnUpdatedScanProgress = null;

        public void InitializeOneNoteWrapper()
        {
            onenote_application = new OneNoteApplicationWrapper();
            onenote_application.InitializeOneNoteTypeLibrary();
            page_info_manager = new OneNotePageInfoManager();
        }

        public bool TryUpdatePageHierarchy()
        {
            string raw_xml_string = "";
            if (onenote_application.TryGetPageHierarchyAsXML(out raw_xml_string))
            {
                return page_info_manager.TryLoadFromXmlString(raw_xml_string);
            }
            else
            {
                return false;
            }
        }

        private void FireEventUpdatedScanProgress(int current, int max, string page_name)
        {
            OnUpdatedScanProgress?.Invoke(current, max, page_name);
        }

        private bool TryGetHashOfOneNotePage(string pageId, out string hash_value)
        {
            hash_value = "";

            // The OneNote page consists of XML-like markups. 
            // Though the innerText is identical, it is common to have different 'objectID' and  'lastModifiedTime' attributes. 
            // These differences would cause a complete different hash value even if the contents are the same.
            // Therefore, I will ignore those attributes by extracting 'innerText' and calculate a hash value without those attributes.

            string page_content = "";
            bool success = onenote_application.TryGetPageContent(pageId, out page_content);

            if (success)
            {
                System.Xml.XmlDocument page_content_xml = new System.Xml.XmlDocument();
                try
                {
                    page_content_xml.LoadXml(page_content);
                }
                catch (System.Exception exception)
                {
                    etc.LoggerHelper.LogUnexpectedException(exception);
                    return false;
                }

                if (TryCalculateHashValue(page_content_xml.InnerText, out hash_value))
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

        private static bool TryCalculateHashValue(string plain_text, out string hash_value)
        {
            hash_value = "";
            try
            {
                byte[] raw_inner_text = Encoding.UTF8.GetBytes(plain_text);
                byte[] computed_hash_value = System.Security.Cryptography.SHA256.Create().ComputeHash(raw_inner_text);
                hash_value = Utils.ConvertToHexString(computed_hash_value);
                return true;
            }
            catch (System.Exception exception)
            {
                etc.LoggerHelper.LogUnexpectedException(exception);
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
    }
}
