using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Threading;

namespace OneNoteDuplicatesRemover
{
    public partial class FormMain : Form
    {
        private OneNoteAccessor accessor = null;
        CancellationTokenSource cancellationTokenSource = null;
        private bool isScanningPages = false;
        private bool isRemovingPages = false;
        private bool isFlatteningPages = false;

        private void UpdateProgressBar(int pbCurrent, int pbMaximum)
        {
            toolStripProgressBarScan.Maximum = pbMaximum;
            toolStripProgressBarScan.Value = pbCurrent;
        }

        private void UpdateProgressScanPages(Tuple<int, int, int, string> details)
        {
            Invoke((MethodInvoker)(() =>
            {
                if (isScanningPages && cancellationTokenSource.Token.IsCancellationRequested == false)
                {
                    if (details.Item2 > 0)
                    {
                        toolStripStatusLabelScan.Text = string.Format("Scanning... {0}/{1} (Failure: {2}) -- {3}", details.Item1, details.Item3, details.Item2, details.Item4);
                    }
                    else
                    {
                        toolStripStatusLabelScan.Text = string.Format("Scanning... {0}/{1} -- {3}", details.Item1, details.Item3, details.Item2, details.Item4);
                    }
                    UpdateProgressBar(details.Item1, details.Item3);
                }
            }));
        }

        private void UpdateProgressRemovePages(Tuple<int, int, int, string> details)
        {
            Invoke((MethodInvoker)(() =>
            {
                if (isRemovingPages && cancellationTokenSource.Token.IsCancellationRequested == false)
                {
                    if (details.Item2 > 0)
                    {
                        toolStripStatusLabelScan.Text = string.Format("Removing... {0}/{1} (Failure: {2}) -- {3}", details.Item1, details.Item3, details.Item2, details.Item4);
                    }
                    else
                    {
                        toolStripStatusLabelScan.Text = string.Format("Removing... {0}/{1} -- {3}", details.Item1, details.Item3, details.Item2, details.Item4);
                    }
                    UpdateProgressBar(details.Item1, details.Item3);
                }
            }));
        }

        private void UpdateProgresFlattenSections(Tuple<int, int, int, string> details)
        {
            Invoke((MethodInvoker)(() =>
            {
                if (isFlatteningPages && cancellationTokenSource.Token.IsCancellationRequested == false)
                {
                    if (details.Item2 > 0)
                    {
                        toolStripStatusLabelScan.Text = string.Format("Flattening... {0}/{1} (Failure: {2}) -- {3}", details.Item1, details.Item3, details.Item2, details.Item4);
                    }
                    else
                    {
                        toolStripStatusLabelScan.Text = string.Format("Flattening... {0}/{1} -- {3}", details.Item1, details.Item3, details.Item2, details.Item4);
                    }
                    UpdateProgressBar(details.Item1, details.Item3);
                }
            }));
        }

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            try
            {
                string timestampNow = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-FFF");
                etc.FileLogger.Instance.Init("log-" + timestampNow + ".log");
                etc.LoggerHelper.EventCategoryCountChanged += LoggerHelper_EventCategoryCountChanged;
                advancedToolStripMenuItem.Visible = false; // Hide advanced features by default
                accessor = new OneNoteAccessor();
                accessor.OnCancelled += Accessor_OnCancelled;
                var retInit = accessor.InitializeOneNoteWrapper();
                if (retInit.Item1 == false)
                {
                    etc.LoggerHelper.LogError(retInit.Item2);
                    SetUIControlEnabled(false);
                    toolStripStatusLabelScan.Text = "Fatal Error";
                }
                else
                {
                    SetUIControlEnabled(true);
                    toolStripStatusLabelScan.Text = "Ready";
                }
            }
            catch (System.Exception exception)
            {
                etc.LoggerHelper.LogUnexpectedException(exception);
            }
        }

        private void Accessor_OnCancelled()
        {
        }

