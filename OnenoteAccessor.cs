using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace OneNoteDuplicatesRemover
{
    public class OneNoteAccessor
    {
        private OneNoteApplicationWrapper onenote_application = null;
        private Dictionary<string, OneNotePageInfo> page_infos = new Dictionary<string, OneNotePageInfo>();
        private string last_selected_page_id = "";

        public delegate void ProgressEventHandler(int current, int max, string page_name);
        public event ProgressEventHandler OnUpdatedScanProgress = null;

        public void InitializeOneNoteWrapper()
        {
            onenote_application = new OneNoteApplicationWrapper();
            onenote_application.InitializeOneNoteTypeLibrary();
        }

        public bool TryUpdatePageHierarchy()
        {
            page_infos = new Dictionary<string, OneNotePageInfo>();

            string hierarchy_xml_string = "";
            if (onenote_application.TryGetPageHierarchyAsXML(out hierarchy_xml_string))
            {
                System.Xml.XmlDocument hierarchy_xml = new System.Xml.XmlDocument();

                try
                {
                    hierarchy_xml.LoadXml(hierarchy_xml_string);
                }
                catch (System.Exception exception)
                {
                    etc.LoggerHelper.LogException(exception);
                    return false;
                }

                if (TryGetOneNotePageInfos(hierarchy_xml, out page_infos))
                {
                    int total_page_count = page_infos.Count;
                    int current_page_count = 0;
                    foreach (KeyValuePair<string, OneNotePageInfo> page_info in page_infos)
                    {
                        current_page_count++;
                        string page_id = page_info.Key;
                        string hash_value_for_inner_text = "";

                        if (TryGetHashOfOneNotePage(page_id, out hash_value_for_inner_text))
                        {
                            page_info.Value.HashValueForInnerText = hash_value_for_inner_text;
                            page_info.Value.IsContentRetrieved = true;

                            FireEventUpdatedScanProgress(current_page_count, total_page_count, page_info.Value.PageTitle);
                        }
                        else
                        {
                            etc.LoggerHelper.LogWarn("Unable to compute a hash value for the page: {0}.", page_id);
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

        private void FireEventUpdatedScanProgress(int current, int max, string page_name)
        {
            OnUpdatedScanProgress?.Invoke(current, max, page_name);
        }

        private bool TryGetOneNotePageInfos(System.Xml.XmlDocument hierarchy_xml, out Dictionary<string /*PageId*/, OneNotePageInfo> page_infos)
        {
            page_infos = new Dictionary<string, OneNotePageInfo>();

            if (hierarchy_xml == null)
            {
                etc.LoggerHelper.LogWarn("hierarchy_xml is null");
                return false;
            }
            else
            {
                System.Xml.XmlNodeList page_node_list = hierarchy_xml.GetElementsByTagName("one:Page");
                foreach (System.Xml.XmlNode page_node in page_node_list)
                {
                    try
                    {
                        string page_unique_id = page_node.Attributes["ID"].Value;
                        string parent_node_name = page_node.ParentNode.Name;

                        if (parent_node_name == "one:Section")
                        {
                            bool is_deleted_page = CheckIfDeleted(page_node);
                            // To avoid the situation that it is going to delete the pages that shouldn't be deleted and to keep the pages in the 'trash' folder.
                            if (is_deleted_page == false)
                            {
                                if (page_infos.ContainsKey(page_unique_id) == false)
                                {
                                    // 'ID', 'path' and 'name' attributes are always existing.
                                    string section_id = page_node.ParentNode.Attributes["ID"].Value;
                                    string section_path = page_node.ParentNode.Attributes["path"].Value;
                                    string section_name = page_node.ParentNode.Attributes["name"].Value;

                                    OneNotePageInfo new_page_info = new OneNotePageInfo();
                                    new_page_info.ParentSectionId = section_id;
                                    new_page_info.ParentSectionFilePath = section_path;
                                    new_page_info.ParentSectionName = section_name;
                                    new_page_info.PageTitle = page_node.Attributes["name"].Value;
                                    page_infos.Add(page_unique_id, new_page_info);
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
        }

        private static bool CheckIfDeleted(System.Xml.XmlNode pageNode)
        {
            System.Xml.XmlAttribute is_deleted_page_attr = pageNode.ParentNode.Attributes["isDeletedPages"];
            // The 'isDeletedPages' attribute is optional. If the attribute doesn't exist, we assume that the page isn't deleted.
            if (is_deleted_page_attr != null)
            {
                return bool.Parse(is_deleted_page_attr.Value);
            }
            else
            {
                return false;
            }
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
                    etc.LoggerHelper.LogException(exception);
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

        public bool HasPageId(string page_id)
        {
            if (page_infos != null)
            {
                return page_infos.ContainsKey(page_id);
            }
            else
            {
                etc.LoggerHelper.LogWarn("Unable to get the full hierarchy. pageId:{0}", page_id);
                return false;
            }
        }

        public Dictionary<string, List<Tuple<string /* pageId */, string /* pageName */ >>> GetDuplicatedGroups()
        {
            Dictionary<string /* innerTextHash */, List<Tuple<string, string>> /* Page Id List */ > duplicated_groups = new Dictionary<string, List<Tuple<string, string>>>();
            foreach (KeyValuePair<string, OneNotePageInfo> page_info in page_infos)
            {
                if (page_info.Value.IsContentRetrieved)
                {
                    string page_id = page_info.Key;
                    string hash_value_for_inner_text = page_info.Value.HashValueForInnerText;

                    if (duplicated_groups.ContainsKey(hash_value_for_inner_text) == false)
                    {
                        duplicated_groups.Add(hash_value_for_inner_text, new List<Tuple<string, string>>());
                    }

                    duplicated_groups[hash_value_for_inner_text].Add(new Tuple<string, string>(page_id, page_info.Value.PageTitle));
                }
            }
            return duplicated_groups;
        }

        public bool TryGetSectionPath(string pageId, out string section_path)
        {
            section_path = "";

            if (page_infos == null)
            {
                return false;
            }
            else
            {
                if (page_infos.ContainsKey(pageId))
                {
                    section_path = page_infos[pageId].ParentSectionFilePath;
                    return true;
                }
                else
                {
                    return false;
                }
            }
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
                etc.LoggerHelper.LogException(exception);
                return false;
            }
        }
    }
}
