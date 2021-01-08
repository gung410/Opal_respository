using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using LearnerApp.Common;
using LearnerApp.Controls.DigitalContentPlayer;
using LearnerApp.Helper;
using LearnerApp.Models;
using LearnerApp.Models.Learner;
using LearnerApp.Models.MyLearning;
using LearnerApp.Models.MyLearning.DigitalContentPlayer;
using LearnerApp.Resources.Texts;
using LearnerApp.Services;
using LearnerApp.Services.Backend;
using LearnerApp.Services.Download;
using LearnerApp.Services.Navigation;
using LearnerApp.ViewModels.Base;
using Plugin.Toast;
using Plugin.Toast.Abstractions;
using Xamarin.Forms;

namespace LearnerApp.ViewModels.MyLearning
{
    public class MyDigitalContentLearningViewModel : BasePageViewModel, IDisposable
    {
        private readonly IUploaderBackendService _uploaderBackendService;
        private readonly ILearnerBackendService _learnerBackendService;
        private readonly IBrokenLinkBackendService _brokenLinkBackendService;
        private readonly IDownloader _downloader;
        private readonly IPathDownload _pathDownload;
        private readonly ICommonServices _commonServices;

        private FeedbackDataTransfer _feedbackData;
        private BaseDigitalContentPlayerData _playerData;
        private string _contentTitle;
        private string _playerSource;
        private string _previousContentId;
        private double _progressBarValue;
        private double _progressBarDownload;
        private bool _isVisibleContent;
        private bool _displayCompleteButton = true;
        private bool _isAllowDownload;
        private bool _isDownloading;
        private DigitalContentPlayerFullscreenHandler _fullscreenHandler;

        public MyDigitalContentLearningViewModel()
        {
            IsVisibleContent = true;
            ProgressBarValue = 0.5;

            _brokenLinkBackendService = CreateRestClientFor<IBrokenLinkBackendService>(GlobalSettings.BackendServiceBrokenLink);
            _learnerBackendService = CreateRestClientFor<ILearnerBackendService>(GlobalSettings.BackendServiceLearner);
            _uploaderBackendService = CreateRestClientFor<IUploaderBackendService>(GlobalSettings.BackendServiceUploader);

            _commonServices = DependencyService.Resolve<ICommonServices>(0);
            _pathDownload = DependencyService.Resolve<IPathDownload>();
            _downloader = DependencyService.Resolve<IDownloader>();

            _downloader.OnFileDownloaded += OnFileDownloaded;
        }

        public MyLearningDigitalContentCardData MyDigitalContentData { get; set; }

