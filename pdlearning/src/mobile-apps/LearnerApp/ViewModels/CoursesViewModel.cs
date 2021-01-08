using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using LearnerApp.Common;
using LearnerApp.Controls.LearnerObservableCollection;
using LearnerApp.Models;
using LearnerApp.Services;
using LearnerApp.Services.Navigation;
using LearnerApp.ViewModels.Base;
using Xamarin.Forms;

namespace LearnerApp.ViewModels
{
    public class CoursesViewModel : BasePageViewModel, INavigationAware
    {
        private readonly ICommonServices _commonService;

        private string _screenTitle;
        private string _userId;
        private int _totalCount;
        private int _paging;
        private LearningScreenType _currentPage;

        public CoursesViewModel()
        {
            CachingMode = PageCachingMode.None;
            _commonService = DependencyService.Resolve<ICommonServices>();
        }

        public ICommand LoadMoreItemCommand => new Command(async () => await GetDataSource());

        public ICommand RefreshCommand => new Command(async () => await InitScreen());

        public LearnerObservableCollection<ItemCard> CourseCollection { get; set; }

        public int TotalCount
        {
            get
            {
                return _totalCount;
            }

            set
            {
                _totalCount = value;

                if (_totalCount != -1)
                {
                    RaisePropertyChanged(() => TotalCount);
                }
            }
        }

        public string ScreenTitle
        {
            get
            {
                return _screenTitle;
            }

            set
            {
                _screenTitle = value;
                RaisePropertyChanged(() => ScreenTitle);
            }
        }

        public override string PageTitle => string.Empty;

        public override string RoutingName => NavigationRoutes.CourseList;

        protected override async Task InternalNavigatedTo(NavigationParameters navigationParameters)
        {
            Enum.TryParse(navigationParameters.GetParameter<string>("SourceScreen"), out LearningScreenType myLearningScreen);

            _currentPage = myLearningScreen;

            ScreenTitle = SeparateStringByUppercase.Convert(myLearningScreen.ToString());

            await InitScreen();
        }

        private async Task InitScreen()
        {
            CourseCollection = new LearnerObservableCollection<ItemCard>();
            TotalCount = -1;
            _paging = 0;

            var user = await IdentityService.GetAccountPropertiesAsync();
            if (user != null)
            {
                _userId = user.User.Sub;
            }

            await GetDataSource();
            RaisePropertyChanged(() => CourseCollection);
        }

        private async Task GetDataSource()
        {
            if (!IsBusy)
            {
                IsBusy = true;
                _paging++;

                List<ItemCard> courses = new List<ItemCard>();
                switch (_currentPage)
                {
                    case LearningScreenType.NewlyAdded:
                        courses = await _commonService.GetNewlyAddedCollection(_paging, totalCount: count =>
                        {
                            TotalCount = count;
                        });
                        break;
                    case LearningScreenType.RecommendationsForYou:
                        courses = await _commonService.GetRecommendationsCollection(
                            _userId,
                            _paging,
                            count =>
                            {
                                TotalCount = count;
                            });
                        break;
                    case LearningScreenType.RecommendationByYourOrganisation:
                        courses =
                            await _commonService.GetRecommendationOrganizationCollection(_paging, count =>
                            {
                                TotalCount = count;
                            });
                        break;
                    default:
                        break;
                }

                CourseCollection.AddRange(courses);
                if (!courses.Any())
                {
                    TotalCount = CourseCollection.Count;
                }

                IsBusy = false;
            }
        }
    }
}
