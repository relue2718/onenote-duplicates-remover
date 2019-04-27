using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OneNoteDuplicatesRemover.etc
{
  public class MyTreeView : System.Windows.Forms.TreeView
  {
    protected override bool DoubleBuffered
    {
      get
      {
        //return base.DoubleBuffered;
        return true;
      }
      set
      {
        //base.DoubleBuffered = value;
        base.DoubleBuffered = true;
      }
    }
  }
}
