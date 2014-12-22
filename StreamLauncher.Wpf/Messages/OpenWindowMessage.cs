namespace StreamLauncher.Wpf.Messages
{
    public class OpenWindowMessage
    {
        public WindowType WindowType { get; set; }
        public string Argument { get; set; }
    }

    public enum WindowType
    {
        Modal,
        NonModal
    }
}