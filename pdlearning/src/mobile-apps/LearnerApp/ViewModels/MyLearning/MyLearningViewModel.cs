using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using LearnerApp.Common;
using LearnerApp.Controls.MyLearning;
using LearnerApp.Helper;
using LearnerApp.Models.Learner;
using LearnerApp.Services.Dialog;
using LearnerApp.Services.Navigation;
using LearnerApp.ViewModels.Base;
using Xamarin.Forms;

namespace LearnerApp.ViewModels
{
    public class MyLearningViewModel : BasePageViewModel
    {
        private readonly IDialogService _dialogService;

        private readonly MyLearningCourseBookmarked _myLearningCourseBookmarked;
        private readonly MyLearningBookmarkGroup _myLearningBookmarkGroup;
        private readonly MyLearningDigitalContent _myLearningDigitalContent;
        private readonly MyLearningDigitalContentBookmarked _myLearningDigitalContentBookmarked;
        private readonly MyLearningMicrolearning _myLearningMicrolearning;
        private readonly MyLearningPathsBookmarked _myLearningPathsBookmarked;

        private readonly MyLearningCommunityBookmarked _myLearningCommunityBookmarked;

        private string _currentMyLearningContent;
        private bool _removeMyLearningContent;
        private bool _isVisibleMyLearningContent = true;
        private ContentView _currentContentSelected;
        private MyLearningCourse _myLearningCourse;
        private MyLearningCommunity _myLearningCommunity;
        private MyLearningPaths _myLearningPath;
        private PdActivityType _currentSelectedActivityType = PdActivityType.Courses;

        public MyLearningViewModel()
        {
            CachingMode = PageCachingMode.None;

            _dialogService = DependencyService.Resolve<IDialogService>();

            _myLearningBookmarkGroup = new MyLearningBookmarkGroup();
            _myLearningBookmarkGroup.BookmarkGroupSelectedEventHandler += OnBookmarkGroupSelected;

            _myLearningCourseBookmarked = new MyLearningCourseBookmarked();
            _myLearningCourseBookmarked.BackEventHandler += OnCourseFilterBack;

            _myLearningDigitalContentBookmarked = new MyLearningDigitalContentBookmarked();
            _myLearningDigitalContentBookmarked.BackEventHandler += OnCourseFilterBack;

            _myLearningPathsBookmarked = new MyLearningPathsBookmarked();
            _myLearningPathsBookmarked.BackEventHandler += OnCourseFilterBack;

            _myLearningDigitalContent = new MyLearningDigitalContent();
            _myLearningMicrolearning = new MyLearningMicrolearning();

            _myLearningCommunityBookmarked = new MyLearningCommunityBookmarked();
            _myLearningCommunityBookmarked.BackEventHandler += OnCourseFilterBack;

            _myLearningPath = new MyLearningPaths();
        }

        public ICommand MyLearningContentCommand => new Command(LoadMyLearningContent);

        public string CurrentMyLearningContent
        {
            get
            {
                return _currentMyLearningContent;
            }

            set
            {
                _currentMyLearningContent = value;
                RaisePropertyChanged(() => CurrentMyLearningContent);
            }
        }

        public bool RemoveMyLearningContent
        {
            get
            {
                return _removeMyLearningContent;
            }

            set
            {
                _removeMyLearningContent = value;
                RaisePropertyChanged(() => RemoveMyLearningContent);
            }
        }

        public bool IsVisibleMyLearningContent
        {
            get
            {
                return _isVisibleMyLearningContent;
            }

            set
            {
                _isVisibleMyLearningContent = value;
                RaisePropertyChanged(() => IsVisibleMyLearningContent);
            }
        }

        public ContentView CurrentContentSelected
        {
            get
            {
                return _currentContentSelected;
            }

            set
            {
                _currentContentSelected = value;
                RaisePropertyChanged(() => CurrentContentSelected);
            }
        }

        public override string PageTitle { get; } = "My Learning";

        public override string RoutingName => NavigationRoutes.MyLearning;

