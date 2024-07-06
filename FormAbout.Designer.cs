namespace OneNoteDuplicatesRemover
{
    partial class FormAbout
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxInformation = new System.Windows.Forms.TextBox();
            this.buttonOpenInstallationPath = new System.Windows.Forms.Button();
            this.buttonCopyText = new System.Windows.Forms.Button();
            this.buttonOkay = new System.Windows.Forms.Button();
            this.buttonOpenWebsite = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(15, 21);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(439, 37);
            this.label1.TabIndex = 0;
            this.label1.Text = "OneNote Duplicates Remover";
            // 
            // textBoxInformation
            // 
            this.textBoxInformation.Location = new System.Drawing.Point(23, 91);
            this.textBoxInformation.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBoxInformation.Multiline = true;
            this.textBoxInformation.Name = "textBoxInformation";
            this.textBoxInformation.ReadOnly = true;
            this.textBoxInformation.Size = new System.Drawing.Size(668, 382);
            this.textBoxInformation.TabIndex = 1;
            // 
            // buttonOpenInstallationPath
            // 
            this.buttonOpenInstallationPath.Location = new System.Drawing.Point(708, 91);
            this.buttonOpenInstallationPath.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.buttonOpenInstallationPath.Name = "buttonOpenInstallationPath";
            this.buttonOpenInstallationPath.Size = new System.Drawing.Size(164, 28);
            this.buttonOpenInstallationPath.TabIndex = 0;
            this.buttonOpenInstallationPath.Text = "&Open Installation Path...";
            this.buttonOpenInstallationPath.UseVisualStyleBackColor = true;
            this.buttonOpenInstallationPath.Click += new System.EventHandler(this.buttonOpenInstallationPath_Click);
            // 
            // buttonCopyText
            // 
            this.buttonCopyText.Location = new System.Drawing.Point(708, 412);
            this.buttonCopyText.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.buttonCopyText.Name = "buttonCopyText";
            this.buttonCopyText.Size = new System.Drawing.Size(164, 28);
            this.buttonCopyText.TabIndex = 3;
            this.buttonCopyText.Text = "&Copy Text";
            this.buttonCopyText.UseVisualStyleBackColor = true;
            this.buttonCopyText.Click += new System.EventHandler(this.buttonCopyText_Click);
            // 
            // buttonOkay
            // 
            this.buttonOkay.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOkay.Location = new System.Drawing.Point(708, 445);
            this.buttonOkay.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.buttonOkay.Name = "buttonOkay";
            this.buttonOkay.Size = new System.Drawing.Size(164, 28);
            this.buttonOkay.TabIndex = 4;
            this.buttonOkay.Text = "&OK";
            this.buttonOkay.UseVisualStyleBackColor = true;
            // 
            // buttonOpenWebsite
            // 
            this.buttonOpenWebsite.Location = new System.Drawing.Point(708, 125);
            this.buttonOpenWebsite.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.buttonOpenWebsite.Name = "buttonOpenWebsite";
            this.buttonOpenWebsite.Size = new System.Drawing.Size(164, 28);
            this.buttonOpenWebsite.TabIndex = 1;
            this.buttonOpenWebsite.Text = "&Website...";
            this.buttonOpenWebsite.UseVisualStyleBackColor = true;
            this.buttonOpenWebsite.Click += new System.EventHandler(this.buttonOpenWebsite_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 77);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(111, 12);
            this.label2.TabIndex = 6;
            this.label2.Text = "Debug Information:";
            // 
            // FormAbout
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(895, 493);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonOpenWebsite);
            this.Controls.Add(this.buttonOkay);
            this.Controls.Add(this.buttonCopyText);
            this.Controls.Add(this.buttonOpenInstallationPath);
            this.Controls.Add(this.textBoxInformation);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormAbout";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About";
            this.Load += new System.EventHandler(this.FormAbout_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxInformation;
        private System.Windows.Forms.Button buttonOpenInstallationPath;
        private System.Windows.Forms.Button buttonCopyText;
        private System.Windows.Forms.Button buttonOkay;
        private System.Windows.Forms.Button buttonOpenWebsite;
        private System.Windows.Forms.Label label2;
    }
}