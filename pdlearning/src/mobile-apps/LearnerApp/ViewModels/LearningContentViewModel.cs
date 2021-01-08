using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using LearnerApp.Common;
using LearnerApp.Common.Enum;
using LearnerApp.Common.Helper;
using LearnerApp.Controls.DigitalContentPlayer;
using LearnerApp.Controls.Learner;
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
using Plugin.DeviceOrientation;
using Plugin.DeviceOrientation.Abstractions;
using Plugin.Toast;
using Plugin.Toast.Abstractions;
using Xamarin.Forms;

namespace LearnerApp.ViewModels
{
    public class LearningContentViewModel : BasePageViewModel
    {
        public string CourseId;

        private readonly ICommonServices _commonService;
        private readonly IContentBackendService _contentBackendService;
        private readonly IUploaderBackendService _uploaderBackendService;
        private readonly ILearnerBackendService _learnerBackendService;
        private readonly IBrokenLinkBackendService _brokenLinkBackendService;
        private readonly ICourseBackendService _courseBackendService;
        private readonly IDownloader _downloader;
        private readonly IPathDownload _pathDownload;
        private readonly StressActionHandler _stressActionHandler = new StressActionHandler(TimeSpan.FromSeconds(2));

        private BaseDigitalContentPlayerData _playerData;
        private List<MyLecturesInfo> _myLecturesInfo;
        private List<TableOfContent> _lectures;
        private FeedbackDataTransfer _learningOpportunityFeedbackTransfer;
        private string _lectureIndexLbl;
        private string _nextLectureBtnText;
        private string _currentLectureTitle;
        private TableOfContent _currentLecture;
        private int _currentLectureIndex;
        private int _totalLecture;
        private double _progressValue;
        private bool _isVisibleBackBtn;
        private bool _isVisibleNextBtn;
        private bool _isEnableNextBtn;
        private bool _isEnableBackBtn;
        private bool _isVisibleLearningContentSelection;
        private bool _isCourseCompleted;
        private bool _isMicrolearningType;
        private string _eCertificatePrerequisite;

        private string _previousContentId;
        private double _progressBarDownload;
        private bool _isVisibleDownload;
        private bool _isAllowDownload;
        private bool _isDownloading;

        // Visible learning content view
        private bool _isVisibleWebviewCard;
        private string _classRunId;
        private DigitalContentPlayerFullscreenHandler _fullscreenHandler;

        public LearningContentViewModel()
        {
            _commonService = DependencyService.Resolve<ICommonServices>();
            _pathDownload = DependencyService.Resolve<IPathDownload>();
            _downloader = DependencyService.Resolve<IDownloader>();

            _courseBackendService = CreateRestClientFor<ICourseBackendService>(GlobalSettings.BackendServiceCourse);
            _contentBackendService = CreateRestClientFor<IContentBackendService>(GlobalSettings.BackendServiceContent);
            _uploaderBackendService = CreateRestClientFor<IUploaderBackendService>(GlobalSettings.BackendServiceUploader);
            _learnerBackendService = CreateRestClientFor<ILearnerBackendService>(GlobalSettings.BackendServiceLearner);
            _brokenLinkBackendService = CreateRestClientFor<IBrokenLinkBackendService>(GlobalSettings.BackendServiceBrokenLink);

            IsVisibleLearningContentSelection = false;

            IsVisibleWebviewCard = true;

            NextLectureBtnText = "Next";

            MessagingCenter.Subscribe<LearningContentSelectionLectureCard, int>(this, "lecture-selected-item", async (sender, args) =>
            {
                _currentLectureIndex = args;

                await CreateLecture();

                ChangeLectureSelectionUi();
            });

            _downloader.OnFileDownloaded += OnFileDownloaded;
        }

        public bool IsViewAgain { get; set; }

        public MyDigitalContentDetails MyDigitalContentDetails { get; set; }

