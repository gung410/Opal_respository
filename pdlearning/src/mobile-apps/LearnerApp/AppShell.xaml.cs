using System;
using System.Collections.Generic;
using LearnerApp.Models.Permission;
using LearnerApp.Services.Navigation;
using LearnerApp.ViewModels;
using LearnerApp.ViewModels.Base;
using LearnerApp.Views;
using LearnerApp.Views.Achievement;
using LearnerApp.Views.Achievement.Badge;
using LearnerApp.Views.Achievement.ECertificate;
using LearnerApp.Views.Calendar;
using LearnerApp.Views.Home.OutstandingTasks;
using LearnerApp.Views.MyLearning;
using LearnerApp.Views.Recording;
using LearnerApp.Views.Report;
using LearnerApp.Views.Sharing;
using LearnerApp.Views.Sharing.Home;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LearnerApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            RegisterRoutes();

            MessagingCenter.Subscribe<HomeViewModel, PermissionTabPage>(this, "tab-page-view", (sender, tabPage) =>
            {
                HomeShell.IsVisible = tabPage.IsVisibleHomePage;
                CatalogueShell.IsVisible = tabPage.IsVisibleCataloguePage;

                // This is trick because Xamarin Shell cannot init page in case Home Page invisible
                if (tabPage.IsVisibleHomePage && !tabPage.IsVisibleLearningPage)
                {
                    LearningShell.IsVisible = false;
                }

                PlannerShell.IsVisible = tabPage.IsVisiblePlannerPage;
            });

            MessagingCenter.Subscribe<MyLearningViewModel>(this, "learning-view", (sender) =>
            {
                LearningShell.IsVisible = false;
            });
        }

        ~AppShell()
        {
            MessagingCenter.Unsubscribe<AppShell>(this, "tab-page-view");
            MessagingCenter.Unsubscribe<AppShell>(this, "learning-view");
        }

        private void RegisterRoutes()
        {
            Dictionary<string, Type> routes = new Dictionary<string, Type>();
            routes.Add(NavigationRoutes.Calendar, typeof(CalendarView));
            routes.Add(NavigationRoutes.EPortfolio, typeof(EportfolioView));
            routes.Add(NavigationRoutes.Social, typeof(SocialView));
            routes.Add(NavigationRoutes.Report, typeof(ReportPageView));

            routes.Add(NavigationRoutes.MyLearningPathDetails, typeof(MyLearningPathsDetailsView));
            routes.Add(NavigationRoutes.MyLearningShowAll, typeof(MyLearningPathsShowAllView));
            routes.Add(NavigationRoutes.MyLearningPathCreate, typeof(MyLearningPathsCreateNewView));
            routes.Add(NavigationRoutes.MyLearningPathShare, typeof(ShareMyLearningPathsView));

            routes.Add(NavigationRoutes.CourseDetails, typeof(CourseDetailsView));
            routes.Add(NavigationRoutes.CourseDetailsCanNotParticipate, typeof(CannotParticipateView));
            routes.Add(NavigationRoutes.CourseDetailsPost, typeof(PostCourseView));
            routes.Add(NavigationRoutes.CourseDetailsWithdrawal, typeof(WithdrawalReasonView));
            routes.Add(NavigationRoutes.LearningContentPlayer, typeof(LearningContentView));

            routes.Add(NavigationRoutes.CourseDetailsAssignment, typeof(AssignmentDetailView));
            routes.Add(NavigationRoutes.AssignmentContentPlayer, typeof(AssignmentLearningView));

            routes.Add(NavigationRoutes.CommunityDetails, typeof(CommunityDetailView));
            routes.Add(NavigationRoutes.CourseList, typeof(CoursesView));

            routes.Add(NavigationRoutes.MyDigitalContentDetails, typeof(MyDigitalContentDetailsView));
            routes.Add(NavigationRoutes.MyDigitalLearningContentPlayer, typeof(MyDigitalContentLearningView));

            routes.Add(NavigationRoutes.Settings, typeof(WidgetSettingView));

            routes.Add(NavigationRoutes.MyProfile, typeof(MyProfileView));
            routes.Add(NavigationRoutes.EditProfile, typeof(EditProfileView));
            routes.Add(NavigationRoutes.ChangePassword, typeof(ChangePasswordView));
            routes.Add(NavigationRoutes.TermsOfUse, typeof(TermsOfUseView));
            routes.Add(NavigationRoutes.PrivacyPolicy, typeof(TermsOfUseView));

            routes.Add(NavigationRoutes.Notifications, typeof(NotificationsView));
            routes.Add(NavigationRoutes.CheckIn, typeof(CheckInView));
            routes.Add(NavigationRoutes.OutstandingTasks, typeof(OutstandingTaskListPageView));
            routes.Add(NavigationRoutes.StandaloneForm, typeof(StandAloneFormView));

            routes.Add(NavigationRoutes.NewsFeed, typeof(ShowAllNewsfeedView));
            routes.Add(NavigationRoutes.SharingContent, typeof(SharingContentFormView));
            routes.Add(NavigationRoutes.SharingList, typeof(SharingListPageView));
            routes.Add(NavigationRoutes.Community, typeof(CommunityView));

            routes.Add(NavigationRoutes.Achievement, typeof(AchievementOverviewView));
            routes.Add(NavigationRoutes.AchievementBadge, typeof(AchievementBadgePageView));
            routes.Add(NavigationRoutes.AchievementECertificate, typeof(AchievementECertificatePageView));

            routes.Add(NavigationRoutes.RecordingPreview, typeof(RecordingPreviewPageView));

            foreach (var item in routes)
            {
                Routing.RegisterRoute(item.Key, item.Value);
            }
        }

        private void AppShell_OnNavigated(object sender, ShellNavigatedEventArgs e)
        {
            // The first level of CurrentItem is a Shell logical item, the second one is the actual page
            var currentItem = Current?.CurrentItem?.CurrentItem as IShellSectionController;

            if (currentItem != null)
            {
                var page = currentItem.PresentedPage;
                if (page.BindingContext is BasePageViewModel pageViewModel)
                {
                    pageViewModel.OnNavigatedTo(
                        NavigationParameterManager.RetrieveTransferParameter(
                            e.Current?.Location?.OriginalString));
                }
            }
        }
    }
}
