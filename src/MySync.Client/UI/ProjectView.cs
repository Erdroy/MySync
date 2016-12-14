// MySync © 2016 Damian 'Erdroy' Korczowski
// under GPL-3.0 license

using System.Linq;
using System.Windows.Forms;
using MetroFramework.Controls;
using MySync.Client.Core;
using MySync.Client.Core.Projects;

namespace MySync.Client.UI
{
    public partial class ProjectView : MetroUserControl
    {
        private Project _project;

        private Timer _updateTimer;

        public ProjectView()
        {
            InitializeComponent();
        }

        private void ProjectView_Load(object sender, System.EventArgs e)
        {
            Resize += ProjectView_Resize;

            _updateTimer = new Timer
            {
                Interval = 2000
            };

            _updateTimer.Start();
            _updateTimer.Tick += UpdateTimer_Tick;

            files.HeaderStyle = ColumnHeaderStyle.None;
        }
        
        private void pull_Click(object sender, System.EventArgs e)
        {
            //_project.Pull();
            MessageBox.Show(@"Not implemented, yet.");
        }

        private void push_Click(object sender, System.EventArgs e)
        {
            //_project.Push();
            MessageBox.Show(@"Not implemented, yet.");
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

        private void UpdateTimer_Tick(object sender, System.EventArgs e)
        {
            if (_project.FileSystem.Changed && MainWindow.Instance.Visible)
            {
                UpdateFiles();
            }
        }

        public void LoadProject(Project project)
        {
            _project = project;
            _project.FileSystem.BuildFilemap();

            // update
            UpdateFiles();
        }

        public void UpdateFiles()
        {
            if (files.Columns.Count == 0)
            {
                var column = files.Columns.Add("Changes");
                column.Width = files.Width - 5;
            }

            _project.FileSystem.BuildFilemap();
            var entries = FileMapping.BuildEntries(_project);
            files.Items.Clear();

            foreach (var entry in entries)
            {
                if (entry.EntryType != CommitEntryType.Deleted)
                {
                    var item = new ListViewItem(files.Groups["unstaged"])
                    {
                        Text = entry.Entry,
                        Tag = entry,
                        Name = entry.Entry
                    };

                    files.Items.Add(item);
                }
            }
        }
    }
}