using System;
using System.IO;
using Xamarin.Essentials;

namespace LearnerApp.Helper
{
    public static class FilePathHelper
    {
        public static string RecordingPath => Path.Combine(DocumentFolder, "Recording");

        private static string DocumentFolder => Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        public static string GetFolderFilePath(string targetFolder, string fileName = null)
        {
            if (!Directory.Exists(targetFolder))
            {
                Directory.CreateDirectory(targetFolder);
            }

            if (fileName == null)
            {
                return targetFolder;
            }

            return Path.Combine(targetFolder, fileName);
        }
    }
}
