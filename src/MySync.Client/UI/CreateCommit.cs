// MySync © 2016 Damian 'Erdroy' Korczowski


using System.Windows.Forms;
using MetroFramework.Forms;

namespace MySync.Client.UI
{
    public partial class CreateCommit : MetroForm
    {
        public static string CommitDesc;

        public CreateCommit()
        {
            InitializeComponent();
        }

        public static DialogResult CreateNew()
        {
            var wnd = new CreateCommit();
            return wnd.ShowDialog();
        }

        private void commit_Click(object sender, System.EventArgs e)
        {
            CommitDesc = desc.Text;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void cancel_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}