        public BaseDigitalContentPlayerData PlayerData
        {
            get
            {
                return _playerData;
            }

            set
            {
                _playerData = value;
                RaisePropertyChanged(() => PlayerData);
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

        public bool IsVisibleContent
        {
            get
            {
                return _isVisibleContent;
            }

            set
            {
                _isVisibleContent = value;
                RaisePropertyChanged(() => IsVisibleContent);
            }
        }

        public double ProgressBarValue
        {
            get
            {
                return _progressBarValue;
            }

            set
            {
                _progressBarValue = value;
                RaisePropertyChanged(() => ProgressBarValue);
            }
        }

        public double ProgressBarDownload
        {
            get
            {
                return _progressBarDownload;
            }

            set
            {
                _progressBarDownload = value;
                RaisePropertyChanged(() => ProgressBarDownload);
            }
        }

        public string PlayerSource
        {
            get
            {
                return _playerSource;
            }

            set
            {
                _playerSource = value;
                RaisePropertyChanged(() => PlayerSource);
            }
        }

        public FeedbackDataTransfer FeedbackData
        {
            get
            {
                return _feedbackData;
            }

            set
            {
                _feedbackData = value;
                RaisePropertyChanged(() => FeedbackData);
            }
        }

        public bool DisplayCompleteButton
        {
            get
            {
                return _displayCompleteButton;
            }

            set
            {
                _displayCompleteButton = value;
                RaisePropertyChanged(() => DisplayCompleteButton);
            }
        }

        public bool IsAllowDownload
        {
            get
            {
                return _isAllowDownload;
            }

            set
            {
                _isAllowDownload = value;
                RaisePropertyChanged(() => IsAllowDownload);
            }
        }

        public bool IsDownloading
        {
            get
            {
                return _isDownloading;
            }

            set
            {
                _isDownloading = value;
                RaisePropertyChanged(() => IsDownloading);
            }
        }

        public ICommand CompleteCommand => new Command(async () => await OnCompleted());

        public ICommand BrokenLinkReportCommand => new Command(async (brokenLinks) => await OnBrokenLinkReport(brokenLinks as List<string>));

        public ICommand DownloadCommand => new Command(async () => await DownloadFile());

        public override string PageTitle { get; } = string.Empty;

        public override string RoutingName { get; } = NavigationRoutes.MyDigitalLearningContentPlayer;

        public static NavigationParameters GetNavigationParameters(
            UserReview userReview,
            MyLearningDigitalContentCardData myLearningDigitalContentCardData)
        {
            var param = new NavigationParameters();
            param.SetParameter("user-review", userReview);
            param.SetParameter("my-learning-digital-content-card-data", myLearningDigitalContentCardData);

            return param;
        }

        public void SetFullscreenPlayerHandler(DigitalContentPlayerFullscreenHandler fullscreenHandler)
        {
            _fullscreenHandler = fullscreenHandler;
        }

        public override void Dispose()
        {
            base.Dispose();
            _downloader.OnFileDownloaded -= OnFileDownloaded;
        }

        public async Task InitMyDigitalContentLearning(UserReview userReview, MyLearningDigitalContentCardData myLearningDigitalContentCardData)
        {
            using (DialogService.DisplayLoadingIndicator())
            {
                MyDigitalContentData = myLearningDigitalContentCardData;
                ContentTitle = MyDigitalContentData.MyDigitalContentDetails.Title;
                IsAllowDownload = MyDigitalContentData.MyDigitalContentDetails.IsAllowDownload && PermissionHelper.GetPermissionForCheckinDoAssignmentDownloadContentDoPostCourse();

                FeedbackData = new FeedbackDataTransfer
                {
                    ContentId = MyDigitalContentData.MyDigitalContentDetails.OriginalObjectId,
                    ContentTitle = MyDigitalContentData.MyDigitalContentDetails.Title,
                    ItemType = PdActivityType.DigitalContent,
                    OwnReview = userReview,
                    IsMicrolearningType = false
                };

                await LoadDigitalContent();
            }
        }

        public async Task OnCompleted()
        {
            using (DialogService.DisplayLoadingIndicator())
            {
                PlayerData?.Close();
                IsVisibleContent = false;
                ProgressBarValue = 1;

                var request = new
                {
                    MyDigitalContentData.MyDigitalContentSummary.DigitalContentId,
                    Status = StatusLearning.Completed.ToString()
                };

                await ExecuteBackendService(() => _learnerBackendService.UpdateDigitalContentStatus(request));
            }
        }

        public async Task DownloadFile()
        {
            if (IsDownloading || (ProgressBarDownload < 1 && ProgressBarDownload != 0))
            {
                IsDownloading = _previousContentId.IsNullOrEmpty() || _previousContentId == MyDigitalContentData.MyDigitalContentDetails.Id;
                string message = _previousContentId.IsNullOrEmpty() || _previousContentId == MyDigitalContentData.MyDigitalContentDetails.Id
                        ? TextsResource.DOWNLOAD_PROCESSING
                        : TextsResource.DOWNLOAD_ANOTHER_PROCESSING;
                CrossToastPopUp.Current.ShowToastWarning(message, ToastLength.Long);
                return;
            }

            IsDownloading = true;
            await CountDownload();
            string correctString = await GetCorrectString();
            string folderName = _pathDownload.GetPath("Download");
            _downloader.DownloadFile(correctString, MyDigitalContentData.MyDigitalContentDetails.FileName, MyDigitalContentData.MyDigitalContentDetails.Id, folderName);
        }

        protected override async Task InternalNavigatedTo(NavigationParameters navigationParameters)
        {
            var userReview = navigationParameters.GetParameter<UserReview>("user-review");
            var myLearningDigitalContentCardData = navigationParameters.GetParameter<MyLearningDigitalContentCardData>("my-learning-digital-content-card-data");

            await InitMyDigitalContentLearning(
                userReview,
                myLearningDigitalContentCardData);
        }

        private async Task CountDownload()
        {
            var payload = new
            {
                ItemId = MyDigitalContentData.MyDigitalContentDetails.Id,
                TrackingAction = "downloadContent",
                TrackingType = "digitalContent"
            };
            await _commonServices.LearningTracking(TrackingEventType.LearningTracking, payload);
        }

        private void OnFileDownloaded(object sender, DownloadEventArgs e)
        {
            // Download complete or fail
            if (e.DownloadState == DownloadState.Completed)
            {
                CompleteDownload();
                CrossToastPopUp.Current.ShowToastSuccess("Downloaded Successfully");
            }
            else if (e.DownloadState == DownloadState.Failed)
            {
                CompleteDownload();
                CrossToastPopUp.Current.ShowToastError("Unable to download. You need to accept the access permission to download the file", ToastLength.Long);
            }
            else
            {
                ProgressBarDownload = e.ProgressPercentage / 100;
                _previousContentId = e.ContentId;
            }
        }

        private void CompleteDownload()
        {
            ProgressBarDownload = 0;
            IsDownloading = false;
            _previousContentId = string.Empty;
        }

        private async Task LoadDigitalContent()
        {
            string accessToken = (await IdentityService.GetAccountPropertiesAsync())?.AccessToken;

            if (MyDigitalContentData.MyDigitalContentDetails.Type == MyDigitalContentType.LearningContent)
            {
                PlayerData = new HtmlDigitalContentPlayerData(accessToken, MyDigitalContentData.MyDigitalContentDetails.HtmlContent);
            }
            else
            {
                if (MyDigitalContentData.MyDigitalContentDetails.FileExtension == "scorm")
                {
                    DisplayCompleteButton = false;
                    var scormData = new ScormDigitalContentPlayerData(
                        accessToken,
                        new ScormData
                        {
                            DigitalContentId = MyDigitalContentData.MyDigitalContentSummary.DigitalContentId,
                            MyLectureId = MyDigitalContentData.MyDigitalContentSummary.MyDigitalContent.Id,
                            ScormType = ScormType.DigitalContent
                        });

                    scormData.OnScormCompleted += () =>
                    {
                        DisplayCompleteButton = true;
                    };
                    PlayerData = scormData;
                }
                else
                {
                    string correctString = await GetCorrectString();

                    switch (MyDigitalContentData.MyDigitalContentDetails.FileExtension)
                    {
                        case "pdf":
                            PlayerData = new PdfDigitalContentPlayerData(correctString);
                            break;
                        case "svg":
                            PlayerData = new SvgImageDigitalContentPlayerData(correctString);
                            break;
                        case "png":
                        case "gif":
                        case "jpeg":
                        case "jpg":
                            PlayerData = new ImageDigitalContentPlayerData(correctString);
                            break;
                        case "ogg":
                        case "mp3":
                            PlayerData = new AudioDigitalContentPlayerData(correctString);
                            break;
                        case "mp4":
                        case "m4v":
                        case "ogv":
                            var videoDigitalContentData = new WebViewVideoDigitalContentPlayerData(
                                accessToken,
                                new VideoData()
                                {
                                    DigitalContentId = MyDigitalContentData.MyDigitalContentSummary.DigitalContentId
                                });

                            _fullscreenHandler.SetWebViewVideoDigitalPlayerVideoData(videoDigitalContentData);
                            PlayerData = videoDigitalContentData;
                            break;
                        case "docx":
                        case "xlsx":
                        case "pptx":
                        case "doc":
                        case "xls":
                        case "ppt":
                            PlayerData = new DocumentViewerDigitalContentPlayerData(
                                GlobalSettings.WebViewGoogleDocumentViewer + System.Web.HttpUtility.UrlEncode(correctString) + "&embedded=true");

                            break;
                    }
                }
            }

            if (PlayerData != null)
            {
                PlayerData.Description = MyDigitalContentData.MyDigitalContentDetails.Description;
            }
        }

        private async Task<string> GetCorrectString()
        {
            var requestFileUrl = await ExecuteBackendService(() => _uploaderBackendService.GetFile(MyDigitalContentData.MyDigitalContentDetails.FileLocation));

            if (requestFileUrl.HasEmptyResult() || string.IsNullOrEmpty(requestFileUrl.Payload))
            {
                return string.Empty;
            }

            // The result string has a lot of special character, the string incorrect so we need to handle.
            return requestFileUrl.Payload[0].Equals('"') ? requestFileUrl.Payload.Substring(1, requestFileUrl.Payload.Length - 2) : requestFileUrl.Payload;
        }

        private async Task OnBrokenLinkReport(List<string> brokenLinks)
        {
            await Device.InvokeOnMainThreadAsync(() =>
            {
                DialogService.ShowBrokenLinkReportPopup(brokenLinks, async (arg) =>
                {
                    var identityModel = Application.Current.Properties.GetAccountProperties();

                    await ExecuteBackendService(() => _brokenLinkBackendService.ReportBrokenLink(new
                    {
                        ObjectId = MyDigitalContentData.MyDigitalContentSummary.DigitalContentId,
                        Url = arg.Link,
                        arg.Description,
                        ObjectDetailUrl = GetDeepLink.GetDigitalContentDeepLink(MyDigitalContentData.MyDigitalContentSummary.DigitalContentId),
                        Module = "Content",
                        ObjectOwnerId = MyDigitalContentData.MyDigitalContentSummary.MyDigitalContent.CreatedBy,
                        ObjectTitle = ContentTitle,
                        MyDigitalContentData.MyDigitalContentDetails.OriginalObjectId,
                        ReporterName = identityModel.User.Name,
                        ContentType = "Lecture"
                    }));

                    CrossToastPopUp.Current.ShowToastSuccess("Report broken link successfully");
                });
            });
        }
    }
}
