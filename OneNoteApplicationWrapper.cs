using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace OneNoteDuplicatesRemover
{
    public class OneNoteApplicationWrapper : IDisposable
    {
        private Microsoft.Office.Interop.OneNote.Application application = null;
        // COM calls run on worker threads; serialize them so Dispose never releases the RCW mid-call.
        private readonly object syncRoot = new object();

        public bool InitializeOneNoteTypeLibrary()
        {
            try
            {
                application = new Microsoft.Office.Interop.OneNote.Application();
                return true;
            }
            catch (Exception exception)
            {
                etc.LoggerHelper.LogUnexpectedException(exception);
                return false;
            }
        }

        public Type GetApplicationType()
        {
            return application?.GetType();
        }

        public bool TryGetPageHierarchyAsXML(out string rawXmlString)
        {
            rawXmlString = "";
            try
            {
                lock (syncRoot)
                {
                    ObjectDisposedException.ThrowIf(application == null, this);
                    application.GetHierarchy(null, Microsoft.Office.Interop.OneNote.HierarchyScope.hsPages, out rawXmlString);
                }
                return true;
            }
            catch (Exception exception)
            {
                etc.LoggerHelper.LogUnexpectedException(exception);
                return false;
            }
        }

        public bool TryGetPageHierarchyAsXMLWithinID(string sectionId, out string rawXmlString)
        {
            rawXmlString = "";
            try
            {
                lock (syncRoot)
                {
                    ObjectDisposedException.ThrowIf(application == null, this);
                    application.GetHierarchy(sectionId, Microsoft.Office.Interop.OneNote.HierarchyScope.hsPages, out rawXmlString);
                }
                return true;
            }
            catch (Exception exception)
            {
                etc.LoggerHelper.LogUnexpectedException(exception);
                return false;
            }
        }

        public bool TryGetSectionHierarchyAsXML(out string rawXmlString)
        {
            rawXmlString = "";
            try
            {
                lock (syncRoot)
                {
                    ObjectDisposedException.ThrowIf(application == null, this);
                    application.GetHierarchy(null, Microsoft.Office.Interop.OneNote.HierarchyScope.hsSections, out rawXmlString);
                }
                return true;
            }
            catch (Exception exception)
            {
                etc.LoggerHelper.LogUnexpectedException(exception);
                return false;
            }
        }

        public bool TryGetPageContent(string pageId, out string pageContent)
        {
            pageContent = "";
            try
            {
                lock (syncRoot)
                {
                    ObjectDisposedException.ThrowIf(application == null, this);
                    application.GetPageContent(pageId, out pageContent, Microsoft.Office.Interop.OneNote.PageInfo.piAll);
                }
                return true;
            }
            catch (Exception exception)
            {
                etc.LoggerHelper.LogUnexpectedException(exception);
                return false;
            }
        }

        public bool TryNavigateTo(string pageId)
        {
            try
            {
                lock (syncRoot)
                {
                    ObjectDisposedException.ThrowIf(application == null, this);
                    application.NavigateTo(pageId /* bstrHierarchyObjectID */, "", false);
                }
                return true;
            }
            catch (Exception exception)
            {
                etc.LoggerHelper.LogUnexpectedException(exception);
                return false;
            }
        }

        public bool TryDeleteHierarchy(string pageId)
        {
            try
            {
                lock (syncRoot)
                {
                    ObjectDisposedException.ThrowIf(application == null, this);
                    application.DeleteHierarchy(pageId);
                }
                return true;
            }
            catch (Exception exception)
            {
                etc.LoggerHelper.LogUnexpectedException(exception);
                return false;
            }
        }

        public bool TryMergeSection(string sourceId, string destinationId)
        {
            try
            {
                lock (syncRoot)
                {
                    ObjectDisposedException.ThrowIf(application == null, this);
                    application.MergeSections(sourceId, destinationId);
                }
                return true;
            }
            catch (Exception exception)
            {
                etc.LoggerHelper.LogUnexpectedException(exception);
                return false;
            }
        }

        public void Dispose()
        {
            // Releasing the last reference lets a OneNote instance started via COM (ONENOTE.EXE -Embedding) exit.
            // Without it, that windowless instance keeps the notebook cache locked and the OneNote app fails to start.
            // Wait for an in-flight call to finish, but don't hang shutdown if OneNote is stuck.
            if (Monitor.TryEnter(syncRoot, TimeSpan.FromSeconds(5)) == false)
            {
                etc.LoggerHelper.LogWarn("Timed out waiting for OneNote; the COM reference was not released.");
                return;
            }
            try
            {
                if (application != null)
                {
                    Marshal.FinalReleaseComObject(application);
                    application = null;
                }
            }
            catch (Exception exception)
            {
                etc.LoggerHelper.LogUnexpectedException(exception);
            }
            finally
            {
                Monitor.Exit(syncRoot);
            }
        }
    }
}
