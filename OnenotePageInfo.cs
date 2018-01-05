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
        public string PageName; // The 'title' of the page
        public string HashOfInnerText; // A hash value of 'innerText' in a page

        public bool IsContentRetrieved = false; // The operation must be done over the pages where their data are available.
    }
}
