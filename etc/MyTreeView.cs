using System;

namespace OneNoteDuplicatesRemover.etc
{
  public class MyTreeView : System.Windows.Forms.TreeView
  {
    protected override void OnHandleCreated(EventArgs e)
    {
      base.OnHandleCreated(e);
      UpdateRowHeight();
      foreach (System.Windows.Forms.TreeNode node in Nodes)
        TreeViewHelper.HideCheckBox(this, node);
    }

    protected override void OnFontChanged(EventArgs e)
    {
      base.OnFontChanged(e);
      UpdateRowHeight();
    }

    protected override void OnDpiChangedAfterParent(EventArgs e)
    {
      base.OnDpiChangedAfterParent(e);
      UpdateRowHeight();
    }

    private void UpdateRowHeight()
    {
      ItemHeight = Math.Max(Font.Height + 10 * DeviceDpi / 96, 30 * DeviceDpi / 96);
    }

    protected override bool DoubleBuffered
    {
      get => true;
      set => base.DoubleBuffered = true;
    }
  }
}
