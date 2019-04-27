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
        private enum CurrentStatus
        {
            Ready,
            Scanning,
            Completed_Scanning,
            Completed_Removing,
            Removing
        };

        private CurrentStatus _currentStatus = CurrentStatus.Ready;
        private OneNoteAccessor _accessor = null;

        private void UpdateCurrentStatus(CurrentStatus newStatus, string additionalInformation = "", int progressBarCurrent = 0, int progressBarMax = 0)
        {
            _currentStatus = newStatus;

            // Do the following stuff
            // 1. Update the status label
            // 2. Update the progress bar
            string appendedStatus = additionalInformation.Length == 0 ? "" : " (" + additionalInformation + ")";
            switch (_currentStatus)
            {
                case CurrentStatus.Ready:
                    UpdateStatusLabel("Ready", appendedStatus);
                    UpdateProgressBar(0, 100);
                    break;
                case CurrentStatus.Scanning:
                    UpdateStatusLabel("Scanning", appendedStatus);
                    UpdateProgressBar(progressBarCurrent, progressBarMax);
                    break;
                case CurrentStatus.Completed_Scanning:
                    UpdateStatusLabel("Scan Completed", appendedStatus);
                    UpdateProgressBar(0, 100);
                    break;
                case CurrentStatus.Removing:
                    UpdateStatusLabel("Removing", appendedStatus);
                    UpdateProgressBar(progressBarCurrent, progressBarMax);
                    break;
                case CurrentStatus.Completed_Removing:
                    UpdateStatusLabel("Removed", appendedStatus);
                    UpdateProgressBar(0, 100);
                    break;
                default:
                    UpdateStatusLabel("Undefined", appendedStatus);
                    UpdateProgressBar(0, 100);
                    break;
            }
        }

        private void UpdateStatusLabel(string status, string appendedStatus)
        {
            toolStripStatusLabelScan.Text = status + appendedStatus;
        }

        private void UpdateProgressBar(int progressBarCurrent, int progressBarMax)
        {
            toolStripProgressBarScan.Value = progressBarCurrent;
            toolStripProgressBarScan.Maximum = progressBarMax;
        }

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
            UpdateCurrentStatus(CurrentStatus.Ready);
        }

        private void InitializeOneNoteAccessor()
        {
            try
            {
                _accessor = new OneNoteAccessor();
                _accessor.OnUpdatedScanProgress += _accessor_OnUpdatedScanProgress; ;
                _accessor.InitializeOneNoteWrapper();
            }
            catch (System.Exception exception)
            {
                etc.LoggerHelper.LogException(exception);
            }
        }

        private void _accessor_OnUpdatedScanProgress(int current, int max, string pageName)
        {
            string additionalInformation = string.Format("{0} of {1}; {2}", current, max, pageName);
            UpdateCurrentStatus(CurrentStatus.Scanning, additionalInformation);
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

        private void buttonTop_Click(object sender, EventArgs e)
        {
            try
            {
                int selectedIndex = listBoxPreferences.SelectedIndex;
                if (selectedIndex <= listBoxPreferences.Items.Count - 1 && selectedIndex != -1)
                {
                    listBoxPreferences.Items.Insert(0, listBoxPreferences.Items[selectedIndex]);
                    listBoxPreferences.Items.RemoveAt(selectedIndex + 1);
                    listBoxPreferences.SelectedIndex = 0;
                }
            }
            catch (System.Exception exception)
            {
                etc.LoggerHelper.LogException(exception);
            }
        }

        private void buttonButtom_Click(object sender, EventArgs e)
        {
            try
            {
                int selectedIndex = listBoxPreferences.SelectedIndex;
                if (selectedIndex <= listBoxPreferences.Items.Count - 1 && selectedIndex != -1)
                {
                    listBoxPreferences.Items.Insert(listBoxPreferences.Items.Count-1, listBoxPreferences.Items[selectedIndex]);
                    listBoxPreferences.Items.RemoveAt(selectedIndex);
                    listBoxPreferences.SelectedIndex = listBoxPreferences.Items.Count-1;
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
                    string lastSelectedPageId = _accessor.GetLastSelectedPageId();
                    if (lastSelectedPageId != e.Node.Name)
                    {
                        _accessor.SetLastSelectedPageId(e.Node.Name);
                        if (_accessor.HasPageId(e.Node.Name))
                        {
                            _accessor.Navigate(e.Node.Name);
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
                        string sectionPath = treeNode.Nodes[i].Tag as string;
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
                    int successCount;
                    int failureCount;

                    List<Tuple<string, bool>> removeResults = null;
                    RemoveSelectedOneNotePages(removingCount, out successCount, out failureCount, out removeResults);
                    ResetTreeViewControl();

                    string generatedHtmlFile = GenerateHtmlReportRemovedFiles(removeResults, successCount, failureCount);

                    if (failureCount != 0)
                    {
                        MessageBox.Show(string.Format("Removed: {0}", successCount), "Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        if (MessageBox.Show(string.Format("Removed : {0}\r\nFailed : {1}\r\n\r\n"
                          + "Some pages couldn't be removed. \r\n"
                          + "The HTML report file has been generated. \r\n"
                          + "Click 'yes' if you want to open the HTML report file.", successCount, failureCount), "Result", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.Yes)
                        {
                            System.Diagnostics.Process.Start(generatedHtmlFile);
                        }
                    }
                }
            }
        }

        private string GenerateHtmlReportRemovedFiles(List<Tuple<string, bool>> removeResults, int successCount, int failureCount)
        {
            string yyyyMMddHHmmss = DateTime.Now.ToString("yyyyMMddHHmmss");
            string filename = string.Format("report-removed-pages-{0}.html", yyyyMMddHHmmss);
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(filename))
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(string.Format("<html><head><title>Removed Pages @ {0}</title></head>", yyyyMMddHHmmss));
                sb.AppendLine(string.Format("<body>"));

                ////////////////////////////////////////////////////////////////////
                sb.AppendLine(string.Format("<h3>Pages that are removed successfully (count:{0})</h3>", successCount));
                sb.AppendLine(string.Format("<table border=1>"));
                foreach (Tuple<string, bool> e in removeResults)
                {
                    if (e.Item2 == true)
                    {
                        sb.AppendLine(string.Format("<tr><td>{0}</td></tr>", e.Item1));
                    }
                }
                sb.AppendLine(string.Format("</table>"));
                ////////////////////////////////////////////////////////////////////
                sb.AppendLine(string.Format("<h3>Pages that can't be removed (count:{0})</h3>", failureCount));
                sb.AppendLine(string.Format("<table border=1>"));
                foreach (Tuple<string, bool> e in removeResults)
                {
                    if (e.Item2 == false)
                    {
                        sb.AppendLine(string.Format("<tr><td>{0}</td></tr>", e.Item1));
                    }
                }
                sb.AppendLine(string.Format("</table>"));
                sb.AppendLine(string.Format("</body></html>"));
                sw.Write(sb.ToString());
            }
            return filename;
        }

        private void RemoveSelectedOneNotePages(int removingCount, out int successCount, out int failureCount, out List<Tuple<string, bool>> removeResults)
        {
            successCount = 0;
            failureCount = 0;
            removeResults = new List<Tuple<string, bool>>();

            int currentCount = 0;

            foreach (TreeNode treeNode in treeViewHierarchy.Nodes)
            {
                foreach (TreeNode childNode in treeNode.Nodes)
                {
                    if (childNode.Checked)
                    {
                        currentCount++;
                        bool result = _accessor.RemovePage(childNode.Name);
                        if (result)
                        {
                            successCount++;
                            removeResults.Add(new Tuple<string, bool>(childNode.Text, true));
                        }
                        else
                        {
                            failureCount++;
                            removeResults.Add(new Tuple<string, bool>(childNode.Text, false));
                        }

                        string additionalInformation = string.Format("{0} of {1}; {2}", currentCount, removingCount, childNode.Name);
                        UpdateCurrentStatus(CurrentStatus.Removing, additionalInformation, currentCount, removingCount);
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
            bool success = _accessor.TryUpdatePageHierarchy();
            UpdateCurrentStatus(CurrentStatus.Completed_Scanning);
            if (success)
            {
                ResetTreeViewControl();

                Dictionary<string /* innerTextHash */, List<Tuple<string, string>> /* < Page Id, Page Name > List */ > duplicatedGroups = null;
                duplicatedGroups = _accessor.GetDuplicatedGroups();

                List<string> preferredPathList = new List<string>();
                UpdateDuplicatedGroupsToUI(duplicatedGroups, preferredPathList);
                preferredPathList = MakePreferredPathListTidy(preferredPathList);
                UpdatePathPreferenceUI(preferredPathList);

                UpdateCurrentStatus(CurrentStatus.Completed_Scanning);
            }
            else
            {
                etc.LoggerHelper.LogWarn("Unable to get a hierarchy.");
            }
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
                    // NOTE: Might be unsafe if the section name contains "OneNote_RecycleBin" (very unlikely)
                    bool isLeftRecycleBin = left.Contains("\\OneNote_RecycleBin");
                    bool isRightRecycleBin = right.Contains("\\OneNote_RecycleBin");
                    if (isLeftRecycleBin && !isRightRecycleBin)
                    {
                        return 1;
                    }
                    else if (!isLeftRecycleBin && isRightRecycleBin)
                    {
                        return -1;
                    }
                    else
                    {
                        return left.CompareTo(right);
                    }
                }
            });
            sectionPathList = sectionPathList.Distinct().ToList();
            return sectionPathList;
        }

        private void UpdateDuplicatedGroupsToUI(Dictionary<string /* innerTextHash */, List<Tuple<string, string>> /* Page Id List */ > duplicatedGroups, List<string> sectionPathList)
        {
            int duplicatedGroupIndex = 0;
            foreach (KeyValuePair<string, List<Tuple<string, string>>> groupInfo in duplicatedGroups)
            {
                if (groupInfo.Value.Count > 1)
                {
                    duplicatedGroupIndex++;
                    TreeNode groupNode = this.treeViewHierarchy.Nodes.Add(groupInfo.Key, string.Format("Duplicated Page Group {0} - {1}", duplicatedGroupIndex, groupInfo.Key == "D41D8CD98F00B204E9800998ECF8427E" ? "Empty Page" : groupInfo.Key));
                    TreeViewHelper.HideCheckBox(treeViewHierarchy, groupNode);
                    for (int i = 0; i < groupInfo.Value.Count; ++i)
                    {
                        string pageId = groupInfo.Value[i].Item1;
                        string sectionPath = null;
                        bool success = _accessor.TryGetSectionPath(pageId, out sectionPath);
                        if (success)
                        {
                            // public virtual TreeNode Add(string key, string text);
                            groupNode.Nodes.Add(pageId, sectionPath + " - " + groupInfo.Value[i].Item2).Tag = sectionPath;
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
