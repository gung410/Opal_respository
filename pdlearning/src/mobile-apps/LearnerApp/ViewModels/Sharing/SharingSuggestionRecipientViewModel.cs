using System.Windows.Input;
using LearnerApp.Common.Helper;
using LearnerApp.Models.UserOnBoarding;
using LearnerApp.ViewModels.Base;
using Xamarin.Forms;

namespace LearnerApp.ViewModels.Sharing
{
    public class SharingSuggestionRecipientViewModel : BaseViewModel
    {
        private readonly SharingContentFormViewModel _sharingContentFormViewModel;
        private readonly UserInformation _recipientDto;
        private readonly StressActionHandler _stressActionHandler = new StressActionHandler();

        private string _fullName;
        private string _emailAddress;
        private string _profilePictureUrl;
        private bool _isShared;
        private bool _isSendingShared;

        public SharingSuggestionRecipientViewModel(
            SharingContentFormViewModel sharingContentFormViewModel,
            UserInformation dto,
            bool isShared)
        {
            _sharingContentFormViewModel = sharingContentFormViewModel;

            _recipientDto = dto;
            _fullName = dto.FullName;
            _emailAddress = dto.EmailAddress;
            _profilePictureUrl = dto.AvatarUrl;
            IsShared = isShared;
        }

        public string FullName
        {
            get => _fullName;
            set
            {
                _fullName = value;
                RaisePropertyChanged(() => FullName);
            }
        }

        public string EmailAddress
        {
            get => _emailAddress;
            set
            {
                _emailAddress = value;
                RaisePropertyChanged(() => EmailAddress);
            }
        }

        public string ProfilePictureUrl
        {
            get => _profilePictureUrl;
            set
            {
                _profilePictureUrl = value;
                RaisePropertyChanged(() => ProfilePictureUrl);
            }
        }

        public bool IsSendingShare
        {
            get => _isSendingShared;
            set
            {
                _isSendingShared = value;
                RaisePropertyChanged(() => ShowShareButton);
                RaisePropertyChanged(() => ShowSharedView);
                RaisePropertyChanged(() => IsSendingShare);
            }
        }

        public bool IsShared
        {
            get => _isShared;
            set
            {
                _isShared = value;
                RaisePropertyChanged(() => IsShared);
            }
        }

        public bool ShowShareButton => IsSendingShare == false && IsShared == false;

        public bool ShowSharedView => IsSendingShare == false && IsShared == true;

        public ICommand AddToShareListCommand => new Command(AddToShareList);

        private async void AddToShareList(object obj)
        {
            await _stressActionHandler.RunAsync(async () =>
            {
                IsSendingShare = true;
                bool success = await _sharingContentFormViewModel.Share(_recipientDto);
                IsShared = success;
                IsSendingShare = false;
            });
        }
    }
}
