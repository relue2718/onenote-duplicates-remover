using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace OneNoteDuplicatesRemover
{
    public partial class FormAbout : Form
    {
        private const string ProjectUrl = "https://github.com/relue2718/onenote-duplicates-remover";
        private readonly OneNoteAccessor accessor;
        private bool diagnosticsExpanded;

        public FormAbout(OneNoteAccessor accessor)
        {
            this.accessor = accessor;
            InitializeComponent();
        }

        private static string ApplicationVersion =>
            typeof(FormAbout).Assembly.GetName().Version?.ToString() ?? "Unknown";

        private void FormAbout_Load(object sender, EventArgs e)
        {
            labelVersion.Text = $"Version {ApplicationVersion} · {IntPtr.Size * 8}-bit";
            textBoxInformation.Text = BuildDiagnosticInformation();
            buttonOpenLogFolder.Enabled = Directory.Exists(etc.FileLogger.Instance.LogDirectory);
            ResizeToContent();
        }

        private string BuildDiagnosticInformation()
        {
            var info = new StringBuilder();
            info.AppendLine("Application: OneNote Duplicates Remover");
            info.AppendLine("Version: " + ApplicationVersion);
            info.AppendLine($"Process: {RuntimeInformation.ProcessArchitecture} ({IntPtr.Size * 8}-bit)");
            info.AppendLine("Windows: " + RuntimeInformation.OSDescription);
            info.AppendLine("OS architecture: " + RuntimeInformation.OSArchitecture);
            info.AppendLine(".NET: " + RuntimeInformation.FrameworkDescription);

            try
            {
                Type comType = accessor?.GetApplicationType();
                if (comType == null)
                {
                    info.AppendLine("OneNote integration: Not initialized");
                }
                else
                {
                    // Describes this session's initialized COM wrapper, not a live connectivity probe.
                    info.AppendLine("OneNote integration: Initialized for this session");
                    info.AppendLine("Interop type: " + comType.FullName);
                    info.AppendLine("Interop assembly: " + comType.Assembly.FullName);
                    info.AppendLine("COM object: " + comType.IsCOMObject);
                }
            }
            catch (Exception exception)
            {
                // Diagnostics must remain available even if the OneNote integration is unavailable.
                info.AppendLine("OneNote integration: Unavailable (" + exception.GetType().Name + ")");
            }
            return info.ToString();
        }

        private void buttonToggleDiagnostics_Click(object sender, EventArgs e)
        {
            diagnosticsExpanded = !diagnosticsExpanded;
            diagnosticsPanel.Visible = diagnosticsExpanded;
            buttonToggleDiagnostics.Text = diagnosticsExpanded ? "Hide &diagnostics" : "Show &diagnostics";
            ResizeToContent();
        }

        private void ResizeToContent()
        {
            layout.PerformLayout();
            var workArea = Screen.FromControl(this).WorkingArea;
            int preferredHeight = layout.GetPreferredSize(new System.Drawing.Size(ClientSize.Width, 0)).Height;
            int availableHeight = Math.Max(1, workArea.Height - (Height - ClientSize.Height));
            AutoScroll = preferredHeight > availableHeight;
            ClientSize = new System.Drawing.Size(ClientSize.Width, Math.Min(preferredHeight, availableHeight));
            if (Visible)
                Top = Math.Max(workArea.Top, Math.Min(Top, workArea.Bottom - Height));
        }

        protected override void OnDpiChanged(DpiChangedEventArgs e)
        {
            base.OnDpiChanged(e);
            ResizeToContent();
        }

        private void buttonCopyText_Click(object sender, EventArgs e)
        {
            CopyDiagnostics(Clipboard.SetText);
        }

        private void CopyDiagnostics(Action<string> copyText)
        {
            try
            {
                copyText(textBoxInformation.Text);
                labelActionStatus.Text = "Diagnostics copied to clipboard.";
                labelActionStatus.ForeColor = AppTheme.Muted;
                buttonCopyText.Text = "Copied";
                copyFeedbackTimer.Stop();
                copyFeedbackTimer.Start();
            }
            catch (ExternalException)
            {
                ShowActionError("Clipboard is busy. Try copying again.");
            }
        }

        private void copyFeedbackTimer_Tick(object sender, EventArgs e)
        {
            copyFeedbackTimer.Stop();
            buttonCopyText.Text = "&Copy diagnostics";
            labelActionStatus.Text = " ";
        }

        private void buttonOpenInstallationPath_Click(object sender, EventArgs e)
        {
            OpenFolder(AppContext.BaseDirectory);
        }

        private void buttonOpenLogFolder_Click(object sender, EventArgs e)
        {
            OpenFolder(etc.FileLogger.Instance.LogDirectory);
        }

        private void OpenFolder(string path)
        {
            if (!Directory.Exists(path))
            {
                ShowActionError("This folder is not available yet.");
                return;
            }
            OpenTarget(path);
        }

        private void projectLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenTarget((string)((LinkLabel)sender).Tag);
        }

        private void OpenTarget(string target)
        {
            try
            {
                Process.Start(new ProcessStartInfo { FileName = target, UseShellExecute = true });
            }
            catch (Exception exception) when (exception is Win32Exception || exception is InvalidOperationException || exception is IOException)
            {
                ShowActionError("Could not open the link or folder. Please try again.");
            }
        }

        private void ShowActionError(string message)
        {
            copyFeedbackTimer.Stop();
            buttonCopyText.Text = "&Copy diagnostics";
            labelActionStatus.ForeColor = AppTheme.Danger;
            labelActionStatus.Text = message;
        }
    }
}