        private void LoggerHelper_EventCategoryCountChanged(int countInfo, int countWarning, int countError, int countException)
        {
            // Delegate a task to the main UI thread
            Invoke((MethodInvoker)(() =>
            {
                labelMessageCounts.Text = string.Format("[Log] Info: {0}, Warning: {1}, Error: {2}, Exception: {3}", countInfo, countWarning, countError, countException);
            }));
        }

        private void buttonUp_Click(object sender, EventArgs e)
        {
            try
            {
                int selectedIndex = listBoxPathPreference.SelectedIndex;
                if (selectedIndex > 0 && selectedIndex != -1)
                {
                    listBoxPathPreference.Items.Insert(selectedIndex - 1, listBoxPathPreference.Items[selectedIndex]);
                    listBoxPathPreference.Items.RemoveAt(selectedIndex + 1);
                    listBoxPathPreference.SelectedIndex = selectedIndex - 1;
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
                int selectedIndex = listBoxPathPreference.SelectedIndex;
                if (selectedIndex < listBoxPathPreference.Items.Count - 1 && selectedIndex != -1)
                {
                    listBoxPathPreference.Items.Insert(selectedIndex + 2, listBoxPathPreference.Items[selectedIndex]);
                    listBoxPathPreference.Items.RemoveAt(selectedIndex);
                    listBoxPathPreference.SelectedIndex = selectedIndex + 1;
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
                int selectedIndex = listBoxPathPreference.SelectedIndex;
                if (selectedIndex <= listBoxPathPreference.Items.Count - 1 && selectedIndex != -1)
                {
                    listBoxPathPreference.Items.Insert(0, listBoxPathPreference.Items[selectedIndex]);
                    listBoxPathPreference.Items.RemoveAt(selectedIndex + 1);
                    listBoxPathPreference.SelectedIndex = 0;
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
                int selectedIndex = listBoxPathPreference.SelectedIndex;
                if (selectedIndex <= listBoxPathPreference.Items.Count - 1 && selectedIndex != -1)
                {
                    listBoxPathPreference.Items.Insert(listBoxPathPreference.Items.Count - 1, listBoxPathPreference.Items[selectedIndex]);
                    listBoxPathPreference.Items.RemoveAt(selectedIndex);
                    listBoxPathPreference.SelectedIndex = listBoxPathPreference.Items.Count - 1;
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
                    string highlightedPageId = e.Node.Name;
                    if (e.Node.Tag != null) // Make sure the selected item is a page
                    {
                        accessor.TryNavigate(highlightedPageId);
                    }
                }
            }
            catch (System.Exception exception)
            {
                etc.LoggerHelper.LogUnexpectedException(exception);
            }
        }

        private void buttonSelectAllExceptOne_Click(object sender, EventArgs e)
        {
            try
            {
                List<string> preference = new List<string>(listBoxPathPreference.Items.Cast<string>()); // Select all except one

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
                        int where = preference.IndexOf(sectionDir);
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

        private void SetUIControlEnabled(bool enabled)
        {
            buttonScanDuplicatedPages.Enabled = enabled;
            buttonSelectAllExceptOne.Enabled = enabled;
            buttonDeselectAll.Enabled = enabled;
            buttonRemoveSelectedPages.Enabled = enabled;
            checkBoxNavigateAutomatically.Enabled = enabled;
            treeViewHierarchy.Enabled = enabled;
            listBoxPathPreference.Enabled = enabled;
            buttonTop.Enabled = enabled;
            buttonUp.Enabled = enabled;
            buttonDown.Enabled = enabled;
            buttonBottom.Enabled = enabled;
            cleanUpUsingJSONToolStripMenuItem.Enabled = enabled;
            flattenSectionsToolStripMenuItem.Enabled = enabled;
            dumpJsonToolStripMenuItem.Enabled = enabled;
            buttonCancel.Enabled = !enabled;
            buttonCancel.Visible = !enabled;
        }

        private void UpdateUIFromResultScanPages(Dictionary<string /* innerTextHash */, List<Tuple<string, string>> /* Page Id List */ > duplicatesGroups, List<string> sectionPathList)
        {
            ResetUIResultScanPages();
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
                        if (accessor.TryGetSectionPath(pageId, out string sectionPath))
                        {
                            groupTreeNode.Nodes.Add(pageId, sectionPath + " - " + groupInfo.Value[i].Item2).Tag = sectionPath;
                        }
                    }
                }
            }
            treeViewHierarchy.ExpandAll();
            foreach (string sectionPath in sectionPathList)
            {
                listBoxPathPreference.Items.Add(sectionPath);
            }
        }

        private void ResetUIResultScanPages()
        {
            treeViewHierarchy.Nodes.Clear();
            listBoxPathPreference.Items.Clear();
        }

        private async void buttonScanDuplicatedPages_Click(object sender, EventArgs e)
        {
            SetUIControlEnabled(false);
            ResetUIResultScanPages();

            isScanningPages = true;
            cancellationTokenSource = new CancellationTokenSource();
            var resultScanOneNotePages = await Task.Run(() => { return accessor.ScanOneNotePages(new Progress<Tuple<int, int, int, string>>(progress => UpdateProgressScanPages(progress)), cancellationTokenSource.Token); }, cancellationTokenSource.Token);
            isScanningPages = false;

            if (resultScanOneNotePages.Item1 == false)
            {
                etc.LoggerHelper.LogError(resultScanOneNotePages.Item2);

                toolStripStatusLabelScan.Text = "Fatal Error";
                UpdateProgressBar(0, 100);
            }
            else
            {
                Dictionary<string, List<Tuple<string, string>>> duplicatesGroups = accessor.GetDuplicatesGroups();
                List<string> sectionPathList = accessor.GetSectionPathList(duplicatesGroups);
                UpdateUIFromResultScanPages(duplicatesGroups, sectionPathList);

                toolStripStatusLabelScan.Text = "Scan Completed";
                UpdateProgressBar(100, 100);
            }

            SetUIControlEnabled(true);
        }

        private async void buttonRemoveSelectedPages_Click(object sender, EventArgs e)
        {
            SetUIControlEnabled(false);

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
                List<Tuple<string, string>> pagesBeingRemoved = PrepareRemovalOperation();
                if (pagesBeingRemoved.Count > 0)
                {
                    if (MessageBox.Show("Are you sure to remove the selected pages?\r\n" + string.Format("The number of the selected pages: {0}", pagesBeingRemoved.Count) + "\r\n\r\nPlease **BACKUP** OneNote notebooks!", "Confirm", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                    {
                        isRemovingPages = true;
                        cancellationTokenSource = new CancellationTokenSource();
                        List<Tuple<string, string, bool>> resultRemovePages = await Task.Run(() =>
                           {
                               return accessor.RemovePages(pagesBeingRemoved, new Progress<Tuple<int, int, int, string>>(progress => UpdateProgressRemovePages(progress)), cancellationTokenSource.Token);
                           }, cancellationTokenSource.Token);
                        isRemovingPages = false;

                        HtmlReportGenerator report = new HtmlReportGenerator();
                        string generatedHtmlFile;
                        report.GenerateReportForRemovalOperation(resultRemovePages, out generatedHtmlFile);
                        System.Diagnostics.Process.Start(generatedHtmlFile);

                        ResetUIResultScanPages();
                        toolStripStatusLabelScan.Text = "Remove Completed";
                        UpdateProgressBar(100, 100);
                        SetUIControlEnabled(true);
                    }
                    else
                    {
                        SetUIControlEnabled(true);
                    }
                }
                else
                {
                    SetUIControlEnabled(true);
                }
            }
        }

        private List<Tuple<string, string>> PrepareRemovalOperation()
        {
            List<Tuple<string, string>> ret = new List<Tuple<string, string>>();
            foreach (TreeNode treeNode in treeViewHierarchy.Nodes)
            {
                foreach (TreeNode childNode in treeNode.Nodes)
                {
                    if (childNode.Checked)
                    {
                        ret.Add(Tuple.Create(childNode.Name, childNode.Text));
                    }
                }
            }
            return ret;
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

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TossCancellationToken();
            Application.Exit();
        }

        private void dumpJsonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dictionary<string, List<Tuple<string, string>>> duplicatesGroups = accessor.GetDuplicatesGroups();
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

        private async void cleanUpUsingJSONToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetUIControlEnabled(false);
            Dictionary<string, List<Tuple<string, string>>> duplicatesGroups = accessor.GetDuplicatesGroups();
            if (duplicatesGroups != null)
            {
                string warningMessage = "** DANGEROUS FEATURE **" + "\r\n\r\n" +
                "It removes pages that are found in the JSON file." + "\r\n" +
                "Please make sure the original notebook is closed in order to prevent data loss." + "\r\n\r\n" +
                "Do you really want to continue?";
                if (MessageBox.Show(warningMessage, "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    OpenFileDialog ofd = new OpenFileDialog();
                    ofd.Filter = "JSON files (*.json)|*.json";
                    ofd.Title = "Save JSON";
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        string jsonText = "";
                        using (System.IO.StreamReader sr = new System.IO.StreamReader(ofd.FileName, Encoding.UTF8, true))
                        {
                            jsonText = sr.ReadToEnd();
                        }
                        Dictionary<string, List<Tuple<string, string>>> archivedPages = new Dictionary<string, List<Tuple<string, string>>>();
                        archivedPages = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonText, archivedPages.GetType()) as Dictionary<string, List<Tuple<string, string>>>;
                        HashSet<string> knownHashes = new HashSet<string>(archivedPages.Keys);

                        List<Tuple<string, string>> pagesBeingRemoved = new List<Tuple<string, string>>();
                        foreach (KeyValuePair<string, List<Tuple<string, string>>> groupInfo in duplicatesGroups)
                        {
                            string sha256sum = groupInfo.Key;
                            if (knownHashes.Contains(sha256sum))
                            {
                                foreach (Tuple<String, String> pageInfo in groupInfo.Value)
                                {
                                    pagesBeingRemoved.Add(Tuple.Create(pageInfo.Item1, pageInfo.Item2));
                                }
                            }
                        }

                        isRemovingPages = true;
                        cancellationTokenSource = new CancellationTokenSource();
                        List<Tuple<string, string, bool>> resultRemovePages = await Task.Run(() =>
                        {
                            return accessor.RemovePages(pagesBeingRemoved, new Progress<Tuple<int, int, int, string>>(progress => UpdateProgressRemovePages(progress)), cancellationTokenSource.Token);
                        }, cancellationTokenSource.Token);
                        isRemovingPages = false;

                        HtmlReportGenerator report = new HtmlReportGenerator();
                        string generatedHtmlFile;
                        report.GenerateReportForRemovalOperation(resultRemovePages, out generatedHtmlFile);
                        System.Diagnostics.Process.Start(generatedHtmlFile);

                        ResetUIResultScanPages();
                        toolStripStatusLabelScan.Text = "Remove Completed";
                        UpdateProgressBar(100, 100);
                    }
                }
            }
            SetUIControlEnabled(true);
        }

        private async void flattenSectionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResetUIResultScanPages();
            SetUIControlEnabled(false);
            isFlatteningPages = true;
            cancellationTokenSource = new CancellationTokenSource();
            var _ = await Task.Run(() => { return accessor.TryFlattenSections("MERGED_ONE", new Progress<Tuple<int, int, int, string>>(progress => UpdateProgresFlattenSections(progress)), cancellationTokenSource.Token); }, cancellationTokenSource.Token);
            isFlatteningPages = false;
            toolStripStatusLabelScan.Text = "Flatten Completed";
            UpdateProgressBar(0, 100);
            SetUIControlEnabled(true);
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            TossCancellationToken();
        }

        private void TossCancellationToken()
        {
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
            }
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            TossCancellationToken();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormAbout formAbout = new FormAbout(accessor);
            formAbout.ShowDialog(); // DialogResult is not used in this context.
        }
    }
}
