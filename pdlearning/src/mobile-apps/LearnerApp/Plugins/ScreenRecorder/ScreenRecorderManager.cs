using System;
using System.IO;
using System.Threading.Tasks;
using LearnerApp.Services.Dialog;
using LearnerApp.Services.Navigation;
using LearnerApp.ViewModels.Recording;
using Xamarin.Forms;

namespace LearnerApp.Plugins.ScreenRecorder
{
    public class ScreenRecorderManager : RecordingPreviewPageTransferData.IRecordingPreviewDelegate
    {
        private readonly IScreenRecorderService _screenRecorderService;
        private readonly IMediaService _mediaService;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;

        public ScreenRecorderManager()
        {
            _screenRecorderService = DependencyService.Resolve<IScreenRecorderService>();
            _mediaService = DependencyService.Resolve<IMediaService>();
            _dialogService = DependencyService.Resolve<IDialogService>();
            _navigationService = DependencyService.Resolve<INavigationService>();
        }

        public async Task StartRecording()
        {
            if (await _screenRecorderService.IsRecording())
            {
                await _dialogService.ShowAlertAsync("An recording instance is running. Please stop the current recording process before starting a new one");
                return;
            }

            await StartRecord();
        }

        public async Task StopRecording()
        {
            string recordedPath = await _screenRecorderService.StopRecording();

            var navigationParams =
                RecordingPreviewPageViewModel.GetNavigationParams(
                    new RecordingPreviewPageTransferData(this, recordedPath, false));

            await _navigationService.NavigateToAsync<RecordingPreviewPageViewModel>(navigationParams);
        }

        public async Task<bool> OnUserCancel(string recordedPath)
        {
            var tcs = new TaskCompletionSource<bool>();
            await _dialogService.ConfirmAsync(
                "The recorded video will be lost. Are you sure to discard this video?", onConfirmed: isOk =>
                {
                    tcs.SetResult(isOk);
                });
            bool result = await tcs.Task;

            if (result)
            {
                DeleteFile(recordedPath);
            }

            return result;
        }

        public async Task<bool> OnTakeAgainRequest(string recordedPath)
        {
            var tcs = new TaskCompletionSource<bool>();
            await _dialogService.ConfirmAsync(
                "The current recorded video will be lost. Are you sure to discard this video?", onConfirmed: isOk =>
                {
                    tcs.SetResult(isOk);
                });

            bool result = await tcs.Task;

            if (result)
            {
                DeleteFile(recordedPath);
#pragma warning disable 4014
                Task.Run(async () =>
#pragma warning restore 4014
                {
                    await Task.Delay(500); // Because the key window is not visible at this time (It's showing as popup), so we have to wait a little for waiting the popup disappear
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await StartRecord();
                    });
                });
            }

            return result;
        }

        public async Task<bool> OnSaveRequest(string recordedPath)
        {
            SaveVideoToPhotosResult result = await _mediaService.SaveVideoToPhotos(recordedPath);
            if (result == SaveVideoToPhotosResult.NoPermission)
            {
                var tcs = new TaskCompletionSource<object>();
                await _dialogService.ShowAlertAsync(
                    "OPAL2 need to access to your photo gallery to save the recorded video. Please go to settings and enable this permission in order to use this feature",
                    onClosed: isOk =>
                    {
                        tcs.SetResult(isOk);
                    });
                await tcs.Task;
                return false;
            }

            if (result == SaveVideoToPhotosResult.Unidentified)
            {
                var tcs = new TaskCompletionSource<object>();
                await _dialogService.ShowAlertAsync(
                    "There is something wrong when trying to save the video. Please contact support for more information.",
                    onClosed: isOk =>
                    {
                        tcs.SetResult(isOk);
                    });
                await tcs.Task;
                return false;
            }

            DeleteFile(recordedPath);

            return true;
        }

        private async Task StartRecord()
        {
            await _screenRecorderService.StartRecording(this, GenerateVideoName(), async (string recordedPath) =>
            {
                var navigationParams =
                    RecordingPreviewPageViewModel.GetNavigationParams(
                        new RecordingPreviewPageTransferData(this, recordedPath, true));

                await _navigationService.NavigateToAsync<RecordingPreviewPageViewModel>(navigationParams);
            });
        }

        private void DeleteFile(string recordedPath)
        {
            try
            {
                File.Delete(recordedPath);
            }
            catch
            {
            }
        }

        private string GenerateVideoName()
        {
            return "screen_recording_" + DateTime.Now.ToString("ddMMyyyyhhmmss");
        }
    }
}
