// MySync © 2016 Damian 'Erdroy' Korczowski


using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using MetroFramework.Forms;

namespace MySync.Client.UI
{
    public partial class CreateProject : MetroForm
    {
        public const string Charset = "a-zA-Z0-9" + "-" + "_";

        public static string ProjectName;
        public static string ServerIp;
        public static string Username;
        public static string Password;
        public static string ProjectDirectory;

        public CreateProject()
        {
            InitializeComponent();
        }

        private void CreateProject_Load(object sender, System.EventArgs e)
        {
            directory.Text = @"C:\\mysync\";
        }

        private void create_Click(object sender, System.EventArgs e)
        {
            if (IsValid(projectName.Text))
            {
                if (!Directory.Exists(directory.Text))
                {
                    Directory.CreateDirectory(directory.Text);
                }

                ProjectName = projectName.Text;
                ServerIp = serverAddress.Text;
                Username = username.Text;
                Password = password.Text;
                ProjectDirectory = directory.Text;

                // TODO: Validate server credidentals

                DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show(@"Project name contains illegal characters! Available: 'A-Z', 'a-z', '-', '_', '0-9', and the minimal/maximal length is: 4/17.");
            }
        }

        private void cancel_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void browse_Click(object sender, System.EventArgs e)
        {
            var openFolder = new FolderBrowserDialog();
            if (openFolder.ShowDialog() == DialogResult.OK)
            {
                directory.Text = openFolder.SelectedPath;
            }
        }

        public static DialogResult CreateNew()
        {
            var wnd = new CreateProject();
            return wnd.ShowDialog();
        }

        private static bool IsValid(string text)
        {
            if (text.Length < 3 || text.Length > 18)
                return false;

            return new Regex("^[" + Charset + "]+$").IsMatch(text);
        }
    }
}
