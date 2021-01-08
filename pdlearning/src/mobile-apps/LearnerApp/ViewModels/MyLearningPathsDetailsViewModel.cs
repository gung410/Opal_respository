using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using LearnerApp.Common;
using LearnerApp.Common.Helper;
using LearnerApp.Controls.LearnerObservableCollection;
using LearnerApp.Helper;
using LearnerApp.Models;
using LearnerApp.Models.Learner;
using LearnerApp.Models.MyLearning;
using LearnerApp.Resources.Texts;
using LearnerApp.Services;
using LearnerApp.Services.Backend;
using LearnerApp.Services.Navigation;
using LearnerApp.ViewModels.Base;
using Plugin.Toast;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace LearnerApp.ViewModels
{
    public class MyLearningPathsDetailsViewModel : BasePageViewModel, INavigationAware
    {
        private static INavigationService _navigationService;
        private readonly ICourseBackendService _courseBackendService;
        private readonly ICommonServices _commonService;
        private readonly ILearnerBackendService _learnerBackendService;
        private readonly StressActionHandler _stressActionHandler = new StressActionHandler();

        private string _title;
        private string _learningPathsImage;
        private int _totalCount;
        private bool _learningPathsHasBookmark;
        private LearningPath _learningPaths;
        private bool _isMyOwnLearningPath;
        private bool _isRefreshing = true;
        private bool _isVisibleMore = true;
        private bool _isVisibileLearningPathCUD = true;

        public MyLearningPathsDetailsViewModel()
        {
            _commonService = DependencyService.Resolve<ICommonServices>();
            _navigationService = DependencyService.Resolve<INavigationService>();
            _learnerBackendService = CreateRestClientFor<ILearnerBackendService>(GlobalSettings.BackendServiceLearner);
            _courseBackendService = CreateRestClientFor<ICourseBackendService>(GlobalSettings.BackendServiceCourse);
            CachingMode = PageCachingMode.None;

            IsMyOwnLearningPath = true;
            TotalCount = 0;

            MessagingCenter.Subscribe<MyLearningPathsCreateNewViewModel, LearningPath>(this, "update-learning-paths", async (sender, arg) =>
            {
                if (LearningPath.Id != arg.Id)
                {
                    return;
                }

                LearningPath = arg;

                await LoadMyLearningPaths();
            });

            MessagingCenter.Subscribe<MyLearningPathsCreateNewViewModel>(this, "deleted-learning-path", async (sender) =>
            {
                await NavigationService.GoBack();
            });
        }

        public override string PageTitle => string.Empty;

        public override string RoutingName => NavigationRoutes.MyLearningPathDetails;

        public LearnerObservableCollection<ItemCard> CourseCollection { get; set; }

        public ICommand MoreCommand => new Command(async () => await OnMoreTapped());

        public ICommand EditLearningPathCommand => new Command(OnEditLearningPaths);

        public ICommand ShareLearningPathCommand => new Command(OnShareLearningPaths);

        public bool IsMyOwnLearningPath
        {
            get
            {
                return _isMyOwnLearningPath;
            }

            set
            {
                _isMyOwnLearningPath = value;
                RaisePropertyChanged(() => IsMyOwnLearningPath);
            }
        }

        public LearningPath LearningPath
        {
            get
            {
                return _learningPaths;
            }

            set
            {
                _learningPaths = value;
                RaisePropertyChanged(() => LearningPath);
            }
        }

        public string LearningPathsImage
        {
            get
            {
                return _learningPathsImage;
            }

            set
            {
                _learningPathsImage = value;
                RaisePropertyChanged(() => LearningPathsImage);
            }
        }

        public bool LearningPathsHasBookmark
        {
            get
            {
                return _learningPathsHasBookmark;
            }

            set
            {
                _learningPathsHasBookmark = value;
                RaisePropertyChanged(() => LearningPathsHasBookmark);
            }
        }

        public bool IsRefreshing
        {
            get
            {
                return _isRefreshing;
            }

            set
            {
                _isRefreshing = value;
                RaisePropertyChanged(() => IsRefreshing);
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

                if (value != -1)
                {
                    RaisePropertyChanged(() => TotalCount);
                }
            }
        }

        public bool IsVisibleMore
        {
            get
            {
                return _isVisibleMore;
            }

            set
            {
                _isVisibleMore = value;
                RaisePropertyChanged(() => IsVisibleMore);
            }
        }

        public bool IsVisibileLearningPathCUD
        {
            get
            {
                return _isVisibileLearningPathCUD;
            }

            set
            {
                _isVisibileLearningPathCUD = value;
                RaisePropertyChanged(() => IsVisibileLearningPathCUD);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            MessagingCenter.Unsubscribe<MyLearningPathsCreateNewViewModel, LearningPath>(this, "update-learning-paths");
            MessagingCenter.Unsubscribe<MyLearningPathsCreateNewViewModel>(this, "deleted-learning-path");
        }

        public async Task LoadMyLearningPaths()
        {
            IsRefreshing = true;
            try
            {
                LearningPathsImage = LearningPath.ThumbnailUrl;
                Title = LearningPath.Title;

                if (LearningPath.Courses.IsNullOrEmpty())
                {
                    TotalCount = 0;
                    CourseCollection = new LearnerObservableCollection<ItemCard>();
                    RaisePropertyChanged(() => CourseCollection);
                    return;
                }

                var sortedIds = LearningPath.Courses.OrderBy(p => p.Order).Select(p => p.CourseId).ToArray();
                var courseExtendedInformation = await ExecuteBackendService(() => _courseBackendService.GetCourseListByIdentifiers(sortedIds));

                if (courseExtendedInformation.HasEmptyResult() || courseExtendedInformation.Payload.Count == 0)
                {
                    TotalCount = 0;
                    CourseCollection = new LearnerObservableCollection<ItemCard>();
                    RaisePropertyChanged(() => CourseCollection);
                    return;
                }

                var courseCard = await _commonService.CreateCourseCardList(courseExtendedInformation.Payload);

                courseCard = courseCard.OrderBy(x => { return Array.IndexOf(sortedIds, x.Id); }).ToList();

                CourseCollection = new LearnerObservableCollection<ItemCard>(courseCard);
                RaisePropertyChanged(() => CourseCollection);

                TotalCount = CourseCollection.Count();
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        protected override async Task InternalNavigatedTo(NavigationParameters navigationParameters)
        {
            LearningPath = navigationParameters.GetParameter<LearningPath>("learning-paths-item");
            IsMyOwnLearningPath = navigationParameters.GetParameter<LearningPathType>("my-learning-path-type") == LearningPathType.MyOwnlearningPath;
            IsVisibileLearningPathCUD = PermissionHelper.GetPermissionForLearningPathCUD();

            var accountProperties = Application.Current.Properties.GetAccountProperties();
            bool isOwner = accountProperties.User?.Sub.ToUpper() == LearningPath.CreatedBy.ToUpper();

            if (!LearningPath.IsFromLMM && !isOwner && !PermissionHelper.GetPermissionForBookmark())
            {
                IsVisibleMore = false;
            }

            await LoadMyLearningPaths();
        }

        private void OnEditLearningPaths(object obj)
        {
            var navParams = new NavigationParameters();
            navParams.SetParameter("learning-paths-item", LearningPath);
            navParams.SetParameter("learning-paths-item-list-course", CourseCollection);

            _navigationService.NavigateToAsync<MyLearningPathsCreateNewViewModel>(navParams);
        }

        private void OnShareLearningPaths(object obj)
        {
            var navParams = new NavigationParameters();
            navParams.SetParameter("learning-paths-item", LearningPath);
            _navigationService.NavigateToAsync<ShareMyLearningPathsViewModel>(navParams);
        }

        private async Task OnMoreTapped()
        {
            await _stressActionHandler.RunAsync(async () =>
            {
                bool isBookmark = LearningPath.BookmarkInfo == null;
                string stringCopyOption = TextsResource.COPY_URL;
                string stringBookmarkOption = isBookmark ? TextsResource.ADD_BOOKMARK : TextsResource.REMOVE_BOOKMARK;

                var options = new Dictionary<string, string>
                {
                    { stringCopyOption, "copy.svg" },
                    { stringBookmarkOption, isBookmark ? "bookmark.svg" : "bookmarked.svg" }
                };

                var accountProperties = Application.Current.Properties.GetAccountProperties();
                bool isOwner = accountProperties.User?.Sub.ToUpper() == LearningPath.CreatedBy.ToUpper();

                if (!LearningPath.IsFromLMM && !isOwner)
                {
                    options.Remove(stringCopyOption);
                }

                bool isVisibleBookmark = PermissionHelper.GetPermissionForBookmark();

                if (!isVisibleBookmark)
                {
                    options.Remove(stringBookmarkOption);
                }

                if (options.IsNullOrEmpty())
                {
                    return;
                }

                await DialogService.ShowDropDownSelectionPopup(options, isSeparateStringByUppercase: false, onSelected: async option =>
                {
                    if (string.IsNullOrEmpty(option))
                    {
                        return;
                    }

                    if (option == stringCopyOption)
                    {
                        string sharedUrl = $"{GlobalSettings.BackendBaseUrl}/app/learner/my-learning/learning-path/{LearningPath.Id}";
                        if (LearningPath.IsFromLMM)
                        {
                            sharedUrl = $"{sharedUrl}/fromlmm";
                        }
                        else
                        {
                            await ExecuteBackendService(() => _learnerBackendService.PublicLearningPath(LearningPath.Id));
                        }

                        await Clipboard.SetTextAsync(sharedUrl);
                        CrossToastPopUp.Current.ShowToastSuccess(TextsResource.COPY_SUCCESSFULLY);
                    }
                    else if (option == stringBookmarkOption)
                    {
                        LearningPath.BookmarkInfo = await _commonService.Bookmark(LearningPath.Id, BookmarkType.LearningPath, isBookmark);

                        MessagingCenter.Unsubscribe<MyLearningPathsDetailsViewModel, LearningPath>(this, "learning-path-bookmarked");
                        MessagingCenter.Send(this, "learning-path-bookmarked", LearningPath);
                    }
                });
            });
        }
    }
}
