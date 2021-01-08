using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using LearnerApp.Common;
using LearnerApp.Common.Helper;
using LearnerApp.Controls.ImagePopup;
using LearnerApp.Helper;
using LearnerApp.Models;
using LearnerApp.Models.Achievement;
using LearnerApp.Resources.Texts;
using LearnerApp.Services;
using LearnerApp.Services.Download;
using LearnerApp.Services.ServiceManager;
using LearnerApp.ViewModels.Base;
using Plugin.Toast;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace LearnerApp.ViewModels.Achievement.ECertificate
{
    public class AchievementECertificateItemViewModel : BaseViewModel
    {
        private readonly AchievementBackendServiceManager _achievementBackendServiceManager;
        private readonly StressActionHandler _stressActionHandler = new StressActionHandler();

        private DateTime _completionDate;
        private bool _isDownloading;
        private double _progressBarDownload;

        public AchievementECertificateItemViewModel(
            AchievementECertificateServiceManagerDto eCertificateManagerDto)
        {
            _achievementBackendServiceManager = new AchievementBackendServiceManager();

            var eCertificate = eCertificateManagerDto.ECertificate;
            ECertificateId = eCertificate.Id;
            CourseId = eCertificate.CourseId;
            if (eCertificateManagerDto.CourseExtendedInformation != null)
            {
                Tags = CourseCardBuilder.GetMetadataTags(eCertificateManagerDto.CourseExtendedInformation);
                CourseTitle = eCertificateManagerDto.CourseExtendedInformation.CourseName;
            }

            _completionDate = eCertificate.LearningCompletedDate;
        }

        public string ECertificateId { get; set; }

        public string CourseId { get; set; }

        public List<string> Tags { get; set; }

        public string CourseTitle { get; set; }

        public string CompletionDateString => _completionDate.ToLocalTime().ToString("dd/MM/yyyy");

        public ICommand OnItemClickedCommand => new Command(OnItemClicked);

        public ICommand OnDownloadFileCommand => new Command(DownloadFile);

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

        private async void OnItemClicked()
        {
            var imageUrl = await _achievementBackendServiceManager.GetCertificateImageFile(ECertificateId);

            var imagePopupVm = new ImagePopupViewModel(imageUrl);
            imagePopupVm.GetDownloadUrl += GetDownloadInfoModel;

            await PopupNavigation.Instance.PushAsync(new ImagePopupPage(imagePopupVm));
        }

        private async void DownloadFile()
        {
            IsDownloading = false;
            await _stressActionHandler.RunAsync(async () =>
            {
                IsDownloading = true;
                var model = GetDownloadInfoModel();

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

        private DownloadInfoModel GetDownloadInfoModel()
        {
            return new DownloadInfoModel(
                _achievementBackendServiceManager.GetDownloadUrlForECertificate(ECertificateId),
                CourseTitle + ".pdf");
        }
    }
}
