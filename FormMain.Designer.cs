using System.Drawing;
using System.Windows.Forms;

namespace OneNoteDuplicatesRemover
{
    partial class FormMain
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                etc.LoggerHelper.EventCategoryCountChanged -= LoggerHelper_EventCategoryCountChanged;
                accessor?.Dispose();
                components?.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            Font = AppTheme.BodyFont;
            BackColor = AppTheme.Background;
            ForeColor = AppTheme.Text;
            ClientSize = new Size(1180, 800);
            MinimumSize = new Size(980, 720);
            StartPosition = FormStartPosition.CenterScreen;
            Name = "FormMain";
            Text = "OneNote Duplicates Remover";

            InitializeMenus();

            var page = AppTheme.Stack();
            page.Padding = new Padding(28, 16, 28, 16);
            page.RowCount = 4;
            page.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            page.RowStyles.Add(new RowStyle(SizeType.Absolute, 104));
            page.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            page.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var header = new TableLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, ColumnCount = 2, Margin = new Padding(0, 0, 0, 20) };
            header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            header.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            var title = AppTheme.Stack();
            title.AutoSize = true;
            title.Controls.Add(AppTheme.Label("NOTEBOOK CLEANUP", color: AppTheme.Accent));
            title.Controls.Add(AppTheme.Label("OneNote Duplicates Remover", AppTheme.HeadingFont));
            title.Controls.Add(AppTheme.Label("Find matching pages. Keep the copies that matter.", color: AppTheme.Muted));
            header.Controls.Add(title, 0, 0);
            var scanActions = AppTheme.Actions();
            scanActions.Anchor = AnchorStyles.Right;
            scanActions.Dock = DockStyle.None;
            scanActions.WrapContents = false;
            buttonScanDuplicatedPages = AppTheme.Button("&Scan notebooks", primary: true);
            buttonScanDuplicatedPages.Click += buttonScanDuplicatedPages_Click;
            buttonCancel = AppTheme.Button("&Cancel");
            buttonCancel.Visible = false;
            buttonCancel.Click += buttonCancel_Click;
            scanActions.Controls.AddRange(new Control[] { buttonScanDuplicatedPages, buttonCancel });
            header.Controls.Add(scanActions, 1, 0);
            page.Controls.Add(header, 0, 0);

            var summary = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, Margin = new Padding(0, 0, 0, 20) };
            for (int i = 0; i < 3; i++) summary.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / 3));
            summary.Controls.Add(CreateSummaryCard("DUPLICATE GROUPS", out labelGroupCount), 0, 0);
            summary.Controls.Add(CreateSummaryCard("PAGES IN GROUPS", out labelPageCount), 1, 0);
            summary.Controls.Add(CreateSummaryCard("SELECTED FOR REMOVAL", out labelSelectedCount), 2, 0);
            summary.Controls[0].Margin = new Padding(0, 0, 12, 0);
            summary.Controls[1].Margin = new Padding(0, 0, 12, 0);
            page.Controls.Add(summary, 0, 1);

            var workspace = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Size = new Size(1124, 420),
                SplitterDistance = 752,
                SplitterWidth = 16,
                FixedPanel = FixedPanel.Panel2,
                BackColor = AppTheme.Background,
                Margin = Padding.Empty,
                TabStop = false
            };
            workspace.Panel1MinSize = 470;
            workspace.Panel2MinSize = 280;
            workspace.Panel1.Controls.Add(CreateResultsCard());
            workspace.Panel2.Controls.Add(CreatePreferencesCard());
            page.Controls.Add(workspace, 0, 2);

            var footer = new TableLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, ColumnCount = 2, Margin = new Padding(0, 16, 0, 0) };
            footer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            footer.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            labelSelectionHint = AppTheme.Label("Select copies to remove after scanning.", color: AppTheme.Muted);
            labelSelectionHint.Dock = DockStyle.Fill;
            labelSelectionHint.TextAlign = ContentAlignment.MiddleLeft;
            labelSelectionHint.Margin = new Padding(0, 0, 16, 0);
            buttonRemoveSelectedPages = AppTheme.Button("&Remove selected", destructive: true);
            buttonRemoveSelectedPages.Margin = Padding.Empty;
            buttonRemoveSelectedPages.Click += buttonRemoveSelectedPages_Click;
            footer.Controls.Add(labelSelectionHint, 0, 0);
            footer.Controls.Add(buttonRemoveSelectedPages, 1, 0);
            page.Controls.Add(footer, 0, 3);

            statusStrip1 = new StatusStrip { BackColor = AppTheme.Surface, Padding = new Padding(28, 6, 20, 6), Font = AppTheme.BodyFont };
            toolStripProgressBarScan = new ToolStripProgressBar { Size = new Size(140, 12), Margin = new Padding(0, 0, 16, 0) };
            toolStripStatusLabelScan = new ToolStripStatusLabel("Connecting to OneNote…") { Spring = true, TextAlign = ContentAlignment.MiddleLeft };
            labelMessageCounts = new ToolStripStatusLabel { ForeColor = AppTheme.Muted };
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripProgressBarScan, toolStripStatusLabelScan, labelMessageCounts });
            Controls.Add(page);
            Controls.Add(statusStrip1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            FormClosing += FormMain_FormClosing;
            Load += FormMain_Load;
            SetUIControlEnabled(false);
            ResumeLayout(true);
        }

        private Panel CreateSummaryCard(string caption, out Label value)
        {
            var card = AppTheme.Card();
            card.Padding = new Padding(18, 10, 18, 8);
            var stack = AppTheme.Stack();
            var label = AppTheme.Label(caption, color: AppTheme.Muted);
            label.Margin = Padding.Empty;
            value = AppTheme.Label("0", AppTheme.NumberFont, AppTheme.Accent);
            value.Margin = Padding.Empty;
            stack.Controls.Add(label);
            stack.Controls.Add(value);
            card.Controls.Add(stack);
            return card;
        }

        private Panel CreateResultsCard()
        {
            var card = AppTheme.Card();
            var layout = AppTheme.Stack();
            layout.RowCount = 5;
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.Controls.Add(AppTheme.Label("Duplicate pages", AppTheme.SectionFont), 0, 0);
            layout.Controls.Add(AppTheme.Label("Review each group and keep at least one copy.", color: AppTheme.Muted), 0, 1);
            var actions = AppTheme.Actions();
            actions.Margin = new Padding(0, 4, 0, 12);
            buttonSelectAllExceptOne = AppTheme.Button("Select &extra copies");
            buttonSelectAllExceptOne.Click += buttonSelectAllExceptOne_Click;
            buttonDeselectAll = AppTheme.Button("&Clear selection");
            buttonDeselectAll.Click += buttonDeselectAll_Click;
            actions.Controls.AddRange(new Control[] { buttonSelectAllExceptOne, buttonDeselectAll });
            layout.Controls.Add(actions, 0, 2);

            var results = new Panel { Dock = DockStyle.Fill, Margin = Padding.Empty };
            treeViewHierarchy = new etc.MyTreeView
            {
                Name = "treeViewHierarchy",
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                BackColor = AppTheme.Surface,
                ForeColor = AppTheme.Text,
                CheckBoxes = true,
                HideSelection = false,
                ShowLines = false,
                ShowNodeToolTips = true,
                ItemHeight = 32,
                Indent = 24,
                AccessibleName = "Duplicate page groups"
            };
            treeViewHierarchy.BeforeSelect += treeViewHierarchy_BeforeSelect;
            treeViewHierarchy.BeforeCheck += treeViewHierarchy_BeforeCheck;
            treeViewHierarchy.AfterCheck += treeViewHierarchy_AfterCheck;
            emptyState = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 4, BackColor = AppTheme.Surface };
            emptyState.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            emptyState.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            emptyState.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            emptyState.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            emptyState.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            labelEmptyTitle = AppTheme.Label("A little less clutter", AppTheme.SectionFont);
            labelEmptyTitle.Anchor = AnchorStyles.None;
            labelEmptyDescription = AppTheme.Label("Scan your notebooks to find duplicate pages.\nYou choose what to remove.", color: AppTheme.Muted);
            labelEmptyDescription.TextAlign = ContentAlignment.MiddleCenter;
            labelEmptyDescription.Anchor = AnchorStyles.None;
            emptyState.Controls.Add(labelEmptyTitle, 0, 1);
            emptyState.Controls.Add(labelEmptyDescription, 0, 2);
            results.Controls.Add(treeViewHierarchy);
            results.Controls.Add(emptyState);
            emptyState.BringToFront();
            layout.Controls.Add(results, 0, 3);

            checkBoxNavigateAutomatically = new CheckBox
            {
                Text = "Open highlighted page in OneNote",
                AutoSize = true,
                Checked = true,
                Margin = new Padding(0, 14, 0, 0)
            };
            layout.Controls.Add(checkBoxNavigateAutomatically, 0, 4);
            card.Controls.Add(layout);
            return card;
        }

        private Panel CreatePreferencesCard()
        {
            var card = AppTheme.Card();
            var layout = AppTheme.Stack();
            layout.RowCount = 4;
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.Controls.Add(AppTheme.Label("Keep preferred copies", AppTheme.SectionFont), 0, 0);
            var description = AppTheme.Label("Move preferred locations to the top.\n“Select extra copies” keeps a copy from\nthe highest ranked location.", color: AppTheme.Muted);
            layout.Controls.Add(description, 0, 1);
            listBoxPathPreference = new ListBox
            {
                Name = "listBoxPathPreference",
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                IntegralHeight = false,
                HorizontalScrollbar = true,
                BackColor = AppTheme.Surface,
                ForeColor = AppTheme.Text,
                Margin = new Padding(0, 12, 0, 12),
                AccessibleName = "Locations in order of preference"
            };
            listBoxPathPreference.SelectedIndexChanged += (sender, e) => UpdatePreferenceButtons();
            layout.Controls.Add(listBoxPathPreference, 0, 2);
            var actions = AppTheme.Actions();
            buttonTop = AppTheme.Button("Top");
            buttonUp = AppTheme.Button("Up");
            buttonDown = AppTheme.Button("Down");
            buttonBottom = AppTheme.Button("Bottom");
            buttonTop.AccessibleName = "Move location to top";
            buttonUp.AccessibleName = "Move location up";
            buttonDown.AccessibleName = "Move location down";
            buttonBottom.AccessibleName = "Move location to bottom";
            buttonTop.Click += buttonTop_Click;
            buttonUp.Click += buttonUp_Click;
            buttonDown.Click += buttonDown_Click;
            buttonBottom.Click += buttonBottom_Click;
            actions.Controls.AddRange(new Control[] { buttonTop, buttonUp, buttonDown, buttonBottom });
            layout.Controls.Add(actions, 0, 3);
            card.Controls.Add(layout);
            return card;
        }

        private void InitializeMenus()
        {
            menuStrip1 = new MenuStrip { BackColor = AppTheme.Surface, ForeColor = AppTheme.Text, Font = AppTheme.BodyFont, Padding = new Padding(20, 6, 0, 6) };
            fileToolStripMenuItem = new ToolStripMenuItem("&File");
            dumpJsonToolStripMenuItem = new ToolStripMenuItem("&Export results to JSON…", null, dumpJsonToolStripMenuItem_Click);
            exitToolStripMenuItem = new ToolStripMenuItem("E&xit", null, exitToolStripMenuItem_Click);
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { dumpJsonToolStripMenuItem, new ToolStripSeparator(), exitToolStripMenuItem });
            advancedToolStripMenuItem = new ToolStripMenuItem("&Advanced") { Visible = false };
            cleanUpUsingJSONToolStripMenuItem = new ToolStripMenuItem("&Clean up using JSON…", null, cleanUpUsingJSONToolStripMenuItem_Click);
            flattenSectionsToolStripMenuItem = new ToolStripMenuItem("&Flatten sections", null, flattenSectionsToolStripMenuItem_Click);
            exportSectionDataToXml = new ToolStripMenuItem("Export &sections to XML…", null, exportSectionDataToXml_Click);
            exportpagesDataToXMLToolStripMenuItem = new ToolStripMenuItem("Export &pages to XML…", null, exportPagesDataToXMLToolStripMenuItem_Click);
            advancedToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { cleanUpUsingJSONToolStripMenuItem, flattenSectionsToolStripMenuItem, exportSectionDataToXml, exportpagesDataToXMLToolStripMenuItem });
            helpToolStripMenuItem = new ToolStripMenuItem("&Help");
            aboutToolStripMenuItem = new ToolStripMenuItem("&About…", null, aboutToolStripMenuItem_Click);
            helpToolStripMenuItem.DropDownItems.Add(aboutToolStripMenuItem);
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, advancedToolStripMenuItem, helpToolStripMenuItem });
        }

        private TreeView treeViewHierarchy;
        private Button buttonScanDuplicatedPages, buttonCancel, buttonSelectAllExceptOne, buttonDeselectAll, buttonRemoveSelectedPages;
        private Button buttonTop, buttonUp, buttonDown, buttonBottom;
        private CheckBox checkBoxNavigateAutomatically;
        private ListBox listBoxPathPreference;
        private Label labelGroupCount, labelPageCount, labelSelectedCount, labelSelectionHint, labelEmptyTitle, labelEmptyDescription;
        private TableLayoutPanel emptyState;
        private StatusStrip statusStrip1;
        private ToolStripProgressBar toolStripProgressBarScan;
        private ToolStripStatusLabel toolStripStatusLabelScan, labelMessageCounts;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem, dumpJsonToolStripMenuItem, exitToolStripMenuItem;
        private ToolStripMenuItem advancedToolStripMenuItem, cleanUpUsingJSONToolStripMenuItem, flattenSectionsToolStripMenuItem;
        private ToolStripMenuItem exportSectionDataToXml, exportpagesDataToXMLToolStripMenuItem, helpToolStripMenuItem, aboutToolStripMenuItem;
    }
}
