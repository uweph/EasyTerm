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
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.cmdLanguage1 = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.cmdLanguage2 = new System.Windows.Forms.ToolStripComboBox();
            this.cmdTermBases = new System.Windows.Forms.ToolStripButton();
            this.txtFindTerm = new System.Windows.Forms.TextBox();
            this.timerFilter = new System.Windows.Forms.Timer(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.lstTerms = new EasyTermViewer.TermListBox();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.termInfoControl = new EasyTermViewer.TermInfoControl();
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
            // cmdLanguage1
            // 
            this.cmdLanguage1.AutoSize = false;
            this.cmdLanguage1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmdLanguage1.Name = "cmdLanguage1";
            this.cmdLanguage1.Size = new System.Drawing.Size(160, 23);
            this.cmdLanguage1.SelectedIndexChanged += new System.EventHandler(this.cmdLanguage_SelectedIndexChanged);
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
            this.cmdLanguage2.SelectedIndexChanged += new System.EventHandler(this.cmdLanguage_SelectedIndexChanged);
            // 
            // cmdTermBases
            // 
            this.cmdTermBases.AccessibleRole = System.Windows.Forms.AccessibleRole.OutlineButton;
            this.cmdTermBases.Image = global::EasyTermViewer.Properties.Resources.Data;
            this.cmdTermBases.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cmdTermBases.Name = "cmdTermBases";
            this.cmdTermBases.Size = new System.Drawing.Size(92, 22);
            this.cmdTermBases.Text = "Termbases...";
            this.cmdTermBases.Click += new System.EventHandler(this.cmdTermBases_Click);
            // 
            // txtFindTerm
            // 
            this.txtFindTerm.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFindTerm.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFindTerm.Location = new System.Drawing.Point(2, 4);
            this.txtFindTerm.Name = "txtFindTerm";
            this.txtFindTerm.Size = new System.Drawing.Size(203, 22);
            this.txtFindTerm.TabIndex = 2;
            this.txtFindTerm.TextChanged += new System.EventHandler(this.txtFindTerm_TextChanged);
            this.txtFindTerm.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtFindTerm_KeyDown);
            // 
            // timerFilter
            // 
            this.timerFilter.Tick += new System.EventHandler(this.timerFilter_Tick);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.lstTerms);
            this.splitContainer1.Panel1.Controls.Add(this.txtFindTerm);
            this.splitContainer1.Panel1MinSize = 100;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.termInfoControl);
            this.splitContainer1.Size = new System.Drawing.Size(814, 485);
            this.splitContainer1.SplitterDistance = 210;
            this.splitContainer1.TabIndex = 4;
            // 
            // lstTerms
            // 
            this.lstTerms.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstTerms.BackColor = System.Drawing.SystemColors.Window;
            this.lstTerms.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lstTerms.FullRowSelect = true;
            this.lstTerms.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lstTerms.HideSelection = false;
            this.lstTerms.Location = new System.Drawing.Point(2, 31);
            this.lstTerms.MultiSelect = false;
            this.lstTerms.Name = "lstTerms";
            this.lstTerms.OwnerDraw = true;
            this.lstTerms.Size = new System.Drawing.Size(203, 454);
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
            this.termInfoControl.Size = new System.Drawing.Size(600, 485);
            this.termInfoControl.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(814, 510);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip1);
            this.MinimumSize = new System.Drawing.Size(800, 500);
            this.Name = "MainForm";
            this.Text = "Easy Term Viewer";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton cmdTermBases;
        private System.Windows.Forms.TextBox txtFindTerm;
        private TermListBox lstTerms;
        private System.Windows.Forms.Timer timerFilter;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ToolStripComboBox cmdLanguage1;
        private System.Windows.Forms.ToolStripComboBox cmdLanguage2;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private TermInfoControl termInfoControl;
    }
}

