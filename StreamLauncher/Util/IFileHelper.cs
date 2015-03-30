namespace StreamLauncher.Util
{
    public interface IFileHelper
    {
        bool FileExistsCaseSensitive(string path);
        bool FileExists(string path);
    }
}