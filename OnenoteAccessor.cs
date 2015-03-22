using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OneNoteDuplicatesRemover
{
  public class OneNoteAccessor
  {
    private BugReport.BugReportManager bugReportManager = null;
    private OneNoteApplicationWrapper instance = null;

    public OneNoteAccessor()
    {
      bugReportManager = new BugReport.BugReportManager();
      instance = new OneNoteApplicationWrapper(bugReportManager);
    }

    public delegate void ProgressEventHandler(int current, int max);
    public event ProgressEventHandler OnProgressEvent = null;

    private void FireProgressEvent(int current, int max)
    {
      if (OnProgressEvent != null)
      {
        OnProgressEvent(current, max);
      }
    }

    public Dictionary<string, OneNotePageInfo> GetFullHierarchy()
    {
      string strXml = "";
      bool success = instance.GetFullHierarchyAsXML(out strXml);

      if (success)
      {
        System.Xml.XmlDocument fullHierarchy = new System.Xml.XmlDocument();
        fullHierarchy.LoadXml(strXml);

        Dictionary<string, OneNotePageInfo> pageInfos = GetOneNotePageInfos(fullHierarchy);
        int totalCount = pageInfos.Count;
        int i = 0;
        foreach (KeyValuePair<string, OneNotePageInfo> pageInfo in pageInfos)
        {
          i++;
          string pageId = pageInfo.Key;
          string pageInnerTextHash = GetHashOfOneNotePage(pageId);
          pageInfo.Value.HashOfInnerText = pageInnerTextHash;
          FireProgressEvent(i, totalCount);
        }
        return pageInfos;
      }
      else
      {
        return null;
      }
    }

    private Dictionary<string, OneNotePageInfo> GetOneNotePageInfos(System.Xml.XmlDocument xmlDocument)
    {
      Dictionary<string, OneNotePageInfo> pageInfos = new Dictionary<string, OneNotePageInfo>();
      if (xmlDocument != null)
      {
        System.Xml.XmlNodeList pageNodeList = xmlDocument.GetElementsByTagName("one:Page");
        foreach (System.Xml.XmlNode pageNode in pageNodeList)
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
                try
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
                catch (System.Exception e)
                {
                  bugReportManager.ReportCaughtException(e);
                }
              }
            }
          }
        }
      }

      return pageInfos;
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

    private string GetHashOfOneNotePage(string pageId)
    {
      // The OneNote page consists of XML-like markups. 
      // Though the innerText is identical, it is common to have different 'objectID' and  'lastModifiedTime' attributes. 
      // These differences would cause a complete different hash value even if the contents are the same.
      // Therefore, I will ignore those attributes by extracting 'innerText' and calculate a hash value without those attributes.

      string pageContents = "";
      instance.GetPageContent(pageId, out pageContents);

      System.Xml.XmlDocument pageXmlContents = new System.Xml.XmlDocument();
      pageXmlContents.LoadXml(pageContents);

      byte[] rawInnerText = Encoding.UTF8.GetBytes(pageXmlContents.InnerText);
      byte[] computedHash = System.Security.Cryptography.MD5.Create().ComputeHash(rawInnerText);

      return Utils.MakeHashString(computedHash);
    }

    public void Navigate(string lastSelectedPageId)
    {
      /*
          http://msdn.microsoft.com/en-us/library/gg649853(v=office.14).aspx
          bstrPageID—The OneNote ID of the page that contains the object to delete.
          bstrObjectID—The OneNote ID of the object that you want to delete. 
       */
      instance.NavigateTo(lastSelectedPageId);
    }

    public bool RemovePage(string pageId)
    {
      try
      {
        instance.DeleteHierarchy(pageId);
        return true;
      }
      catch (System.Exception e)
      {
        bugReportManager.ReportCaughtException(e);
        return false;
      }
    }
  }
}
