using System;
using System.Windows.Forms;

namespace StreamLauncher.Services
{
    public class DialogService : IDialogService
    {
        public void ShowError(string errorMessage, string title, string buttonText)
        {
            MessageBox.Show(errorMessage, title, MessageBoxButtons.OK);
        }

        public void ShowError(Exception error, string title, string buttonText)
        {
            MessageBox.Show(error.Message, title, MessageBoxButtons.OK);
        }

        public void ShowMessage(string message, string title, string buttonText)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK);
        }
    }
}