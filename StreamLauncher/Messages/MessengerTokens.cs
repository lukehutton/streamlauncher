using System;

namespace StreamLauncher.Messages
{
    public class MessengerTokens
    {
        public static readonly Guid MainViewModelToken = Guid.NewGuid();
        public static readonly Guid ChooseFeedsViewModelToken = new Guid();
    }
}