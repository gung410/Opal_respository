using System;
using System.Linq;
using LearnerApp.Controls.LearnerObservableCollection;
using LearnerApp.Helper;
using LearnerApp.Models.Course;
using LearnerApp.Models.Learner;
using LearnerApp.Services.Navigation;
using LearnerApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LearnerApp.Controls.Learner
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LearningOpportunityClassRunCardView : ContentView
    {
        public static readonly BindableProperty DataProperty =
            BindableProperty.Create(
                nameof(Data),
                typeof(LearningOpportunityClassRunCardTransfer),
                typeof(LearningOpportunityClassRunCardView),
                null,
                propertyChanged: OnClassRunChanged);

        private static LearningOpportunityClassRunCardViewModel _viewModel;
        private static LearningOpportunityClassRunCardView _view;
        private static LearningOpportunityClassRunCardTransfer _classRunCardTransfer;

        private static INavigationService _navigationService;

        private static bool _isBusy;

        public LearningOpportunityClassRunCardView()
        {
            InitializeComponent();

            _viewModel = new LearningOpportunityClassRunCardViewModel();
            _navigationService = DependencyService.Resolve<INavigationService>();
        }

        public event EventHandler OnSetLearningStatus;

        public LearningOpportunityClassRunCardTransfer Data
        {
            get { return (LearningOpportunityClassRunCardTransfer)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        private static async void OnClassRunChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (!(newValue is LearningOpportunityClassRunCardTransfer data))
            {
                return;
            }

            _view = (LearningOpportunityClassRunCardView)bindable;

            _viewModel.ClassRunCollection = new LearnerObservableCollection<ClassRun>();
            _classRunCardTransfer = data;

            // List class run
            _view.ClassRunCollection.ItemsSource = Enumerable.Empty<ClassRun>();
            var classRunCollection = await _viewModel.GetClassRunCollection(data);

            // Can do apply course, change class run, withdraw from class run by access rights
            if (!PermissionHelper.GetPermissionForClassRun())
            {
                foreach (var classRun in classRunCollection)
                {
                    classRun.IsVisibleClassRun = false;
                }
            }

            _view.ClassRunCollection.ItemsSource = classRunCollection;
        }

        private async void ApplyButton_Clicked(object sender, EventArgs e)
        {
            if (!(sender is Button applyButton))
            {
                return;
            }

            var classRunId = applyButton.CommandParameter.ToString();
            var classRunCollection = await _viewModel.ApplyClassRun(_classRunCardTransfer, classRunId, OnSetLearningStatus);
            _view.ClassRunCollection.ItemsSource = classRunCollection;
        }

        private async void JoinWebinar_Clicked(object sender, EventArgs e)
        {
            if (!(sender is Button joinWebinarButton))
            {
                return;
            }

            var sessionId = joinWebinarButton.CommandParameter.ToString();
            await _viewModel.JoinWebinar(sessionId);
        }

        private void ShowClassRunDetail_Tapped(object sender, EventArgs e)
        {
            if (!(e is TappedEventArgs tappedEventArgs))
            {
                return;
            }

            string classRunId = tappedEventArgs.Parameter.ToString();

            var response = _viewModel.ShowClassRunDetail(classRunId);

            _view.ClassRunCollection.ItemsSource = response.Item1;
        }

        private async void WithdrawButton_Clicked(object sender, EventArgs e)
        {
            if (!(sender is Button withdrawButton))
            {
                return;
            }

            var classRunId = withdrawButton.CommandParameter.ToString();

            if (string.IsNullOrEmpty(_classRunCardTransfer.ApprovingOfficer))
            {
                return;
            }

            await _viewModel.WithdrawClassRun(_classRunCardTransfer.MyClassRun, classRunId, _classRunCardTransfer.RegistrationMethod, OnSetLearningStatus);
        }

        private async void ShowMoreButton_OnClicked(object sender, EventArgs e)
        {
            if (_isBusy)
            {
                return;
            }

            _isBusy = true;
            _view.ClassRunCollection.ItemsSource = Enumerable.Empty<ClassRun>();
            var classRunCollection = await _viewModel.GetClassRunCollection(_classRunCardTransfer);
            _view.ClassRunCollection.ItemsSource = classRunCollection;
            _isBusy = false;
        }

        private async void ChangeClassRunButton_Clicked(object sender, EventArgs e)
        {
            if (!(sender is Button changeClassButton))
            {
                return;
            }

            var classRunId = changeClassButton.CommandParameter.ToString();
            await _viewModel.ChangeClassRun(_classRunCardTransfer.MyClassRun, classRunId, _classRunCardTransfer.RegistrationMethod, OnSetLearningStatus);
        }

        private void ShowClassRunInformationDetail_Tapped(object sender, EventArgs e)
        {
            if (!(e is TappedEventArgs tappedEventArgs))
            {
                return;
            }

            var classRunId = tappedEventArgs.Parameter.ToString();
            var response = _viewModel.ShowClassRunInformationDetail(classRunId);
            _view.ClassRunCollection.ItemsSource = response.Item1;
        }

        private void ShowSessionDetail_Tapped(object sender, EventArgs e)
        {
            if (!(e is TappedEventArgs tappedEventArgs))
            {
                return;
            }

            var sessionId = tappedEventArgs.Parameter.ToString();

            var response = _viewModel.ShowSessionDetail(sessionId);

            _view.ClassRunCollection.ItemsSource = response.Item1;
        }

        private async void Checkin_OnClicked(object sender, EventArgs e)
        {
            await _viewModel.OpenCheckInPopup();
        }

        private async void CannotParticipate_Tapped(object sender, EventArgs e)
        {
            if (!(e is TappedEventArgs tappedEventArgs))
            {
                return;
            }

            var sessionId = tappedEventArgs.Parameter.ToString();

            await _viewModel.OpenCannotParticipatePopup(sessionId);
        }

        private async void ShowAbsenceMessage_Tapped(object sender, EventArgs e)
        {
            if (!(e is TappedEventArgs tappedEventArgs))
            {
                return;
            }

            var sessionId = tappedEventArgs.Parameter.ToString();

            await _viewModel.OpenAbsenceMessagePopup(sessionId);
        }

        private async void WithdrawalReason_Clicked(object sender, EventArgs e)
        {
            if (!(e is TappedEventArgs tappedEventArgs))
            {
                return;
            }

            var classRunId = tappedEventArgs.Parameter.ToString();

            var registrationId = string.Empty;

            // Check registration in list of my class run
            var myClassRun = _classRunCardTransfer.MyClassRuns.FirstOrDefault(p => p.ClassRunId == classRunId);

            if (myClassRun != null)
            {
                registrationId = myClassRun.RegistrationId;
            }

            // Check registration in list of withdraw my class run
            if (string.IsNullOrEmpty(registrationId))
            {
                // Check registration in list of withdraw my class run
                var withdrawnMyClassRun = _classRunCardTransfer.WithdrawnMyClassRuns.FirstOrDefault(p => p.ClassRunId == classRunId);

                if (withdrawnMyClassRun != null)
                {
                    registrationId = withdrawnMyClassRun.RegistrationId;
                }
            }

            if (string.IsNullOrEmpty(registrationId))
            {
                return;
            }

            var parameters = new NavigationParameters();
            parameters.SetParameter("Title", "Withdrawal Reason");
            parameters.SetParameter("RegistrationId", registrationId);
            await _navigationService.NavigateToAsync<WithdrawalReasonViewModel>(parameters);
        }

        private async void OpenClassCommunity(object sender, EventArgs e)
        {
            if (!(e is TappedEventArgs tappedEventArgs))
            {
                return;
            }

            var classRunId = tappedEventArgs.Parameter.ToString();
            await _viewModel.OpenClassCommunity(classRunId);
        }
    }
}
