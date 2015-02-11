using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OneNoteDuplicatesRemover
{
    public class OnenotePageInfo
    {
        public string ParentSectionId; 
        public string ParentSectionName; 
        public string ParentSectionFilePath; 
        public string PageName;
        public string HashOfInnerText; // A hash value of inner text in a page
    }
}
