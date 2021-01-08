using System.Threading.Tasks;
using LearnerApp.Services.Dialog;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;

namespace LearnerApp.Services.MediaPicker
{
    public class FilePickerService : IFilePickerService
    {
        private readonly IDialogService _dialogService;

        public FilePickerService()
        {
            _dialogService = DependencyService.Resolve<IDialogService>();
        }

        public async Task<MediaFile> PickPhotoAsync(PickMediaOptions options)
        {
            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await _dialogService.ShowAlertAsync("Photos Not supported or permission not granted");

                return null;
            }

            return await CrossMedia.Current.PickPhotoAsync(options);
        }

        public async Task<FileData> PickAFileAsync(string[] allowedTypes)
        {
            FileData fileData = await CrossFilePicker.Current.PickFile(allowedTypes);

            return fileData;
        }
    }
}
