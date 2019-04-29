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
        public string HashValueForInnerText = null; // The hash value must not be a null value.
    }
}
