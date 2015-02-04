namespace StreamLauncher.Services
{
    public interface IMessengerService
    {
        void Send<TMessage>(TMessage message);
        void Send<TMessage>(TMessage message, object token);
    }
}