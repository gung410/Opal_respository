using System;
using System.Windows.Input;
using LearnerApp.Controls.ImagePopup;
using LearnerApp.Models;
using LearnerApp.Models.Achievement;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace LearnerApp.ViewModels.Achievement.ECertificate
{
    public class AchievementBadgeItemViewModel
    {
        private DateTime _receivedDate;

        public AchievementBadgeItemViewModel(AchievementBadgeModel dto)
        {
            ImageUrl = dto.BadgeImageUrl;
            BadgeName = dto.BadgeName;
            AdditionalInfo = dto.AdditionalInfo;
            _receivedDate = dto.IssuedDate;
        }

        public string ImageUrl { get; set; }

        public string BadgeName { get; set; }

        public string AdditionalInfo { get; set; }

        public string ReceivedDateStr => _receivedDate.ToLocalTime().ToString("dd/MM/yyyy");

        public ICommand OnBadgeClickedCommand => new Command(OnBadgeClicked);

        private void OnBadgeClicked()
        {
            var imagePopupVm = new ImagePopupViewModel(ImageUrl);
            imagePopupVm.GetDownloadUrl += () => new DownloadInfoModel(ImageUrl, BadgeName + ".png");

            PopupNavigation.Instance.PushAsync(new ImagePopupPage(imagePopupVm));
        }
    }
}
