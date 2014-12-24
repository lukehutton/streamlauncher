using System.Windows;
using Microsoft.Practices.ServiceLocation;
using StreamLauncher.Wpf.ViewModel;

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
            var main = ServiceLocator.Current.GetInstance<MainViewModel>();
            main.AuthenticateUser();
        }        
    }
}