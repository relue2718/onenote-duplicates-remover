using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Net.Sockets;

namespace OneNoteDuplicatesRemover
{
    public partial class FormAbout : Form
    {
        private OneNoteAccessor accessor = null;
        public FormAbout(OneNoteAccessor accessor)
        {
            this.accessor = accessor;
            InitializeComponent();
        }

        private void buttonOpenInstallationPath_Click(object sender, EventArgs e)
        {
            string currentAssemblyPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string currentAssemblyDirectory = System.IO.Path.GetDirectoryName(currentAssemblyPath);
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
            {
                FileName = currentAssemblyDirectory,
                UseShellExecute = true,
                Verb = "open"
            });
        }
        private void buttonOpenWebsite_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
            {
                FileName = "https://relue2718.com",
                UseShellExecute = true,
                Verb = "open"
            });
        }

        private void buttonCopyText_Click(object sender, EventArgs e)
        {
            if (textBoxInformation.Text.Length > 0)
            {
                Clipboard.SetText(textBoxInformation.Text);
            }
        }

        private void appendInfo(StringBuilder sb, string name, Func<string> valueProvider)
        {
            try
            {
                sb.AppendLine(string.Format("{0} = {1}", name, valueProvider()));
            }
            catch (System.Exception exception)
            {
                etc.LoggerHelper.LogUnexpectedException(exception);
            }
        }

        private void FormAbout_Load(object sender, EventArgs e)
        {
            Type comObjectType = accessor.GetApplicationType();
            StringBuilder sb = new StringBuilder();

            appendInfo(sb, "Program Version", () => System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString());

            if (comObjectType != null)
            {
                appendInfo(sb, "Assembly.CodeBase", () => comObjectType.Assembly.CodeBase);
                appendInfo(sb, "Assembly.FullName", () => comObjectType.Assembly.FullName);
                appendInfo(sb, "Assembly.ImageRuntimeVersion", () => comObjectType.Assembly.ImageRuntimeVersion);
                appendInfo(sb, "Assembly.IsFullyTrusted", () => comObjectType.Assembly.IsFullyTrusted.ToString());
                appendInfo(sb, "Assembly.Location", () => comObjectType.Assembly.Location.ToString());
                appendInfo(sb, "FullName", () => comObjectType.FullName);
                appendInfo(sb, "IsCOMObject", () => comObjectType.IsCOMObject.ToString());
                appendInfo(sb, "Module.Name", () => comObjectType.Module.Name);
            }
            else
            {
                sb.AppendLine(string.Format("Unable to retrieve type information."));
            }

            textBoxInformation.Text = sb.ToString();
        }
    }
}
