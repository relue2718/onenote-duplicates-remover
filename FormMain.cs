using System;
using System.Collections.Generic;
using System.Data;
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
            ScanCompleted,
            RemoveCompleted,
            Removing
        };

        private CurrentStatus currentStatus = CurrentStatus.Ready;
        private OneNoteAccessor accessor = null;

        public FormMain()
        {
            InitializeComponent();
        }

        private void InitializeFileLogger()
        {
            string currentTimestamp = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-FFF");
            etc.FileLogger.Instance.Init("log-" + currentTimestamp + ".log");
        }

        private void InitializeUIComponent()
        {
            UpdateCurrentStatus(CurrentStatus.Ready);
        }

        private void InitializeOneNoteAccessor()
        {
            try
            {
                accessor = new OneNoteAccessor();
                accessor.EventProgressEvent += Accessor_EventProgress;
                accessor.EventScanCompleted += Accessor_EventScanCompleted;
                accessor.InitializeOneNoteWrapper();
            }
            catch (System.Exception exception)
            {
                etc.LoggerHelper.LogUnexpectedException(exception);
            }
        }

        private void Accessor_EventScanCompleted()
        {
            // Main UI thread will do this task.
            Invoke((MethodInvoker)(() =>
            {
                UpdateCurrentStatus(CurrentStatus.ScanCompleted);

                Dictionary<string, List<Tuple<string, string>>> duplicatesGroups = accessor.GetDuplicatesGroups();
                if (SanityCheck(duplicatesGroups))
                {
                    List<string> sortedSectionPathList = SortSectionPathList(GetSectionPathList(duplicatesGroups));
                    UpdateUIFromDuplicatesGroups(duplicatesGroups);
                    UpdatePathPreferenceUI(sortedSectionPathList);
                }
            }));
        }

        private bool SanityCheck(Dictionary<string, List<Tuple<string, string>>> duplicatesGroups)
        {
            HashSet<string> knownPagesEntire = new HashSet<string>();
            foreach (KeyValuePair<string, List<Tuple<string, string>>> groupInfo in duplicatesGroups)
            {
                if (groupInfo.Value.Count > 1)
                {
                    HashSet<string> knownPagesGroup = new HashSet<string>();
                    foreach (Tuple<string /* page ID */, string> pageInfo in groupInfo.Value)
                    {
                        knownPagesGroup.Add(pageInfo.Item1);
                        if (!knownPagesEntire.Contains(pageInfo.Item1))
                        {
                            knownPagesEntire.Add(pageInfo.Item1);
                        }
                        else
                        {
                            etc.LoggerHelper.LogError("Page ID must be unique in all groups.");
                            return false;
                        }
                    }
                    if (knownPagesGroup.Count != groupInfo.Value.Count)
                    {
                        etc.LoggerHelper.LogError("Page ID must be unique in each group.");
                    }
                }
            }
            return true;
        }

        private void Accessor_EventProgress(int countReadPages_Success, int countReadPages_Failed, int countHashedPages_Success, int countHashedPages_Failed, int countTotalPages, string lastPageTitle)
        {
            // Main UI thread will do this task.
            Invoke((MethodInvoker)(() =>
            {
                int countReadAhead = countReadPages_Success - countHashedPages_Success;
                string additionalInfo = string.Format("Hashed:({1}/{2}), Read Ahead:{0}, Errs:({3}, {4}) -- {5}", countReadAhead, countHashedPages_Success, countTotalPages, countReadPages_Failed, countHashedPages_Failed, lastPageTitle);
                UpdateCurrentStatus(CurrentStatus.Scanning, additionalInfo);
            }));
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            try
            {
                etc.LoggerHelper.EventCategoryCountChanged += LoggerHelper_EventCategoryCountChanged;
                InitializeFileLogger();
                InitializeUIComponent();
                InitializeOneNoteAccessor();
            }
            catch (System.Exception exception)
            {
                etc.LoggerHelper.LogUnexpectedException(exception);
            }
        }

        private void LoggerHelper_EventCategoryCountChanged(int countInfo, int countWarning, int countError, int countException)
        {
            Invoke((MethodInvoker)(() =>
            {
                labelMessageCounts.Text = string.Format("[Log] Info: {0}, Warning: {1}, Error: {2}, Exception: {3}", countInfo, countWarning, countError, countException);
            }));
        }

        private void UpdateCurrentStatus(CurrentStatus newStatus, string additionalInfo = "", int progressBarCurrent = 0, int progressBarMaximum = 0)
        {
            currentStatus = newStatus;
            string additionalStatus = additionalInfo.Length == 0 ? "" : " (" + additionalInfo + ")";
            switch (currentStatus)
            {
                case CurrentStatus.Ready:
                    UpdateStatusLabel("Ready", additionalStatus);
                    UpdateProgressBar(0, 100);
                    break;
                case CurrentStatus.Scanning:
                    UpdateStatusLabel("Scanning", additionalStatus);
                    UpdateProgressBar(progressBarCurrent, progressBarMaximum);
                    break;
                case CurrentStatus.ScanCompleted:
                    SetUIControlEnabled(true);
                    etc.LoggerHelper.LogInfo("Scan completed.");
                    UpdateStatusLabel("Scan Completed", additionalStatus);
                    UpdateProgressBar(0, 100);
                    break;
                case CurrentStatus.Removing:
                    UpdateStatusLabel("Removing", additionalStatus);
                    UpdateProgressBar(progressBarCurrent, progressBarMaximum);
                    break;
                case CurrentStatus.RemoveCompleted:
                    SetUIControlEnabled(true);
                    etc.LoggerHelper.LogInfo("Remove completed.");
                    UpdateStatusLabel("Removed", additionalStatus);
                    UpdateProgressBar(0, 100);
                    break;
                default:
                    SetUIControlEnabled(false);
                    etc.LoggerHelper.LogError("Invalid status! {0}", currentStatus);
                    UpdateStatusLabel("Undefined", additionalStatus);
                    UpdateProgressBar(0, 100);
                    break;
            }
        }

        private void UpdateStatusLabel(string status, string additionalStatus)
        {
            toolStripStatusLabelScan.Text = status + additionalStatus;
        }

        private void UpdateProgressBar(int progressBarCurrent, int progressBarMaximum)
        {
            toolStripProgressBarScan.Value = progressBarCurrent;
            toolStripProgressBarScan.Maximum = progressBarMaximum;
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
                etc.LoggerHelper.LogUnexpectedException(exception);
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
                etc.LoggerHelper.LogUnexpectedException(exception);
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
                etc.LoggerHelper.LogUnexpectedException(exception);
            }
        }

        private void buttonBottom_Click(object sender, EventArgs e)
        {
            try
            {
                int selectedIndex = listBoxPreferences.SelectedIndex;
                if (selectedIndex <= listBoxPreferences.Items.Count - 1 && selectedIndex != -1)
                {
                    listBoxPreferences.Items.Insert(listBoxPreferences.Items.Count - 1, listBoxPreferences.Items[selectedIndex]);
                    listBoxPreferences.Items.RemoveAt(selectedIndex);
                    listBoxPreferences.SelectedIndex = listBoxPreferences.Items.Count - 1;
                }
            }
            catch (System.Exception exception)
            {
                etc.LoggerHelper.LogUnexpectedException(exception);
            }
        }

        private void treeViewHierarchy_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            try
            {
                if (checkBoxNavigateAutomatically.Checked == true)
                {
                    string lastSelectedPageId = accessor.GetLastSelectedPageId();
                    string highlightedPageId = e.Node.Name;
                    if (e.Node.Tag != null)
                    {
                        if (lastSelectedPageId != highlightedPageId)
                        {
                            accessor.SetLastSelectedPageId(highlightedPageId);
                            if (accessor.CheckIfPageExists(highlightedPageId))
                            {
                                accessor.Navigate(highlightedPageId);
                            }
                            else
                            {
                                etc.LoggerHelper.LogError("Unable to find the page ID: {0}", highlightedPageId);
                            }
                        }
                    }
                    else
                    {
                        // Ignore this case: the selected item is not a page.
                    }
                }
            }
            catch (System.Exception exception)
            {
                etc.LoggerHelper.LogUnexpectedException(exception);
            }
        }

        private void buttonScanDuplicatedPages_Click(object sender, EventArgs e)
        {
            ResetUIControl();
            SetUIControlEnabled(false);
            if (accessor.InvokeScanPages())
            {
                // Successfully invoked
                etc.LoggerHelper.LogInfo("Successfully loaded the page hierarchy.");
            }
            else
            {
                // Outright failure
                ResetUIControl();
                SetUIControlEnabled(true);
                etc.LoggerHelper.LogError("Failed to parse raw_xml_string or retrieve page hierarchy.");
            }
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
                etc.LoggerHelper.LogUnexpectedException(exception);
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
                etc.LoggerHelper.LogUnexpectedException(exception);
            }
        }

        private void buttonRemoveSelectedPages_Click(object sender, EventArgs e)
        {
            SetUIControlEnabled(false);

            // ** for the safety ** Not to allow user to select every page in a duplicated group.
            TreeNode selectedTreeNode = null;
            bool isEveryPageSelected = CheckIfEveryPageIsSelected(out selectedTreeNode);
            if (isEveryPageSelected)
            {
                MessageBox.Show("WARNING: Data might be lost!\r\n\r\n" + "You have selected every page in the same group.\r\n" + string.Format("Name: {0}.\r\n", selectedTreeNode.Text) + "\r\n\r\nThe removal operation has been canceled.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                SetUIControlEnabled(true);
                treeViewHierarchy.SelectedNode = selectedTreeNode;
                treeViewHierarchy.Focus();
            }
            else
            {
                int countPagesBeingRemoved = GetCountPagesBeingRemoved();
                if (countPagesBeingRemoved > 0)
                {
                    if (MessageBox.Show("Are you sure to remove the selected pages?\r\n" + string.Format("The number of the selected pages: {0}", countPagesBeingRemoved) + "\r\n\r\nPlease **BACKUP** OneNote notebooks!", "Confirm", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                    {
                        int countRemoval_Success;
                        int countRemoval_Failed;
                        List<Tuple<string, string, bool>> resultRemoval = null;
                        RemoveSelectedPages(countPagesBeingRemoved, out countRemoval_Success, out countRemoval_Failed, out resultRemoval);
                        string generatedHtmlFile = GenerateHtmlReportRemovedFiles(resultRemoval, countRemoval_Success, countRemoval_Failed);

                        MessageBox.Show(string.Format("Removed: {0} pages", countRemoval_Success) + "\r\n" +
                            string.Format("Failed to remove: {0} pages", countRemoval_Failed), "Result", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        System.Diagnostics.Process.Start(generatedHtmlFile);
                    }
                    ResetUIControl();
                }
                SetUIControlEnabled(true);
            }
        }

        private string GenerateHtmlReportRemovedFiles(List<Tuple<string, string, bool>> resultRemoval, int countSuccess, int countFailure)
        {
            DateTime currentTimestamp = DateTime.Now;
            string yyyyMMddHHmmss = currentTimestamp.ToString("yyyy-MM-dd-HH-mm-ss");
            string filename = string.Format("report-{0}.html", yyyyMMddHHmmss);
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(filename))
            {
                StringBuilder sb = new StringBuilder();
                string title = string.Format("Report @ {0}", currentTimestamp.ToString());
                sb.AppendLine(string.Format("<html><head><title>{0}</title></head>", title));
                sb.AppendLine(string.Format("<body>"));
                sb.AppendLine(string.Format("<h1>{0}</h1>", title));
                ////////////////////////////////////////////////////////////////////
                sb.AppendLine(string.Format("<h3>Removed Pages (Count: {0})</h3>", countSuccess));
                sb.AppendLine(string.Format("<table border=1>"));
                sb.AppendLine(string.Format("<thead><td>Page ID</td><td>Section Title</td></thead>"));
                foreach (Tuple<string, string, bool> e in resultRemoval)
                {
                    if (e.Item3 == true)
                    {
                        sb.AppendLine(string.Format("<tr><td>{0}</td><td>{1}</td></tr>", e.Item1, e.Item2));
                    }
                }
                sb.AppendLine(string.Format("</table>"));
                ////////////////////////////////////////////////////////////////////
                sb.AppendLine(string.Format("<h3>Pages that could not be removed (Count:{0})</h3>", countFailure));
                sb.AppendLine(string.Format("<table border=1>"));
                sb.AppendLine(string.Format("<thead><td>Page ID</td><td>Section Title</td></thead>"));
                foreach (Tuple<string, string, bool> e in resultRemoval)
                {
                    if (e.Item3 == false)
                    {
                        sb.AppendLine(string.Format("<tr><td>{0}</td><td>{1}</td></tr>", e.Item1, e.Item2));
                    }
                }
                sb.AppendLine(string.Format("</table>"));
                sb.AppendLine(string.Format("</body></html>"));
                sw.Write(sb.ToString());
            }
            return filename;
        }

        private void RemoveSelectedPages(int countPagesBeingRemoved, out int countSucess, out int countFailed, out List<Tuple<string, string, bool>> resultRemoval)
        {
            countSucess = 0;
            countFailed = 0;
            resultRemoval = new List<Tuple<string, string, bool>>();

            int currentCount = 0;

            foreach (TreeNode treeNode in treeViewHierarchy.Nodes)
            {
                foreach (TreeNode childNode in treeNode.Nodes)
                {
                    if (childNode.Checked)
                    {
                        currentCount++;
                        bool result = accessor.RemovePage(childNode.Name);
                        if (result)
                        {
                            countSucess++;
                            resultRemoval.Add(new Tuple<string, string, bool>(childNode.Name, childNode.Text, true));
                        }
                        else
                        {
                            countFailed++;
                            resultRemoval.Add(new Tuple<string, string, bool>(childNode.Name, childNode.Text, false));
                        }

                        string additionalInformation = string.Format("{0}/{1} -- {2}", currentCount, countPagesBeingRemoved, childNode.Name);
                        UpdateCurrentStatus(CurrentStatus.Removing, additionalInformation, currentCount, countPagesBeingRemoved);
                    }
                }
            }
        }

        private int GetCountPagesBeingRemoved()
        {
            int countPagesBeingRemoved = 0;
            foreach (TreeNode treeNode in treeViewHierarchy.Nodes)
            {
                foreach (TreeNode childNode in treeNode.Nodes)
                {
                    if (childNode.Checked == true)
                    {
                        countPagesBeingRemoved++;
                    }
                }
            }
            return countPagesBeingRemoved;
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

        private void UpdatePathPreferenceUI(List<string> sectionPathList)
        {
            listBoxPreferences.Items.Clear();
            foreach (string sectionPath in sectionPathList)
            {
                listBoxPreferences.Items.Add(sectionPath);
            }
        }

        private static List<string> SortSectionPathList(List<string> sectionPathList)
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

        private List<string> GetSectionPathList(Dictionary<string /* innerTextHash */, List<Tuple<string, string>> /* Page Id List */ > duplicatesGroups)
        {
            List<string> section_path_list = new List<string>();
            foreach (KeyValuePair<string, List<Tuple<string, string>>> group_info in duplicatesGroups)
            {
                if (group_info.Value.Count > 1)
                {
                    for (int i = 0; i < group_info.Value.Count; ++i)
                    {
                        string page_id = group_info.Value[i].Item1;
                        string section_path = "";
                        if (accessor.TryGetSectionPath(page_id, out section_path))
                        {
                            section_path_list.Add(System.IO.Path.GetDirectoryName(section_path));
                        }
                    }
                }
            }
            return section_path_list;
        }

        private void UpdateUIFromDuplicatesGroups(Dictionary<string /* innerTextHash */, List<Tuple<string, string>> /* Page Id List */ > duplicatesGroups)
        {
            int duplicatesGroupIndex = 0;
            foreach (KeyValuePair<string, List<Tuple<string, string>>> groupInfo in duplicatesGroups)
            {
                if (groupInfo.Value.Count > 1)
                {
                    duplicatesGroupIndex++;
                    TreeNode groupTreeNode = treeViewHierarchy.Nodes.Add(groupInfo.Key, string.Format("Duplicated Page Group {0} - {1}", duplicatesGroupIndex, groupInfo.Key == "E3B0C44298FC1C149AFBF4C8996FB92427AE41E4649B934CA495991B7852B855" ? "Empty Page" : groupInfo.Key));
                    TreeViewHelper.HideCheckBox(treeViewHierarchy, groupTreeNode);
                    for (int i = 0; i < groupInfo.Value.Count; ++i)
                    {
                        string pageId = groupInfo.Value[i].Item1;
                        string section_path = "";
                        if (accessor.TryGetSectionPath(pageId, out section_path))
                        {
                            groupTreeNode.Nodes.Add(pageId, section_path + " - " + groupInfo.Value[i].Item2).Tag = section_path;
                        }
                    }
                }
            }
            treeViewHierarchy.ExpandAll();
        }

        private void ResetUIControl()
        {
            UpdateCurrentStatus(CurrentStatus.Ready);
            treeViewHierarchy.Nodes.Clear();
            listBoxPreferences.Items.Clear();
        }

        private void SetUIControlEnabled(bool enabled)
        {
            buttonScanDuplicatedPages.Enabled = enabled;
            buttonSelectAllExceptOne.Enabled = enabled;
            buttonDeselectAll.Enabled = enabled;
            buttonRemoveSelectedPages.Enabled = enabled;
            checkBoxNavigateAutomatically.Enabled = enabled;
            treeViewHierarchy.Enabled = enabled;
            listBoxPreferences.Enabled = enabled;
            buttonTop.Enabled = enabled;
            buttonUp.Enabled = enabled;
            buttonDown.Enabled = enabled;
            buttonBottom.Enabled = enabled;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // TODO: Graceful exit.
            Application.Exit();
        }

        private void dumpJsonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dictionary<string, List<Tuple<string, string>>> duplicatesGroups = accessor.GetDuplicatesGroups();
            if (duplicatesGroups != null)
            {
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(duplicatesGroups);
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.FileName = "dump-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".json";
                sfd.Filter = "JSON files (*.json)|*.json";
                sfd.Title = "Save JSON";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(sfd.FileName, false, Encoding.UTF8))
                    {
                        sw.Write(json);
                    }
                }
            }
            else
            {
                MessageBox.Show("Scanning is not completed.");
            }
        }
    }
}
