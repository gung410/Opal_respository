using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using LearnerApp.Common;
using LearnerApp.Controls.LearnerObservableCollection;
using LearnerApp.Helper;
using LearnerApp.Models.Learner;
using LearnerApp.Models.MyLearning;
using LearnerApp.Services;
using LearnerApp.Services.Backend;
using LearnerApp.Services.Navigation;
using LearnerApp.ViewModels.Base;
using Xamarin.Forms;

namespace LearnerApp.ViewModels
{
    public class MyLearningPathsShowAllViewModel : BasePageViewModel, INavigationAware
    {
        private static INavigationService _navigationService;
        private readonly ILearnerBackendService _learnerBackendService;
        private readonly ICommonServices _commonService;

        private int _totalCount;
        private string _pageTitle;
        private bool _isRefreshing;
        private int _paging;
        private bool _isVisibileLearningPathCUD = true;
        private LearningPathType _myLearningPathType;

        public MyLearningPathsShowAllViewModel()
        {
            _navigationService = DependencyService.Resolve<INavigationService>();
            _commonService = DependencyService.Resolve<ICommonServices>();
            _learnerBackendService = CreateRestClientFor<ILearnerBackendService>(GlobalSettings.BackendServiceLearner);
            CachingMode = PageCachingMode.None;

            MessagingCenter.Subscribe<MyLearningPathsCreateNewViewModel, LearningPath>(this, "update-learning-paths", (sender, arg) =>
            {
                if (MyLearningPathsCollection == null)
                {
                    return;
                }

                LearningPath item = MyLearningPathsCollection.FirstOrDefault(p => p.Id == arg.Id);

                if (item != null)
                {
                    item = arg;
                }
                else
                {
                    MyLearningPathsCollection.Insert(0, arg);
                }

                RaisePropertyChanged(() => MyLearningPathsCollection);
            });
        }

        public ICommand CreateNewLearningPathCommand => new Command(CreateNewLearningPaths);

        public ICommand RefreshCommand => new Command(async () => await OnRefresh());

        public ICommand LoadmoreCommand => new Command(async () => await OnLoadmore());

        public LearnerObservableCollection<LearningPath> MyLearningPathsCollection { get; set; }

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

        public override string PageTitle
        {
            get
            {
                return _pageTitle;
            }
        }

        public override string RoutingName => NavigationRoutes.MyLearningShowAll;

        public LearningPathType MyLearningPathType
        {
            get
            {
                return _myLearningPathType;
            }

            set
            {
                _myLearningPathType = value;
                RaisePropertyChanged(() => MyLearningPathType);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            MessagingCenter.Unsubscribe<MyLearningPathsCreateNewViewModel, LearningPath>(this, "update-learning-paths");
        }

        protected override async Task InternalNavigatedTo(NavigationParameters navigationParameters)
        {
            SetPageTitle(SeparateStringByUppercase.Convert(navigationParameters.GetParameter<string>("learning-paths-page-title")));

            MyLearningPathType = navigationParameters.GetParameter<LearningPathType>("is-my-own-learning-path");

            IsVisibileLearningPathCUD = MyLearningPathType == LearningPathType.MyOwnlearningPath && PermissionHelper.GetPermissionForLearningPathCUD();

            await LoadLearningPathsByType(false);
        }

        private void SetPageTitle(string title)
        {
            _pageTitle = title;
            RaisePropertyChanged(() => PageTitle);
        }

        private async Task LoadSharingLearningPath(bool isLoadmore = false)
        {
            using (DialogService.DisplayLoadingIndicator())
            {
                var listLearningPath = await ExecuteBackendService(() => _learnerBackendService.GetLearningPathsSharedToMe(skipCount: _paging * GlobalSettings.MaxResultPerPage));

                if (!listLearningPath.IsError)
                {
                    TotalCount = listLearningPath.Payload.TotalCount;

                    if (!isLoadmore)
                    {
                        MyLearningPathsCollection = new LearnerObservableCollection<LearningPath>(listLearningPath.Payload.Items);
                        RaisePropertyChanged(() => MyLearningPathsCollection);
                    }
                    else
                    {
                        MyLearningPathsCollection.AddRange(listLearningPath.Payload.Items);
                    }
                }
            }
        }

        private async Task LoadMyOwnLearningPaths(bool isLoadmore = false)
        {
            using (DialogService.DisplayLoadingIndicator())
            {
                var listLearningPath = await ExecuteBackendService(() => _learnerBackendService.SearchLearningPath(new
                {
                    SkipCount = _paging * GlobalSettings.MaxResultPerPage,
                    MaxResultCount = GlobalSettings.MaxResultPerPage
                }));

                if (!listLearningPath.IsError)
                {
                    TotalCount = listLearningPath.Payload.TotalCount;

                    var sortedItems = listLearningPath.Payload.Items.OrderByDescending(p => p.CreatedDate).ToList();

                    if (!isLoadmore)
                    {
                        MyLearningPathsCollection = new LearnerObservableCollection<LearningPath>(sortedItems);
                        RaisePropertyChanged(() => MyLearningPathsCollection);
                    }
                    else
                    {
                        MyLearningPathsCollection.AddRange(sortedItems);
                    }
                }
            }
        }

        private async Task LoadRecommendationLearningPath(bool isLoadmore = false)
        {
            if (MyLearningPathsCollection != null && MyLearningPathsCollection.Count >= TotalCount)
            {
                return;
            }

            var learningPaths = await _commonService.RecommendationLearningPathsCollection(userId: Application.Current.Properties.GetAccountProperties().User.Sub, paging: _paging, totalCount: count =>
            {
                TotalCount = count;
            });

            if (learningPaths.IsNullOrEmpty())
            {
                return;
            }

            if (!isLoadmore)
            {
                MyLearningPathsCollection = new LearnerObservableCollection<LearningPath>(learningPaths);
            }
            else
            {
                MyLearningPathsCollection.AddRange(learningPaths);
                RaisePropertyChanged(() => MyLearningPathsCollection);
            }
        }

        private void CreateNewLearningPaths(object obj)
        {
            _navigationService.NavigateToAsync<MyLearningPathsCreateNewViewModel>();
        }

        private async Task OnRefresh()
        {
            await LoadLearningPathsByType(false);
        }

        private async Task OnLoadmore()
        {
            if (!IsBusy && MyLearningPathsCollection.Count < TotalCount)
            {
                IsBusy = true;

                await LoadLearningPathsByType(true);
            }

            IsBusy = false;
        }

        private async Task LoadLearningPathsByType(bool isEnableLoadMore)
        {
            if (isEnableLoadMore)
            {
                _paging++;
            }
            else
            {
                if (MyLearningPathType == LearningPathType.RecommendationLearningPath)
                {
                    _paging = 1;
                }
                else
                {
                    _paging = 0;
                }
            }

            switch (MyLearningPathType)
            {
                case LearningPathType.MyOwnlearningPath:
                    await LoadMyOwnLearningPaths(isEnableLoadMore);
                    break;
                case LearningPathType.SharedLearningPath:
                    await LoadSharingLearningPath(isEnableLoadMore);
                    break;
                case LearningPathType.RecommendationLearningPath:
                    await LoadRecommendationLearningPath(isEnableLoadMore);
                    break;
                default:
                    break;
            }
        }
    }
}
