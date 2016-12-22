namespace MySync.Client.UI
{
    partial class Message
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
            this.title = new MetroFramework.Controls.MetroLabel();
            this.messageText = new MetroFramework.Controls.MetroLabel();
            this.buttonOk = new MetroFramework.Controls.MetroTile();
            this.SuspendLayout();
            // 
            // title
            // 
            this.title.AutoSize = true;
            this.title.Location = new System.Drawing.Point(23, 13);
            this.title.Name = "title";
            this.title.Size = new System.Drawing.Size(33, 19);
            this.title.TabIndex = 0;
            this.title.Text = "Title";
            // 
            // messageText
            // 
            this.messageText.AutoSize = true;
            this.messageText.FontSize = MetroFramework.MetroLabelSize.Tall;
            this.messageText.Location = new System.Drawing.Point(23, 32);
            this.messageText.Name = "messageText";
            this.messageText.Size = new System.Drawing.Size(78, 25);
            this.messageText.TabIndex = 1;
            this.messageText.Text = "Message";
            // 
            // buttonOk
            // 
            this.buttonOk.ActiveControl = null;
            this.buttonOk.Location = new System.Drawing.Point(446, 56);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(42, 38);
            this.buttonOk.Style = MetroFramework.MetroColorStyle.Orange;
            this.buttonOk.TabIndex = 2;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseSelectable = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // Message
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 106);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.messageText);
            this.Controls.Add(this.title);
            this.DisplayHeader = false;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(500, 106);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(500, 106);
            this.Name = "Message";
            this.Padding = new System.Windows.Forms.Padding(20, 30, 20, 20);
            this.Resizable = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Style = MetroFramework.MetroColorStyle.Orange;
            this.Text = "Message";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MetroFramework.Controls.MetroLabel title;
        private MetroFramework.Controls.MetroLabel messageText;
        private MetroFramework.Controls.MetroTile buttonOk;
    }
}