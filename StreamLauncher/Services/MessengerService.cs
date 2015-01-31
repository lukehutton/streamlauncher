using GalaSoft.MvvmLight.Messaging;

namespace StreamLauncher.Services
{
    public class MessengerService : IMessengerService
    {
        public void Send<TMessage>(TMessage message)
        {
            Messenger.Default.Send(message); // todo send a key since we want choosefeedsviewmodel to catch the playing stream message
        }
    }
}