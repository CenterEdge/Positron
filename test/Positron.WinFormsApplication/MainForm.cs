using System;
using System.Windows.Forms;
using Positron.UI.WinFormsInterop;

namespace Positron.WinFormsApplication
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            var dialog = Program.WindowHandler.CreateWindowFromWinForms(this, "http://positron/Positron.Application");
            dialog.ShowDialog();
        }
    }
}
