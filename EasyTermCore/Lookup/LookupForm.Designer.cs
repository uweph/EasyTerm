namespace EasyTermCore
{
    partial class LookupForm
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
            this.txtSrcTerm = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbMatches = new System.Windows.Forms.ComboBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.termInfoControl = new EasyTermCore.TermInfoControl();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Lookup Term";
            // 
            // txtSrcTerm
            // 
            this.txtSrcTerm.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSrcTerm.Location = new System.Drawing.Point(16, 30);
            this.txtSrcTerm.Name = "txtSrcTerm";
            this.txtSrcTerm.ReadOnly = true;
            this.txtSrcTerm.Size = new System.Drawing.Size(168, 22);
            this.txtSrcTerm.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(197, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Matches";
            // 
            // cmbMatches
            // 
            this.cmbMatches.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMatches.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbMatches.FormattingEnabled = true;
            this.cmbMatches.Location = new System.Drawing.Point(200, 30);
            this.cmbMatches.Name = "cmbMatches";
            this.cmbMatches.Size = new System.Drawing.Size(266, 24);
            this.cmbMatches.TabIndex = 4;
            this.cmbMatches.SelectedIndexChanged += new System.EventHandler(this.cmbMatches_SelectedIndexChanged);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(556, 12);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 5;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // termInfoControl
            // 
            this.termInfoControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.termInfoControl.Location = new System.Drawing.Point(16, 60);
            this.termInfoControl.Name = "termInfoControl";
            this.termInfoControl.Size = new System.Drawing.Size(619, 404);
            this.termInfoControl.TabIndex = 0;
            // 
            // LookupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(643, 474);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.cmbMatches);
            this.Controls.Add(this.txtSrcTerm);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.termInfoControl);
            this.MinimumSize = new System.Drawing.Size(650, 500);
            this.Name = "LookupForm";
            this.Text = "LookupForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TermInfoControl termInfoControl;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtSrcTerm;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbMatches;
        private System.Windows.Forms.Button btnClose;
    }
}