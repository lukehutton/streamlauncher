using System;
using System.Windows;
using StreamLauncher.Repositories;

namespace StreamLauncher.Wpf
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Closed += OnClosed;
        }

        private void OnClosed(object sender, EventArgs e)
        {
            var userSettings = new UserSettings();
            userSettings.Save();
        }
    }
}