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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Staged files", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("Unstaged files", System.Windows.Forms.HorizontalAlignment.Left);
            this.pull = new MetroFramework.Controls.MetroTile();
            this.push = new MetroFramework.Controls.MetroTile();
            this.history = new MetroFramework.Controls.MetroTile();
            this.stage = new MetroFramework.Controls.MetroTile();
            this.unstage = new MetroFramework.Controls.MetroTile();
            this.files = new System.Windows.Forms.ListView();
            this.column = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.stageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unstageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.discardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenu.SuspendLayout();
            this.SuspendLayout();
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
            // files
            // 
            this.files.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.files.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.files.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.column});
            this.files.ContextMenuStrip = this.contextMenu;
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
            this.files.TabIndex = 10;
            this.files.UseCompatibleStateImageBehavior = false;
            this.files.View = System.Windows.Forms.View.Details;
            // 
            // column
            // 
            this.column.Text = "";
            // 
            // contextMenu
            // 
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.stageToolStripMenuItem,
            this.unstageToolStripMenuItem,
            this.discardToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.lockToolStripMenuItem});
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.Size = new System.Drawing.Size(153, 136);
            // 
            // stageToolStripMenuItem
            // 
            this.stageToolStripMenuItem.Name = "stageToolStripMenuItem";
            this.stageToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.stageToolStripMenuItem.Text = "Stage";
            this.stageToolStripMenuItem.Click += new System.EventHandler(this.stageToolStripMenuItem_Click);
            // 
            // unstageToolStripMenuItem
            // 
            this.unstageToolStripMenuItem.Name = "unstageToolStripMenuItem";
            this.unstageToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.unstageToolStripMenuItem.Text = "Unstage";
            this.unstageToolStripMenuItem.Click += new System.EventHandler(this.unstageToolStripMenuItem_Click);
            // 
            // discardToolStripMenuItem
            // 
            this.discardToolStripMenuItem.Name = "discardToolStripMenuItem";
            this.discardToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.discardToolStripMenuItem.Text = "Discard";
            this.discardToolStripMenuItem.Click += new System.EventHandler(this.discardToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // lockToolStripMenuItem
            // 
            this.lockToolStripMenuItem.Name = "lockToolStripMenuItem";
            this.lockToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.lockToolStripMenuItem.Text = "Lock";
            this.lockToolStripMenuItem.Click += new System.EventHandler(this.lockToolStripMenuItem_Click);
            // 
            // ProjectView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.files);
            this.Controls.Add(this.unstage);
            this.Controls.Add(this.stage);
            this.Controls.Add(this.history);
            this.Controls.Add(this.push);
            this.Controls.Add(this.pull);
            this.Name = "ProjectView";
            this.Size = new System.Drawing.Size(778, 470);
            this.Style = MetroFramework.MetroColorStyle.Orange;
            this.Load += new System.EventHandler(this.ProjectView_Load);
            this.contextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private MetroFramework.Controls.MetroTile pull;
        private MetroFramework.Controls.MetroTile push;
        private MetroFramework.Controls.MetroTile history;
        private MetroFramework.Controls.MetroTile stage;
        private MetroFramework.Controls.MetroTile unstage;
        private System.Windows.Forms.ListView files;
        private System.Windows.Forms.ColumnHeader column;
        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolStripMenuItem stageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem unstageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem discardToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lockToolStripMenuItem;
    }
}
