// MySync © 2016 Damian 'Erdroy' Korczowski


using System;
using System.Collections.Generic;
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

        private void ProjectView_Load(object sender, EventArgs e)
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
        
        private void pull_Click(object sender, EventArgs e)
        {
            Progress.ShowWindow("Pulling...");
            Progress.Message = "";
            TaskManager.QueueTask(
                delegate
                {
                    _project.Pull();
                },
                Progress.CloseWindow
            );
        }

        private void push_Click(object sender, EventArgs e)
        {
            var unstaged = new List<Commit.CommitEntry>();

            if (CreateCommit.CreateNew() == DialogResult.OK)
            {
                _project.CreateCommit(CreateCommit.CommitDesc);

                foreach (ListViewItem item in files.Items)
                {
                    var entry = (Commit.CommitEntry) item.Tag;

                    if (item.Group.Name == "staged")
                    {
                        _project.Commit.FileChanges.Add(entry);
                    }
                    else
                    {
                        unstaged.Add(entry);
                    }
                }

                if (_project.Commit.FileChanges.Count == 0)
                {
                    Message.ShowMessage("", "No changes!");
                    return;
                }

                _project.FileSystem.BuildFilemap();
                _project.Push(unstaged);
                UpdateFiles();
            }
        }

        private void history_Click(object sender, EventArgs e)
        {
            Message.NotImplemented();
        }

        private void stage_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in files.SelectedItems)
            {
                if (item.Group.Name == "unstaged")
                {
                    item.Group = files.Groups["staged"];
                }
            }
        }

        private void unstage_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in files.SelectedItems)
            {
                if (item.Group.Name == "staged")
                {
                    item.Group = files.Groups["unstaged"];
                }
            }
        }
        
        private void ProjectView_Resize(object sender, EventArgs e)
        {
            files.Columns[0].Width = files.Width - 20;
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            if (_project.FileSystem.Changed && MainWindow.Instance.Visible)
            {
                UpdateFiles();
            }
        }

        public void LoadProject(Project project)
        {
            _project = project;

            // update
            UpdateFiles();
        }

        public void UpdateFiles()
        {
            if (files.Columns.Count == 0)
            {
                var col = files.Columns.Add("Changes");
                col.Width = files.Width - 20;
            }

            var entries = FileMapping.BuildEntries(_project);
            files.Items.Clear();

            foreach (var entry in entries)
            {
                var item = new ListViewItem(files.Groups["unstaged"])
                {
                    Text = @"[" + entry.EntryType + @"] " + entry.Entry,
                    Tag = entry,
                    Name = entry.Entry
                };

                files.Items.Add(item);
            }

            _project.FileSystem.Changed = false;
        }

        private void stageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Progress.ShowWindow("Stage...");
            Progress.Message = "";
            TaskManager.QueueTask(
                delegate
                {
                    stage_Click(null, null);
                },
                Progress.CloseWindow
            );
        }

        private void unstageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Progress.ShowWindow("Unstage...");
            Progress.Message = "";
            TaskManager.QueueTask(
                delegate
                {
                    unstage_Click(null, null);
                },
                Progress.CloseWindow
            );
        }

        private void discardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Progress.ShowWindow("Discarding...");
            Progress.Message = "";
            TaskManager.QueueTask(
                delegate
                {
                    foreach (ListViewItem item in files.SelectedItems)
                    {
                        _project.Discard((Commit.CommitEntry)item.Tag);
                    }
                },
                Progress.CloseWindow
            );
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException("Not implemented");
        }

        private void lockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException("Not implemented");
        }
    }
}