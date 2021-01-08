using System;
using LearnerApp.Common;
using LearnerApp.Models;
using LearnerApp.Models.Newsfeed;
using LearnerApp.Services.Dialog;
using LearnerApp.Services.Navigation;
using LearnerApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LearnerApp.Views.Home.Newsfeed
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PdpmSuggestCourseFeedItem : ContentView
    {
        private static INavigationService _navigationService;
        private static IDialogService _dialogService;

        public PdpmSuggestCourseFeedItem()
        {
            InitializeComponent();

            _dialogService = DependencyService.Resolve<IDialogService>();
            _navigationService = DependencyService.Resolve<INavigationService>();
        }

        private async void CourseDetails_Tapped(object sender, EventArgs e)
        {
            ItemCard courseCard = null;

            using (_dialogService.DisplayLoadingIndicator())
            {
                Feed feed = (e as TappedEventArgs).Parameter as Feed;

                var model = new NewsfeedViewModel();
                courseCard = await model.GetCourseCard(feed.CourseId);
            }

            if (courseCard == null)
            {
                return;
            }

            if (!courseCard.IsExpired && !courseCard.CourseStatus.Equals(nameof(StatusCourse.Unpublished)) && !courseCard.CourseStatus.Equals(nameof(StatusLearning.Archived)))
            {
                await _navigationService.NavigateToAsync<CourseDetailsViewModel>(
                    CourseDetailsViewModel.GetNavigationParameters(courseCard.Id, courseCard.CardType));
            }
        }
    }
}
