using System.Windows;
using System.Windows.Controls;

namespace StreamLauncher.Wpf.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void UserName_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            ErrorMessage.Content = string.Empty;
        }
    }
}
