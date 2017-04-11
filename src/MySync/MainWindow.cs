using System;
using System.Windows.Forms;

namespace MySync
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            Controls.Add(Program.Browser);
        }
    }
}
