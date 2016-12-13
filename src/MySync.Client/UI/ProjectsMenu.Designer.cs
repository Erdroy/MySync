namespace MySync.Client.UI
{
    partial class ProjectsMenu
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
            this.labelTitle = new MetroFramework.Controls.MetroLabel();
            this.tabs = new MetroFramework.Controls.MetroTabControl();
            this.labelNewProject = new MetroFramework.Controls.MetroLabel();
            this.SuspendLayout();
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.Location = new System.Drawing.Point(3, 0);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(55, 19);
            this.labelTitle.TabIndex = 0;
            this.labelTitle.Text = "Projects";
            // 
            // tabs
            // 
            this.tabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabs.Location = new System.Drawing.Point(3, 22);
            this.tabs.Margin = new System.Windows.Forms.Padding(0);
            this.tabs.Name = "tabs";
            this.tabs.Size = new System.Drawing.Size(563, 405);
            this.tabs.Style = MetroFramework.MetroColorStyle.Orange;
            this.tabs.TabIndex = 1;
            this.tabs.UseSelectable = true;
            // 
            // labelNewProject
            // 
            this.labelNewProject.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelNewProject.AutoSize = true;
            this.labelNewProject.Cursor = System.Windows.Forms.Cursors.Hand;
            this.labelNewProject.Location = new System.Drawing.Point(473, 3);
            this.labelNewProject.Name = "labelNewProject";
            this.labelNewProject.Size = new System.Drawing.Size(93, 19);
            this.labelNewProject.TabIndex = 2;
            this.labelNewProject.Text = "Create Project";
            this.labelNewProject.Click += new System.EventHandler(this.labelNewProject_Click);
            // 
            // ProjectsMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labelNewProject);
            this.Controls.Add(this.tabs);
            this.Controls.Add(this.labelTitle);
            this.Name = "ProjectsMenu";
            this.Size = new System.Drawing.Size(569, 430);
            this.Load += new System.EventHandler(this.ProjectsMenu_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MetroFramework.Controls.MetroLabel labelTitle;
        private MetroFramework.Controls.MetroTabControl tabs;
        private MetroFramework.Controls.MetroLabel labelNewProject;
    }
}
