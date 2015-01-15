using System.IO;

namespace StreamLauncher.Util
{
    public class FileHelper : IFileHelper
    {
        public bool FileExists(string path)
        {
            return File.Exists(path);
        }
    }
}