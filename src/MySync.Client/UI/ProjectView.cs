// MySync © 2016 Damian 'Erdroy' Korczowski
// under GPL-3.0 license

using System.Windows.Forms;
using MetroFramework.Controls;
using MySync.Client.Core;
using MySync.Client.Core.Projects;

namespace MySync.Client.UI
{
    public partial class ProjectView : MetroUserControl
    {
        private Project _project;

        public ProjectView()
        {
            InitializeComponent();
        }

        private void ProjectView_Load(object sender, System.EventArgs e)
        {
            Resize += ProjectView_Resize;
        }

        private void pull_Click(object sender, System.EventArgs e)
        {

        }

        private void push_Click(object sender, System.EventArgs e)
        {

        }

        private void history_Click(object sender, System.EventArgs e)
        {
            MessageBox.Show(@"Not implemented, yet.");
        }

        private void stage_Click(object sender, System.EventArgs e)
        {
            foreach (ListViewItem item in files.SelectedItems)
            {
                if (item.Group.Name == "unstaged")
                {
                    item.Group = files.Groups["staged"];
                }
            }
        }

        private void unstage_Click(object sender, System.EventArgs e)
        {
            foreach (ListViewItem item in files.SelectedItems)
            {
                if (item.Group.Name == "staged")
                {
                    item.Group = files.Groups["unstaged"];
                }
            }
        }

        private void files_SelectedIndexChanged(object sender, System.EventArgs e)
        {

        }

        private void ProjectView_Resize(object sender, System.EventArgs e)
        {
            files.Columns[0].Width = files.Width - 5;
        }

        public void LoadProject(Project project)
        {
            _project = project;

            // update
            UpdateFiles();
        }

        public void UpdateFiles()
        {
            files.Clear();
            var column = files.Columns.Add("Changes");
            files.HeaderStyle = ColumnHeaderStyle.None;

            column.Width = files.Width - 5;

            var entries = FileMapping.BuildEntries(_project);

            foreach (var entry in entries)
            {
                var item = new ListViewItem(files.Groups["unstaged"])
                {
                    Text = entry.Entry,
                    Tag = entry
                };

                files.Items.Add(item);
            }
        }
    }
}