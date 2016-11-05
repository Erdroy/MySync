using System;
using System.Windows.Forms;

namespace MySync.Client.UI
{
    public partial class InfoBox : Form
    {
        public static InfoBox Instance;

        public InfoBox()
        {
            InitializeComponent();
        }

        private void InfoBox_Load(object sender, EventArgs e)
        {

        }

        public static void ShowMessage(string title, string message = "")
        {
            Instance = new InfoBox();
            Instance.Show();
            Instance.Text = title;
            Instance.message.Text = message;
        }

        public static void HideMessage()
        {
            Instance.Close();
        }
    }
}