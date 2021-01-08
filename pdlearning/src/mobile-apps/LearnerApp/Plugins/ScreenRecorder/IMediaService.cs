using System.Threading.Tasks;

namespace LearnerApp.Plugins.ScreenRecorder
{
    public interface IMediaService
    {
        Task<SaveVideoToPhotosResult> SaveVideoToPhotos(string mediaPath);
    }
}
