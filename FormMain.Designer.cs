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
            this.buttonScanDuplicatedPages = new System.Windows.Forms.Button();
            this.treeViewHierarchy = new System.Windows.Forms.TreeView();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonSelectAllExceptOne = new System.Windows.Forms.Button();
            this.buttonDeselectAll = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.listBoxPreferences = new System.Windows.Forms.ListBox();
            this.buttonUp = new System.Windows.Forms.Button();
            this.buttonDown = new System.Windows.Forms.Button();
            this.buttonRemoveSelectedPages = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.label4 = new System.Windows.Forms.Label();
            this.checkBoxNavigateAutomatically = new System.Windows.Forms.CheckBox();
            this.labelProgress = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonScanDuplicatedPages
            // 
            this.buttonScanDuplicatedPages.BackColor = System.Drawing.Color.LightSkyBlue;
            this.buttonScanDuplicatedPages.Cursor = System.Windows.Forms.Cursors.Default;
            this.buttonScanDuplicatedPages.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonScanDuplicatedPages.Location = new System.Drawing.Point(10, 467);
            this.buttonScanDuplicatedPages.Name = "buttonScanDuplicatedPages";
            this.buttonScanDuplicatedPages.Size = new System.Drawing.Size(214, 26);
            this.buttonScanDuplicatedPages.TabIndex = 2;
            this.buttonScanDuplicatedPages.Text = "Scan your duplicated OneNote Files";
            this.buttonScanDuplicatedPages.UseVisualStyleBackColor = false;
            this.buttonScanDuplicatedPages.Click += new System.EventHandler(this.buttonScanDuplicatedPages_Click);
            // 
            // treeViewHierarchy
            // 
            this.treeViewHierarchy.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.treeViewHierarchy.CheckBoxes = true;
            this.treeViewHierarchy.Location = new System.Drawing.Point(10, 32);
            this.treeViewHierarchy.Name = "treeViewHierarchy";
            this.treeViewHierarchy.Size = new System.Drawing.Size(688, 429);
            this.treeViewHierarchy.TabIndex = 1;
            this.treeViewHierarchy.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeViewHierarchy_BeforeSelect);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(230, 467);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(468, 26);
            this.progressBar1.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(532, 532);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(166, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Select pages to be removed";
            // 
            // buttonSelectAllExceptOne
            // 
            this.buttonSelectAllExceptOne.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonSelectAllExceptOne.Location = new System.Drawing.Point(535, 548);
            this.buttonSelectAllExceptOne.Name = "buttonSelectAllExceptOne";
            this.buttonSelectAllExceptOne.Size = new System.Drawing.Size(163, 23);
            this.buttonSelectAllExceptOne.TabIndex = 6;
            this.buttonSelectAllExceptOne.Text = "Select all except one";
            this.buttonSelectAllExceptOne.UseVisualStyleBackColor = true;
            this.buttonSelectAllExceptOne.Click += new System.EventHandler(this.buttonSelectAllExceptOne_Click);
            // 
            // buttonDeselectAll
            // 
            this.buttonDeselectAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonDeselectAll.Location = new System.Drawing.Point(535, 577);
            this.buttonDeselectAll.Name = "buttonDeselectAll";
            this.buttonDeselectAll.Size = new System.Drawing.Size(163, 24);
            this.buttonDeselectAll.TabIndex = 7;
            this.buttonDeselectAll.Text = "Deselect all";
            this.buttonDeselectAll.UseVisualStyleBackColor = true;
            this.buttonDeselectAll.Click += new System.EventHandler(this.buttonDeselectAll_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.Location = new System.Drawing.Point(7, 532);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(496, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Set section path preferences (TOPMOST will be preferred one, i.e, not to be remov" +
    "ed)";
            // 
            // listBoxPreferences
            // 
            this.listBoxPreferences.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listBoxPreferences.FormattingEnabled = true;
            this.listBoxPreferences.Location = new System.Drawing.Point(10, 547);
            this.listBoxPreferences.Name = "listBoxPreferences";
            this.listBoxPreferences.Size = new System.Drawing.Size(427, 106);
            this.listBoxPreferences.TabIndex = 3;
            // 
            // buttonUp
            // 
            this.buttonUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonUp.Location = new System.Drawing.Point(443, 547);
            this.buttonUp.Name = "buttonUp";
            this.buttonUp.Size = new System.Drawing.Size(71, 24);
            this.buttonUp.TabIndex = 4;
            this.buttonUp.Text = "Up";
            this.buttonUp.UseVisualStyleBackColor = true;
            this.buttonUp.Click += new System.EventHandler(this.buttonUp_Click);
            // 
            // buttonDown
            // 
            this.buttonDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonDown.Location = new System.Drawing.Point(443, 577);
            this.buttonDown.Name = "buttonDown";
            this.buttonDown.Size = new System.Drawing.Size(71, 24);
            this.buttonDown.TabIndex = 5;
            this.buttonDown.Text = "Down";
            this.buttonDown.UseVisualStyleBackColor = true;
            this.buttonDown.Click += new System.EventHandler(this.buttonDown_Click);
            // 
            // buttonRemoveSelectedPages
            // 
            this.buttonRemoveSelectedPages.BackColor = System.Drawing.Color.LightSalmon;
            this.buttonRemoveSelectedPages.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonRemoveSelectedPages.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonRemoveSelectedPages.Location = new System.Drawing.Point(10, 678);
            this.buttonRemoveSelectedPages.Name = "buttonRemoveSelectedPages";
            this.buttonRemoveSelectedPages.Size = new System.Drawing.Size(296, 24);
            this.buttonRemoveSelectedPages.TabIndex = 8;
            this.buttonRemoveSelectedPages.Text = "Remove the selected pages";
            this.buttonRemoveSelectedPages.UseVisualStyleBackColor = false;
            this.buttonRemoveSelectedPages.Click += new System.EventHandler(this.buttonRemoveSelectedPages_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Red;
            this.label3.Location = new System.Drawing.Point(7, 662);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(299, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Please backup before confirming the remove action";
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(541, 689);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(157, 13);
            this.linkLabel1.TabIndex = 12;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "http://eenet.snu.ac.kr/~xflos43";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(541, 676);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(111, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "Visit the author\'s page";
            // 
            // checkBoxNavigateAutomatically
            // 
            this.checkBoxNavigateAutomatically.AutoSize = true;
            this.checkBoxNavigateAutomatically.Checked = true;
            this.checkBoxNavigateAutomatically.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxNavigateAutomatically.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxNavigateAutomatically.Location = new System.Drawing.Point(10, 9);
            this.checkBoxNavigateAutomatically.Name = "checkBoxNavigateAutomatically";
            this.checkBoxNavigateAutomatically.Size = new System.Drawing.Size(275, 17);
            this.checkBoxNavigateAutomatically.TabIndex = 14;
            this.checkBoxNavigateAutomatically.Text = "Navigate the highlighted page automatically";
            this.checkBoxNavigateAutomatically.UseVisualStyleBackColor = true;
            // 
            // labelProgress
            // 
            this.labelProgress.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelProgress.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelProgress.ForeColor = System.Drawing.Color.Black;
            this.labelProgress.Location = new System.Drawing.Point(230, 496);
            this.labelProgress.Name = "labelProgress";
            this.labelProgress.Size = new System.Drawing.Size(468, 16);
            this.labelProgress.TabIndex = 15;
            this.labelProgress.Text = "--";
            this.labelProgress.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(710, 708);
            this.Controls.Add(this.labelProgress);
            this.Controls.Add(this.checkBoxNavigateAutomatically);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.buttonRemoveSelectedPages);
            this.Controls.Add(this.buttonDown);
            this.Controls.Add(this.buttonUp);
            this.Controls.Add(this.listBoxPreferences);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonDeselectAll);
            this.Controls.Add(this.buttonSelectAllExceptOne);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.treeViewHierarchy);
            this.Controls.Add(this.buttonScanDuplicatedPages);
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "OneNoteDuplicatesRemover";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonScanDuplicatedPages;
        private System.Windows.Forms.TreeView treeViewHierarchy;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonSelectAllExceptOne;
        private System.Windows.Forms.Button buttonDeselectAll;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox listBoxPreferences;
        private System.Windows.Forms.Button buttonUp;
        private System.Windows.Forms.Button buttonDown;
        private System.Windows.Forms.Button buttonRemoveSelectedPages;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox checkBoxNavigateAutomatically;
        private System.Windows.Forms.Label labelProgress;
    }
}