namespace StreamLauncher.Messages
{
    public class BusyStatusMessage
    {
        public BusyStatusMessage(
            bool isBusy,
            string status)
        {
            IsBusy = isBusy;
            Status = status;
        }

        public string Status { get; private set; }
        public bool IsBusy { get; private set; }
    }
}