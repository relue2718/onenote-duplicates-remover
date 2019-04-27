using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OneNoteDuplicatesRemover
{
    public class OneNotePageInfo
    {
        public string ParentSectionId = null;
        public string ParentSectionName = null;
        public string ParentSectionFilePath = null;
        public string PageTitle = null;
        public string HashValueForInnerText = null;

        public string PageContent = null;
        public System.Xml.XmlDocument PageContentXml = null;

        // This flag must be true to make sure the content is retrieved.
        public bool IsContentRetrieved = false;
        
    }
}
