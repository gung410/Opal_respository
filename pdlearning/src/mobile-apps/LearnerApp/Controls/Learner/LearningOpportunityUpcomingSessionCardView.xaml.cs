using System;
using LearnerApp.Helper;
using LearnerApp.Models.Course;
using LearnerApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LearnerApp.Controls.Learner
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LearningOpportunityUpcomingSessionCardView : ContentView
    {
        public static readonly BindableProperty UpComingSessionProperty = BindableProperty.Create(
            nameof(UpComingSession),
            typeof(UpComingTransferData),
            typeof(LearningOpportunityUpcomingSessionCardView),
            null,
            propertyChanged: OnUpComingSessionChanged);

        private static LearningOpportunityUpcomingSessionCardViewModel _viewModel;

        private static string _sessionId;

        public LearningOpportunityUpcomingSessionCardView()
        {
            InitializeComponent();
            _viewModel = new LearningOpportunityUpcomingSessionCardViewModel();
        }

        public UpComingTransferData UpComingSession
        {
            get { return (UpComingTransferData)GetValue(UpComingSessionProperty); }
            set { SetValue(UpComingSessionProperty, value); }
        }

        private static void OnUpComingSessionChanged(BindableObject bindable, object oldvalue, object newValue)
        {
            var upComingTransferData = (UpComingTransferData)newValue;

            var view = (LearningOpportunityUpcomingSessionCardView)bindable;

            Device.BeginInvokeOnMainThread(() =>
            {
                _sessionId = upComingTransferData.GetSessionId();
                view.Checkin.IsVisible = PermissionHelper.GetPermissionForCheckinDoAssignmentDownloadContentDoPostCourse();
                view.Title.Text = upComingTransferData.GetSessionTitle();
                view.StartDate.Text = upComingTransferData.GetStartDate();
                view.EndDate.Text = upComingTransferData.GetEndDate();
                view.Venue.Text = upComingTransferData.GetSessionVenue();
                view.Checkin.IsVisible = upComingTransferData.CanShowCheckin();
                view.CannotParticipateBtn.IsVisible = upComingTransferData.CanShowAbsent();
            });
        }

        private async void CannotParticipate_Tapped(object sender, EventArgs e)
        {
            await _viewModel.OpenCannotParticipatePopup(_sessionId);
        }

        private async void Checkin_OnClicked(object sender, EventArgs e)
        {
            await _viewModel.OpenCheckInPopup();
        }
    }
}
