using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

        private void buttonCopyText_Click(object sender, EventArgs e)
        {
            if (textBoxInformation.Text.Length > 0)
            {
                Clipboard.SetText(textBoxInformation.Text);
            }

        }

        private void FormAbout_Load(object sender, EventArgs e)
        {
            Type comObjectType = accessor.GetApplicationType();
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendLine(string.Format("Program Version = {0}", System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString()));
            }
            catch (System.Exception exception)
            {
                etc.LoggerHelper.LogUnexpectedException(exception);
            }

            if (comObjectType != null)
            {
                try
                {
                    sb.AppendLine(string.Format("Assembly.CodeBase = {0}", comObjectType.Assembly.CodeBase));
                }
                catch (System.Exception exception)
                {
                    etc.LoggerHelper.LogUnexpectedException(exception);
                }

                try
                {
                    sb.AppendLine(string.Format("Assembly.FullName = {0}", comObjectType.Assembly.FullName));
                }
                catch (System.Exception exception)
                {
                    etc.LoggerHelper.LogUnexpectedException(exception);
                }

                try
                {
                    sb.AppendLine(string.Format("Assembly.ImageRuntimeVersion = {0}", comObjectType.Assembly.ImageRuntimeVersion));
                }
                catch (System.Exception exception)
                {
                    etc.LoggerHelper.LogUnexpectedException(exception);
                }

                try
                {
                    sb.AppendLine(string.Format("Assembly.IsFullyTrusted = {0}", comObjectType.Assembly.IsFullyTrusted));
                }
                catch (System.Exception exception)
                {
                    etc.LoggerHelper.LogUnexpectedException(exception);
                }

                try
                {
                    sb.AppendLine(string.Format("Assembly.Location = {0}", comObjectType.Assembly.Location));
                }
                catch (System.Exception exception)
                {
                    etc.LoggerHelper.LogUnexpectedException(exception);
                }
                try
                {
                    sb.AppendLine(string.Format("FullName = {0}", comObjectType.FullName));
                }
                catch (System.Exception exception)
                {
                    etc.LoggerHelper.LogUnexpectedException(exception);
                }
                try
                {
                    sb.AppendLine(string.Format("IsCOMObject = {0}", comObjectType.IsCOMObject));
                }
                catch (System.Exception exception)
                {
                    etc.LoggerHelper.LogUnexpectedException(exception);
                }
                try
                {
                    sb.AppendLine(string.Format("Module.Name = {0}", comObjectType.Module.Name));
                }
                catch (System.Exception exception)
                {
                    etc.LoggerHelper.LogUnexpectedException(exception);
                }
            }
            else
            {
                sb.AppendLine(string.Format("Unable to retrieve type information."));
            }
            textBoxInformation.Text = sb.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
            {
                FileName = "https://relue2718.com",
                UseShellExecute = true,
                Verb = "open"
            });
        }
    }
}
