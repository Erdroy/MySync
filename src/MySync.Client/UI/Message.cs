// MySync © 2016 Damian 'Erdroy' Korczowski

using System.Windows.Forms;
using MetroFramework.Forms;

namespace MySync.Client.UI
{
    public partial class Message : MetroForm
    {
        public Message()
        {
            InitializeComponent();
        }

        public static void NotImplemented()
        {
            ShowMessage("error(MS-0_NI)", "Not implemented feature.");
        }

        public static void ShowMessage(string title, string text)
        {
            var msg = new Message
            {
                title =
                {
                    Text = title
                },
                messageText =
                {
                    Text = text
                }
            };

            MainWindow.Instance.AddOwnedForm(msg);


            msg.ShowDialog();
        }

        private void buttonOk_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}