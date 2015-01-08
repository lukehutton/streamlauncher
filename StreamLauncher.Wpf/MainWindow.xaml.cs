using System;
using System.Windows;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.ServiceLocation;
using StreamLauncher.Services;
using StreamLauncher.Wpf.ViewModel;

namespace StreamLauncher.Wpf
{
    public partial class MainWindow : Window, IDialogService
    {
        public MainWindow()
        {            
            SimpleIoc.Default.Register<IDialogService>(() => this);            
            Messenger.Default.Register<NotificationMessage>(this, HandleNotificationMessage);            
            InitializeComponent();
            var main = ServiceLocator.Current.GetInstance<MainViewModel>();
            main.AuthenticateUser();
        }

        private void HandleNotificationMessage(NotificationMessage notificationMessage)
        {
            if (notificationMessage.Notification == "HideMainWindow")
            {
                if (notificationMessage.Sender == DataContext)
                    Hide();
            }
            else if (notificationMessage.Notification == "ShowMainWindow")
            {
                if (notificationMessage.Sender == DataContext)
                    Show();
            }
        }

        public void ShowError(string errorMessage, string title, string buttonText)
        {
            ShowMessage(errorMessage, title, buttonText);
        }

        public void ShowError(Exception error, string title, string buttonText)
        {
            ShowMessage(error.Message, title, buttonText);
        }

        public void ShowMessage(string message, string title, string buttonText)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK);
        }
    }
}