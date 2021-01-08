using System;
using System.Windows.Input;
using LearnerApp.Common;
using LearnerApp.Common.Helper;
using LearnerApp.Helper;
using LearnerApp.Models;
using LearnerApp.Resources.Texts;
using LearnerApp.Services.Download;
using LearnerApp.ViewModels.Base;
using Plugin.Toast;
using Xamarin.Forms;

namespace LearnerApp.Controls.ImagePopup
{
    public class ImagePopupViewModel : BaseViewModel
    {
        private readonly StressActionHandler _stressActionHandler;

        private readonly string _imageUrl;
        private bool _isDownloading;
        private double _progressBarDownload;

        public ImagePopupViewModel(string imageUrl)
        {
            _stressActionHandler = new StressActionHandler();
            _imageUrl = imageUrl;
        }

        public Func<DownloadInfoModel> GetDownloadUrl { get; set; }

        public string ImageUrl => _imageUrl;

        public bool IsDownloadButtonVisible => GetDownloadUrl != null;

        public ICommand DownloadFileCommand => new Command(DownloadFile);

        public bool IsDownloading
        {
            get
            {
                return _isDownloading;
            }

            set
            {
                _isDownloading = value;
                RaisePropertyChanged(() => IsDownloading);
            }
        }

        public double ProgressBarDownload
        {
            get
            {
                return _progressBarDownload;
            }

            set
            {
                _progressBarDownload = value;
                RaisePropertyChanged(() => ProgressBarDownload);
            }
        }

        private async void DownloadFile()
        {
            IsDownloading = false;
            await _stressActionHandler.RunAsync(async () =>
            {
                IsDownloading = true;
                var model = GetDownloadUrl?.Invoke();

                var downloadHelper = new DownloadHelper();
                await downloadHelper.StartDownloadFile(model, HandleDownload);
            });
        }

        private void HandleDownload(object sender, DownloadEventArgs e)
        {
            if (e.DownloadState == DownloadState.Failed)
            {
                IsDownloading = false;
            }

            if (e.DownloadState == DownloadState.Completed)
            {
                CrossToastPopUp.Current.ShowToastSuccess(TextsResource.DOWNLOAD_SUCCESSFULLY);
                IsDownloading = false;
            }

            ProgressBarDownload = e.ProgressPercentage / 100;
        }
    }
}
