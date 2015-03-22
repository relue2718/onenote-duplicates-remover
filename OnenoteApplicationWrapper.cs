using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace OneNoteDuplicatesRemover
{
  class OneNoteApplicationWrapper
  {
    private Microsoft.Office.Interop.OneNote.Application application = null;
    private BugReport.BugReportManager bugReportManager = null;

    public OneNoteApplicationWrapper(BugReport.BugReportManager bugReportManager)
    {
      this.bugReportManager = bugReportManager;

      try
      {
        application = new Microsoft.Office.Interop.OneNote.Application();
      }
      catch (Exception e)
      {
        bugReportManager.ReportCaughtException(e);
      }
    }

    public bool GetFullHierarchyAsXML(out string strXml)
    {
      strXml = "";

      try
      {
        application.GetHierarchy(null, Microsoft.Office.Interop.OneNote.HierarchyScope.hsPages, out strXml);
      }
      catch (Exception e)
      {
        bugReportManager.ReportCaughtException(e);
        return false;
      }
      return true;
    }

    public bool GetPageContent(string pageId, out string pageContents)
    {
      pageContents = "";

      try
      {
        application.GetPageContent(pageId, out pageContents, Microsoft.Office.Interop.OneNote.PageInfo.piAll);
      }
      catch (Exception e)
      {
        bugReportManager.ReportCaughtException(e);
        return false;
      }
      return true;
    }

    public bool NavigateTo(string lastSelectedPageId)
    {
      try
      {
        application.NavigateTo(lastSelectedPageId /* bstrHierarchyObjectID */, "", false);
      }
      catch (Exception e)
      {
        bugReportManager.ReportCaughtException(e);
        return false;
      }
      return true;
    }

    public bool DeleteHierarchy(string pageId)
    {
      try
      {
        application.DeleteHierarchy(pageId);
      }
      catch (Exception e)
      {
        bugReportManager.ReportCaughtException(e);
        return false;
      }
      return true;
    }
  }
}
