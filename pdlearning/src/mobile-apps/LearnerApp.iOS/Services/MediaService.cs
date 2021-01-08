using System.Threading.Tasks;
using LearnerApp.iOS.Services;
using LearnerApp.Plugins.ScreenRecorder;

[assembly: Xamarin.Forms.Dependency(typeof(MediaService))]

namespace LearnerApp.iOS.Services
{
    public class MediaService : IMediaService
    {
        public Task<SaveVideoToPhotosResult> SaveVideoToPhotos(string mediaPath)
        {
            var tcs = new TaskCompletionSource<SaveVideoToPhotosResult>();
            UIKit.UIVideo.SaveToPhotosAlbum(mediaPath, (path, error) =>
            {
                if (error != null)
                {
                    tcs.SetResult(SaveVideoToPhotosResult.Unidentified);
                    return;
                }

                tcs.SetResult(SaveVideoToPhotosResult.Succeeded);
            });

            return tcs.Task;
        }
    }
}
