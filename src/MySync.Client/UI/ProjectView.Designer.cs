namespace MySync.Client.UI
{
    partial class ProjectView
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
            System.Windows.Forms.ListViewGroup listViewGroup3 = new System.Windows.Forms.ListViewGroup("Staged files", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup4 = new System.Windows.Forms.ListViewGroup("Unstaged files", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("assets.astdb");
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("game.exe");
            this.files = new MetroFramework.Controls.MetroListView();
            this.pull = new MetroFramework.Controls.MetroTile();
            this.push = new MetroFramework.Controls.MetroTile();
            this.SuspendLayout();
            // 
            // files
            // 
            this.files.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.files.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.files.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.files.FullRowSelect = true;
            listViewGroup3.Header = "Staged files";
            listViewGroup3.Name = "staged";
            listViewGroup4.Header = "Unstaged files";
            listViewGroup4.Name = "unstaged";
            this.files.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup3,
            listViewGroup4});
            listViewItem3.Group = listViewGroup3;
            listViewItem3.StateImageIndex = 0;
            listViewItem3.ToolTipText = "assets.astdb";
            listViewItem4.Group = listViewGroup4;
            listViewItem4.StateImageIndex = 0;
            this.files.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem3,
            listViewItem4});
            this.files.Location = new System.Drawing.Point(69, 3);
            this.files.Name = "files";
            this.files.OwnerDraw = true;
            this.files.Size = new System.Drawing.Size(706, 464);
            this.files.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.files.TabIndex = 4;
            this.files.UseCompatibleStateImageBehavior = false;
            this.files.UseSelectable = true;
            // 
            // pull
            // 
            this.pull.ActiveControl = null;
            this.pull.Location = new System.Drawing.Point(3, 3);
            this.pull.Name = "pull";
            this.pull.Size = new System.Drawing.Size(60, 50);
            this.pull.Style = MetroFramework.MetroColorStyle.Orange;
            this.pull.TabIndex = 5;
            this.pull.Text = "Pull";
            this.pull.UseSelectable = true;
            // 
            // push
            // 
            this.push.ActiveControl = null;
            this.push.Location = new System.Drawing.Point(3, 59);
            this.push.Name = "push";
            this.push.Size = new System.Drawing.Size(60, 50);
            this.push.Style = MetroFramework.MetroColorStyle.Orange;
            this.push.TabIndex = 6;
            this.push.Text = "Push";
            this.push.UseSelectable = true;
            // 
            // ProjectView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.push);
            this.Controls.Add(this.pull);
            this.Controls.Add(this.files);
            this.Name = "ProjectView";
            this.Size = new System.Drawing.Size(778, 470);
            this.Style = MetroFramework.MetroColorStyle.Orange;
            this.Load += new System.EventHandler(this.ProjectView_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private MetroFramework.Controls.MetroListView files;
        private MetroFramework.Controls.MetroTile pull;
        private MetroFramework.Controls.MetroTile push;
    }
}
