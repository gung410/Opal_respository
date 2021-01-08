using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using LearnerApp.Common;
using LearnerApp.Models.Course;
using LearnerApp.Services;
using LearnerApp.Services.Backend;
using LearnerApp.Services.Dialog;
using LearnerApp.Services.MediaPicker;
using LearnerApp.Services.Navigation;
using LearnerApp.ViewModels.Base;
using Plugin.FilePicker.Abstractions;
using Plugin.Toast;
using Xamarin.Forms;

namespace LearnerApp.ViewModels
{
    public class CannotParticipateViewModel : BasePageViewModel
    {
        private readonly ICourseBackendService _courseBackendService;
        private readonly IS3UploadService _uploadService;
        private readonly IFilePickerService _filePickerService;
        private readonly IFilePicker _filePicker;
        private readonly IDialogService _dialogService;

        private string _absenceReason;
        private string _fileUploadedPath;
        private string _attachFile;
        private string _sessionId;
        private string _absenceReasonSelected;
        private Color _absenceReasonBorderColor;
        private bool _isEnableAttachFile;
        private bool _isVisibleAbsenceReason;

        private Dictionary<string, string> _absenceReasonDict = new Dictionary<string, string>
        {
            { "Adoption/childcare/maternity/paternity leave", null },
            { "Sick/hospitalisation leave", null },
            { "Compassionate leave", null },
            { "Marriage leave", null },
            { "Operationally Ready National Service", null },
            { "Parent-care leave", null },
            { "Preparation for exam", null },
            { "Official duties", null },
            { "Other", null }
        };

        public CannotParticipateViewModel()
        {
            _filePickerService = DependencyService.Resolve<IFilePickerService>();
            _uploadService = DependencyService.Resolve<IS3UploadService>();
            _dialogService = DependencyService.Resolve<IDialogService>();

            _courseBackendService = CreateRestClientFor<ICourseBackendService>(GlobalSettings.BackendServiceCourse);
            _filePicker = DependencyService.Resolve<IFilePicker>();

            AbsenceReasonBorderColor = Color.LightGray;
            AttachFile = "Attach file";
            IsEnableAttachFile = true;

            AbsenceReasonSelected = _absenceReasonDict.ElementAt(0).Key;
            AbsenceReason = AbsenceReasonSelected.Equals("Other") ? string.Empty : AbsenceReasonSelected;
        }

        public bool IsEnableAttachFile
        {
            get
            {
                return _isEnableAttachFile;
            }

            set
            {
                _isEnableAttachFile = value;
                RaisePropertyChanged(() => IsEnableAttachFile);
            }
        }

        public string AttachFile
        {
            get
            {
                return _attachFile;
            }

            set
            {
                _attachFile = value;
                RaisePropertyChanged(() => AttachFile);
            }
        }

        public Color AbsenceReasonBorderColor
        {
            get
            {
                return _absenceReasonBorderColor;
            }

            set
            {
                _absenceReasonBorderColor = value;
                RaisePropertyChanged(() => AbsenceReasonBorderColor);
            }
        }

        public string AbsenceReason
        {
            get
            {
                return _absenceReason;
            }

            set
            {
                _absenceReason = value;

                AbsenceReasonBorderColor = !string.IsNullOrEmpty(value) ? Color.LightGray : Color.Red;

                RaisePropertyChanged(() => AbsenceReason);
            }
        }

        public string AbsenceReasonSelected
        {
            get
            {
                return _absenceReasonSelected;
            }

            set
            {
                _absenceReasonSelected = value;
                RaisePropertyChanged(() => AbsenceReasonSelected);
            }
        }

        public bool IsVisibleAbsenceReason
        {
            get
            {
                return _isVisibleAbsenceReason;
            }

            set
            {
                _isVisibleAbsenceReason = value;
                RaisePropertyChanged(() => IsVisibleAbsenceReason);
            }
        }

        public ICommand FilePickerCommand => new Command(async () => await OnPickAFile());

