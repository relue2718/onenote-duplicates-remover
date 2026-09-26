using System.Drawing;
using System.Windows.Forms;

namespace OneNoteDuplicatesRemover
{
    partial class FormAbout
    {
        private System.ComponentModel.IContainer components;

        protected override void Dispose(bool disposing)
        {
            if (disposing) components?.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            SuspendLayout();
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            Font = AppTheme.BodyFont;
            BackColor = AppTheme.Background;
            ForeColor = AppTheme.Text;
            ClientSize = new Size(660, 380);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowIcon = false;
            ShowInTaskbar = false;
            Name = "FormAbout";
            Text = "About OneNote Duplicates Remover";

            layout = AppTheme.Stack();
            layout.Dock = DockStyle.Top;
            layout.AutoSize = true;
            layout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            layout.Padding = new Padding(28, 24, 28, 24);
            layout.RowCount = 9;
            for (int i = 0; i < layout.RowCount; i++)
                layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            layout.Controls.Add(AppTheme.Label("OneNote Duplicates Remover", AppTheme.HeadingFont), 0, 0);
            layout.Controls.Add(AppTheme.Label("Find and remove duplicate pages in your OneNote notebooks.", color: AppTheme.Muted), 0, 1);
            labelVersion = AppTheme.Label("", AppTheme.SectionFont, AppTheme.Accent);
            labelVersion.Margin = new Padding(0, 8, 0, 12);
            layout.Controls.Add(labelVersion, 0, 2);

            var credits = AppTheme.Actions();
            credits.Controls.Add(AppTheme.Label("Developed by Yoo Seunghyun", color: AppTheme.Muted));
            var license = CreateProjectLink("MIT license", ProjectUrl + "/blob/master/LICENSE");
            license.Margin = new Padding(18, 0, 0, 0);
            credits.Controls.Add(license);
            layout.Controls.Add(credits, 0, 3);

            var links = AppTheme.Actions();
            links.Margin = new Padding(0, 8, 0, 18);
            links.Controls.Add(CreateProjectLink("GitHub", ProjectUrl));
            links.Controls.Add(CreateProjectLink("Report an issue", ProjectUrl + "/issues/new"));
            links.Controls.Add(CreateProjectLink("Release history", ProjectUrl + "/releases"));
            layout.Controls.Add(links, 0, 4);

            buttonToggleDiagnostics = AppTheme.Button("Show &diagnostics");
            buttonToggleDiagnostics.Margin = Padding.Empty;
            buttonToggleDiagnostics.Click += buttonToggleDiagnostics_Click;
            layout.Controls.Add(buttonToggleDiagnostics, 0, 5);

            diagnosticsPanel = AppTheme.Stack();
            diagnosticsPanel.Height = 224;
            diagnosticsPanel.MinimumSize = new Size(0, 224);
            diagnosticsPanel.Visible = false;
            diagnosticsPanel.Margin = new Padding(0, 12, 0, 0);
            diagnosticsPanel.RowCount = 2;
            diagnosticsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            diagnosticsPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            var infoCard = AppTheme.Card();
            infoCard.Padding = new Padding(12);
            infoCard.Margin = new Padding(0, 0, 0, 12);
            textBoxInformation = new TextBox
            {
                Dock = DockStyle.Fill, Multiline = true, ReadOnly = true,
                BorderStyle = BorderStyle.None, BackColor = AppTheme.Surface,
                ForeColor = AppTheme.Text, ScrollBars = ScrollBars.Vertical, WordWrap = true,
                AccessibleName = "Application and OneNote diagnostic information"
            };
            infoCard.Controls.Add(textBoxInformation);
            diagnosticsPanel.Controls.Add(infoCard, 0, 0);
            var folderActions = AppTheme.Actions();
            buttonOpenInstallationPath = AppTheme.Button("Installation &folder");
            buttonOpenInstallationPath.Click += buttonOpenInstallationPath_Click;
            buttonOpenLogFolder = AppTheme.Button("Open &log folder");
            buttonOpenLogFolder.Click += buttonOpenLogFolder_Click;
            folderActions.Controls.AddRange(new Control[] { buttonOpenInstallationPath, buttonOpenLogFolder });
            diagnosticsPanel.Controls.Add(folderActions, 0, 1);
            layout.Controls.Add(diagnosticsPanel, 0, 6);

            labelActionStatus = AppTheme.Label(" ", color: AppTheme.Muted);
            labelActionStatus.Margin = new Padding(0, 12, 0, 12);
            labelActionStatus.AccessibleName = "Action status";
            layout.Controls.Add(labelActionStatus, 0, 7);

            var footer = new TableLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, ColumnCount = 2, Margin = Padding.Empty };
            footer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            footer.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            buttonCopyText = AppTheme.Button("&Copy diagnostics");
            buttonCopyText.Click += buttonCopyText_Click;
            buttonOkay = AppTheme.Button("&Close", primary: true);
            buttonOkay.Margin = Padding.Empty;
            buttonOkay.DialogResult = DialogResult.OK;
            footer.Controls.Add(buttonCopyText, 0, 0);
            footer.Controls.Add(buttonOkay, 1, 0);
            layout.Controls.Add(footer, 0, 8);
            Controls.Add(layout);
            AcceptButton = buttonOkay;
            CancelButton = buttonOkay;
            copyFeedbackTimer = new Timer(components) { Interval = 2000 };
            copyFeedbackTimer.Tick += copyFeedbackTimer_Tick;
            Load += FormAbout_Load;
            ResumeLayout(true);
        }

        private LinkLabel CreateProjectLink(string text, string url)
        {
            var link = new LinkLabel
            {
                Text = text, Tag = url, AutoSize = true, UseMnemonic = false,
                LinkColor = AppTheme.Accent, ActiveLinkColor = AppTheme.Accent,
                VisitedLinkColor = AppTheme.Accent, LinkBehavior = LinkBehavior.HoverUnderline,
                Margin = new Padding(0, 0, 22, 0)
            };
            link.LinkClicked += projectLink_LinkClicked;
            return link;
        }

        private TableLayoutPanel layout, diagnosticsPanel;
        private Label labelVersion, labelActionStatus;
        private TextBox textBoxInformation;
        private Button buttonToggleDiagnostics, buttonOpenInstallationPath, buttonOpenLogFolder, buttonCopyText, buttonOkay;
        private Timer copyFeedbackTimer;
    }
}
