using System;
using System.IO;
using System.Threading.Tasks;
using Android.OS;
using LearnerApp.Droid.Services;
using LearnerApp.Plugins.ScreenRecorder;

#pragma warning disable

[assembly: Xamarin.Forms.Dependency(typeof(MediaService))]

namespace LearnerApp.Droid.Services
{
    public class MediaService : IMediaService
    {
        public Task<SaveVideoToPhotosResult> SaveVideoToPhotos(string sourcePath)
        {
            var fileName = Path.GetFileName(sourcePath);
            var fileDestination = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDcim) + $"/Screen recordings/{fileName}";

            try
            {
                File.Copy(sourcePath, fileDestination);
                return Task.FromResult(SaveVideoToPhotosResult.Succeeded);
            }
            catch (Exception e)
            {
                return Task.FromResult(SaveVideoToPhotosResult.Unidentified);
            }
        }
    }
}