        public ICommand CancelCommand => new Command(async () => await OnCancel());

        public ICommand ConfirmCommand => new Command(async () => await OnConfirmed());

        public ICommand EnableUploadCommand => new Command(OnEnableUpload);

        public ICommand SelectAbsenceReasonCommand => new Command(SelectAbsenceReason);

        public override string PageTitle => string.Empty;

        public override string RoutingName => NavigationRoutes.CourseDetailsCanNotParticipate;

        protected override Task InternalNavigatedTo(NavigationParameters navigationParameters)
        {
            _sessionId = navigationParameters.GetParameter<string>("sessionId");

            return Task.CompletedTask;
        }

        private async Task OnPickAFile()
        {
            string[] allowedTypes = _filePicker.GetMineTypeForCannotParticipate();

            FileData fileData = await _filePickerService.PickAFileAsync(allowedTypes);

            if (fileData == null)
            {
                return;
            }

            // Check file size over 1MB
            int maximumSize = 1024 * 1024;

            if (fileData.DataArray.Length > maximumSize)
            {
                await DialogService.ShowAlertAsync("Maximum uploads file size: 1MB.", "Close", isVisibleIcon: false);

                return;
            }

            using (DialogService.DisplayLoadingIndicator())
            {
                _fileUploadedPath = await _uploadService.UploadFile(fileData, "learner-attachments");

                if (!string.IsNullOrEmpty(_fileUploadedPath))
                {
                    AttachFile = fileData.FileName;
                    IsEnableAttachFile = false;
                }
            }
        }

        private async Task OnConfirmed()
        {
            if (string.IsNullOrEmpty(AbsenceReason))
            {
                AbsenceReasonBorderColor = Color.Red;
            }
            else
            {
                var accountProperties = Application.Current.Properties.GetAccountProperties();

                if (accountProperties == null)
                {
                    return;
                }

                var absenceReason = new AbsenceReason
                {
                    SessionId = _sessionId,
                    UserId = accountProperties.User.Sub,
                    Reason = AbsenceReason
                };

                if (!string.IsNullOrEmpty(_fileUploadedPath))
                {
                    absenceReason.Attachment = new[] { _fileUploadedPath };
                }

                await ExecuteBackendService(() => _courseBackendService.ChangeAbsenceReason(absenceReason));

                MessagingCenter.Unsubscribe<CannotParticipateViewModel>(this, "submit-absence-reason-success");
                MessagingCenter.Send(this, "submit-absence-reason-success");

                CrossToastPopUp.Current.ShowToastSuccess("Your reason for absence is successfully submitted.");

                // Because Toast take time in iOS so NavigationService.RemoveLastFromNavigationStackAsync not trigger consistently
                // We delay about 1 second to trigger NavigationService.RemoveLastFromNavigationStackAsync
                if (Device.RuntimePlatform == Device.iOS)
                {
                    await Task.Delay(1000);
                }

                await NavigationService.GoBack();
            }
        }

        private void OnEnableUpload(object obj)
        {
            AttachFile = "Attach file";
            IsEnableAttachFile = true;
        }

        private async Task OnCancel()
        {
            await NavigationService.GoBack();
        }

        private void SelectAbsenceReason()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                if (IsBusy)
                {
                    return;
                }

                IsBusy = true;

                _dialogService.ShowDropDownSelectionPopup(_absenceReasonDict, isFullScreen: false, isSeparateStringByUppercase: false, onSelected: absenceReasonSelected =>
                {
                    if (string.IsNullOrEmpty(absenceReasonSelected))
                    {
                        return;
                    }

                    AbsenceReasonSelected = absenceReasonSelected;
                    IsVisibleAbsenceReason = absenceReasonSelected.Equals("Other");
                    AbsenceReason = absenceReasonSelected.Equals("Other") ? string.Empty : AbsenceReasonSelected;
                });

                IsBusy = false;
            });
        }
    }
}
