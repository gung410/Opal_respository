using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using LearnerApp.Common;
using LearnerApp.Common.Enum;
using LearnerApp.Common.Helper;
using LearnerApp.Common.MessagingCenterManager;
using LearnerApp.Helper;
using LearnerApp.Models;
using LearnerApp.Models.Learner;
using LearnerApp.Models.MyLearning;
using LearnerApp.Models.Sharing;
using LearnerApp.Models.UserOnBoarding;
using LearnerApp.Resources.Texts;
using LearnerApp.Services;
using LearnerApp.Services.Backend;
using LearnerApp.Services.Navigation;
using LearnerApp.ViewModels.Base;
using LearnerApp.ViewModels.MyLearning;
using LearnerApp.ViewModels.Sharing;
using LearnerApp.Views.MyLearning;
using Plugin.Toast;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace LearnerApp.ViewModels
{
    public class MyDigitalContentDetailsViewModel : BasePageViewModel, INavigationAware, ISharingContentFormDelegate
    {
        private readonly StressActionHandler _stressActionHandler = new StressActionHandler();
        private readonly ICommonServices _commonService;
        private readonly ILearnerBackendService _learnerBackendService;
        private readonly IMetadataBackendService _metadataBackendService;
        private readonly IContentBackendService _contentBackendService;

        private string _contentTitle;
        private string _title;
        private string _startButtonLabel;
        private string _digitalContentStatus;
        private string _fileExtension;
        private string _userId;

        private bool _isVisibleReviewGroup;
        private bool _isVisibleDigitalContentStatus = true;
        private bool _isDigitalLearningClosed;
        private bool _isRefreshing = true;
        private bool _isVisibleStart = true;
        private double _rating;
        private List<string> _tags;

        private MyLearningDigitalContentMetadataTransfer _myLearningDigitalContentMetadataTransfer;
        private ObservableCollection<UserReview> _reviewCollection;
        private MyDigitalContentDetails _myDigitalContentDetails;
        private UserReview _ownReview;
        private MyLearningDigitalContentCardData _myDigitalContentData;
        private string _digitalContentId;

        // Permission
        private bool _isVisibleLike = true;
        private bool _isVisibleMore = true;
        private bool _isVisibleBookmark = true;
        private bool _isVisibleCopy = true;
        private bool _isVisibleShare = true;

        public MyDigitalContentDetailsViewModel()
        {
            CachingMode = PageCachingMode.None;
            _commonService = DependencyService.Resolve<ICommonServices>();
            _learnerBackendService = CreateRestClientFor<ILearnerBackendService>(GlobalSettings.BackendServiceLearner);
            _metadataBackendService = CreateRestClientFor<IMetadataBackendService>(GlobalSettings.BackendServiceTagging);
            _contentBackendService = CreateRestClientFor<IContentBackendService>(GlobalSettings.BackendServiceContent);

            MessagingCenter.Subscribe<MyDigitalContentLearningView>(this, "on-digital-content-learning-closed", async sender =>
            {
                _isDigitalLearningClosed = true;
                await InitDigitalContent();
                await InitMetadata();
            });
        }

        public ICommand MoreCommand => new Command(async () => await OnMoreTapped());

        public ICommand StartCommand => new Command(async () => await OnStartLearning());

        public ICommand RefreshCommand => new Command(async () => await Refresh());

        public ICommand EditUserCommand => new Command(review => OnEditUserComment(review as UserReview));

        public ICommand LikeCommand => new Command(async () => await LikeTapped());

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

        public string StartButtonLabel
        {
            get
            {
                return _startButtonLabel;
            }

            set
            {
                _startButtonLabel = value;

                RaisePropertyChanged(() => StartButtonLabel);
            }
        }

        public string ContentTitle
        {
            get
            {
                return _contentTitle;
            }

            set
            {
                _contentTitle = value;
                RaisePropertyChanged(() => ContentTitle);
            }
        }

        public bool IsVisibleReviewGroup
        {
            get
            {
                return _isVisibleReviewGroup;
            }

            set
            {
                _isVisibleReviewGroup = value;

                RaisePropertyChanged(() => IsVisibleReviewGroup);
            }
        }

        public bool IsVisibleDigitalContentStatus
        {
            get
            {
                return _isVisibleDigitalContentStatus;
            }

            set
            {
                _isVisibleDigitalContentStatus = value;

                RaisePropertyChanged(() => IsVisibleDigitalContentStatus);
            }
        }

        public MyLearningDigitalContentMetadataTransfer MyLearningDigitalContentMetadataTransfer
        {
            get
            {
                return _myLearningDigitalContentMetadataTransfer;
            }

            set
            {
                _myLearningDigitalContentMetadataTransfer = value;
                RaisePropertyChanged(() => MyLearningDigitalContentMetadataTransfer);
            }
        }

        public ObservableCollection<UserReview> ReviewCollection
        {
            get
            {
                return _reviewCollection;
            }

            set
            {
                _reviewCollection = value;

                RaisePropertyChanged(() => ReviewCollection);
            }
        }

        public string DigitalContentStatus
        {
            get
            {
                return _digitalContentStatus;
            }

            set
            {
                _digitalContentStatus = value;

                IsVisibleDigitalContentStatus = string.IsNullOrEmpty(_digitalContentStatus);

                RaisePropertyChanged(() => DigitalContentStatus);
            }
        }

        public double Rating
        {
            get
            {
                return _rating;
            }

            set
            {
                _rating = value;

                RaisePropertyChanged(() => Rating);
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
                RaisePropertyChanged(() => IsRefreshingForRefreshView);
                RaisePropertyChanged(() => IsRefreshing);
            }
        }

        public bool IsRefreshingForRefreshView
        {
            get
            {
                return false;
            }

            set
            {
                if (_isRefreshing == false)
                {
                    RaisePropertyChanged(() => IsRefreshingForRefreshView);
                }
            }
        }

        public string FileExtension
        {
            get
            {
                return _fileExtension;
            }

            set
            {
                _fileExtension = value;
                RaisePropertyChanged(() => FileExtension);
            }
        }

        public List<string> Tags
        {
            get
            {
                return _tags;
            }

            set
            {
                _tags = value;
                RaisePropertyChanged(() => Tags);
            }
        }

        public bool IsVisibleLike
        {
            get
            {
                return _isVisibleLike;
            }

            set
            {
                _isVisibleLike = value;
                RaisePropertyChanged(() => IsVisibleLike);
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

        public bool IsVisibleStart
        {
            get
            {
                return _isVisibleStart;
            }

            set
            {
                _isVisibleStart = value;
                RaisePropertyChanged(() => IsVisibleStart);
            }
        }

        public override string PageTitle => string.Empty;

        public override string RoutingName => NavigationRoutes.MyDigitalContentDetails;

        public static NavigationParameters GetNavigationParameters(string digitalContentId, bool startPlayer = false)
        {
            var navigationParameter = new NavigationParameters();
            navigationParameter.SetParameter("digital-content-id", digitalContentId);
            navigationParameter.SetParameter("start-player", startPlayer);
            return navigationParameter;
        }

        public override void Dispose()
        {
            base.Dispose();
            MessagingCenter.Unsubscribe<MyDigitalContentLearningView>(this, "on-digital-content-learning-closed");
        }

        public async Task<bool> AddShareUser(UserInformation userInformation)
        {
            var result = await ExecuteBackendService(
                () => _learnerBackendService.ShareContent(
                    new ShareContentArgumentsPayload()
                    {
                        ItemId = _digitalContentId,
                        ItemType = BookmarkType.DigitalContent.ToString(),
                        SharedUsers = new string[] { userInformation.UserCxId }
                    }));

            return result;
        }

        protected override async Task InternalNavigatedTo(NavigationParameters navigationParameters)
        {
            _digitalContentId = navigationParameters.GetParameter<string>("digital-content-id");
            var startPlayer = navigationParameters.GetParameter<bool>("start-player");

            await Init();

            if (startPlayer)
            {
                await OnStartLearning();
            }
        }

        private async Task OnMoreTapped()
        {
            await _stressActionHandler.RunAsync(async () =>
            {
                var isBookmark = _myDigitalContentData.MyDigitalContentSummary.BookmarkInfo == null;
                var stringBookmarkOption = isBookmark ? TextsResource.ADD_BOOKMARK : TextsResource.REMOVE_BOOKMARK;
                var stringCopyOption = TextsResource.COPY_URL;
                var stringShareOption = TextsResource.SHARE_OPTION;

                var items = new Dictionary<string, string>();

                if (_isVisibleShare && _isVisibleCopy)
                {
                    items.Add(stringShareOption, "social_share.svg");
                    items.Add(stringCopyOption, "copy.svg");
                }

                if (_isVisibleBookmark)
                {
                    items.Add(stringBookmarkOption, isBookmark ? "bookmark.svg" : "bookmarked.svg");
                }

                if (items.IsNullOrEmpty())
                {
                    return;
                }

                await DialogService.ShowDropDownSelectionPopup(items, isSeparateStringByUppercase: false, onSelected: async option =>
                {
                    if (string.IsNullOrEmpty(option))
                    {
                        return;
                    }

                    if (option == stringCopyOption)
                    {
                        await Clipboard.SetTextAsync(GetDeepLink.GetDigitalContentDeepLink(_digitalContentId));
                        CrossToastPopUp.Current.ShowToastSuccess(TextsResource.COPY_SUCCESSFULLY);
                    }
                    else if (option == stringBookmarkOption)
                    {
                        _myDigitalContentData.MyDigitalContentSummary.BookmarkInfo = await _commonService.Bookmark(_digitalContentId, BookmarkType.DigitalContent, isBookmark);

                        MyDigitalContentBookmarkMessagingCenter.Send(this, new DigitalContentBookmarkMessagingCenterArgs()
                        {
                            DigitalContentId = _digitalContentId,
                            IsBookmarked = isBookmark
                        });
                    }
                    else if (option == stringShareOption)
                    {
                        await NavigationService.NavigateToAsync<SharingContentFormViewModel>(
                            SharingContentFormViewModel.GetNavigationParameters(this));
                    }
                });
            });
        }

        private async Task GetCurrentReview()
        {
            // Get current review of learner
            var currentReviewResponse = await ExecuteBackendService(() => _learnerBackendService.GetCurrentUserReview(_myDigitalContentDetails.OriginalObjectId, PdActivityType.DigitalContent));

            if (!currentReviewResponse.HasEmptyResult())
            {
                _ownReview = currentReviewResponse.Payload;
            }
        }

        private void GetUserInformation()
        {
            var accountProperties = Application.Current.Properties.GetAccountProperties();

            if (accountProperties == null)
            {
                return;
            }

            _userId = accountProperties.User.Sub;
        }

        private async Task UpdateNumberOfView()
        {
            var payload = new TrackingDigitalContent
            {
                ItemId = _digitalContentId,
                ItemType = "digitalContent",
                TrackingAction = "view"
            };

            await _commonService.LearningTracking(TrackingEventType.LearningTracking, payload);
        }

        private async Task Refresh()
        {
            _isDigitalLearningClosed = true;

            await Init();
        }

        private async Task Init()
        {
            IsRefreshing = true;
            try
            {
                GetUserInformation();
                await InitDigitalContent();
                await InitMetadata();
                SettingAccessRight();
                await UpdateNumberOfView();
                await GetCurrentReview();
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        private async Task InitDigitalContent()
        {
            var digitalContentResponse = await ExecuteBackendService(() => _contentBackendService.GetDigitalContentDetails(_digitalContentId));
            if (digitalContentResponse.HasEmptyResult())
            {
                 return;
            }

            _myDigitalContentDetails = digitalContentResponse.Payload;
            ContentTitle = _myDigitalContentDetails.Title;

            FileExtension = _myDigitalContentDetails.FileExtension ?? "document";

            Tags = new List<string> { FileExtension.ToUpper() };

            var myDigitalContentSummaryResponse = await ExecuteBackendService(() => _learnerBackendService.GetDigitalContentDetails(_myDigitalContentDetails.OriginalObjectId));

            if (myDigitalContentSummaryResponse.HasEmptyResult())
            {
                return;
            }

            var myDigitalContentSummary = myDigitalContentSummaryResponse.Payload;

            Rating = myDigitalContentSummary.Rating;
            StartButtonLabel = myDigitalContentSummary.GetDigitalContentText();
            DigitalContentStatus = myDigitalContentSummary.GetDigitalContentStatus();

            _myDigitalContentData = new MyLearningDigitalContentCardData
            {
                MyDigitalContentSummary = myDigitalContentSummary,
                MyDigitalContentDetails = _myDigitalContentDetails
            };

            await LoadDigitalContentReviews();
        }

        private async Task LoadDigitalContentReviews()
        {
            if (!IsBusy)
            {
                IsBusy = true;

                // Get list reviews of content
                var reviewResponse = await ExecuteBackendService(() => _learnerBackendService.GetUserReviews(_myDigitalContentDetails.OriginalObjectId, PdActivityType.DigitalContent));

                if (reviewResponse.HasEmptyResult() || !reviewResponse.Payload.Items.Any())
                {
                    IsVisibleReviewGroup = false;
                }
                else
                {
                    foreach (var item in reviewResponse.Payload.Items)
                    {
                        item.IsOwnerReview = item.UserId == _userId;
                    }

                    ReviewCollection = new ObservableCollection<UserReview>(reviewResponse.Payload.Items);

                    IsVisibleReviewGroup = true;
                }

                IsBusy = false;
            }
        }

        private async Task OnStartLearning()
        {
            if (!IsBusy)
            {
                IsBusy = true;

                if (DigitalContentStatus != StatusLearning.InProgress.ToString() &&
                    DigitalContentStatus != StatusLearning.Completed.ToString())
                {
                    await ExecuteBackendService(() => _learnerBackendService.PostEnrollDigitalContent(new { DigitalContentId = _myDigitalContentDetails.OriginalObjectId }));
                    await InitDigitalContent();
                }

                await NavigationService.NavigateToAsync<MyDigitalContentLearningViewModel>(
                    MyDigitalContentLearningViewModel.GetNavigationParameters(
                        _ownReview,
                        _myDigitalContentData));

                IsBusy = false;
            }
        }

        private async Task InitMetadata()
        {
            var metadataTagging = Application.Current.Properties.GetMetadataTagging();

            if (metadataTagging.IsNullOrEmpty())
            {
                var tagging = await ExecuteBackendService(() => _metadataBackendService.GetMetadata());

                if (!tagging.HasEmptyResult())
                {
                    Application.Current.Properties.AddMetadataTagging(tagging.Payload);
                    metadataTagging = Application.Current.Properties.GetMetadataTagging();
                }
            }

            var myDigitalContentMetaDataResponse = await ExecuteBackendService(() => _metadataBackendService.GetDigitalContentMetadata(_myDigitalContentDetails.OriginalObjectId));

            var myDigitalContentMetaData = myDigitalContentMetaDataResponse.HasEmptyResult()
                ? new Resource()
                : myDigitalContentMetaDataResponse.Payload;

            var trackingInfoResponse = await ExecuteBackendService(() => _learnerBackendService.GetTrackingInfo(new
            {
                itemId = _digitalContentId,
                itemType = "DigitalContent"
            }));

            if (!trackingInfoResponse.HasEmptyResult())
            {
                _myDigitalContentData.MyDigitalContentSummary.SharesCount = trackingInfoResponse.Payload.TotalShare;
                _myDigitalContentData.MyDigitalContentSummary.TotalLike = trackingInfoResponse.Payload.TotalLike;
                _myDigitalContentData.MyDigitalContentSummary.IsLike = trackingInfoResponse.Payload.IsLike;
            }

            MyLearningDigitalContentMetadataTransfer = new MyLearningDigitalContentMetadataTransfer(metadataTagging, myDigitalContentMetaData, _myDigitalContentData, _isDigitalLearningClosed);

            _isDigitalLearningClosed = false;
        }

        private void OnEditUserComment(UserReview review)
        {
            DialogService.ShowEditCommentPopup(review, onSaved: async userReview =>
            {
                var courseReview = new
                {
                    ItemId = _myDigitalContentDetails.OriginalObjectId,
                    Rating = (int)review.Rate,
                    review.CommentContent,
                    ItemType = PdActivityType.DigitalContent.ToString()
                };

                // update comment
                var result = await ExecuteBackendService(() => _learnerBackendService.UpdateUserReview(_myDigitalContentDetails.OriginalObjectId, courseReview));

                if (!result.IsError)
                {
                    await LoadDigitalContentReviews();
                    CrossToastPopUp.Current.ShowToastSuccess("Update comment successfully");
                }
                else
                {
                    CrossToastPopUp.Current.ShowToastError("Update comment failed");
                }
            });
        }

        private async Task LikeTapped()
        {
            if (!IsBusy)
            {
                IsBusy = true;
                using (DialogService.DisplayLoadingIndicator())
                {
                    var likeResponse = await ExecuteBackendService(() => _learnerBackendService.Like(new
                    {
                        itemId = _digitalContentId,
                        itemType = "DigitalContent",
                        isLike = !MyLearningDigitalContentMetadataTransfer.IsLike
                    }));

                    if (!likeResponse.HasEmptyResult())
                    {
                        MyLearningDigitalContentMetadataTransfer.IsLike = likeResponse.Payload.IsLike;
                        MyLearningDigitalContentMetadataTransfer.TotalLike = likeResponse.Payload.TotalLike;
                        RaisePropertyChanged(() => MyLearningDigitalContentMetadataTransfer);
                    }

                    IsBusy = false;
                }
            }
        }

        /// <summary>
        /// Setting general permsission: bookmark, like, share, copy.
        /// </summary>
        private void SettingAccessRight()
        {
            _isVisibleBookmark = PermissionHelper.GetPermissionForBookmark();
            bool isShowLikeShareCopy = PermissionHelper.GetPermissionForLikeShareCopy(CardType.DigitalContent);
            IsVisibleLike = _isVisibleCopy = _isVisibleShare = isShowLikeShareCopy;
            if (!_isVisibleBookmark && !isShowLikeShareCopy)
            {
                IsVisibleMore = false;
            }

            if (StartButtonLabel == TextsResource.START_LEARNING)
            {
                if (!PermissionHelper.GetPermissionForStartLearning())
                {
                    IsVisibleStart = false;
                }
            }
        }
    }
}
