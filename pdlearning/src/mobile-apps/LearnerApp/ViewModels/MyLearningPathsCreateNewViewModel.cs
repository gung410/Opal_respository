using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using LearnerApp.Common;
using LearnerApp.Helper;
using LearnerApp.Models;
using LearnerApp.Models.MyLearning;
using LearnerApp.Services;
using LearnerApp.Services.Backend;
using LearnerApp.Services.Dialog;
using LearnerApp.Services.MediaPicker;
using LearnerApp.Services.Navigation;
using LearnerApp.ViewModels.Base;
using Plugin.Media.Abstractions;
using Plugin.Toast;
using Xamarin.Forms;

namespace LearnerApp.ViewModels
{
    public class MyLearningPathsCreateNewViewModel : BasePageViewModel, INavigationAware
    {
        private readonly IS3UploadService _uploadService;
        private readonly IFilePickerService _filePickerService;
        private readonly IDialogService _dialogService;
        private readonly ILearnerBackendService _learnerBackendService;

        private LearningPath _learningPathsUpdateItem;
        private Color _emptyNameBorderColor;
        private string _learningPathsName;
        private string _learningPathImageSource;
        private string _fileUploadedPath;
        private string _title;
        private string _confirmedButtonLbl;
        private int _totalCount;
        private int _totalItemSelected;
        private bool _isVisibleHint;
        private bool _isVisibleDeleteButton;

        public MyLearningPathsCreateNewViewModel()
        {
            _uploadService = DependencyService.Resolve<IS3UploadService>();
            _filePickerService = DependencyService.Resolve<IFilePickerService>();
            _dialogService = DependencyService.Resolve<IDialogService>();
            _learnerBackendService = CreateRestClientFor<ILearnerBackendService>(GlobalSettings.BackendServiceLearner);

            CourseSelectedCollection = new ObservableCollection<ItemCard>();
            LearningPathImageSource = "image_place_holder_h150.png";
            IsVisibleHint = false;
        }

        public ObservableCollection<ItemCard> CourseSelectedCollection { get; set; }

        public ICommand CancelCommand => new Command(OnCancel);

        public ICommand CreateCommand => new Command(async () => await OnCreateLearningPaths());

        public ICommand UploadImageCommand => new Command(async () => await OnUploadLearningPathsImage());

        public ICommand SelectCourseCommand => new Command(OnSelectCourse);

        public ICommand DeleteLearningPathsCommand => new Command(OnDeleteLearningPaths);

        public Color EmptyNameBorderColor
        {
            get
            {
                return _emptyNameBorderColor;
            }

            set
            {
                _emptyNameBorderColor = value;
                RaisePropertyChanged(() => EmptyNameBorderColor);
            }
        }

        public string Title
        {
            get
            {
                return _title;
            }

            set
            {
                _title = value;
                RaisePropertyChanged(() => Title);
            }
        }

        public int TotalCount
        {
            get
            {
                return _totalCount;
            }

            set
            {
                _totalCount = value;
                RaisePropertyChanged(() => TotalCount);
            }
        }

        public string ConfirmedButtonLbl
        {
            get
            {
                return _confirmedButtonLbl;
            }

            set
            {
                _confirmedButtonLbl = value;
                RaisePropertyChanged(() => ConfirmedButtonLbl);
            }
        }

        public bool IsVisibleHint
        {
            get
            {
                return _isVisibleHint;
            }

            set
            {
                _isVisibleHint = value;
                RaisePropertyChanged(() => IsVisibleHint);
            }
        }

        public int TotalItemSelected
        {
            get
            {
                return _totalItemSelected;
            }

            set
            {
                _totalItemSelected = value;
                RaisePropertyChanged(() => TotalItemSelected);
            }
        }

        public string LearningPathImageSource
        {
            get
            {
                return _learningPathImageSource;
            }

            set
            {
                _learningPathImageSource = value;
                RaisePropertyChanged(() => LearningPathImageSource);
            }
        }

        public string LearningPathsName
        {
            get
            {
                return _learningPathsName;
            }

            set
            {
                _learningPathsName = value;

                if (!string.IsNullOrEmpty(value))
                {
                    EmptyNameBorderColor = Color.LightGray;
                }
                else
                {
                    EmptyNameBorderColor = Color.Red;
                }

                RaisePropertyChanged(() => LearningPathsName);
            }
        }

        public bool IsVisibleDeleteButton
        {
            get
            {
                return _isVisibleDeleteButton;
            }

            set
            {
                _isVisibleDeleteButton = value;
                RaisePropertyChanged(() => IsVisibleDeleteButton);
            }
        }

        public override string PageTitle => string.Empty;

        public override string RoutingName => NavigationRoutes.MyLearningPathCreate;

        public void OnRemoveCourseItem(ItemCard item)
        {
            CourseSelectedCollection.Remove(item);
            RaisePropertyChanged(() => CourseSelectedCollection);

            TotalCount = CourseSelectedCollection.Count;

            if (CourseSelectedCollection.IsNullOrEmpty())
            {
                IsVisibleHint = false;
            }
        }

