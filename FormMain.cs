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
            toolStripProgressBarScan.Maximum = Math.Max(1, pbMaximum);
            toolStripProgressBarScan.Value = Math.Clamp(pbCurrent, 0, toolStripProgressBarScan.Maximum);
        }

        private void UpdateProgressScanPages(Tuple<int, int, int, string> details)
        {
            Invoke((MethodInvoker)(() =>
            {
                if (isScanningPages && cancellationTokenSource.Token.IsCancellationRequested == false)
                {
                    scanFailureCount = details.Item2;
                    if (details.Item2 > 0)
                    {
                        toolStripStatusLabelScan.Text = string.Format("Scanning... {0}/{1} (Failure: {2}) -- {3}", details.Item1, details.Item3, details.Item2, details.Item4);
                    }
                    else
                    {
                        toolStripStatusLabelScan.Text = string.Format("Scanning... {0}/{1} -- {3}", details.Item1, details.Item3, details.Item2, details.Item4);
                    }
                    UpdateProgressBar(details.Item1 + details.Item2, details.Item3);
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
                    toolStripStatusLabelScan.Text = "Unable to connect to OneNote";
                    ShowEmptyState("OneNote is unavailable", "Open the OneNote desktop app, then restart this tool.");
                }
                else
                {
                    SetUIControlEnabled(true);
                    toolStripStatusLabelScan.Text = "Ready to scan";
                }
            }
            catch (System.Exception exception)
            {
                etc.LoggerHelper.LogUnexpectedException(exception);
                SetUIControlEnabled(false);
                toolStripStatusLabelScan.Text = "Unable to connect to OneNote";
                ShowEmptyState("OneNote is unavailable", "Open the OneNote desktop app, then restart this tool.");
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
                labelMessageCounts.Text = string.Format("{0} warnings · {1} errors", countWarning, countError + countException);
                labelMessageCounts.ToolTipText = string.Format("Log: {0} info, {1} warnings, {2} errors, {3} exceptions", countInfo, countWarning, countError, countException);
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
                    object selectedItem = listBoxPathPreference.Items[selectedIndex];
                    listBoxPathPreference.Items.RemoveAt(selectedIndex);
                    listBoxPathPreference.Items.Add(selectedItem);
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
            updatingSelection = true;
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
            finally
            {
                updatingSelection = false;
                UpdateSelectionSummary();
            }
        }

        private void buttonDeselectAll_Click(object sender, EventArgs e)
        {
            updatingSelection = true;
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
            finally
            {
                updatingSelection = false;
                UpdateSelectionSummary();
            }
        }

        private void SetUIControlEnabled(bool enabled)
        {
            uiEnabled = enabled;
            buttonScanDuplicatedPages.Enabled = enabled;
            checkBoxNavigateAutomatically.Enabled = enabled;
            treeViewHierarchy.Enabled = enabled;
            listBoxPathPreference.Enabled = enabled;
            cleanUpUsingJSONToolStripMenuItem.Enabled = enabled;
            flattenSectionsToolStripMenuItem.Enabled = enabled;
            exportSectionDataToXml.Enabled = enabled;
            exportpagesDataToXMLToolStripMenuItem.Enabled = enabled;
            buttonCancel.Enabled = !enabled;
            buttonCancel.Visible = !enabled && (isScanningPages || isRemovingPages || isFlatteningPages);
            buttonCancel.Text = "&Cancel";
            UpdateSelectionSummary();
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
                    string title = groupInfo.Key == "E3B0C44298FC1C149AFBF4C8996FB92427AE41E4649B934CA495991B7852B855"
                        ? "Empty page" : groupInfo.Value[0].Item2;
                    if (string.IsNullOrWhiteSpace(title)) title = "Untitled page";
                    TreeNode groupTreeNode = treeViewHierarchy.Nodes.Add(groupInfo.Key, string.Format("{0}. {1} · {2} copies", duplicatesGroupIndex, title, groupInfo.Value.Count));
                    groupTreeNode.NodeFont = AppTheme.SectionFont;
                    groupTreeNode.ToolTipText = "Content hash: " + groupInfo.Key;
                    TreeViewHelper.HideCheckBox(treeViewHierarchy, groupTreeNode);
                    for (int i = 0; i < groupInfo.Value.Count; ++i)
                    {
                        string pageId = groupInfo.Value[i].Item1;
                        if (accessor.TryGetSectionPath(pageId, out string sectionPath))
                        {
                            TreeNode pageNode = groupTreeNode.Nodes.Add(pageId, groupInfo.Value[i].Item2 + " — " + sectionPath);
                            pageNode.Tag = sectionPath;
                            pageNode.ToolTipText = groupInfo.Value[i].Item2 + Environment.NewLine + sectionPath;
                        }
                    }
                }
            }
            treeViewHierarchy.ExpandAll();
            if (treeViewHierarchy.Nodes.Count > 0) treeViewHierarchy.TopNode = treeViewHierarchy.Nodes[0];
            foreach (string sectionPath in sectionPathList)
            {
                listBoxPathPreference.Items.Add(sectionPath);
            }
            if (listBoxPathPreference.Items.Count > 0) listBoxPathPreference.SelectedIndex = 0;
            UpdateSelectionSummary();
        }

        private void ResetUIResultScanPages()
        {
            treeViewHierarchy.Nodes.Clear();
            listBoxPathPreference.Items.Clear();
            ShowEmptyState("Ready for another scan", "Scan your notebooks to refresh the results.");
            UpdateSelectionSummary();
        }

        private async void buttonScanDuplicatedPages_Click(object sender, EventArgs e)
        {
            isScanningPages = true;
            SetUIControlEnabled(false);
            ResetUIResultScanPages();
            scanFailureCount = 0;
            ShowEmptyState("Looking for matching pages…", "You can review the results when the scan finishes.");
            toolStripStatusLabelScan.Text = "Reading notebooks…";
            UpdateProgressBar(0, 100);
            cancellationTokenSource = new CancellationTokenSource();
            try
            {
                var progress = new Progress<Tuple<int, int, int, string>>(UpdateProgressScanPages);
                var result = await Task.Run(() => accessor.ScanOneNotePages(progress, cancellationTokenSource.Token), cancellationTokenSource.Token);
                cancellationTokenSource.Token.ThrowIfCancellationRequested();
                if (!result.Item1)
                {
                    etc.LoggerHelper.LogError(result.Item2);
                    toolStripStatusLabelScan.Text = "Scan failed";
                    ShowEmptyState("The scan could not finish", "Check that OneNote is open, then try again.\nSee the log for details.");
                    UpdateProgressBar(0, 100);
                }
                else
                {
                    var duplicatesGroups = accessor.GetDuplicatesGroups();
                    UpdateUIFromResultScanPages(duplicatesGroups, accessor.GetSectionPathList(duplicatesGroups));
                    toolStripStatusLabelScan.Text = scanFailureCount > 0
                        ? $"Scan finished · {scanFailureCount:N0} pages could not be read"
                        : "Scan complete";
                    ShowEmptyState(scanFailureCount > 0 ? "Some pages could not be checked" : "No duplicate pages found",
                        scanFailureCount > 0 ? "No matches among the pages checked.\nSee the log and try scanning again." : "No matching pages were found in this scan.");
                    UpdateProgressBar(100, 100);
                }
            }
            catch (OperationCanceledException)
            {
                toolStripStatusLabelScan.Text = "Scan cancelled";
                ShowEmptyState("Scan cancelled", "Start a new scan when you are ready.");
                UpdateProgressBar(0, 100);
            }
            catch (Exception exception)
            {
                etc.LoggerHelper.LogUnexpectedException(exception);
                toolStripStatusLabelScan.Text = "Scan failed";
                ShowEmptyState("The scan could not finish", "Check that OneNote is open, then try again.\nSee the log for details.");
                UpdateProgressBar(0, 100);
            }
            finally
            {
                isScanningPages = false;
                cancellationTokenSource.Dispose();
                cancellationTokenSource = null;
                SetUIControlEnabled(true);
            }
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
                        SetUIControlEnabled(false);
                        toolStripStatusLabelScan.Text = "Removing selected pages…";
                        cancellationTokenSource = new CancellationTokenSource();
                        List<Tuple<string, string, bool>> resultRemovePages = await Task.Run(() =>
                           {
                               return accessor.RemovePages(pagesBeingRemoved, new Progress<Tuple<int, int, int, string>>(progress => UpdateProgressRemovePages(progress)), cancellationTokenSource.Token);
                           }, cancellationTokenSource.Token);
                        isRemovingPages = false;

                        HtmlReportGenerator report = new HtmlReportGenerator();
                        string generatedHtmlFile;
                        report.GenerateReportForRemovalOperation(resultRemovePages, out generatedHtmlFile);
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                        {
                            FileName = generatedHtmlFile,
                            UseShellExecute = true
                        });

                        ResetUIResultScanPages();
                        toolStripStatusLabelScan.Text = cancellationTokenSource.IsCancellationRequested ? "Removal cancelled · see report" : "Removal finished · see report";
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
            string json = System.Text.Json.JsonSerializer.Serialize(duplicatesGroups);
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
                        Dictionary<string, List<Tuple<string, string>>> archivedPages = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, List<Tuple<string, string>>>>(jsonText);
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
                        SetUIControlEnabled(false);
                        cancellationTokenSource = new CancellationTokenSource();
                        List<Tuple<string, string, bool>> resultRemovePages = await Task.Run(() =>
                        {
                            return accessor.RemovePages(pagesBeingRemoved, new Progress<Tuple<int, int, int, string>>(progress => UpdateProgressRemovePages(progress)), cancellationTokenSource.Token);
                        }, cancellationTokenSource.Token);
                        isRemovingPages = false;

                        HtmlReportGenerator report = new HtmlReportGenerator();
                        string generatedHtmlFile;
                        report.GenerateReportForRemovalOperation(resultRemovePages, out generatedHtmlFile);
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                        {
                            FileName = generatedHtmlFile,
                            UseShellExecute = true
                        });

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
            isFlatteningPages = true;
            SetUIControlEnabled(false);
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
            buttonCancel.Enabled = false;
            buttonCancel.Text = "Cancelling…";
            toolStripStatusLabelScan.Text = "Cancelling after the current page…";
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
            using (FormAbout formAbout = new FormAbout(accessor))
            {
                formAbout.ShowDialog(this);
            }
        }

        private void exportSectionDataToXml_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                FileName = "dump-sections-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".xml",
                Filter = "XML files (*.xml)|*.xml",
                Title = "Save XML"
            };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                if (accessor.TryExportSectionHierarchyAsXML(sfd.FileName))
                {
                    MessageBox.Show("Exported successfully!", "Export Sections Hierarchy", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Failed to export!", "Export Sections Hierarchy", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void exportPagesDataToXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                FileName = "dump-pages-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".xml",
                Filter = "XML files (*.xml)|*.xml",
                Title = "Save XML"
            };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                if (accessor.TryExportPageHierarchyAsXML(sfd.FileName))
                {
                    MessageBox.Show("Exported successfully!", "Export Pages Hierarchy", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Failed to export!", "Export Pages Hierarchy", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
