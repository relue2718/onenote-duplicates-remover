using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OneNoteDuplicatesRemover
{
  public partial class FormMain : Form
  {
    private OneNoteAccessor onenoteAccessor = null;

    public FormMain()
    {
      InitializeComponent();
    }

    private void InitializeFileLogger()
    {
      string today = DateTime.Now.ToString("yyyyMMdd-HHmmssFFF");
      etc.FileLogger.Instance.Init("onenote-duplicates-remover-" + today + ".log");
    }

    private void InitializeUIComponent()
    {
      UpdateStatus("Ready");
    }

    private void InitializeOneNoteAccessor()
    {
      try
      {
        onenoteAccessor = new OneNoteAccessor();
        onenoteAccessor.OnProgressEvent += onenoteAccessor_OnProgressEvent;
        onenoteAccessor.OnAbortedEvent += onenoteAccessor_OnAbortedEvent;
        onenoteAccessor.InitializeOneNoteWrapper();
      }
      catch (System.Exception exception)
      {
        etc.LoggerHelper.LogException(exception);
      }
    }

    private void onenoteAccessor_OnAbortedEvent(string msg)
    {
      MessageBox.Show(string.Format("Sorry for the inconvenience.\r\n\r\n" +
        "The program has encountered with an unrecoverable error.\r\n\r\n" +
        "Message: {0}", msg), "Aborted", MessageBoxButtons.OK, MessageBoxIcon.Error);
      this.Close();
    }

    void onenoteAccessor_OnProgressEvent(int current, int max)
    {
      SetProgressBar(current, max);
    }

    private void buttonUp_Click(object sender, EventArgs e)
    {
      try
      {
        int selectedIndex = listBoxPreferences.SelectedIndex;
        if (selectedIndex > 0 && selectedIndex != -1)
        {
          listBoxPreferences.Items.Insert(selectedIndex - 1, listBoxPreferences.Items[selectedIndex]);
          listBoxPreferences.Items.RemoveAt(selectedIndex + 1);
          listBoxPreferences.SelectedIndex = selectedIndex - 1;
        }
      }
      catch (System.Exception exception)
      {
        etc.LoggerHelper.LogException(exception);
      }
    }

    private void buttonDown_Click(object sender, EventArgs e)
    {
      try
      {
        int selectedIndex = listBoxPreferences.SelectedIndex;
        if (selectedIndex < listBoxPreferences.Items.Count - 1 && selectedIndex != -1)
        {
          listBoxPreferences.Items.Insert(selectedIndex + 2, listBoxPreferences.Items[selectedIndex]);
          listBoxPreferences.Items.RemoveAt(selectedIndex);
          listBoxPreferences.SelectedIndex = selectedIndex + 1;
        }
      }
      catch (System.Exception exception)
      {
        etc.LoggerHelper.LogException(exception);
      }
    }

    private void treeViewHierarchy_BeforeSelect(object sender, TreeViewCancelEventArgs e)
    {
      try
      {
        if (checkBoxNavigateAutomatically.Checked == true)
        {
          string lastSelectedPageId = onenoteAccessor.GetLastSelectedPageId();
          if (lastSelectedPageId != e.Node.Name)
          {
            onenoteAccessor.SetLastSelectedPageId(e.Node.Name);
            if (onenoteAccessor.HasPageId(e.Node.Name))
            {
              onenoteAccessor.Navigate(e.Node.Name);
            }
          }
        }
      }
      catch (System.Exception exception)
      {
        etc.LoggerHelper.LogException(exception);
      }
    }

    private void buttonScanDuplicatedPages_Click(object sender, EventArgs e)
    {
      ScanDuplicatedPages();
    }

    private void buttonSelectAllExceptOne_Click(object sender, EventArgs e)
    {
      try
      {
        List<string> preferences = new List<string>(listBoxPreferences.Items.Cast<string>()); // Select all except one

        foreach (TreeNode treeNode in treeViewHierarchy.Nodes)
        {
          int childCount = treeNode.Nodes.Count;
          int[] priorities = new int[childCount];
          int whereMin = int.MaxValue;
          int wherePos = -1;
          for (int i = 0; i < childCount; ++i)
          {
            string sectionPath = treeNode.Nodes[i].Text;
            string sectionDir = System.IO.Path.GetDirectoryName(sectionPath);
            int where = preferences.IndexOf(sectionDir);
            if (whereMin > where)
            {
              whereMin = where;
              wherePos = i;
            }
          }
          for (int i = 0; i < childCount; ++i)
          {
            treeNode.Nodes[i].Checked = (i != wherePos);
          }
        }
      }
      catch (System.Exception exception)
      {
        etc.LoggerHelper.LogException(exception);
      }
    }


    private void buttonDeselectAll_Click(object sender, EventArgs e)
    {
      try
      {
        foreach (TreeNode treeNode in treeViewHierarchy.Nodes)
        {
          foreach (TreeNode childNode in treeNode.Nodes)
          {
            childNode.Checked = false;
          }
        }
      }
      catch (System.Exception exception)
      {
        etc.LoggerHelper.LogException(exception);
      }
    }

    private void buttonRemoveSelectedPages_Click(object sender, EventArgs e)
    {
      // ** for the safety ** Not to allow user to select every page in a duplicated group.
      TreeNode selectedTreeNode = null;
      bool isEveryPageSelected = CheckIfEveryPageIsSelected(out selectedTreeNode);
      if (isEveryPageSelected)
      {
        MessageBox.Show(
          "De-duplication is aborted. \r\n\r\n" +
          "You've selected every page in the same group. (name:" + selectedTreeNode.Text + ")\r\n\r\n" +
          "You should keep at least one copy of pages in order to keep your data.",
          "Aborted", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

        treeViewHierarchy.SelectedNode = selectedTreeNode;
        treeViewHierarchy.Focus();
      }
      else
      {
        int removingCount = GetCountOfPageBeingRemoved();
        if (MessageBox.Show("Are you sure to remove the selected pages? (page count: " + removingCount.ToString() + ")\r\n\r\nPlease *BACKUP* before proceeding de-duplication.", "Confirm", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
        {
          ResetProgressBar();
          int successCount;
          int failureCount;
          RemoveSelectedOneNotePages(removingCount, out successCount, out failureCount);
          ResetProgressBar();
          ResetTreeViewControl();
          MessageBox.Show(string.Format("Deleted : {0}\r\nFailed : {1}", successCount, failureCount), "Summary", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
      }
    }

    private void RemoveSelectedOneNotePages(int removingCount, out int successCount, out int failureCount)
    {
      successCount = 0;
      failureCount = 0;

      int currentCount = 0;

      foreach (TreeNode treeNode in treeViewHierarchy.Nodes)
      {
        foreach (TreeNode childNode in treeNode.Nodes)
        {
          if (childNode.Checked == true)
          {
            currentCount++;
            bool result = onenoteAccessor.RemovePage(childNode.Name);
            if (result)
            {
              successCount++;
            }
            else
            {
              failureCount++;
            }
            SetProgressBar(currentCount, removingCount);
          }
        }
      }
    }

    private int GetCountOfPageBeingRemoved()
    {
      int removingCount = 0;
      foreach (TreeNode treeNode in treeViewHierarchy.Nodes)
      {
        foreach (TreeNode childNode in treeNode.Nodes)
        {
          if (childNode.Checked == true)
          {
            removingCount++;
          }
        }
      }
      return removingCount;
    }

    private bool CheckIfEveryPageIsSelected(out TreeNode selectedTreeNode)
    {
      selectedTreeNode = null;
      foreach (TreeNode treeNode in treeViewHierarchy.Nodes)
      {
        int checkedCount = 0;
        foreach (TreeNode childNode in treeNode.Nodes)
        {
          if (childNode.Checked == true)
          {
            checkedCount++;
          }
        }
        if (treeNode.Nodes.Count == checkedCount)
        {
          selectedTreeNode = treeNode;
          return true;
        }
      }
      return false;
    }

    private void ScanDuplicatedPages()
    {
      ResetProgressBar();
      bool success = onenoteAccessor.UpdateHierarchy();
      if (success)
      {
        ResetProgressBar();
        ResetTreeViewControl();

        Dictionary<string /* innerTextHash */, List<string> /* Page Id List */ > duplicatedGroups = null;
        duplicatedGroups = onenoteAccessor.GetDuplicatedGroups();

        List<string> preferredPathList = new List<string>();
        UpdateDuplicatedGroupsToUI(duplicatedGroups, preferredPathList);
        preferredPathList = MakePreferredPathListTidy(preferredPathList);
        UpdatePathPreferenceUI(preferredPathList);

        ResetProgressBar();
        UpdateStatus("Scan Finished");
      }
      else
      {
        etc.LoggerHelper.LogWarn("Unable to get a hierarchy.");
      }
    }

    private void UpdateStatus(string msg)
    {
      toolStripStatusLabelScan.Text = msg;
    }

    private void UpdatePathPreferenceUI(List<string> sectionPathList)
    {
      listBoxPreferences.Items.Clear();
      foreach (string sectionPath in sectionPathList)
      {
        listBoxPreferences.Items.Add(sectionPath);
      }
    }

    private static List<string> MakePreferredPathListTidy(List<string> sectionPathList)
    {
      sectionPathList.Sort((string left, string right) =>
      {
        bool isLeftCloud = (left.IndexOf("https:") == 0);
        bool isRightCloud = (right.IndexOf("https:") == 0);
        if (isLeftCloud && !isRightCloud)
        {
          return -1;
        }
        else if (!isLeftCloud && isRightCloud)
        {
          return 1;
        }
        else
        {
          return left.CompareTo(right);
        }
      });
      sectionPathList = sectionPathList.Distinct().ToList();
      return sectionPathList;
    }

    private void UpdateDuplicatedGroupsToUI(Dictionary<string /* innerTextHash */, List<string> /* Page Id List */ > duplicatedGroups, List<string> sectionPathList)
    {
      int duplicatedGroupIndex = 0;
      foreach (KeyValuePair<string, List<string>> groupInfo in duplicatedGroups)
      {
        if (groupInfo.Value.Count > 1)
        {
          duplicatedGroupIndex++;
          TreeNode groupNode = this.treeViewHierarchy.Nodes.Add(groupInfo.Key, string.Format("Duplicated Page Group {0} - {1}", duplicatedGroupIndex, groupInfo.Key == "D41D8CD98F00B204E9800998ECF8427E" ? "Empty Page" : groupInfo.Key));
          TreeViewHelper.HideCheckBox(treeViewHierarchy, groupNode);
          for (int i = 0; i < groupInfo.Value.Count; ++i)
          {
            string pageId = groupInfo.Value[i];
            string sectionPath = null;
            bool success = onenoteAccessor.TryGetSectionPath(pageId, out sectionPath);
            if (success)
            {
              groupNode.Nodes.Add(pageId, sectionPath);
              sectionPathList.Add(System.IO.Path.GetDirectoryName(sectionPath));
            }
          }
        }
      }
      treeViewHierarchy.ExpandAll();
    }

    private void ResetTreeViewControl()
    {
      this.treeViewHierarchy.Nodes.Clear();
    }

    private void SetProgressBar(int current, int max)
    {
      try
      {
        toolStripProgressBarScan.Maximum = max;
        toolStripProgressBarScan.Value = current;
        UpdateStatus(string.Format("{0} of {1}", current, max));
        Application.DoEvents(); // While scanning, try to update the label.
      }
      catch (System.Exception exception)
      {
        etc.LoggerHelper.LogException(exception);
      }
    }

    private void ResetProgressBar()
    {
      this.toolStripProgressBarScan.Value = 0;
    }

    private void FormMain_Load(object sender, EventArgs e)
    {
      try
      {
        InitializeFileLogger();
        InitializeUIComponent();
        InitializeOneNoteAccessor();
      }
      catch (System.Exception exception)
      {
        etc.LoggerHelper.LogException(exception);
      }
    }
  }
}
