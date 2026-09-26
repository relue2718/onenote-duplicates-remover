using System;
using System.Linq;
using System.Windows.Forms;

namespace OneNoteDuplicatesRemover
{
    public partial class FormMain
    {
        private bool uiEnabled;
        private bool updatingSelection;
        private int scanFailureCount;

        private void ShowEmptyState(string title, string description)
        {
            labelEmptyTitle.Text = title;
            labelEmptyDescription.Text = description;
            emptyState.Visible = treeViewHierarchy.Nodes.Count == 0;
            treeViewHierarchy.Visible = treeViewHierarchy.Nodes.Count > 0;
        }

        private void treeViewHierarchy_BeforeCheck(object sender, TreeViewCancelEventArgs e)
        {
            // Group nodes are headings, including when Space is pressed on the keyboard.
            e.Cancel = e.Node.Parent == null;
            if (e.Cancel && treeViewHierarchy.IsHandleCreated)
                BeginInvoke((MethodInvoker)(() =>
                {
                    if (!IsDisposed && e.Node.TreeView == treeViewHierarchy)
                        TreeViewHelper.HideCheckBox(treeViewHierarchy, e.Node);
                }));
        }

        private void treeViewHierarchy_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (!updatingSelection) UpdateSelectionSummary();
        }

        private void UpdateSelectionSummary()
        {
            int pageCount = 0;
            int selectedCount = 0;
            bool hasEntireGroupSelected = false;
            foreach (TreeNode group in treeViewHierarchy.Nodes)
            {
                int checkedCount = group.Nodes.Cast<TreeNode>().Count(node => node.Checked);
                pageCount += group.Nodes.Count;
                selectedCount += checkedCount;
                hasEntireGroupSelected |= group.Nodes.Count > 0 && checkedCount == group.Nodes.Count;
            }

            labelGroupCount.Text = treeViewHierarchy.Nodes.Count.ToString("N0");
            labelPageCount.Text = pageCount.ToString("N0");
            labelSelectedCount.Text = selectedCount.ToString("N0");
            buttonSelectAllExceptOne.Enabled = uiEnabled && pageCount > 0;
            buttonDeselectAll.Enabled = uiEnabled && selectedCount > 0;
            buttonRemoveSelectedPages.Enabled = uiEnabled && selectedCount > 0 && !hasEntireGroupSelected;
            dumpJsonToolStripMenuItem.Enabled = uiEnabled && pageCount > 0;
            labelSelectionHint.ForeColor = hasEntireGroupSelected ? AppTheme.Danger : AppTheme.Muted;
            labelSelectionHint.Text = hasEntireGroupSelected
                ? "Keep at least one copy in every group before removing pages."
                : selectedCount > 0
                    ? $"{selectedCount:N0} selected · {pageCount - selectedCount:N0} kept in these groups"
                    : "Select copies to remove after scanning.";
            emptyState.Visible = treeViewHierarchy.Nodes.Count == 0;
            UpdatePreferenceButtons();
            treeViewHierarchy.Visible = treeViewHierarchy.Nodes.Count > 0;
        }

        private void UpdatePreferenceButtons()
        {
            int index = listBoxPathPreference.SelectedIndex;
            bool canMove = uiEnabled && index >= 0;
            buttonTop.Enabled = buttonUp.Enabled = canMove && index > 0;
            buttonDown.Enabled = buttonBottom.Enabled = canMove && index < listBoxPathPreference.Items.Count - 1;
        }
    }
}
