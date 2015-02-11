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
    private OnenoteAccessor onenoteAccessor = null;
    private Dictionary<string, OnenotePageInfo> fullHierarchy = new Dictionary<string, OnenotePageInfo>();
    private string lastSelectedPageId = "";

    public FormMain()
    {
      InitializeFileLogger();

      InitializeComponent();
      InitializeUIComponent();

      onenoteAccessor = new OnenoteAccessor();
      onenoteAccessor.OnProgressEvent += onenoteAccessor_OnProgressEvent;
    }

    private static void InitializeFileLogger()
    {
      string today = DateTime.Now.ToString("yyyyMMdd-HHmmssFFF");
      etc.FileLogger.Instance.Init("onenote-duplicates-remover-" + today + ".log");
    }

    private void InitializeUIComponent()
    {
      toolStripStatusLabelScan.Text = "Ready";
    }

    void onenoteAccessor_OnProgressEvent(int current, int max)
    {
      SetProgressBar(current, max);
    }

    private void buttonUp_Click(object sender, EventArgs e)
    {
      int selectedIndex = listBoxPreferences.SelectedIndex;
      if (selectedIndex > 0 && selectedIndex != -1)
      {
        listBoxPreferences.Items.Insert(selectedIndex - 1, listBoxPreferences.Items[selectedIndex]);
        listBoxPreferences.Items.RemoveAt(selectedIndex + 1);
        listBoxPreferences.SelectedIndex = selectedIndex - 1;
      }
    }

    private void buttonDown_Click(object sender, EventArgs e)
    {
      int selectedIndex = listBoxPreferences.SelectedIndex;
      if (selectedIndex < listBoxPreferences.Items.Count - 1 && selectedIndex != -1)
      {
        listBoxPreferences.Items.Insert(selectedIndex + 2, listBoxPreferences.Items[selectedIndex]);
        listBoxPreferences.Items.RemoveAt(selectedIndex);
        listBoxPreferences.SelectedIndex = selectedIndex + 1;
      }
    }

    private void treeViewHierarchy_BeforeSelect(object sender, TreeViewCancelEventArgs e)
    {
      if (lastSelectedPageId != e.Node.Name)
      {
        lastSelectedPageId = e.Node.Name;
        if (fullHierarchy.ContainsKey(lastSelectedPageId))
        {
          if (checkBoxNavigateAutomatically.Checked == true)
          {
            onenoteAccessor.Navigate(lastSelectedPageId);
          }
        }
      }
    }

    private void buttonScanDuplicatedPages_Click(object sender, EventArgs e)
    {
      ScanDuplicated();
    }

    private void buttonSelectAllExceptOne_Click(object sender, EventArgs e)
    {
      // Select all except one
      List<string> preferences = new List<string>(listBoxPreferences.Items.Cast<string>());

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

    private void buttonDeselectAll_Click(object sender, EventArgs e)
    {
      foreach (TreeNode treeNode in treeViewHierarchy.Nodes)
      {
        foreach (TreeNode childNode in treeNode.Nodes)
        {
          childNode.Checked = false;
        }
      }
    }

    private void buttonRemoveSelectedPages_Click(object sender, EventArgs e)
    {
      // ** for the safety ** Not to allow user to select every page in a duplicated group.
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
          MessageBox.Show("Please review your selection: \r\n"+
            "You selected every page in the duplicated group; it means that you will lose the data.\r\n\r\n"+
            "group: " + treeNode.Text, "De-duplication job is aborted.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
          treeViewHierarchy.SelectedNode = treeNode;
          treeViewHierarchy.Focus();
          return;
        }
      }

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

      if (MessageBox.Show("Are you sure to remove the selected pages (count: " + removingCount.ToString() + ")?\r\n\r\nPlease 'BACKUP' before proceeding.", "Remove", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
      {
        ResetProgressBar();
        int currentCount = 0;
        int successCount = 0;
        int failureCount = 0;
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
        ResetProgressBar();
        MessageBox.Show(string.Format("Deleted : {0}\r\nFailed : {1}", successCount, failureCount), "Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
      }
    }

    private void ScanDuplicated()
    {
      ResetProgressBar();
      fullHierarchy = onenoteAccessor.GetFullHierarchy();
      ResetProgressBar();
      this.treeViewHierarchy.Nodes.Clear();

      Dictionary<string /* innerTextHash */, List<string> /* Page Id List */ > duplicatedGroups = new Dictionary<string, List<string>>();
      foreach (KeyValuePair<string, OnenotePageInfo> pageInfo in fullHierarchy)
      {
        string pageId = pageInfo.Key;
        string pageInnerTextHash = pageInfo.Value.HashOfInnerText;

        if (duplicatedGroups.ContainsKey(pageInnerTextHash) == false)
        {
          duplicatedGroups.Add(pageInnerTextHash, new List<string>());
        }

        duplicatedGroups[pageInnerTextHash].Add(pageId);
      }

      List<string> sectionPathList = new List<string>();

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
            string sectionPath = fullHierarchy[groupInfo.Value[i]].ParentSectionFilePath;
            groupNode.Nodes.Add(groupInfo.Value[i], sectionPath);
            sectionPathList.Add(System.IO.Path.GetDirectoryName(sectionPath));
          }
        }
      }

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

      listBoxPreferences.Items.Clear();
      foreach (string sectionPath in sectionPathList)
      {
        listBoxPreferences.Items.Add(sectionPath);
      }

      treeViewHierarchy.ExpandAll();
      
      ResetProgressBar();
      toolStripStatusLabelScan.Text = "Scan Finished";
    }

    #region Progress Bar
    private void SetProgressBar(int current, int max)
    {
      toolStripProgressBarScan.Maximum = max;
      toolStripProgressBarScan.Value = current;
      
      toolStripStatusLabelScan.Text = string.Format("{0} of {1}", current, max);
      
      Application.DoEvents(); // While scanning, try to update the label.
    }

    private void ResetProgressBar()
    {
      this.toolStripProgressBarScan.Value = 0;
    }
    #endregion
  }
}
