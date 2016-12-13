using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MySync.Client.UI
{
    public partial class NewProject : Form
    {
        public const string Charset = "a-zA-Z0-9" + "-" + "_";

        public static string ProjectName;

        public NewProject()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            if (IsValid(inputName.Text))
            {
                ProjectName = inputName.Text;
                DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show(@"Project name contains illegal characters! Available: 'A-Z', 'a-z', '-', '_', '0-9', and the minimal/maximal length is: 4/17.");
            }
        }

        public static DialogResult CreateNew()
        {
            var wnd = new NewProject();
            return wnd.ShowDialog();
        }

        private static bool IsValid(string nickname)
        {
            if (nickname.Length < 3 || nickname.Length > 18)
                return false;

            return new Regex("^[" + Charset + "]+$").IsMatch(nickname);
        }
    }
}

