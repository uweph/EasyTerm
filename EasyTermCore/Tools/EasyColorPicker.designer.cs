namespace EasyTermCore
{
    partial class EasyColorPicker
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.linkMore = new System.Windows.Forms.LinkLabel();
            this.lblColors = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // linkMore
            // 
            this.linkMore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.linkMore.AutoSize = true;
            this.linkMore.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.linkMore.Location = new System.Drawing.Point(2, 213);
            this.linkMore.Name = "linkMore";
            this.linkMore.Size = new System.Drawing.Size(40, 13);
            this.linkMore.TabIndex = 4;
            this.linkMore.TabStop = true;
            this.linkMore.Text = "More...";
            this.linkMore.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkMore_LinkClicked);
            // 
            // lblColors
            // 
            this.lblColors.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblColors.Location = new System.Drawing.Point(0, 0);
            this.lblColors.Name = "lblColors";
            this.lblColors.Size = new System.Drawing.Size(210, 201);
            this.lblColors.TabIndex = 5;
            this.lblColors.Paint += new System.Windows.Forms.PaintEventHandler(this.lblColors_Paint);
            this.lblColors.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lblColors_MouseClick);
            this.lblColors.MouseLeave += new System.EventHandler(this.lblColors_MouseLeave);
            this.lblColors.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lblColors_MouseMove);
            // 
            // EasyColorPicker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblColors);
            this.Controls.Add(this.linkMore);
            this.Name = "EasyColorPicker";
            this.Size = new System.Drawing.Size(210, 232);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.LinkLabel linkMore;
        private System.Windows.Forms.Label lblColors;
    }
}
