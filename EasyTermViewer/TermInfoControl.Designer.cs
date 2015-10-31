namespace EasyTermViewer
{
    partial class TermInfoControl
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
            this.webControl = new System.Windows.Forms.WebBrowser();
            this.txtTermBaseInfo = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // webControl
            // 
            this.webControl.AllowNavigation = false;
            this.webControl.AllowWebBrowserDrop = false;
            this.webControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.webControl.IsWebBrowserContextMenuEnabled = false;
            this.webControl.Location = new System.Drawing.Point(0, 31);
            this.webControl.MinimumSize = new System.Drawing.Size(20, 20);
            this.webControl.Name = "webControl";
            this.webControl.ScriptErrorsSuppressed = true;
            this.webControl.Size = new System.Drawing.Size(549, 464);
            this.webControl.TabIndex = 2;
            this.webControl.WebBrowserShortcutsEnabled = false;
            // 
            // txtTermBaseInfo
            // 
            this.txtTermBaseInfo.Location = new System.Drawing.Point(0, 0);
            this.txtTermBaseInfo.Name = "txtTermBaseInfo";
            this.txtTermBaseInfo.ReadOnly = true;
            this.txtTermBaseInfo.Size = new System.Drawing.Size(549, 20);
            this.txtTermBaseInfo.TabIndex = 3;
            // 
            // TermInfoControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtTermBaseInfo);
            this.Controls.Add(this.webControl);
            this.Name = "TermInfoControl";
            this.Size = new System.Drawing.Size(549, 498);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.WebBrowser webControl;
        private System.Windows.Forms.TextBox txtTermBaseInfo;
    }
}
