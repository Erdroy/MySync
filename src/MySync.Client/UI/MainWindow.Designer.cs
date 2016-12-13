namespace MySync.Client.UI
{
    partial class MainWindow
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
            this.labelCopyinfo = new MetroFramework.Controls.MetroLabel();
            this.SuspendLayout();
            // 
            // labelCopyinfo
            // 
            this.labelCopyinfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelCopyinfo.AutoSize = true;
            this.labelCopyinfo.Location = new System.Drawing.Point(0, 431);
            this.labelCopyinfo.Name = "labelCopyinfo";
            this.labelCopyinfo.Size = new System.Drawing.Size(264, 38);
            this.labelCopyinfo.TabIndex = 0;
            this.labelCopyinfo.Text = "MySync © 2016 Damian \'Erdroy\' Korczowski\r\n";
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(700, 450);
            this.Controls.Add(this.labelCopyinfo);
            this.MinimumSize = new System.Drawing.Size(700, 450);
            this.Name = "MainWindow";
            this.ShadowType = MetroFramework.Forms.MetroFormShadowType.None;
            this.Style = MetroFramework.MetroColorStyle.Orange;
            this.Text = "MySync";
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MetroFramework.Controls.MetroLabel labelCopyinfo;
    }
}