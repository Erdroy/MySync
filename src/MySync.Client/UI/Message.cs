// MySync © 2016 Damian 'Erdroy' Korczowski

using System;
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
            if (Progress.Current != null)
            {
                // lock dispatcher
                MainWindow.Lock = true;
            }

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
            
            
            msg.ShowDialog();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            MainWindow.Lock = false;
        }
    }
}