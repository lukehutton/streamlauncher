using System.Windows;

namespace StreamLauncher.Util
{
    public class ApplicationDispatcher : IApplicationDispatcher
    {
        public void Shutdown()
        {
            Application.Current.Shutdown();
        }
    }
}