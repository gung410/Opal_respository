using System.IO;
using LearnerApp.Droid.Services;
using LearnerApp.Services.Download;

[assembly: Xamarin.Forms.Dependency(typeof(PathDownload))]

namespace LearnerApp.Droid.Services
{
    public class PathDownload : IPathDownload
    {
        public string GetPath(string folderName)
        {
            return Path.Combine(Android.OS.Environment.ExternalStorageDirectory?.AbsolutePath ?? string.Empty, folderName);
        }
    }
}
