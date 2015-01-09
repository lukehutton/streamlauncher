namespace StreamLauncher.Services
{
    public interface IMessengerService
    {
        void Send<TMessage>(TMessage message);
    }
}