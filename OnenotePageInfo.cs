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
    public string PageName;
    public string HashOfInnerText; // A hash value of 'innerText' in a page

    public bool IsOkay = false;
  }
}
