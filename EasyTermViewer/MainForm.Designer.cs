namespace EasyTermViewer
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.txtFind = new System.Windows.Forms.ToolStripTextBox();
            this.btnFind = new System.Windows.Forms.ToolStripSplitButton();
            this.btnFindText = new System.Windows.Forms.ToolStripMenuItem();
            this.btnFindTerm = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.cmdLanguage1 = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.cmdLanguage2 = new System.Windows.Forms.ToolStripComboBox();
            this.cmdTermBases = new System.Windows.Forms.ToolStripButton();
            this.timerFilter = new System.Windows.Forms.Timer(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.lstTerminology = new EasyTermViewer.TerminologyListBox();
            this.lstTerms = new EasyTermViewer.TermListBox();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.termInfoControl = new EasyTermCore.TermInfoControl();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.txtFind,
            this.btnFind,
            this.toolStripSeparator1,
            this.cmdLanguage1,
            this.toolStripLabel1,
            this.cmdLanguage2,
            this.cmdTermBases});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(814, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // txtFind
            // 
            this.txtFind.Name = "txtFind";
            this.txtFind.Size = new System.Drawing.Size(150, 25);
            this.txtFind.ToolTipText = "Enter text or term to find";
            this.txtFind.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtFind_KeyDown);
            this.txtFind.TextChanged += new System.EventHandler(this.txtFind_TextChanged);
            // 
            // btnFind
            // 
            this.btnFind.AutoSize = false;
            this.btnFind.DropDownButtonWidth = 20;
            this.btnFind.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnFindText,
            this.btnFindTerm});
            this.btnFind.Image = ((System.Drawing.Image)(resources.GetObject("btnFind.Image")));
            this.btnFind.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(149, 22);
            this.btnFind.Text = "<FindType>";
            this.btnFind.ButtonClick += new System.EventHandler(this.btnFind_ButtonClick);
            // 
            // btnFindText
            // 
            this.btnFindText.Image = global::EasyTermViewer.Properties.Resources.search;
            this.btnFindText.Name = "btnFindText";
            this.btnFindText.Size = new System.Drawing.Size(141, 22);
            this.btnFindText.Text = "Term List";
            this.btnFindText.ToolTipText = "Lists all terms. Enter text to filter the list.";
            this.btnFindText.Click += new System.EventHandler(this.btnFindText_Click);
            // 
            // btnFindTerm
            // 
            this.btnFindTerm.Name = "btnFindTerm";
            this.btnFindTerm.Size = new System.Drawing.Size(141, 22);
            this.btnFindTerm.Text = "Terminology";
            this.btnFindTerm.ToolTipText = "Enter text and press Enter for a terminology search.";
            this.btnFindTerm.Click += new System.EventHandler(this.btnFindTerm_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // cmdLanguage1
            // 
            this.cmdLanguage1.AutoSize = false;
            this.cmdLanguage1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmdLanguage1.Name = "cmdLanguage1";
            this.cmdLanguage1.Size = new System.Drawing.Size(160, 23);
            this.cmdLanguage1.ToolTipText = "Source language";
            this.cmdLanguage1.SelectedIndexChanged += new System.EventHandler(this.cmdLanguage1_SelectedIndexChanged);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.AutoSize = false;
            this.toolStripLabel1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripLabel1.Image = global::EasyTermViewer.Properties.Resources.ArrowRight;
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(20, 22);
            this.toolStripLabel1.Text = "toolStripLabel1";
            // 
            // cmdLanguage2
            // 
            this.cmdLanguage2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmdLanguage2.Name = "cmdLanguage2";
            this.cmdLanguage2.Size = new System.Drawing.Size(160, 25);
            this.cmdLanguage2.ToolTipText = "Target language";
            this.cmdLanguage2.SelectedIndexChanged += new System.EventHandler(this.cmdLanguage2_SelectedIndexChanged);
            // 
            // cmdTermBases
            // 
            this.cmdTermBases.AccessibleRole = System.Windows.Forms.AccessibleRole.OutlineButton;
            this.cmdTermBases.Image = global::EasyTermViewer.Properties.Resources.Data;
            this.cmdTermBases.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cmdTermBases.Name = "cmdTermBases";
            this.cmdTermBases.Size = new System.Drawing.Size(92, 22);
            this.cmdTermBases.Text = "Termbases...";
            this.cmdTermBases.ToolTipText = "Manage termbases";
            this.cmdTermBases.Click += new System.EventHandler(this.cmdTermBases_Click);
            // 
            // timerFilter
            // 
            this.timerFilter.Tick += new System.EventHandler(this.timerFilter_Tick);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 33);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.lstTerminology);
            this.splitContainer1.Panel1.Controls.Add(this.lstTerms);
            this.splitContainer1.Panel1MinSize = 100;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.termInfoControl);
            this.splitContainer1.Size = new System.Drawing.Size(814, 477);
            this.splitContainer1.SplitterDistance = 210;
            this.splitContainer1.SplitterWidth = 8;
            this.splitContainer1.TabIndex = 4;
            // 
            // lstTerminology
            // 
            this.lstTerminology.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstTerminology.FullRowSelect = true;
            this.lstTerminology.Location = new System.Drawing.Point(0, 240);
            this.lstTerminology.Name = "lstTerminology";
            this.lstTerminology.OwnerDraw = true;
            this.lstTerminology.Size = new System.Drawing.Size(205, 245);
            this.lstTerminology.TabIndex = 4;
            this.lstTerminology.UseCompatibleStateImageBehavior = false;
            this.lstTerminology.View = System.Windows.Forms.View.Details;
            this.lstTerminology.VirtualMode = true;
            this.lstTerminology.SelectedIndexChanged += new System.EventHandler(this.lstTerminology_SelectedIndexChanged);
            // 
            // lstTerms
            // 
            this.lstTerms.BackColor = System.Drawing.SystemColors.Window;
            this.lstTerms.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstTerms.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lstTerms.FullRowSelect = true;
            this.lstTerms.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lstTerms.HideSelection = false;
            this.lstTerms.Location = new System.Drawing.Point(2, 3);
            this.lstTerms.MultiSelect = false;
            this.lstTerms.Name = "lstTerms";
            this.lstTerms.OwnerDraw = true;
            this.lstTerms.Size = new System.Drawing.Size(203, 231);
            this.lstTerms.TabIndex = 3;
            this.lstTerms.UseCompatibleStateImageBehavior = false;
            this.lstTerms.View = System.Windows.Forms.View.Details;
            this.lstTerms.VirtualMode = true;
            this.lstTerms.SelectedIndexChanged += new System.EventHandler(this.lstTerms_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 96;
            // 
            // termInfoControl
            // 
            this.termInfoControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.termInfoControl.Location = new System.Drawing.Point(0, 0);
            this.termInfoControl.Name = "termInfoControl";
            this.termInfoControl.Size = new System.Drawing.Size(596, 477);
            this.termInfoControl.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 25);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(814, 8);
            this.panel1.TabIndex = 5;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(814, 510);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.toolStrip1);
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(800, 500);
            this.Name = "MainForm";
            this.Text = "Easy Term Viewer";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton cmdTermBases;
        private TermListBox lstTerms;
        private System.Windows.Forms.Timer timerFilter;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ToolStripComboBox cmdLanguage1;
        private System.Windows.Forms.ToolStripComboBox cmdLanguage2;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private EasyTermCore.TermInfoControl termInfoControl;
        private System.Windows.Forms.ToolStripTextBox txtFind;
        private System.Windows.Forms.ToolStripSplitButton btnFind;
        private System.Windows.Forms.ToolStripMenuItem btnFindText;
        private System.Windows.Forms.ToolStripMenuItem btnFindTerm;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.ToolTip toolTip1;
        private TerminologyListBox lstTerminology;
        private System.Windows.Forms.Panel panel1;
    }
}

