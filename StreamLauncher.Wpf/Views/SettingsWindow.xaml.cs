using System.Windows;
using System.Windows.Controls;

namespace StreamLauncher.Wpf.Views
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void LiveStreamerPath_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            ErrorMessage.Content = string.Empty;
        }

        private void MediaPlayerPath_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            ErrorMessage.Content = string.Empty;            
        }

        private void MediaPlayerArgs_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            ErrorMessage.Content = string.Empty;                        
        }
    }
}
