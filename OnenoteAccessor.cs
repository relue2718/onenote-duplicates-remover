using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OneNoteDuplicatesRemover
{
    public class OnenoteAccessor
    {
        private Microsoft.Office.Interop.OneNote.Application application = new Microsoft.Office.Interop.OneNote.Application();

        public delegate void LogEventHandler( string logText );
        public event LogEventHandler OnLogEvent = null;
        public delegate void ProgressEventHandler( int current, int max );
        public event ProgressEventHandler OnProgressEvent = null;

        private void FireLogEvent( string format, params object[] args )
        {
            if ( OnLogEvent != null )
            {
                OnLogEvent( string.Format( format, args ) );
            }
        }

        private void FireProgressEvent( int current, int max )
        {
            if ( OnProgressEvent != null )
            {
                OnProgressEvent( current, max );
            }
        }

        public Dictionary<string, OnenotePageInfo> GetFullHierarchy()
        {
            System.Xml.XmlDocument fullHierarchy = GetFullHierarchyAsXml();
            Dictionary<string, OnenotePageInfo> onenotePageInfos = GetOnenotePageInfos( fullHierarchy );
            int totalCount = onenotePageInfos.Count;
            int i = 0;
            foreach ( KeyValuePair<string, OnenotePageInfo> pageInfo in onenotePageInfos )
            {
                i++;
                string pageId = pageInfo.Key;
                string pageInnerTextHash = GetOnenoteInnerTextHash( pageId );
                pageInfo.Value.InnerTextHash = pageInnerTextHash;
                FireProgressEvent( i, totalCount );
            }
            return onenotePageInfos;
        }

        private System.Xml.XmlDocument GetFullHierarchyAsXml()
        {
            string strXml = "";
            application.GetHierarchy( null, Microsoft.Office.Interop.OneNote.HierarchyScope.hsPages, out strXml );
            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.LoadXml( strXml );
            return xmlDocument;
        }

        private Dictionary<string, OnenotePageInfo> GetOnenotePageInfos( System.Xml.XmlDocument xmlDocument )
        {
            Dictionary<string, OnenotePageInfo> pageInfos = new Dictionary<string, OnenotePageInfo>();
            if ( xmlDocument != null )
            {
                System.Xml.XmlNodeList pageNodeList = xmlDocument.GetElementsByTagName( "one:Page" );
                foreach ( System.Xml.XmlNode pageNode in pageNodeList )
                {
                    string pageUniqueId = pageNode.Attributes["ID"].Value;
                    string parentNodeName = pageNode.ParentNode.Name;

                    if ( parentNodeName == "one:Section" )
                    {
                        // Optional한 Attribute임. 없으면 지워지지 않은 것이므로 false로 생각하고 넘어감.
                        System.Xml.XmlAttribute IsDeletedPagesAttribute = pageNode.ParentNode.Attributes["isDeletedPages"];
                        bool isDeletedPages = false;
                        if ( IsDeletedPagesAttribute != null )
                        {
                            isDeletedPages = bool.Parse( IsDeletedPagesAttribute.Value );
                        }

                        // 지워지지 않은 Page를 대상으로만 처리한다. (휴지통에 있는 걸 남기고 원본을 지워버리는 불상사 방지)
                        if ( isDeletedPages == false )
                        {
                            if ( pageInfos.ContainsKey( pageUniqueId ) == false )
                            {
                                // 항상 있는 Attribute임
                                string sectionId = pageNode.ParentNode.Attributes["ID"].Value;
                                string sectionPath = pageNode.ParentNode.Attributes["path"].Value;
                                string sectionName = pageNode.ParentNode.Attributes["name"].Value;

                                OnenotePageInfo pageInfo = new OnenotePageInfo();
                                pageInfo.ParentSectionId = sectionId;
                                pageInfo.ParentSectionPath = sectionPath;
                                pageInfo.ParentSectionName = sectionName;
                                pageInfo.PageName = pageNode.Attributes["name"].Value;
                                pageInfos.Add( pageUniqueId, pageInfo );
                            }
                        }
                    }
                }
            }

            return pageInfos;
        }

        private string GetOnenoteInnerTextHash( string pageId )
        {
            // InnerText는 동일하지만 ObjectId와 LastModified 항목만 다른 경우가 많다.
            string pageContents = "";
            application.GetPageContent( pageId, out pageContents, Microsoft.Office.Interop.OneNote.PageInfo.piAll, Microsoft.Office.Interop.OneNote.XMLSchema.xs2010 );

            System.Xml.XmlDocument pageXmlContents = new System.Xml.XmlDocument();
            pageXmlContents.LoadXml( pageContents );

            byte[] rawInnerText = Encoding.UTF8.GetBytes( pageXmlContents.InnerText );
            byte[] computedHash = System.Security.Cryptography.MD5.Create().ComputeHash( rawInnerText );

            return Utils.MakeHashString( computedHash );
        }

        public void Navigate( string lastSelectedPageId )
        {
            application.NavigateTo( lastSelectedPageId );
        }

        public bool RemovePage( string pageId )
        {
            try
            {
                application.DeleteHierarchy( pageId );
                return true;
            }
            catch ( System.Exception )
            {
                return false;
            }
        }
    }
}