        public FeedbackDataTransfer LearningOpportunityFeedbackTransfer
        {
            get
            {
                return _learningOpportunityFeedbackTransfer;
            }

            set
            {
                _learningOpportunityFeedbackTransfer = value;
                RaisePropertyChanged(() => LearningOpportunityFeedbackTransfer);
            }
        }

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

        public List<TableOfContent> Lectures
        {
            get
            {
                return _lectures;
            }

            set
            {
                _lectures = value;
                RaisePropertyChanged(() => Lectures);
            }
        }

        public List<MyLecturesInfo> MyLecturesInfo
        {
            get
            {
                return _myLecturesInfo;
            }

            set
            {
                _myLecturesInfo = value;
                RaisePropertyChanged(() => MyLecturesInfo);
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

        public string QuizFormId { get; set; }

        public string MyCourseId { get; set; }

        public string LectureId { get; set; }

        public LectureType CurrentLectureContentType { get; set; }

        public string CurrentDigitalContentType { get; set; }

        public string NextLectureBtnText
        {
            get
            {
                return _nextLectureBtnText;
            }

            set
            {
                _nextLectureBtnText = value;
                RaisePropertyChanged(() => NextLectureBtnText);
            }
        }

        public string LectureIndexLbl
        {
            get
            {
                return _lectureIndexLbl;
            }

            set
            {
                _lectureIndexLbl = value;

                RaisePropertyChanged(() => LectureIndexLbl);

                // Set progress bar value
                ProgressValue = (float)_currentLectureIndex / _totalLecture;

                // Set Next button lecture is Finish when all lecture finish
                NextLectureBtnText = _currentLectureIndex == _totalLecture - 1 ? "Finish" : "Next";

                // Set visible or enable Next and Prev Ui
                if (_totalLecture == 1)
                {
                    IsVisibleBackBtn = false;
                    IsVisibleNextBtn = true;
                }
                else
                {
                    if (_currentLectureIndex == 0)
                    {
                        IsVisibleBackBtn = false;
                        IsVisibleNextBtn = true;
                    }
                    else
                    {
                        IsVisibleBackBtn = true;
                        IsEnableBackBtn = true;
                        IsVisibleNextBtn = true;
                    }
                }
            }
        }

        public double ProgressValue
        {
            get
            {
                return _progressValue;
            }

            set
            {
                _progressValue = value;
                RaisePropertyChanged(() => ProgressValue);
            }
        }

        public string CurrentLectureTitle
        {
            get
            {
                return _currentLectureTitle;
            }

            set
            {
                _currentLectureTitle = value;
                RaisePropertyChanged(() => CurrentLectureTitle);
            }
        }

        public TableOfContent CurrentLecture
        {
            get
            {
                return _currentLecture;
            }

            set
            {
                _currentLecture = value;
                RaisePropertyChanged(() => CurrentLecture);
            }
        }

        public bool IsVisibleLearningContentSelection
        {
            get
            {
                return _isVisibleLearningContentSelection;
            }

            set
            {
                _isVisibleLearningContentSelection = value;
                RaisePropertyChanged(() => IsVisibleLearningContentSelection);
            }
        }

        public bool IsVisibleWebviewCard
        {
            get
            {
                return _isVisibleWebviewCard;
            }

            set
            {
                _isVisibleWebviewCard = value;
                RaisePropertyChanged(() => IsVisibleWebviewCard);
            }
        }

        public bool IsVisibleBackBtn
        {
            get
            {
                return _isVisibleBackBtn;
            }

            set
            {
                _isVisibleBackBtn = value;
                RaisePropertyChanged(() => IsVisibleBackBtn);
            }
        }

        public bool IsVisibleNextBtn
        {
            get
            {
                return _isVisibleNextBtn;
            }

            set
            {
                _isVisibleNextBtn = value;
                RaisePropertyChanged(() => IsVisibleNextBtn);
            }
        }

        public bool IsEnableNextBtn
        {
            get
            {
                return _isEnableNextBtn;
            }

            set
            {
                _isEnableNextBtn = value;
                RaisePropertyChanged(() => IsEnableNextBtn);
            }
        }

        public bool IsEnableBackBtn
        {
            get
            {
                return _isEnableBackBtn;
            }

            set
            {
                _isEnableBackBtn = value;
                RaisePropertyChanged(() => IsEnableBackBtn);
            }
        }

        public bool IsVisibleDownload
        {
            get
            {
                return _isVisibleDownload;
            }

            set
            {
                _isVisibleDownload = value;
                RaisePropertyChanged(() => IsVisibleDownload);
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
                IsEnableNextBtn = IsEnableBackBtn = !_isDownloading;
            }
        }

        public ICommand CloseCommand => new Command(async () => await OnCloseTapped());

        public ICommand BackCommand => new Command(async () => await OnBackTapped());

        public ICommand NextCommand => new Command(async () => await OnNextTapped());

        public ICommand LectureSelectionCommand => new Command(async () => await OnLectureSelectionTapped());

        public ICommand BrokenLinkReportCommand => new Command(async (data) => await OnBrokenLinkReport(data as List<string>));

        public ICommand DownloadCommand => new Command(async () => await DownloadFile());

        public override string PageTitle { get; } = string.Empty;

        public override string RoutingName => NavigationRoutes.LearningContentPlayer;

        public static NavigationParameters GetNavigationParameters(LearningContentTransfer learningContentTransfer, bool isMicrolearningType, string eCertificatePrerequisite, bool isViewAgain)
        {
            var navigationParameter = new NavigationParameters();
            navigationParameter.SetParameter("learning-content-transfer", learningContentTransfer);
            navigationParameter.SetParameter("is-micro-learning-type", isMicrolearningType);
            navigationParameter.SetParameter("e-certificate-prerequisite", eCertificatePrerequisite);
            navigationParameter.SetParameter("is-view-again", isViewAgain);
            return navigationParameter;
        }

        public void SetFullscreenPlayerHandler(DigitalContentPlayerFullscreenHandler fullscreenHandler)
        {
            _fullscreenHandler = fullscreenHandler;
        }

        public override void Dispose()
        {
            base.Dispose();
            MessagingCenter.Unsubscribe<LearningContentSelectionLectureCard, int>(this, "lecture-selected-item");
            _downloader.OnFileDownloaded -= OnFileDownloaded;
        }

        public async Task InitLearningContent(LearningContentTransfer learningContentTransfer, bool isMicrolearningType, string eCertificatePrerequisite)
        {
            using (DialogService.DisplayLoadingIndicator())
            {
                LearningContentTransfer lecturesData = learningContentTransfer;
                _isMicrolearningType = isMicrolearningType;
                _eCertificatePrerequisite = eCertificatePrerequisite;

                Lectures = lecturesData.Lectures;
                CourseId = lecturesData.CourseId;
                MyCourseId = lecturesData.MyCourseId;

                _totalLecture = lecturesData.Lectures.Count;
                _isCourseCompleted = lecturesData.IsCourseCompleted;
                _classRunId = learningContentTransfer.ClassRunId;

                LearningOpportunityFeedbackTransfer = new FeedbackDataTransfer
                {
                    ContentId = CourseId,
                    ContentTitle = lecturesData.CourseName,
                    ItemType = PdActivityType.Courses,
                    OwnReview = lecturesData.OwnReview,
                    IsMicrolearningType = _isMicrolearningType,
                    HasContentChanged = learningContentTransfer.HasContentChanged
                };

                if (lecturesData.LectureIndex == -1)
                {
                    // In case click Enroll button
                    if (_isCourseCompleted)
                    {
                        _currentLectureIndex = 0;
                    }
                    else
                    {
                        await GetCourseProgress();
                    }
                }
                else
                {
                    // In case, jump from content list
                    _currentLectureIndex = lecturesData.LectureIndex;
                }

                await GetMyLecturesInfo(CourseId);
                await CreateLecture();
            }
        }

        public async Task OnNextLectureFromQuizView()
        {
            await _stressActionHandler.RunAsync(async () =>
            {
                await MakeLectureCompleted();
                _currentLectureIndex++;
                await CreateLecture();
            });
        }

        protected override async Task InternalNavigatedTo(NavigationParameters navigationParameters)
        {
            IsViewAgain = navigationParameters.GetParameter<bool>("is-view-again");

            await InitLearningContent(
                navigationParameters.GetParameter<LearningContentTransfer>("learning-content-transfer"),
                navigationParameters.GetParameter<bool>("is-micro-learning-type"),
                navigationParameters.GetParameter<string>("e-certificate-prerequisite"));
        }

        private async Task CheckDownload()
        {
            var lectureResource =
                await ExecuteBackendService(() => _courseBackendService.GetLectureResourceInfo(LectureId));

            if (lectureResource.IsError)
            {
                return;
            }

            var canDownload = lectureResource.Payload.DigitalContentConfig?.CanDownload;

            var resourceId = lectureResource.Payload.ResourceId;

            var lectureContent =
                await ExecuteBackendService(() => _contentBackendService.GetLectureContents(resourceId));

            if (lectureContent.IsError)
            {
                return;
            }

            var isUploadContent = lectureContent.Payload.Type == ContentType.UploadedContent;

            IsVisibleDownload = isUploadContent
                && canDownload.HasValue
                && canDownload.Value
                && _isAllowDownload
                && PermissionHelper.GetPermissionForCheckinDoAssignmentDownloadContentDoPostCourse();
        }

        private async Task DownloadFile()
        {
            if (IsDownloading || (ProgressBarDownload < 1 && ProgressBarDownload != 0))
            {
                IsDownloading = _previousContentId.IsNullOrEmpty() || _previousContentId == LectureId;
                string message = _previousContentId.IsNullOrEmpty() || _previousContentId == LectureId
                    ? TextsResource.DOWNLOAD_PROCESSING
                    : TextsResource.DOWNLOAD_ANOTHER_PROCESSING;
                CrossToastPopUp.Current.ShowToastWarning(message, ToastLength.Long);
                return;
            }

            IsDownloading = true;
            IsEnableNextBtn = false;
            await CountDownload();
            string correctString = await GetCorrectString();
            string folderName = _pathDownload.GetPath("Download");
            _downloader.DownloadFile(correctString, MyDigitalContentDetails.FileName, LectureId, folderName);
        }

        private async Task<string> GetCorrectString()
        {
            var requestFileUrl = await ExecuteBackendService(() => _uploaderBackendService.GetFile(MyDigitalContentDetails.FileLocation));

            if (requestFileUrl.HasEmptyResult() || string.IsNullOrEmpty(requestFileUrl.Payload))
            {
                return string.Empty;
            }

            // The result string has a lot of special character, the string incorrect so we need to handle.
            return requestFileUrl.Payload[0].Equals('"') ? requestFileUrl.Payload.Substring(1, requestFileUrl.Payload.Length - 2) : requestFileUrl.Payload;
        }

        private void OnFileDownloaded(object sender, DownloadEventArgs e)
        {
            switch (e.DownloadState)
            {
                case DownloadState.Completed:
                    CompleteDownload();
                    CrossToastPopUp.Current.ShowToastSuccess("Downloaded Successfully");
                    break;
                case DownloadState.Failed:
                    CompleteDownload();
                    CrossToastPopUp.Current.ShowToastError("Unable to download. You need to accept the access permission to download the file", ToastLength.Long);
                    break;
                default:
                    ProgressBarDownload = e.ProgressPercentage / 100;
                    _previousContentId = e.ContentId;
                    break;
            }
        }

        private void CompleteDownload()
        {
            ProgressBarDownload = 0;
            IsDownloading = false;
            _previousContentId = string.Empty;
        }

        private async Task CountDownload()
        {
            var payload = new
            {
                ItemId = MyDigitalContentDetails.Id,
                TrackingAction = "downloadContent",
                TrackingType = "digitalContent"
            };
            await _commonService.LearningTracking(TrackingEventType.LearningTracking, payload);
        }

        private async Task OnCloseTapped()
        {
            await _stressActionHandler.Run(() =>
            {
                if (IsDownloading)
                {
                    CrossToastPopUp.Current.ShowToastWarning(TextsResource.DOWNLOAD_PROCESSING, ToastLength.Long);
                }
                else
                {
                    PlayerData?.Close();

                    MessagingCenter.Send(this, "on-closed-quiz");
                }
            });
        }

        private async Task OnNextTapped()
        {
            await _stressActionHandler.RunAsync(async () =>
            {
                CrossDeviceOrientation.Current.LockOrientation(DeviceOrientations.Portrait);

                PlayerData?.Close();
                IsVisibleLearningContentSelection = false;

                // Make lecture completed when Quiz or Scorm completed
                await MakeLectureCompleted();
                _currentLectureIndex++;
                await CreateLecture();
            });
        }

        private async void OnLectureFinished(string data)
        {
            await _stressActionHandler.RunAsync(async () =>
            {
                await MakeLectureCompleted();
            });
        }

        private async Task OnBackTapped()
        {
            await _stressActionHandler.RunAsync(async () =>
            {
                CrossDeviceOrientation.Current.LockOrientation(DeviceOrientations.Portrait);

                PlayerData?.Close();
                IsVisibleLearningContentSelection = false;

                _currentLectureIndex--;
                await CreateLecture();
            });
        }

        private async Task OnLectureSelectionTapped()
        {
            using (DialogService.DisplayLoadingIndicator())
            {
                if (!IsVisibleLearningContentSelection)
                {
                    await GetMyLecturesInfo(CourseId);
                }

                ChangeLectureSelectionUi();
            }
        }

        private void ChangeLectureSelectionUi()
        {
            IsVisibleLearningContentSelection = !IsVisibleLearningContentSelection;
        }

        private async Task CreateLecture()
        {
            // Set default for next button on start is true
            IsEnableNextBtn = false;

            // Finish course, mark complete
            if (_currentLectureIndex == _totalLecture)
            {
                await MarkCompleteCourse();

                return;
            }

            // Reset webview with change WebviewContentType and WebviewSource in case same value source like: digital content, ...
            IsVisibleWebviewCard = true;
            CurrentDigitalContentType = string.Empty;
            CurrentLectureContentType = LectureType.None;
            LectureIndexLbl = $"{_currentLectureIndex + 1} of {_totalLecture}";
            MyLecturesInfo myLectureInfo = _myLecturesInfo.FirstOrDefault(p => p.LectureId == LectureId && p.Status == StatusLearning.Completed);

            if (_currentLectureIndex < _totalLecture)
            {
                TableOfContent currentLecture = _lectures[_currentLectureIndex];
                CurrentLecture = currentLecture;
                CurrentLectureTitle = currentLecture.Title;
                CurrentLectureContentType = currentLecture.AdditionalInfo.Type;
                LectureId = currentLecture.Id;

                string accessToken = (await IdentityService.GetAccountPropertiesAsync()).AccessToken;

                // Create lecture
                switch (CurrentLectureContentType)
                {
                    case LectureType.Quiz:
                        var quizContentPlayerData = new QuizDigitalContentPlayerData(accessToken, new QuizData
                        {
                            CourseId = CourseId,
                            QuizFormId = CurrentLecture.AdditionalInfo.ResourceId,
                            AdditionalInfo = CurrentLecture.AdditionalInfo,
                            IsViewAgain = IsViewAgain,
                            MyCourseId = MyCourseId
                        });

                        quizContentPlayerData.OnLectureFinished += OnLectureFinished;
                        quizContentPlayerData.OnQuizFinished += OnQuizFinish;

                        if (myLectureInfo != null)
                        {
                            // Enable next button when quiz start
                            IsEnableNextBtn = true;
                        }

                        PlayerData = quizContentPlayerData;
                        break;
                    case LectureType.InlineContent:
                        PlayerData = new HtmlDigitalContentPlayerData(
                            accessToken,
                            CurrentLecture.AdditionalInfo.Value);

                        // Enable next button in case inline content
                        IsEnableNextBtn = true;
                        break;
                    case LectureType.DigitalContent:
                        var digitalContentResponse = await ExecuteBackendService(() => _contentBackendService.GetDigitalContentDetails(currentLecture.AdditionalInfo.ResourceId));

                        if (!digitalContentResponse.HasEmptyResult())
                        {
                            MyDigitalContentDetails = digitalContentResponse.Payload;
                            CurrentDigitalContentType = digitalContentResponse.Payload.FileExtension;
                            _isAllowDownload = digitalContentResponse.Payload.IsAllowDownload;

                            if (MyDigitalContentDetails.Type == MyDigitalContentType.LearningContent)
                            {
                                PlayerData = new HtmlDigitalContentPlayerData(
                                    accessToken,
                                    MyDigitalContentDetails?.HtmlContent);
                            }
                            else
                            {
                                await CheckDownload();

                                if (MyDigitalContentDetails.FileExtension == "scorm")
                                {
                                    var lectureInfo = _myLecturesInfo.FirstOrDefault(p => p.LectureId == currentLecture.Id);

                                    PlayerData = new ScormDigitalContentPlayerData(
                                        accessToken,
                                        new ScormData
                                        {
                                            DigitalContentId = currentLecture.AdditionalInfo.ResourceId,
                                            MyLectureId = lectureInfo?.Id,
                                            ScormType = ScormType.LearningContent
                                        });
                                }
                                else
                                {
                                    MyDigitalContentDetails.SignedUrl = await GetCorrectString();

                                    switch (MyDigitalContentDetails.FileExtension)
                                    {
                                        case "pdf":
                                            PlayerData = new PdfDigitalContentPlayerData(MyDigitalContentDetails.SignedUrl);
                                            break;
                                        case "svg":
                                            PlayerData = new SvgImageDigitalContentPlayerData(MyDigitalContentDetails.SignedUrl);
                                            break;
                                        case "png":
                                        case "gif":
                                        case "jpeg":
                                        case "jpg":
                                            PlayerData = new ImageDigitalContentPlayerData(MyDigitalContentDetails.SignedUrl);
                                            break;
                                        case "ogg":
                                        case "mp3":
                                            PlayerData = new AudioDigitalContentPlayerData(MyDigitalContentDetails.SignedUrl);
                                            break;
                                        case "mp4":
                                        case "m4v":
                                        case "ogv":
                                            var videoDigitalContentData = new WebViewVideoDigitalContentPlayerData(accessToken, new VideoData()
                                            {
                                                LectureId = this.LectureId,
                                                DigitalContentId = currentLecture.AdditionalInfo.ResourceId,
                                                ClassRunId = _classRunId,
                                                MyLectureId = myLectureInfo?.Id ?? currentLecture.Id,
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
                                            PlayerData =
                                                new DocumentViewerDigitalContentPlayerData(
                                                    GlobalSettings.WebViewGoogleDocumentViewer
                                                    + System.Web.HttpUtility.UrlEncode(MyDigitalContentDetails.SignedUrl)
                                                    + "&embedded=true");
                                            break;
                                    }
                                }
                            }
                        }

                        PlayerData.Description = MyDigitalContentDetails?.Description;

                        IsEnableNextBtn = true;

                        break;
                }
            }
        }

        private async Task MarkCompleteCourse()
        {
            ProgressValue = 1;

            CurrentLectureTitle = string.Empty;

            IsVisibleWebviewCard = false;
            IsVisibleBackBtn = true;
            IsEnableBackBtn = true;
            IsVisibleNextBtn = false;

            if (_isMicrolearningType)
            {
                if (_isCourseCompleted)
                {
                    return;
                }

                await UpdateStatusCourse();
            }
            else
            {
                if (!_isCourseCompleted && "CompletionCourse".Equals(_eCertificatePrerequisite))
                {
                    await UpdateStatusCourse();
                }
            }
        }

        private async Task UpdateStatusCourse()
        {
            var request = new
            {
                CourseId,
                Status = StatusLearning.Completed.ToString()
            };

            await ExecuteBackendService(() => _learnerBackendService.UpdateCourseStatus(request));
        }

        private async Task MakeLectureCompleted()
        {
            // Mark completed lecture
            if (!_isCourseCompleted && !_myLecturesInfo.IsNullOrEmpty())
            {
                MyLecturesInfo myLectureInfo = _myLecturesInfo.FirstOrDefault(p => p.LectureId == LectureId && p.Status != StatusLearning.Completed);

                if (myLectureInfo != null)
                {
                    var payload = new TrackingLecture
                    {
                        LectureId = myLectureInfo.Id,
                        CourseId = CourseId
                    };

                    await _commonService.LearningTracking(TrackingEventType.FinishLecture, payload);
                    await _learnerBackendService.CompleteCourseLecture(myLectureInfo.Id);
                    myLectureInfo.Status = StatusLearning.Completed;
                }
            }
        }

        private async Task GetCourseProgress()
        {
            var result = await ExecuteBackendService(() => _learnerBackendService.GetCourseDataFromLearners(new { CourseIds = new[] { CourseId } }));

            if (!result.HasEmptyResult())
            {
                List<MyCourseSummary> courses = result.Payload;

                if (!courses.IsNullOrEmpty())
                {
                    MyCourseSummary currentMyCourseSummary = courses.FirstOrDefault();

                    if (currentMyCourseSummary?.MyCourseInfo != null)
                    {
                        _currentLectureIndex = (int)Math.Round((float)currentMyCourseSummary.MyCourseInfo.ProgressMeasure * _totalLecture / 100);
                    }
                }
            }
        }

        private async Task GetMyLecturesInfo(string courseId)
        {
            var result = await ExecuteBackendService(() => _learnerBackendService.GetMyCourseSummary(courseId));

            if (!result.HasEmptyResult())
            {
                MyCourseSummary myLectures = result.Payload;

                if (myLectures != null && !myLectures.MyLecturesInfo.IsNullOrEmpty())
                {
                    MyLecturesInfo = myLectures.MyLecturesInfo;
                }
            }
        }

        private async Task OnBrokenLinkReport(List<string> brokenLinks)
        {
            await Device.InvokeOnMainThreadAsync(() =>
            {
                DialogService.ShowBrokenLinkReportPopup(brokenLinks, async (arg) =>
                {
                    var lectureResource =
                        await ExecuteBackendService(() => _courseBackendService.GetLectureResourceInfo(LectureId));

                    if (lectureResource.IsError)
                    {
                        return;
                    }

                    var resourceId = lectureResource.Payload.ResourceId;

                    var identityModel = Application.Current.Properties.GetAccountProperties();

                    bool result = await ExecuteBackendService(() => _brokenLinkBackendService.ReportBrokenLink(new
                    {
                        ObjectId = resourceId,
                        Url = arg.Link,
                        arg.Description,
                        ObjectDetailUrl = GetDeepLink.GetCourseDeepLink(CourseId),
                        Module = ModuleType.Content.ToString(),
                        ObjectOwnerId = MyDigitalContentDetails.OwnerId,
                        ParentId = CourseId,
                        ObjectTitle = CurrentLectureTitle,
                        MyDigitalContentDetails.OriginalObjectId,
                        ReporterName = identityModel.User.Name,
                        ContentType = ContentType.LearningContent.ToString()
                    }));

                    if (result)
                    {
                        CrossToastPopUp.Current.ShowToastSuccess("Report broken link successfully");
                    }
                });
            });
        }

        private async void OnQuizFinish(string data)
        {
            await OnNextLectureFromQuizView();
        }
    }
}
