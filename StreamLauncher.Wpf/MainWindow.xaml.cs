using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.ServiceLocation;
using StreamLauncher.Wpf.ViewModel;

namespace StreamLauncher.Wpf
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var main = ServiceLocator.Current.GetInstance<MainViewModel>();
            main.AuthenticateUser();

            Messenger.Default.Register<NotificationMessage>(this, HandleNotificationMessage);
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
    }
}