using System;
using System.IO;
using LearnerApp.iOS.Services;
using LearnerApp.Services.Download;

[assembly: Xamarin.Forms.Dependency(typeof(PathDownload))]

namespace LearnerApp.iOS.Services
{
    public class PathDownload : IPathDownload
    {
        public string GetPath(string folderName)
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), folderName);
        }
    }
}
