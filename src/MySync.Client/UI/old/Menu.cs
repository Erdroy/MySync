// MySync © 2016 Damian 'Erdroy' Korczowski
// under GPL-3.0 license

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using MySync.Client.Core;
using MySync.Client.Core.Projects;
using MySync.Client.Utilities;

namespace MySync.Client.UI
{
    public sealed partial class Menu : UserControl
    {
        private readonly Font _font;
        private readonly Font _fontAlt;

        private Point _clickPos;

        public List<string> Options = new List<string>();
        public List<Project> Projects = new List<Project>();

        public Menu()
        {
            InitializeComponent();

            MouseMove += Menu_MouseMove;
            Click += Menu_Click;
            DoubleBuffered = true;

            _font = new Font(new FontFamily("Calibri"), 16.0f);
            _fontAlt = new Font(new FontFamily("Calibri"), 32.0f);

            Options.Add("New project");
            Options.Add("Open project");
            Options.Add("Options");
            
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // load projects
            foreach (var projectToOpen in ClientSettings.Instance.OpenedProjects)
            {
                var project = ProjectsManager.Instance.OpenProject(projectToOpen.Name, projectToOpen.LocalDir);

                Projects.Add(project);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var gfx = e.Graphics;

            gfx.SmoothingMode = SmoothingMode.None;

            // draw background
            gfx.FillRectangle(Brushes.DarkSlateGray, new Rectangle(0, 0, Width, Height));

            // draw logo
            gfx.DrawString("MySync", _fontAlt, Brushes.Azure, 15, 15);

            var cursor = new Rectangle(Cursor.Position.X, Cursor.Position.Y, 1, 1);
            cursor = RectangleToClient(cursor);

            // draw options
            var offset = 100;
            foreach (var option in Options)
            {
                var stringSize = gfx.MeasureString(option, _font);
                var cliprect = new RectangleF(10, offset, stringSize.Width, stringSize.Height);

                if (cliprect.IntersectsWith(cursor))
                {
                    gfx.FillRectangle(Brushes.Chartreuse, new Rectangle(10, offset, 4, 25));
                    gfx.DrawString(option, _font, Brushes.Azure, 15, offset);

                    if (cliprect.IntersectsWith(new Rectangle(_clickPos, new Size(1, 1))))
                    {
                        OnOptionClicked(option);
                    }
                }
                else
                {
                    gfx.FillRectangle(Brushes.Crimson, new Rectangle(10, offset, 4, 25));
                    gfx.DrawString(option, _font, Brushes.Azure, 15, offset);
                }

                offset += 30;
            }

            offset += 15;

            // draw projects
            gfx.DrawString("PROJECTS", _font, Brushes.Azure, 15, offset);
            offset += 25;
            gfx.FillRectangle(Brushes.Crimson, new Rectangle(15, offset, 100, 2));
            offset += 25;

            foreach (var project in Projects)
            {
                var stringSize = gfx.MeasureString(project.Name, _font);
                var cliprect = new RectangleF(10, offset, stringSize.Width, stringSize.Height);

                if (cliprect.IntersectsWith(cursor))
                {
                    gfx.FillRectangle(Brushes.Chartreuse, new Rectangle(10, offset, 4, 25));
                    gfx.DrawString(project.Name, _font, Brushes.Azure, 15, offset);

                    if (cliprect.IntersectsWith(new Rectangle(_clickPos, new Size(1, 1))))
                    {
                        OnProjectClicked(project);
                    }
                }
                else
                {
                    gfx.FillRectangle(Brushes.Crimson, new Rectangle(10, offset, 4, 25));
                    gfx.DrawString(project.Name, _font, Brushes.Azure, 15, offset);
                }

                offset += 30;
            }

            _clickPos = Point.Empty;
        }

        private void Menu_MouseMove(object sender, MouseEventArgs e)
        {
            Invalidate();
        }
        
        private void Menu_Click(object sender, EventArgs e)
        {
            _clickPos = PointToClient(Cursor.Position);
        }

        private void OnOptionClicked(string clicked)
        {
            if (clicked == Options[0])
            {
                // new project
                if (NewProject.CreateNew() == DialogResult.OK)
                {
                    ProjectsManager.Instance.CreateProject(NewProject.ProjectName); // create project

                    // select the local project dir and open the project
                    var outputFolder = new FolderBrowserDialog
                    {
                        Description = @"Select where the project should be stored on local machine."
                    };

                    if (outputFolder.ShowDialog() == DialogResult.OK)
                    {
                        var project = ProjectsManager.Instance.OpenProject(NewProject.ProjectName, outputFolder.SelectedPath);

                        Projects.Add(project);
                        OnProjectClicked(project);

                        // save opened projects
                        ClientSettings.Instance.OpenedProjects = new ClientSettings.OpenedProject[Projects.Count];

                        for (var i = 0; i < Projects.Count; i++)
                        {
                            ClientSettings.Instance.OpenedProjects[i] = new ClientSettings.OpenedProject
                            {
                                Name = Projects[i].Name,
                                LocalDir = Projects[i].LocalDirectory
                            };
                        }
                        ClientSettings.Instance.Save();

                        return;
                    }
                }
            }
        }

        private void OnProjectClicked(Project project)
        {
            // TODO: Select opened project
            // TODO: Project view UI
        }
    }
}