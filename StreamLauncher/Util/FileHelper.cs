using System;
using System.IO;

namespace StreamLauncher.Util
{
    public class FileHelper : IFileHelper
    {
        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public bool FileExistsCaseSensitive(string path)
        {
            return InternalCaseSensitiveFileExists(path);
        }

        internal static bool InternalCaseSensitiveFileExists(string file)
        {
            string pathCheck = Path.GetDirectoryName(file);

            string filePart = Path.GetFileName(file);

            if (string.IsNullOrEmpty(pathCheck))
            {
                throw new ArgumentException("The file must include a full path", file);
            }

            string[] checkFiles = Directory.GetFiles(pathCheck, filePart, SearchOption.TopDirectoryOnly);
            if (checkFiles.Length > 0)
            {
                //must be a binary compare
                return Path.GetFileName(checkFiles[0]) == filePart;
            }

            return false;
        }
    }
}