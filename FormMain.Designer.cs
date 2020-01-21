namespace OneNoteDuplicatesRemover
{
    partial class FormMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if ( disposing && ( components != null ) )
            {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.labelMessageCounts = new System.Windows.Forms.Label();
            this.buttonScanDuplicatedPages = new System.Windows.Forms.Button();
            this.checkBoxNavigateAutomatically = new System.Windows.Forms.CheckBox();
            this.buttonSelectAllExceptOne = new System.Windows.Forms.Button();
            this.buttonDeselectAll = new System.Windows.Forms.Button();
            this.buttonRemoveSelectedPages = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeViewHierarchy = new System.Windows.Forms.TreeView();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.buttonBottom = new System.Windows.Forms.Button();
            this.buttonTop = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonUp = new System.Windows.Forms.Button();
            this.buttonDown = new System.Windows.Forms.Button();
            this.listBoxPathPreference = new System.Windows.Forms.ListBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripProgressBarScan = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabelScan = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dumpJsonToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.advancedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cleanUpUsingJSONToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.flattenSectionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.toolStripContainer1.BottomToolStripPanel.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer2
            // 
            this.splitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.buttonCancel);
            this.splitContainer2.Panel1.Controls.Add(this.labelMessageCounts);
            this.splitContainer2.Panel1.Controls.Add(this.buttonScanDuplicatedPages);
            this.splitContainer2.Panel1.Controls.Add(this.checkBoxNavigateAutomatically);
            this.splitContainer2.Panel1.Controls.Add(this.buttonSelectAllExceptOne);
            this.splitContainer2.Panel1.Controls.Add(this.buttonDeselectAll);
            this.splitContainer2.Panel1.Controls.Add(this.buttonRemoveSelectedPages);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.splitContainer1);
            this.splitContainer2.Size = new System.Drawing.Size(978, 564);
            this.splitContainer2.SplitterDistance = 66;
            this.splitContainer2.TabIndex = 18;
            // 
            // buttonCancel
            // 
            this.buttonCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.buttonCancel.Location = new System.Drawing.Point(873, 12);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(91, 26);
            this.buttonCancel.TabIndex = 7;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = false;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // labelMessageCounts
            // 
            this.labelMessageCounts.AutoSize = true;
            this.labelMessageCounts.Location = new System.Drawing.Point(483, 44);
            this.labelMessageCounts.Name = "labelMessageCounts";
            this.labelMessageCounts.Size = new System.Drawing.Size(0, 13);
            this.labelMessageCounts.TabIndex = 6;
            // 
            // buttonScanDuplicatedPages
            // 
            this.buttonScanDuplicatedPages.BackColor = System.Drawing.Color.LightSkyBlue;
            this.buttonScanDuplicatedPages.Cursor = System.Windows.Forms.Cursors.Default;
            this.buttonScanDuplicatedPages.Location = new System.Drawing.Point(12, 12);
            this.buttonScanDuplicatedPages.Name = "buttonScanDuplicatedPages";
            this.buttonScanDuplicatedPages.Size = new System.Drawing.Size(148, 26);
            this.buttonScanDuplicatedPages.TabIndex = 0;
            this.buttonScanDuplicatedPages.Text = "Scan duplicates";
            this.buttonScanDuplicatedPages.UseVisualStyleBackColor = false;
            this.buttonScanDuplicatedPages.Click += new System.EventHandler(this.buttonScanDuplicatedPages_Click);
            // 
            // checkBoxNavigateAutomatically
            // 
            this.checkBoxNavigateAutomatically.AutoSize = true;
            this.checkBoxNavigateAutomatically.Checked = true;
            this.checkBoxNavigateAutomatically.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxNavigateAutomatically.Location = new System.Drawing.Point(12, 44);
            this.checkBoxNavigateAutomatically.Name = "checkBoxNavigateAutomatically";
            this.checkBoxNavigateAutomatically.Size = new System.Drawing.Size(180, 17);
            this.checkBoxNavigateAutomatically.TabIndex = 5;
            this.checkBoxNavigateAutomatically.Text = "Navigate to the highlighted page";
            this.checkBoxNavigateAutomatically.UseVisualStyleBackColor = true;
            // 
            // buttonSelectAllExceptOne
            // 
            this.buttonSelectAllExceptOne.Location = new System.Drawing.Point(173, 12);
            this.buttonSelectAllExceptOne.Name = "buttonSelectAllExceptOne";
            this.buttonSelectAllExceptOne.Size = new System.Drawing.Size(148, 26);
            this.buttonSelectAllExceptOne.TabIndex = 1;
            this.buttonSelectAllExceptOne.Text = "Select all except one";
            this.buttonSelectAllExceptOne.UseVisualStyleBackColor = true;
            this.buttonSelectAllExceptOne.Click += new System.EventHandler(this.buttonSelectAllExceptOne_Click);
            // 
            // buttonDeselectAll
            // 
            this.buttonDeselectAll.Location = new System.Drawing.Point(325, 12);
            this.buttonDeselectAll.Name = "buttonDeselectAll";
            this.buttonDeselectAll.Size = new System.Drawing.Size(148, 26);
            this.buttonDeselectAll.TabIndex = 3;
            this.buttonDeselectAll.Text = "Deselect all";
            this.buttonDeselectAll.UseVisualStyleBackColor = true;
            this.buttonDeselectAll.Click += new System.EventHandler(this.buttonDeselectAll_Click);
            // 
            // buttonRemoveSelectedPages
            // 
            this.buttonRemoveSelectedPages.BackColor = System.Drawing.Color.LightSalmon;
            this.buttonRemoveSelectedPages.Location = new System.Drawing.Point(486, 12);
            this.buttonRemoveSelectedPages.Name = "buttonRemoveSelectedPages";
            this.buttonRemoveSelectedPages.Size = new System.Drawing.Size(244, 26);
            this.buttonRemoveSelectedPages.TabIndex = 4;
            this.buttonRemoveSelectedPages.Text = "Remove the selected pages";
            this.buttonRemoveSelectedPages.UseVisualStyleBackColor = false;
            this.buttonRemoveSelectedPages.Click += new System.EventHandler(this.buttonRemoveSelectedPages_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeViewHierarchy);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer3);
            this.splitContainer1.Size = new System.Drawing.Size(978, 494);
            this.splitContainer1.SplitterDistance = 329;
            this.splitContainer1.TabIndex = 18;
            // 
            // treeViewHierarchy
            // 
            this.treeViewHierarchy.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeViewHierarchy.CheckBoxes = true;
            this.treeViewHierarchy.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewHierarchy.Location = new System.Drawing.Point(0, 0);
            this.treeViewHierarchy.Name = "treeViewHierarchy";
            this.treeViewHierarchy.Size = new System.Drawing.Size(974, 325);
            this.treeViewHierarchy.TabIndex = 6;
            this.treeViewHierarchy.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeViewHierarchy_BeforeSelect);
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.buttonBottom);
            this.splitContainer3.Panel1.Controls.Add(this.buttonTop);
            this.splitContainer3.Panel1.Controls.Add(this.label2);
            this.splitContainer3.Panel1.Controls.Add(this.buttonUp);
            this.splitContainer3.Panel1.Controls.Add(this.buttonDown);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.listBoxPathPreference);
            this.splitContainer3.Size = new System.Drawing.Size(974, 157);
            this.splitContainer3.SplitterDistance = 118;
            this.splitContainer3.TabIndex = 12;
            // 
            // buttonBottom
            // 
            this.buttonBottom.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonBottom.Location = new System.Drawing.Point(8, 119);
            this.buttonBottom.Name = "buttonBottom";
            this.buttonBottom.Size = new System.Drawing.Size(105, 24);
            this.buttonBottom.TabIndex = 10;
            this.buttonBottom.Text = "Bottom";
            this.buttonBottom.UseVisualStyleBackColor = true;
            this.buttonBottom.Click += new System.EventHandler(this.buttonBottom_Click);
            // 
            // buttonTop
            // 
            this.buttonTop.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonTop.Location = new System.Drawing.Point(8, 29);
            this.buttonTop.Name = "buttonTop";
            this.buttonTop.Size = new System.Drawing.Size(105, 24);
            this.buttonTop.TabIndex = 7;
            this.buttonTop.Text = "Top";
            this.buttonTop.UseVisualStyleBackColor = true;
            this.buttonTop.Click += new System.EventHandler(this.buttonTop_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Section Preference";
            // 
            // buttonUp
            // 
            this.buttonUp.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonUp.Location = new System.Drawing.Point(8, 59);
            this.buttonUp.Name = "buttonUp";
            this.buttonUp.Size = new System.Drawing.Size(105, 24);
            this.buttonUp.TabIndex = 8;
            this.buttonUp.Text = "Up";
            this.buttonUp.UseVisualStyleBackColor = true;
            this.buttonUp.Click += new System.EventHandler(this.buttonUp_Click);
            // 
            // buttonDown
            // 
            this.buttonDown.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonDown.Location = new System.Drawing.Point(8, 89);
            this.buttonDown.Name = "buttonDown";
            this.buttonDown.Size = new System.Drawing.Size(105, 24);
            this.buttonDown.TabIndex = 9;
            this.buttonDown.Text = "Down";
            this.buttonDown.UseVisualStyleBackColor = true;
            this.buttonDown.Click += new System.EventHandler(this.buttonDown_Click);
            // 
            // listBoxPathPreference
            // 
            this.listBoxPathPreference.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listBoxPathPreference.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxPathPreference.FormattingEnabled = true;
            this.listBoxPathPreference.Location = new System.Drawing.Point(0, 0);
            this.listBoxPathPreference.Name = "listBoxPathPreference";
            this.listBoxPathPreference.Size = new System.Drawing.Size(852, 157);
            this.listBoxPathPreference.TabIndex = 11;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripProgressBarScan,
            this.toolStripStatusLabelScan});
            this.statusStrip1.Location = new System.Drawing.Point(0, 0);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(978, 22);
            this.statusStrip1.TabIndex = 19;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripProgressBarScan
            // 
            this.toolStripProgressBarScan.Name = "toolStripProgressBarScan";
            this.toolStripProgressBarScan.Size = new System.Drawing.Size(100, 16);
            // 
            // toolStripStatusLabelScan
            // 
            this.toolStripStatusLabelScan.Name = "toolStripStatusLabelScan";
            this.toolStripStatusLabelScan.Size = new System.Drawing.Size(118, 17);
            this.toolStripStatusLabelScan.Text = "toolStripStatusLabel1";
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.BottomToolStripPanel
            // 
            this.toolStripContainer1.BottomToolStripPanel.Controls.Add(this.statusStrip1);
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.splitContainer2);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(978, 564);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 24);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(978, 611);
            this.toolStripContainer1.TabIndex = 20;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.advancedToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(978, 24);
            this.menuStrip1.TabIndex = 21;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dumpJsonToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // dumpJsonToolStripMenuItem
            // 
            this.dumpJsonToolStripMenuItem.Name = "dumpJsonToolStripMenuItem";
            this.dumpJsonToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.dumpJsonToolStripMenuItem.Text = "&Dump JSON...";
            this.dumpJsonToolStripMenuItem.Click += new System.EventHandler(this.dumpJsonToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(149, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // advancedToolStripMenuItem
            // 
            this.advancedToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cleanUpUsingJSONToolStripMenuItem,
            this.flattenSectionsToolStripMenuItem});
            this.advancedToolStripMenuItem.Name = "advancedToolStripMenuItem";
            this.advancedToolStripMenuItem.Size = new System.Drawing.Size(72, 20);
            this.advancedToolStripMenuItem.Text = "&Advanced";
            // 
            // cleanUpUsingJSONToolStripMenuItem
            // 
            this.cleanUpUsingJSONToolStripMenuItem.Name = "cleanUpUsingJSONToolStripMenuItem";
            this.cleanUpUsingJSONToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.cleanUpUsingJSONToolStripMenuItem.Text = "&Clean up using JSON...";
            this.cleanUpUsingJSONToolStripMenuItem.Click += new System.EventHandler(this.cleanUpUsingJSONToolStripMenuItem_Click);
            // 
            // flattenSectionsToolStripMenuItem
            // 
            this.flattenSectionsToolStripMenuItem.Name = "flattenSectionsToolStripMenuItem";
            this.flattenSectionsToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.flattenSectionsToolStripMenuItem.Text = "&Flatten Sections";
            this.flattenSectionsToolStripMenuItem.Click += new System.EventHandler(this.flattenSectionsToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.aboutToolStripMenuItem.Text = "&About...";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(978, 635);
            this.Controls.Add(this.toolStripContainer1);
            this.Controls.Add(this.menuStrip1);
            this.ForeColor = System.Drawing.Color.Black;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "OneNoteDuplicatesRemover";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel1.PerformLayout();
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.toolStripContainer1.BottomToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.BottomToolStripPanel.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.TreeView treeViewHierarchy;
        private System.Windows.Forms.Button buttonScanDuplicatedPages;
        private System.Windows.Forms.CheckBox checkBoxNavigateAutomatically;
        private System.Windows.Forms.Button buttonSelectAllExceptOne;
        private System.Windows.Forms.Button buttonDeselectAll;
        private System.Windows.Forms.Button buttonRemoveSelectedPages;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBarScan;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelScan;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.Button buttonDown;
        private System.Windows.Forms.Button buttonUp;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox listBoxPathPreference;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.Button buttonBottom;
        private System.Windows.Forms.Button buttonTop;
        private System.Windows.Forms.Label labelMessageCounts;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem dumpJsonToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem advancedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cleanUpUsingJSONToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem flattenSectionsToolStripMenuItem;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
    }
}