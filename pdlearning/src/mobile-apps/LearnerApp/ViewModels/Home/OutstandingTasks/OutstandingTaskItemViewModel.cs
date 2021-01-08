using System;
using System.Windows.Input;
using LearnerApp.Common.MessagingCenterManager;
using LearnerApp.Converters;
using LearnerApp.Converters.StandaloneForm;
using LearnerApp.Helper;
using LearnerApp.Models.Learner;
using LearnerApp.Models.OutstandingTask;
using LearnerApp.Services.Navigation;
using LearnerApp.ViewModels.Base;
using Xamarin.Forms;

namespace LearnerApp.ViewModels.Home.OutstandingTasks
{
    public class OutstandingTaskItemViewModel : BaseViewModel
    {
        private string _title;
        private float _progress;
        private DateTime? _dueDate;
        private OutstandingTaskStatusEum _status;
        private OutstandingTaskTypeEnum _type;
        private string _fileExtension;
        private bool _canStart;
        private DateTime? _startDate;
        private INavigationService _navigationService;
        private OutstandingTask _task;

        public OutstandingTaskItemViewModel(OutstandingTask task)
        {
            _navigationService = DependencyService.Resolve<INavigationService>();

            _task = task;
            _title = task.Name;
            _status = task.Status;
            _dueDate = task.DueDate;
            _progress = task.Progress / 100;
            _type = task.Type;
            _fileExtension = task.FileExtension;
            _startDate = task.StartDate;
            _canStart = task.StartDate == null || task.StartDate <= DateTime.Now;
        }

        public string Id => _task.Id;

        public OutstandingTaskStatusEum Status
        {
            get => _status;
            set => _status = value;
        }

        public string FileExtension
        {
            get => _fileExtension;
        }

        public ImageSource ThumbnailImageSource
        {
            get
            {
                switch (_type)
                {
                    case OutstandingTaskTypeEnum.DigitalContent:
                        return DigitalContentExtensionConverterHelper.Convert(FileExtension);
                    case OutstandingTaskTypeEnum.Assignment:
                        return "assignments.svg";
                    case OutstandingTaskTypeEnum.StandaloneForm:
                        return StandaloneFormTypeIconConverterHelper.Convert(_task.FormType);
                }

                return null;
            }
        }

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                RaisePropertyChanged(() => Title);
            }
        }

        public DateTime? DueDate
        {
            get => _dueDate;
            set
            {
                _dueDate = value;
                RaisePropertyChanged(() => DueDateString);
            }
        }

        public float Progress
        {
            get => _progress;
            set
            {
                _progress = value;
                RaisePropertyChanged(() => Progress);
                RaisePropertyChanged(() => ProgressString);
            }
        }

        public string DueDateString => DueDate != null ? DueDate.Value.ToLocalTime().ToString("dd/MM/yyyy") : "N/A";

        public string ProgressString => (int)(Progress * 100) + "%";

        public bool ShouldShowThumbnail =>
            _type == OutstandingTaskTypeEnum.Assignment
            || _type == OutstandingTaskTypeEnum.DigitalContent
            || _type == OutstandingTaskTypeEnum.StandaloneForm;

        public bool ShouldShowProgress => _type != OutstandingTaskTypeEnum.DigitalContent && Status == OutstandingTaskStatusEum.Continue;

        public bool ContinueButtonVisibility
        {
            get
            {
                if (_type == OutstandingTaskTypeEnum.Assignment)
                {
                    return Status == OutstandingTaskStatusEum.Continue && PermissionHelper.GetPermissionForCheckinDoAssignmentDownloadContentDoPostCourse();
                }
                else
                {
                    return Status == OutstandingTaskStatusEum.Continue;
                }
            }
        }

        public bool CanNotStartButtonVisibility => !_canStart && Status == OutstandingTaskStatusEum.NotStarted;

        public string StartDateString => _startDate != null ? _startDate.Value.ToLocalTime().ToString("dd/MM/yyyy") : "N/A";

        public bool StartButtonVisibility
        {
            get
            {
                bool isVisible = _canStart && Status == OutstandingTaskStatusEum.NotStarted;
                if (_type == OutstandingTaskTypeEnum.Assignment)
                {
                    return isVisible && PermissionHelper.GetPermissionForCheckinDoAssignmentDownloadContentDoPostCourse();
                }
                else if (_type == OutstandingTaskTypeEnum.Course || _type == OutstandingTaskTypeEnum.Microlearning || _type == OutstandingTaskTypeEnum.DigitalContent)
                {
                    return isVisible && PermissionHelper.GetPermissionForStartLearning();
                }
                else
                {
                    return isVisible;
                }
            }
        }

        public ICommand OnItemClickedCommand => new Command(() =>
        {
            switch (_type)
            {
                case OutstandingTaskTypeEnum.Microlearning:
                case OutstandingTaskTypeEnum.Course:
                    _navigationService
                        .NavigateToAsync<CourseDetailsViewModel>(CourseDetailsViewModel.GetNavigationParameters(
                            _task.CourseId,
                            GetBookmarkType(),
                            true));
                    break;
                case OutstandingTaskTypeEnum.DigitalContent:
                    _navigationService
                        .NavigateToAsync<MyDigitalContentDetailsViewModel>(
                            MyDigitalContentDetailsViewModel.GetNavigationParameters(
                                _task.DigitalContentId,
                                true));
                    break;
                case OutstandingTaskTypeEnum.Assignment:
                    _navigationService
                        .NavigateToAsync<AssignmentDetailViewModel>(
                            AssignmentDetailViewModel.GetNavigationParameters(
                                _task.CourseId,
                                _task.AssignmentId));
                    break;
                case OutstandingTaskTypeEnum.StandaloneForm:
                    if (string.IsNullOrEmpty(_task.FormId))
                    {
                        break;
                    }

                    var navigationParameter = new NavigationParameters();
                    navigationParameter.SetParameter("form-id", _task.FormId);
                    _navigationService.NavigateToAsync<StandAloneFormViewModel>(navigationParameter);
                    break;
            }

            OutstandingTaskNavigationMessagingCenter.SendOnNavigatedTo(
                this,
                new OutstandingTaskNavigatedToArguments(_task.Id));
        });

        private BookmarkType GetBookmarkType()
        {
            switch (_type)
            {
                case OutstandingTaskTypeEnum.Assignment:
                case OutstandingTaskTypeEnum.Course:
                    return BookmarkType.Course;
                case OutstandingTaskTypeEnum.DigitalContent:
                    return BookmarkType.DigitalContent;
            }

            return BookmarkType.Course;
        }
    }
}
