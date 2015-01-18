using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using log4net;
using log4net.Config;

namespace StreamLauncher.Wpf
{
    public partial class App : Application
    {
        private ILog _log;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            XmlConfigurator.Configure();

            _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

            DispatcherUnhandledException += OnAppDispatcherUnhandledException;
        }

        private void OnAppDispatcherUnhandledException(
            object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "streamlauncher");
            MessageBox.Show(string.Format("Sorry, an unhandled exception has occurred and has been logged to folder {0}. The application will shutdown.", folder)  +
                            Environment.NewLine + Environment.NewLine +
                            e.Exception.GetType().Name +
                            Environment.NewLine + Environment.NewLine +
                            e.Exception.Message);
            e.Handled = true;
            _log.Error(e.Exception);
            Shutdown();
        }
    }
}