        protected override Task InternalNavigatedTo(NavigationParameters navigationParameters)
        {
            if (navigationParameters == null)
            {
                Title = "Create New Learning Path";
                ConfirmedButtonLbl = "Create";
                IsVisibleDeleteButton = false;
            }
            else
            {
                Title = "Edit Learning Path";
                ConfirmedButtonLbl = "Update";
                _learningPathsUpdateItem = navigationParameters.GetParameter<LearningPath>("learning-paths-item");
                var listCourse = navigationParameters.GetParameter<ObservableCollection<ItemCard>>("learning-paths-item-list-course");

                LearningPathsName = _learningPathsUpdateItem.Title;
                LearningPathImageSource = _learningPathsUpdateItem.ThumbnailUrl;

                IsVisibleHint = true;
                IsVisibleDeleteButton = PermissionHelper.GetPermissionForLearningPathCUD();

                if (!listCourse.IsNullOrEmpty())
                {
                    CourseSelectedCollection = new ObservableCollection<ItemCard>(listCourse);
                    RaisePropertyChanged(() => CourseSelectedCollection);
                }

                TotalCount = _learningPathsUpdateItem.Courses.Count;
            }

            return Task.CompletedTask;
        }

        private void OnCancel(object obj)
        {
            string text = ConfirmedButtonLbl == "Create" ? "creating" : "updating";

            _dialogService.ConfirmAsync($"Do you want to cancel {text} new learning path?", "No", "Yes", onConfirmed: async (confirmed) =>
            {
                if (confirmed)
                {
                    await NavigationService.GoBack();
                }
            });
        }

        private async Task OnCreateLearningPaths()
        {
            if (IsBusy)
            {
                return;
            }

            IsBusy = true;

            using (DialogService.DisplayLoadingIndicator())
            {
                if (string.IsNullOrEmpty(LearningPathsName))
                {
                    EmptyNameBorderColor = Color.Red;
                }
                else
                {
                    if (!CourseSelectedCollection.IsNullOrEmpty())
                    {
                        var listCourse = new List<LearningPathsCourse>();

                        for (int i = 0; i < CourseSelectedCollection.Count; ++i)
                        {
                            listCourse.Add(new LearningPathsCourse
                            {
                                CourseId = CourseSelectedCollection[i].Id,
                                Order = i
                            });
                        }

                        LearningPath learningPathResult;

                        if (ConfirmedButtonLbl == "Create")
                        {
                            learningPathResult = await CreateLearningPaths(listCourse);
                        }
                        else
                        {
                            learningPathResult = await UpdateLearningPaths(listCourse);
                        }

                        if (learningPathResult != null)
                        {
                            MessagingCenter.Unsubscribe<MyLearningPathsCreateNewViewModel, LearningPath>(this, "update-learning-paths");
                            MessagingCenter.Send(this, "update-learning-paths", learningPathResult);

                            string text = ConfirmedButtonLbl == "Create" ? "Created" : "Updated";

                            CrossToastPopUp.Current.ShowToastSuccess($"{text} successfully");

                            await NavigationService.GoBack();
                        }
                    }
                    else
                    {
                        CrossToastPopUp.Current.ShowToastError("Please select PD Opportunities");
                    }
                }
            }

            IsBusy = false;
        }

        private async Task<LearningPath> UpdateLearningPaths(List<LearningPathsCourse> listCourse)
        {
            var learningPath = new LearningPath
            {
                Id = _learningPathsUpdateItem.Id,
                Title = LearningPathsName,
                ThumbnailUrl = string.IsNullOrEmpty(_fileUploadedPath) ? _learningPathsUpdateItem.ThumbnailUrl : _fileUploadedPath,
                ListCourses = listCourse
            };

            var result = await ExecuteBackendService(() => _learnerBackendService.UpdateLearningPath(learningPath));

            return result.Payload;
        }

        private async Task<LearningPath> CreateLearningPaths(List<LearningPathsCourse> listCourse)
        {
            var learningPath = new LearningPath
            {
                Title = LearningPathsName,
                ThumbnailUrl = _fileUploadedPath,
                ListCourses = listCourse
            };

            var result = await ExecuteBackendService(() => _learnerBackendService.CreateLearningPath(learningPath));

            return result.Payload;
        }

        private async Task OnUploadLearningPathsImage()
        {
            MediaFile fileData = await _filePickerService.PickPhotoAsync(new PickMediaOptions());

            if (fileData == null)
            {
                return;
            }

            using (DialogService.DisplayLoadingIndicator())
            {
                _fileUploadedPath = await _uploadService.UploadFile(fileData, "learner-attachments");

                if (!string.IsNullOrEmpty(_fileUploadedPath))
                {
                    LearningPathImageSource = _fileUploadedPath;
                }
            }
        }

        private void OnSelectCourse(object obj)
        {
            _dialogService.CreateNewLearningPathsPopup(CourseSelectedCollection, (agr) =>
            {
                TotalItemSelected = agr.Count();

                if (TotalItemSelected == 0)
                {
                    IsVisibleHint = false;

                    return;
                }

                IsVisibleHint = true;

                CourseSelectedCollection = new ObservableCollection<ItemCard>(agr);
                RaisePropertyChanged(() => CourseSelectedCollection);

                TotalCount = CourseSelectedCollection.Count;
            });
        }

        private void OnDeleteLearningPaths()
        {
            _dialogService.ConfirmAsync("Do you want to delete this learning path?", "No", "Yes", onConfirmed: async (confirmed) =>
            {
                if (confirmed)
                {
                    await ExecuteBackendService(() => _learnerBackendService.DeleteLearningPath(_learningPathsUpdateItem.Id));

                    CrossToastPopUp.Current.ShowToastSuccess("Deleted learning path successfully");

                    await NavigationService.GoBack();

                    MessagingCenter.Unsubscribe<MyLearningPathsCreateNewViewModel>(this, "deleted-learning-path");
                    MessagingCenter.Send(this, "deleted-learning-path");
                }
            });
        }
    }
}
