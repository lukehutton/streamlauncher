namespace StreamLauncher.Wpf.ViewModel
{
    public interface IViewModelLocator
    {
        MainViewModel Main { get; }
        StreamsViewModel Streams { get; }
        ILoginViewModel Login { get; }
        ISettingsViewModel Settings { get; }
        IChooseFeedsViewModel ChooseFeeds { get; }
    }
}