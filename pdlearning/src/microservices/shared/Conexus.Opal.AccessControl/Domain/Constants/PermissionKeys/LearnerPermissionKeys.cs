namespace Conexus.Opal.AccessControl.Domain.Constants.PermissionKeys
{
    public static class LearnerPermissionKeys
    {
        /// <summary>
        /// Like, share, copy URL of courses.
        /// </summary>
        public const string ActionCourseLikeShareCopy = "Learner.Action.Course.Like-Share-Copy";

        /// <summary>
        /// Like, share, copy URL of Microlearning.
        /// </summary>
        public const string ActionMicrolearningLikeShareCopy = "Learner.Action.Microlearning.Like-Share-Copy";

        /// <summary>
        /// Like, share, copy URL of Digital Contents.
        /// </summary>
        public const string ActionDigitalContentLikeShareCopy = "Learner.Action.DigitalContent.Like-Share-Copy";

        /// <summary>
        /// Bookmark courses, microlearning, digital contents, learning paths, communities.
        /// </summary>
        public const string ActionBookmark = "Learner.Action.Bookmark";

        /// <summary>
        /// Start Learning course, microlearning, digital content.
        /// </summary>
        public const string StartLearning = "Learner.Action.StartLearning";

        /// <summary>
        /// Customize homepage widgets.
        /// </summary>
        public const string HomeSetting = "Learner.Home.Setting";

        /// <summary>
        /// View, access Courses.
        /// </summary>
        public const string ViewCourse = "Learner.MyLearning.Course";

        /// <summary>
        /// View, access Microlearning.
        /// </summary>
        public const string ViewMicrolearning = "Learner.MyLearning.Microlearning";

        /// <summary>
        /// View, access Digital Content.
        /// </summary>
        public const string ViewDigitalContent = "Learner.MyLearning.DigitalContent";

        /// <summary>
        /// View, access Learning Paths.
        /// </summary>
        public const string ViewLearningPath = "Learner.MyLearning.LearningPath";

        /// <summary>
        /// View, access Bookmarks.
        /// </summary>
        public const string ViewUserBookmark = "Learner.MyLearning.Bookmark";

        /// <summary>
        /// Create, edit, delete learning path.
        /// </summary>
        public const string CudLearningPath = "Learner.MyLearning.LearningPath.CUD";

        /// <summary>
        /// Client and pdpm module will handle this permission.
        /// Carry out, review Learning Needs Analysis.
        /// </summary>
        public const string ActionLearningAnalysis = "Learner.PDPlan.LearningNeedsAnalysis";

        /// <summary>
        /// Client and pdpm module will handle this permission.
        /// View Learning Needs chart.
        /// </summary>
        public const string ViewLearningChart = "Learner.PDPlan.LearningNeeds";

        /// <summary>
        /// Client and pdpm module will handle this permission.
        /// View Career Aspiration chart.
        /// </summary>
        public const string ViewAspirationChart = "Learner.PDPlan.LearningNeeds.Chart";

        /// <summary>
        /// Client and pdpm module will handle this permission.
        /// View PD Plan.
        /// </summary>
        public const string ViewPdPlan = "Learner.PDPlan.YourPDPlan";

        /// <summary>
        /// Client and pdpm module will handle this permission.
        /// Add, remove PD Opportunity.
        /// </summary>
        public const string ActionPdo = "Learner.PDPlan.YourPDPlan.PDO.CUD";

        /// <summary>
        /// Client and pdpm module will handle this permission.
        /// Add, edit, remove External PD Opportunity.
        /// </summary>
        public const string ActionExternalPdo = "Learner.PDPlan.YourPDPlan.ExternalPDO.CUD";

        /// <summary>
        /// Client and pdpm module will handle this permission.
        /// Add, view, edit, delete Comments.
        /// </summary>
        public const string ActionPdoComment = "Learner.PDPlan.Comment";

        /// <summary>
        /// Client and pdpm module will handle this permission.
        /// Mark external PD Opportunity as completed/incompleted.
        /// </summary>
        public const string MarkExternalPdoCompleted = "Learner.PDPlan.YourPDPlan.ExternalPDO.MarkCompletion";

        /// <summary>
        /// Client and pdpm module will handle this permission.
        /// Submit PD Opportunity in ''Your PD Plan'' for Acknowledgement.
        /// </summary>
        public const string SubmitPdPlan = "Learner.PDPlan.YourPDPlan.Submit";

        /// <summary>
        /// Client and e-portfolio module will handle this permission.
        /// View experience.
        /// </summary>
        public const string ViewExperience = "Learner.EPortfolio.Experience";

        /// <summary>
        /// Client and e-portfolio module will handle this permission.
        /// View showcase.
        /// </summary>
        public const string ViewShowcase = "Learner.EPortfolio.Showcase";

        /// <summary>
        /// Client and e-portfolio module will handle this permission.
        /// Adjust privacy settings for showcase (public/private).
        /// </summary>
        public const string ActionShowcaseSetting = "Learner.EPortfolio.Showcase.Setting";

        /// <summary>
        /// Client and e-portfolio module will handle this permission.
        /// Add, edit showcase.
        /// </summary>
        public const string ActionShowcase = "Learner.EPortfolio.Showcase.CUD";

        /// <summary>
        /// Client and e-portfolio module will handle this permission.
        /// Add, edit About.
        /// </summary>
        public const string ActionAbout = "Learner.EPortfolio.Experience.About.CUD";

        /// <summary>
        /// Client and e-portfolio module will handle this permission.
        /// Add, edit Interests.
        /// </summary>
        public const string ActionInterest = "Learner.EPortfolio.Experience.Interests.CUD";

        /// <summary>
        /// Client and e-portfolio module will handle this permission.
        /// Add, edit Achievement.
        /// </summary>
        public const string ActionAchievement = "Learner.EPortfolio.Experience.Achievement.CUD";

        /// <summary>
        /// Client and e-portfolio module will handle this permission.
        /// Add, edit My Reflections.
        /// </summary>
        public const string ActionMyReflections = "Learner.EPortfolio.Experience.MyReflections.CUD";

        /// <summary>
        /// Client and report module will handle this permission.
        /// View, access report.
        /// </summary>
        public const string ActionReport = "Learner.Report.ListOfReports";

        /// <summary>
        /// Client and course module will handle this permission.
        /// Check-in, do assignment, download content, do post-course evaluation.
        /// </summary>
        public const string ActionRelatedToUserIsParticipant = "Learner.Action.Checkin-DoAssignment-DownloadContent-DoPostCourse";

        /// <summary>
        /// Client and course module will handle this permission.
        /// Apply course, change class run, withdraw from class run.
        /// </summary>
        public const string ActionRegistration = "Learner.Action.ClassRun";

        /// <summary>
        /// Client and CSL module will handle this permission.
        /// View, access Communities.
        /// </summary>
        public const string ViewCommunity = "Learner.MyLearning.Community";

        /// <summary>
        /// Client and Pdpm module will handle this permission.
        /// Add course to PD Plan, remove course from PD Plan.
        /// </summary>
        public const string ActionAddPlan = "Learner.Action.Plan";
    }
}
