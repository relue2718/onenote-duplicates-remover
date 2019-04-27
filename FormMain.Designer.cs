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
            this.listBoxPreferences = new System.Windows.Forms.ListBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripProgressBarScan = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabelScan = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.labelMessageCounts = new System.Windows.Forms.Label();
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
            this.splitContainer2.Size = new System.Drawing.Size(978, 588);
            this.splitContainer2.SplitterDistance = 66;
            this.splitContainer2.TabIndex = 18;
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
            this.splitContainer1.Size = new System.Drawing.Size(978, 518);
            this.splitContainer1.SplitterDistance = 346;
            this.splitContainer1.TabIndex = 18;
            // 
            // treeViewHierarchy
            // 
            this.treeViewHierarchy.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeViewHierarchy.CheckBoxes = true;
            this.treeViewHierarchy.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewHierarchy.Location = new System.Drawing.Point(0, 0);
            this.treeViewHierarchy.Name = "treeViewHierarchy";
            this.treeViewHierarchy.Size = new System.Drawing.Size(974, 342);
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
            this.splitContainer3.Panel2.Controls.Add(this.listBoxPreferences);
            this.splitContainer3.Size = new System.Drawing.Size(974, 164);
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
            // listBoxPreferences
            // 
            this.listBoxPreferences.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listBoxPreferences.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxPreferences.FormattingEnabled = true;
            this.listBoxPreferences.Location = new System.Drawing.Point(0, 0);
            this.listBoxPreferences.Name = "listBoxPreferences";
            this.listBoxPreferences.Size = new System.Drawing.Size(852, 164);
            this.listBoxPreferences.TabIndex = 11;
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
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(978, 588);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(978, 635);
            this.toolStripContainer1.TabIndex = 20;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // labelMessageCounts
            // 
            this.labelMessageCounts.AutoSize = true;
            this.labelMessageCounts.Location = new System.Drawing.Point(483, 44);
            this.labelMessageCounts.Name = "labelMessageCounts";
            this.labelMessageCounts.Size = new System.Drawing.Size(0, 13);
            this.labelMessageCounts.TabIndex = 6;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(978, 635);
            this.Controls.Add(this.toolStripContainer1);
            this.ForeColor = System.Drawing.Color.Black;
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "OneNoteDuplicatesRemover";
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
            this.ResumeLayout(false);

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
        private System.Windows.Forms.ListBox listBoxPreferences;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.Button buttonBottom;
        private System.Windows.Forms.Button buttonTop;
        private System.Windows.Forms.Label labelMessageCounts;
    }
}