using System;
using System.Linq;
using System.Threading.Tasks;
using LearnerApp.Common;
using LearnerApp.Controls;
using LearnerApp.Models.Course;
using LearnerApp.Services;
using Newtonsoft.Json;
using Plugin.HybridWebView.Shared.Enumerations;
using Xamarin.Forms;

namespace LearnerApp.Models.MyLearning.DigitalContentPlayer
{
    public class QuizDigitalContentPlayerData : BaseDigitalContentPlayerData
    {
        public Action<string> OnLectureFinished;
        public Action<string> OnQuizFinished;

        private readonly string _accessToken;
        private readonly QuizData _quizData;
        private readonly ICommonServices _commonService;

        private Controls.DigitalContentPlayer.DigitalContentPlayer _digitalContentPlayer;
        private string _playingSessionId;

        public QuizDigitalContentPlayerData(string accessToken, QuizData quizData)
        {
            _accessToken = accessToken;
            _quizData = quizData;
            _commonService = DependencyService.Resolve<ICommonServices>();
        }

        public override void Close()
        {
            if (ShouldTrackingQuiz())
            {
                var payload = new TrackingQuiz
                {
                    PlayingSessionId = _playingSessionId,
                    FormId = _quizData.QuizFormId,
                    FormAnswerId = null
                };

                _commonService.LearningTracking(TrackingEventType.StopQuiz, payload);
            }

            _playingSessionId = null;
            base.Close();
        }

        public override View GetContentView()
        {
            var quiz = new CustomHybridWebView
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                ContentType = WebViewContentType.Internet,
                Source = GlobalSettings.WebViewUrlQuizPlayer
            };
            quiz.OnNavigationStarted += (obj, args) => ShouldHandleUrlNavigation(args, GlobalSettings.WebViewUrlQuizPlayer);
            quiz.OnNavigationCompleted += OnQuizNavigationCompleted;

            return quiz;
        }

        public override void LoadStyleForDigitalContentPlayer(Controls.DigitalContentPlayer.DigitalContentPlayer digitalContentPlayer)
        {
            _digitalContentPlayer = digitalContentPlayer;
        }

        private async void OnQuizNavigationCompleted(object sender, string e)
        {
            var card = sender as CustomHybridWebView;

            if (Device.RuntimePlatform == Device.iOS)
            {
                await InitQuizPlayer(card);
                card.AddLocalCallback("CCPM_Mobile_On_Quiz_Finished_Handler", (data) => OnQuizFinishHandler(data));
                await card.InjectJavascriptAsync("AppGlobal.quizPlayerIntegrations.onQuizFinishedForMobile = CCPM_Mobile_On_Quiz_Finished_Handler;");

                card.AddLocalCallback("CCPM_Mobile_On_Quiz_Submitted_Handler", (data) => OnQuizSubmitted(data));
                await card.InjectJavascriptAsync("AppGlobal.quizPlayerIntegrations.onQuizSubmitted = CCPM_Mobile_On_Quiz_Submitted_Handler;");
            }
            else
            {
                var observer = new Observer();
                observer.CallAfter(
                    async () =>
                    {
                        using (DialogService.DisplayLoadingIndicator())
                        {
                            await InitQuizPlayer(card);

                            card.AddLocalCallback("CCPM_Mobile_On_Quiz_Finished_Handler", async (data) =>
                            {
                                OnQuizFinishHandler(data);
                                await InitQuizPlayer(card);
                            });

                            await card.InjectJavascriptAsync("AppGlobal.quizPlayerIntegrations.onQuizFinishedForMobile = CCPM_Mobile_On_Quiz_Finished_Handler;");

                            card.AddLocalCallback("CCPM_Mobile_On_Quiz_Submitted_Handler", (data) => OnQuizSubmitted(data));
                            await card.InjectJavascriptAsync("AppGlobal.quizPlayerIntegrations.onQuizSubmitted = CCPM_Mobile_On_Quiz_Submitted_Handler;");
                        }
                    },
                    2000);
            }
        }

        private async Task InitQuizPlayer(CustomHybridWebView card)
        {
            _playingSessionId = Guid.NewGuid().ToString();

            if (ShouldTrackingQuiz())
            {
                var payload = new TrackingQuiz
                {
                    PlayingSessionId = _playingSessionId,
                    FormId = _quizData.QuizFormId,
                    FormAnswerId = null
                };

                await _commonService.LearningTracking(TrackingEventType.PlayQuiz, payload);
            }

            string stringEnablePassingRate = EnablePassingRate(_quizData.AdditionalInfo.QuizConfig) ? "true" : "false";
            string isViewAgainStr = _quizData.IsViewAgain ? "true" : "false";

            await card.InjectJavascriptAsync($"AppGlobal.quizPlayerIntegrations.setAuthToken('{_accessToken}')");
            await card.InjectJavascriptAsync($"AppGlobal.quizPlayerIntegrations.setFormId('{_quizData.QuizFormId}')");
            await card.InjectJavascriptAsync($"AppGlobal.quizPlayerIntegrations.setResourceId('{_quizData.CourseId}')");
            await card.InjectJavascriptAsync($"AppGlobal.quizPlayerIntegrations.setPassingRateEnableStringValue('{stringEnablePassingRate}')");
            await card.InjectJavascriptAsync($"AppGlobal.quizPlayerIntegrations.setReviewOnlyStringValue('{isViewAgainStr}')");
            await card.InjectJavascriptAsync($"AppGlobal.quizPlayerIntegrations.setMyCourseId('{_quizData.MyCourseId}')");
        }

        private void OnQuizFinishHandler(string data)
        {
            OnQuizFinished?.Invoke(data);
        }

        private void OnQuizSubmitted(string data)
        {
            if (ShouldTrackingQuiz())
            {
                var detail = JsonConvert.DeserializeObject<AnswerDetail>(data);

                var payload = new TrackingQuiz
                {
                    PlayingSessionId = _playingSessionId,
                    FormId = _quizData.QuizFormId,
                    FormAnswerId = detail.QuestionAnswers.First()?.FormAnswerId
                };

                _commonService.LearningTracking(TrackingEventType.SubmittedQuizAnswer, payload);
            }

            OnLectureFinished?.Invoke(data);
        }

        private bool EnablePassingRate(QuizConfig quizConfig)
        {
            return quizConfig == null || quizConfig.ByPassPassingRate == false;
        }

        private bool ShouldTrackingQuiz()
        {
            if (_digitalContentPlayer == null)
            {
                return false;
            }

            return _playingSessionId != null && _quizData.QuizFormId != null;
        }
    }
}
