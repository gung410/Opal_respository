using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using LearnerApp.Common;
using LearnerApp.Controls.LearnerObservableCollection;
using LearnerApp.Models;
using LearnerApp.Services;
using LearnerApp.Services.Backend;
using LearnerApp.ViewModels.Base;
using Xamarin.Forms;

namespace LearnerApp.ViewModels.MyLearning
{
    public class MyLearningCommunityBookmarkViewModel : BaseViewModel
    {
        private readonly ILearnerBackendService _learnerBackendService;
        private readonly ICommunityBackendService _communityBackendService;

        private int _totalBookmarkCount;

        public MyLearningCommunityBookmarkViewModel()
        {
            _learnerBackendService = CreateRestClientFor<ILearnerBackendService>(GlobalSettings.BackendServiceLearner);
            _communityBackendService = CreateRestClientFor<ICommunityBackendService>(GlobalSettings.BackendServiceSocial);

            TotalBookmarkCount = -1;
        }

        public ICommand LoadmoreBookmarkCommand => new Command(async () => await LoadCommunityBookmarkCollection(true));

        public ICommand RefreshCommand => new Command(async () => await OnRefresh());

        public LearnerObservableCollection<ItemCard> CommunityCollection { get; set; }

        public int TotalBookmarkCount
        {
            get
            {
                return _totalBookmarkCount;
            }

            set
            {
                _totalBookmarkCount = value;

                if (value != -1)
                {
                    RaisePropertyChanged(() => TotalBookmarkCount);
                }
            }
        }

        public async Task LoadCommunityBookmarkCollection(bool isLoadmore = false)
        {
            if (IsBusy)
            {
                return;
            }

            IsBusy = true;

            using (DialogService.DisplayLoadingIndicator())
            {
                if (!isLoadmore)
                {
                    CommunityCollection = new LearnerObservableCollection<ItemCard>();
                }

                var bookmarkInfos = await ExecuteBackendService(() => _learnerBackendService.GetMyBookmarkedCommunityInfos(skipCount: CommunityCollection.IsNullOrEmpty() ? 0 : CommunityCollection.Count));

                if (!bookmarkInfos.HasEmptyResult() && bookmarkInfos.Payload.TotalCount > 0)
                {
                    TotalBookmarkCount = bookmarkInfos.Payload.TotalCount;

                    var guids = bookmarkInfos.Payload.Items.Select(p => p.ItemId).ToArray();

                    ApiResponse<ListCSLResultDto<Community>> communityDtoList = await ExecuteBackendService(() => _communityBackendService.GetCommunityByIds(
                        new GetCommunityByIdRequestModel(guids)));

                    if (!communityDtoList.HasEmptyResult() && communityDtoList.Payload.Total > 0)
                    {
                        var communitiesCard = CommunityCardBuilder.BuildCommunityCardListAsync(communityDtoList.Payload.Results, bookmarkInfos.Payload.Items);
                        CommunityCollection.AddRange(communitiesCard);
                        RaisePropertyChanged(() => CommunityCollection);
                    }
                }
            }

            IsBusy = false;
        }

        private async Task OnRefresh()
        {
            if (!CommunityCollection.IsNullOrEmpty())
            {
                CommunityCollection.Clear();
            }

            await LoadCommunityBookmarkCollection();
        }
    }
}
