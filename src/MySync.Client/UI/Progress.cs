// MySync © 2016 Damian 'Erdroy' Korczowski

using System;
using MetroFramework.Forms;

namespace MySync.Client.UI
{
    public partial class Progress : MetroForm
    {
        public static Progress Current;

        public Progress()
        {
            InitializeComponent();
        }

        private void Progress_Load(object sender, EventArgs e)
        {

        }

        public static void ShowWindow(string title)
        {
            if(Current != null)
                throw new Exception("Cannot show second window.");

            // show window
            Current = new Progress
            {
                title =
                {
                    Text = title
                }
            };
            MainWindow.Instance.AddOwnedForm(Current);
            Current.Show();
        }

        public static void CloseWindow()
        {
            Current.Close();
            Current = null;

            MainWindow.Instance.FocusMe();
        }
        
        public static string Message
        {
            get { return Current.message.Text; }
            set { Current.message.Text = value; }
        }
    }
}