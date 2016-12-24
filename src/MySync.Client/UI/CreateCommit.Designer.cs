namespace MySync.Client.UI
{
    partial class CreateCommit
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
            this.desc = new MetroFramework.Controls.MetroTextBox();
            this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.commit = new MetroFramework.Controls.MetroTile();
            this.cancel = new MetroFramework.Controls.MetroTile();
            this.SuspendLayout();
            // 
            // desc
            // 
            this.desc.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // 
            // 
            this.desc.CustomButton.Image = null;
            this.desc.CustomButton.Location = new System.Drawing.Point(250, 1);
            this.desc.CustomButton.Name = "";
            this.desc.CustomButton.Size = new System.Drawing.Size(123, 123);
            this.desc.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.desc.CustomButton.TabIndex = 1;
            this.desc.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.desc.CustomButton.UseSelectable = true;
            this.desc.CustomButton.Visible = false;
            this.desc.Lines = new string[0];
            this.desc.Location = new System.Drawing.Point(23, 82);
            this.desc.MaxLength = 32767;
            this.desc.Multiline = true;
            this.desc.Name = "desc";
            this.desc.PasswordChar = '\0';
            this.desc.PromptText = "Enter commit description";
            this.desc.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.desc.SelectedText = "";
            this.desc.SelectionLength = 0;
            this.desc.SelectionStart = 0;
            this.desc.ShortcutsEnabled = true;
            this.desc.Size = new System.Drawing.Size(374, 125);
            this.desc.TabIndex = 0;
            this.desc.UseSelectable = true;
            this.desc.WaterMark = "Enter commit description";
            this.desc.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.desc.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // metroLabel1
            // 
            this.metroLabel1.AutoSize = true;
            this.metroLabel1.Location = new System.Drawing.Point(23, 60);
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Size = new System.Drawing.Size(126, 19);
            this.metroLabel1.TabIndex = 1;
            this.metroLabel1.Text = "Commit Description";
            // 
            // commit
            // 
            this.commit.ActiveControl = null;
            this.commit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.commit.Location = new System.Drawing.Point(322, 213);
            this.commit.Name = "commit";
            this.commit.Size = new System.Drawing.Size(75, 39);
            this.commit.Style = MetroFramework.MetroColorStyle.Orange;
            this.commit.TabIndex = 2;
            this.commit.Text = "Commit";
            this.commit.UseSelectable = true;
            this.commit.Click += new System.EventHandler(this.commit_Click);
            // 
            // cancel
            // 
            this.cancel.ActiveControl = null;
            this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancel.Location = new System.Drawing.Point(241, 213);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(75, 39);
            this.cancel.Style = MetroFramework.MetroColorStyle.Orange;
            this.cancel.TabIndex = 3;
            this.cancel.Text = "Cancel";
            this.cancel.UseSelectable = true;
            this.cancel.Click += new System.EventHandler(this.cancel_Click);
            // 
            // CreateCommit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(420, 275);
            this.Controls.Add(this.cancel);
            this.Controls.Add(this.commit);
            this.Controls.Add(this.metroLabel1);
            this.Controls.Add(this.desc);
            this.Name = "CreateCommit";
            this.ShowInTaskbar = false;
            this.Style = MetroFramework.MetroColorStyle.Orange;
            this.Text = "Commit";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MetroFramework.Controls.MetroTextBox desc;
        private MetroFramework.Controls.MetroLabel metroLabel1;
        private MetroFramework.Controls.MetroTile commit;
        private MetroFramework.Controls.MetroTile cancel;
    }
}