        protected override async Task InternalNavigatedTo(NavigationParameters navigationParameters)
        {
            SettingAccessRights();

            if (!PermissionHelper.GetPermissionForMyLearningPage())
            {
                return;
            }

            if (navigationParameters != null)
            {
                switch (navigationParameters.GetParameter<string>("SourceScreen"))
                {
                    case "MyLearning":
                        await InitLearningContent(PdActivityType.Courses);
                        break;
                    case "Bookmarks":
                        await InitLearningContent(PdActivityType.Bookmarks);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                if (PermissionHelper.GetPermissionForMyLearningCourse())
                {
                    _currentSelectedActivityType = PdActivityType.Courses;
                }
                else if (PermissionHelper.GetPermissionForMyLearningMLU())
                {
                    _currentSelectedActivityType = PdActivityType.MicrolearningUnits;
                }
                else if (PermissionHelper.GetPermissionForMyLearningDigitalContent())
                {
                    _currentSelectedActivityType = PdActivityType.DigitalContent;
                }
                else if (PermissionHelper.GetPermissionForMyLearningLearningPath())
                {
                    _currentSelectedActivityType = PdActivityType.LearningPaths;
                }
                else if (PermissionHelper.GetPermissionForMyLearningCommunity())
                {
                    _currentSelectedActivityType = PdActivityType.Communities;
                }
                else if (PermissionHelper.GetPermissionForMyLearningBookmark())
                {
                    _currentSelectedActivityType = PdActivityType.Bookmarks;
                }
                else
                {
                    IsVisibleMyLearningContent = false;
                    return;
                }

                await InitLearningContent(_currentSelectedActivityType);
            }
        }

        private void LoadMyLearningContent()
        {
            var options = new Dictionary<string, string>();

            if (PermissionHelper.GetPermissionForMyLearningCourse())
            {
                options.Add(PdActivityType.Courses.ToString(), null);
            }

            if (PermissionHelper.GetPermissionForMyLearningMLU())
            {
                options.Add(PdActivityType.MicrolearningUnits.ToString(), null);
            }

            if (PermissionHelper.GetPermissionForMyLearningDigitalContent())
            {
                options.Add(PdActivityType.DigitalContent.ToString(), null);
            }

            if (PermissionHelper.GetPermissionForMyLearningLearningPath())
            {
                options.Add(PdActivityType.LearningPaths.ToString(), null);
            }

            if (PermissionHelper.GetPermissionForMyLearningCommunity())
            {
                options.Add(PdActivityType.Communities.ToString(), null);
            }

            if (PermissionHelper.GetPermissionForMyLearningBookmark())
            {
                options.Add(PdActivityType.Bookmarks.ToString(), null);
            }

            if (options.IsNullOrEmpty())
            {
                return;
            }

            _dialogService.ShowDropDownSelectionPopup(
                options,
                isFullScreen: true,
                onSelected: async myLearningItemSelected =>
                {
                    if (!string.IsNullOrEmpty(myLearningItemSelected))
                    {
                        Enum.TryParse(myLearningItemSelected, out PdActivityType itemSelected);
                        await InitLearningContent(itemSelected);
                    }
                });
        }

        private async Task InitLearningContent(PdActivityType item)
        {
            _currentSelectedActivityType = item;
            using (DialogService.DisplayLoadingIndicator())
            {
                CurrentMyLearningContent = SeparateStringByUppercase.Convert(item.ToString());

                switch (item)
                {
                    case PdActivityType.Courses:
                        if (_myLearningCourse == null)
                        {
                            _myLearningCourse = new MyLearningCourse();
                        }

                        await _myLearningCourse.Init();

                        CurrentContentSelected = _myLearningCourse;
                        break;
                    case PdActivityType.DigitalContent:
                        await _myLearningDigitalContent.LoadMyDigitalContent();
                        CurrentContentSelected = _myLearningDigitalContent;
                        break;
                    case PdActivityType.LearningPaths:
                        _myLearningPath.Init();
                        CurrentContentSelected = _myLearningPath;
                        break;
                    case PdActivityType.Bookmarks:
                        await _myLearningBookmarkGroup.GetMyBookmarkGroupCount();

                        CurrentContentSelected = _myLearningBookmarkGroup;
                        break;
                    case PdActivityType.MicrolearningUnits:
                        await _myLearningMicrolearning.Init();
                        CurrentContentSelected = _myLearningMicrolearning;
                        break;
                    case PdActivityType.Communities:
                        if (_myLearningCommunity == null)
                        {
                            _myLearningCommunity = new MyLearningCommunity();
                        }

                        await _myLearningCommunity.Init();
                        CurrentContentSelected = _myLearningCommunity;
                        break;
                    default:
                        break;
                }
            }
        }

        private async void OnBookmarkGroupSelected(object sender, BookmarkType e)
        {
            switch (e)
            {
                case BookmarkType.Course:
                    await _myLearningCourseBookmarked.LoadCourseBookmarked(BookmarkType.Course);
                    CurrentContentSelected = _myLearningCourseBookmarked;
                    break;
                case BookmarkType.DigitalContent:
                    await _myLearningDigitalContentBookmarked.LoadMyDigitalContentBookmarked();
                    CurrentContentSelected = _myLearningDigitalContentBookmarked;
                    break;
                case BookmarkType.LearningPath:
                    await _myLearningPathsBookmarked.LoadLearningPathsBookmarked();
                    CurrentContentSelected = _myLearningPathsBookmarked;
                    break;
                case BookmarkType.Microlearning:
                    await _myLearningCourseBookmarked.LoadCourseBookmarked(BookmarkType.Microlearning);
                    CurrentContentSelected = _myLearningCourseBookmarked;
                    break;
                case BookmarkType.Community:
                    await _myLearningCommunityBookmarked.LoadCommunityBookmarked();
                    CurrentContentSelected = _myLearningCommunityBookmarked;
                    break;
                default:
                    break;
            }
        }

        private async void OnCourseFilterBack(object sender, EventArgs e)
        {
            await _myLearningBookmarkGroup.GetMyBookmarkGroupCount();

            CurrentContentSelected = _myLearningBookmarkGroup;
        }

        private void SettingAccessRights()
        {
            // This is trick because Xamarin Shell cannot init page in case Home Page invisible
            if (!PermissionHelper.GetPermissionForHomePage() && !PermissionHelper.GetPermissionForMyLearningPage())
            {
                MessagingCenter.Unsubscribe<AppShell>(this, "learning-view");
                MessagingCenter.Send(this, "learning-view");
            }
        }
    }
}
