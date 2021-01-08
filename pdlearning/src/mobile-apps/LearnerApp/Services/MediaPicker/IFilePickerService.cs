using System.Threading.Tasks;
using Plugin.FilePicker.Abstractions;
using Plugin.Media.Abstractions;

namespace LearnerApp.Services.MediaPicker
{
    public interface IFilePickerService
    {
        Task<MediaFile> PickPhotoAsync(PickMediaOptions options);

        Task<FileData> PickAFileAsync(string[] allowedTypes);
    }
}
