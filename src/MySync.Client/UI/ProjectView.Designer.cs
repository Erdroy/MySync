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
            System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Staged files", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("Unstaged files", System.Windows.Forms.HorizontalAlignment.Left);
            this.files = new MetroFramework.Controls.MetroListView();
            this.pull = new MetroFramework.Controls.MetroTile();
            this.push = new MetroFramework.Controls.MetroTile();
            this.history = new MetroFramework.Controls.MetroTile();
            this.stage = new MetroFramework.Controls.MetroTile();
            this.unstage = new MetroFramework.Controls.MetroTile();
            this.changed = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // files
            // 
            this.files.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.files.AllowSorting = true;
            this.files.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.files.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.files.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.changed});
            this.files.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.files.FullRowSelect = true;
            listViewGroup1.Header = "Staged files";
            listViewGroup1.Name = "staged";
            listViewGroup2.Header = "Unstaged files";
            listViewGroup2.Name = "unstaged";
            this.files.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2});
            this.files.Location = new System.Drawing.Point(69, 3);
            this.files.Name = "files";
            this.files.Size = new System.Drawing.Size(706, 464);
            this.files.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.files.Style = MetroFramework.MetroColorStyle.Orange;
            this.files.TabIndex = 4;
            this.files.UseCompatibleStateImageBehavior = false;
            this.files.UseSelectable = true;
            this.files.View = System.Windows.Forms.View.Details;
            this.files.SelectedIndexChanged += new System.EventHandler(this.files_SelectedIndexChanged);
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
            this.pull.Click += new System.EventHandler(this.pull_Click);
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
            this.push.Click += new System.EventHandler(this.push_Click);
            // 
            // history
            // 
            this.history.ActiveControl = null;
            this.history.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.history.Location = new System.Drawing.Point(3, 417);
            this.history.Name = "history";
            this.history.Size = new System.Drawing.Size(60, 50);
            this.history.Style = MetroFramework.MetroColorStyle.Orange;
            this.history.TabIndex = 7;
            this.history.Text = "History";
            this.history.UseSelectable = true;
            this.history.Click += new System.EventHandler(this.history_Click);
            // 
            // stage
            // 
            this.stage.ActiveControl = null;
            this.stage.Location = new System.Drawing.Point(3, 131);
            this.stage.Name = "stage";
            this.stage.Size = new System.Drawing.Size(60, 50);
            this.stage.Style = MetroFramework.MetroColorStyle.Orange;
            this.stage.TabIndex = 8;
            this.stage.Text = "Stage";
            this.stage.TileTextFontSize = MetroFramework.MetroTileTextSize.Small;
            this.stage.UseSelectable = true;
            this.stage.Click += new System.EventHandler(this.stage_Click);
            // 
            // unstage
            // 
            this.unstage.ActiveControl = null;
            this.unstage.Location = new System.Drawing.Point(3, 187);
            this.unstage.Name = "unstage";
            this.unstage.Size = new System.Drawing.Size(60, 50);
            this.unstage.Style = MetroFramework.MetroColorStyle.Orange;
            this.unstage.TabIndex = 9;
            this.unstage.Text = "Unstage";
            this.unstage.TileTextFontSize = MetroFramework.MetroTileTextSize.Small;
            this.unstage.UseSelectable = true;
            this.unstage.Click += new System.EventHandler(this.unstage_Click);
            // 
            // changed
            // 
            this.changed.Text = "Changed files";
            this.changed.Width = 485;
            // 
            // ProjectView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.unstage);
            this.Controls.Add(this.stage);
            this.Controls.Add(this.history);
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
        private MetroFramework.Controls.MetroTile history;
        private MetroFramework.Controls.MetroTile stage;
        private MetroFramework.Controls.MetroTile unstage;
        private System.Windows.Forms.ColumnHeader changed;
    }
}
