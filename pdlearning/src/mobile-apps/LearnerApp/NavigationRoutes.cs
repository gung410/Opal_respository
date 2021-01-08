namespace LearnerApp
{
    public static class NavigationRoutes
    {
        public const string Login = "//login";
        public const string Logout = "//logout";
        public const string LandingPage = "//landing-page";
        public const string Onboarding = "//onboarding";

        public const string Main = "//main";

        public const string Home = "home";
        public const string Catalogue = "catalogue";

        public const string MyLearning = "my-learning";
        public const string MyLearningShowAll = "show-all";

        public const string PdPlanner = "pd-planner";

        public const string Calendar = "calendar";
        public const string EPortfolio = "e-portfolio";
        public const string Social = "social";
        public const string Report = "report";

        public const string MyLearningPathDetails = "my-learning-path-details";
        public const string MyLearningPathCreate = MyLearningPathDetails + "/create";
        public const string MyLearningPathShare = MyLearningPathDetails + "/share";

        public const string CourseDetails = "course-details";
        public const string CourseDetailsCanNotParticipate = CourseDetails + "/can-not-participate";
        public const string CourseDetailsAssignment = "assignment";
        public const string AssignmentContentPlayer = CourseDetailsAssignment + "/assignment-content-player";

        public const string CourseDetailsPost = CourseDetails + "/post";
        public const string CourseDetailsWithdrawal = CourseDetails + "/withdrawal";
        public const string LearningContentPlayer = CourseDetails + "/learning-content-player";

        public const string CourseList = "course-list";

        public const string CommunityDetails = "community-details";
        public const string MyDigitalContentDetails = "my-digital-content-details";
        public const string MyDigitalLearningContentPlayer = MyDigitalContentDetails + "/my-digital-learning-content-player";

        public const string Settings = "settings";

        public const string MyProfile = "my-profile";
        public const string ChangePassword = MyProfile + "/change-password";
        public const string EditProfile = MyProfile + "/edit";
        public const string TermsOfUse = MyProfile + "/terms-of-use";
        public const string PrivacyPolicy = MyProfile + "/privacy-policy";

        public const string Notifications = "notifications";
        public const string NewsFeed = "news-feed";
        public const string NewsFeedDetails = NewsFeed + "/details";

        public const string CheckIn = "check-in";

        public const string OutstandingTasks = "outstanding-tasks";
        public const string StandaloneForm = "stand-alone-form";

        public const string SharingContent = "sharing-content-form";
        public const string SharingList = "sharing-list";
        public const string Community = "community";

        public const string Achievement = "achievement";
        public const string AchievementECertificate = Achievement + "/e-certificate";
        public const string AchievementBadge = Achievement + "/badge";

        public const string RecordingPreview = "/recording-preview";

        public static string GetNavigationRoutesForTabItem(string routeName)
        {
            return NavigationRoutes.Main + "/" + routeName;
        }
    }
}
