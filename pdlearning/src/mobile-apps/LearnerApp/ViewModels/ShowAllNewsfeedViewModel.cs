using System;
using System.Threading.Tasks;
using System.Windows.Input;
using LearnerApp.Common.Helper;
using LearnerApp.Controls.LearnerObservableCollection;
using LearnerApp.Models.Newsfeed;
using LearnerApp.Services;
using LearnerApp.Services.Navigation;
using LearnerApp.ViewModels.Base;
using Xamarin.Forms;

namespace LearnerApp.ViewModels
{
    public class ShowAllNewsfeedViewModel : BasePageViewModel, INavigationAware
    {
        private readonly ICommonServices _commonService;
        private readonly StressActionHandler _loadMoreStressActionHandler = new StressActionHandler(TimeSpan.FromSeconds(2));
        private int _totalCount;

        public ShowAllNewsfeedViewModel()
        {
            _commonService = DependencyService.Resolve<ICommonServices>();
        }

        public ICommand RefreshCommand => new Command(async () =>
        {
            NewsfeedCollection.Clear();
            await GetNewsfeed();
        });

        public ICommand LoadmoreCommand => new Command(async () =>
        {
            await GetNewsfeed();
        });

        public LearnerObservableCollection<Feed> NewsfeedCollection { get; set; }

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

        public override string PageTitle => string.Empty;

        public override string RoutingName => NavigationRoutes.NewsFeed;

        public async Task GetNewsfeed()
        {
            await _loadMoreStressActionHandler.Run(async () =>
            {
                var newsFeed = await _commonService.GetNewsfeed(skipCount: NewsfeedCollection.Count, count: count =>
                {
                    TotalCount = count;
                });

                NewsfeedCollection.AddRange(newsFeed);
            });
        }

        protected override async Task InternalNavigatedTo(NavigationParameters navigationParameters)
        {
            NewsfeedCollection = new LearnerObservableCollection<Feed>();
            RaisePropertyChanged(() => NewsfeedCollection);
            await GetNewsfeed();
        }
    }
}
