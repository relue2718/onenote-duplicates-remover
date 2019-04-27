using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OneNoteDuplicatesRemover
{
    public class OneNotePageInfo
    {
        public string ParentSectionId;
        public string ParentSectionName;
        public string ParentSectionFilePath;
        public string PageTitle;
        public string HashValueForInnerText;

        // This flag must be true to make sure the content is retrieved.
        public bool IsContentRetrieved = false;
    }
}
