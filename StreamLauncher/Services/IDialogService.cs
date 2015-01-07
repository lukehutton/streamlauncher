using System;

namespace StreamLauncher.Services
{
    public interface IDialogService
    {
        void ShowError(string errorMessage, string title, string buttonText);
        void ShowError(Exception error, string title, string buttonText);
        void ShowMessage(string message, string title, string buttonText);
    }
}