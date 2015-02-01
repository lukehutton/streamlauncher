using GalaSoft.MvvmLight.Messaging;

namespace StreamLauncher.Services
{
    public class MessengerService : IMessengerService
    {
        public void Send<TMessage>(TMessage message)
        {
            Messenger.Default.Send(message); 
        }

        public void Send<TMessage>(TMessage message, object token)
        {
            Messenger.Default.Send(message, token);
        }
    }
}