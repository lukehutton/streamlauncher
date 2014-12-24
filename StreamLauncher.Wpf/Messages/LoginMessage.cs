namespace StreamLauncher.Wpf.Messages
{
    public class LoginMessage
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ErrorMessage { get; set; }
        public bool RememberMe { get; set; }
    }
}