using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using OneNoteDuplicatesRemover.Core;

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

        // The tree's check boxes hold the selection. Page nodes are named by page ID.
        private HashSet<string> GetSelectedPageIds()
        {
            HashSet<string> pageIds = new HashSet<string>();
            foreach (TreeNode group in treeViewHierarchy.Nodes)
            {
                foreach (TreeNode page in group.Nodes)
                {
                    if (page.Checked) pageIds.Add(page.Name);
                }
            }
            return pageIds;
        }

        private void ApplySelection(ISet<string> pageIds)
        {
            foreach (TreeNode group in treeViewHierarchy.Nodes)
            {
                foreach (TreeNode page in group.Nodes)
                {
                    page.Checked = pageIds.Contains(page.Name);
                }
            }
        }

        private void ShowLocationPreference(int selectedIndex)
        {
            listBoxPathPreference.BeginUpdate();
            listBoxPathPreference.Items.Clear();
            listBoxPathPreference.Items.AddRange(locationPreference.Locations.ToArray());
            listBoxPathPreference.SelectedIndex = selectedIndex;
            listBoxPathPreference.EndUpdate();
        }

        private void UpdateSelectionSummary()
        {
            HashSet<string> selectedPageIds = GetSelectedPageIds();
            int pageCount = duplicateGroups.Sum(group => group.Pages.Count);
            int selectedCount = selectedPageIds.Count;
            bool hasEntireGroupSelected = KeepPolicy.FindGroupWithoutKeptCopy(duplicateGroups, selectedPageIds) != null;

            labelGroupCount.Text = duplicateGroups.Count.ToString("N0");
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
