using System.Collections.Generic;
using LearnerApp.Common.Enum;
using LearnerApp.Common.Permission;

namespace LearnerApp.Helper
{
    public static class PermissionHelper
    {
        /// <summary>
        /// Get like/share/copy permission for courses/microlearning/digital contents/learning paths/communities.
        /// </summary>
        /// <param name="cardType">Course/Microlearning/DigitalContent.</param>
        /// <returns>Can do like/share/copy.</returns>
        public static bool GetPermissionForLikeShareCopy(CardType cardType)
        {
            Dictionary<string, bool> accessRights = Services.Identity.IdentityService.AccessRights;

            bool isShowLikeShareCopy = true;

            switch (cardType)
            {
                case CardType.Course:
                    isShowLikeShareCopy = accessRights.TryGetValue(AccessRightKeys.ActionCourseLikeShareCopy, out _);
                    break;
                case CardType.Microlearning:
                    isShowLikeShareCopy = accessRights.TryGetValue(AccessRightKeys.ActionMicrolearningLikeShareCopy, out _);
                    break;
                case CardType.DigitalContent:
                    isShowLikeShareCopy = accessRights.TryGetValue(AccessRightKeys.ActionDigitalContentLikeShareCopy, out _);
                    break;
            }

            return isShowLikeShareCopy;
        }

        /// <summary>
        /// Get bookmark permission for courses/microlearning/digital contents/learning paths/communities.
        /// </summary>
        /// <returns>Can do bookmark.</returns>
        public static bool GetPermissionForBookmark()
        {
            Dictionary<string, bool> accessRights = Services.Identity.IdentityService.AccessRights;
            return accessRights.TryGetValue(AccessRightKeys.ActionBookmark, out _);
        }

        /// <summary>
        /// View Home Page.
        /// </summary>
        /// <returns>Can view Home Page.</returns>
        public static bool GetPermissionForHomePage()
        {
            Dictionary<string, bool> accessRights = Services.Identity.IdentityService.AccessRights;
            return accessRights.TryGetValue(AccessRightKeys.Home, out _);
        }

        /// <summary>
        /// View Home Page.
        /// </summary>
        /// <returns>Can view Home Page.</returns>
        public static bool GetPermissionForCataloguePage()
        {
            Dictionary<string, bool> accessRights = Services.Identity.IdentityService.AccessRights;
            return accessRights.TryGetValue(AccessRightKeys.Catalogue, out _);
        }

        /// <summary>
        /// View My Learning Page.
        /// </summary>
        /// <returns>Can view My Learning Page.</returns>
        public static bool GetPermissionForMyLearningPage()
        {
            Dictionary<string, bool> accessRights = Services.Identity.IdentityService.AccessRights;
            return accessRights.TryGetValue(AccessRightKeys.MyLearning, out _);
        }

        /// <summary>
        /// View PdPlanner Page.
        /// </summary>
        /// <returns>Can view PdPlanner Page.</returns>
        public static bool GetPermissionForPdPlannerPage()
        {
            Dictionary<string, bool> accessRights = Services.Identity.IdentityService.AccessRights;
            return accessRights.TryGetValue(AccessRightKeys.PDPlan, out _);
        }

        /// <summary>
        /// View Calendar Page.
        /// </summary>
        /// <returns>Can view Calendar Page.</returns>
        public static bool GetPermissionForCalendarPage()
        {
            Dictionary<string, bool> accessRights = Services.Identity.IdentityService.AccessRights;
            return accessRights.TryGetValue(AccessRightKeys.Calendar, out _);
        }

        /// <summary>
        /// View E-Portfolio Page.
        /// </summary>
        /// <returns>Can view E-Portfolio Page.</returns>
        public static bool GetPermissionForEPortfolioPage()
        {
            Dictionary<string, bool> accessRights = Services.Identity.IdentityService.AccessRights;
            return accessRights.TryGetValue(AccessRightKeys.EPortfolio, out _);
        }

        /// <summary>
        /// View Report Page.
        /// </summary>
        /// <returns>Can view Report Page.</returns>
        public static bool GetPermissionForReportPage()
        {
            Dictionary<string, bool> accessRights = Services.Identity.IdentityService.AccessRights;
            return accessRights.TryGetValue(AccessRightKeys.Report, out _);
        }

        /// <summary>
        /// View My Learning Course Page.
        /// </summary>
        /// <returns>Can view My Learning Course Page.</returns>
        public static bool GetPermissionForMyLearningCourse()
        {
            Dictionary<string, bool> accessRights = Services.Identity.IdentityService.AccessRights;
            return accessRights.TryGetValue(AccessRightKeys.MyLearningCourse, out _);
        }

        /// <summary>
        /// View My Learning MLU Page.
        /// </summary>
        /// <returns>Can view My Learning MLU Page.</returns>
        public static bool GetPermissionForMyLearningMLU()
        {
            Dictionary<string, bool> accessRights = Services.Identity.IdentityService.AccessRights;
            return accessRights.TryGetValue(AccessRightKeys.MyLearningMicrolearning, out _);
        }

        /// <summary>
        /// View My Learning Digital Content Page.
        /// </summary>
        /// <returns>Can view My Learning Digital Content Page.</returns>
        public static bool GetPermissionForMyLearningDigitalContent()
        {
            Dictionary<string, bool> accessRights = Services.Identity.IdentityService.AccessRights;
            return accessRights.TryGetValue(AccessRightKeys.MyLearningDigitalContent, out _);
        }

        /// <summary>
        /// View My Learning Learning Path Page.
        /// </summary>
        /// <returns>Can view My Learning Learning Path Page.</returns>
        public static bool GetPermissionForMyLearningLearningPath()
        {
            Dictionary<string, bool> accessRights = Services.Identity.IdentityService.AccessRights;
            return accessRights.TryGetValue(AccessRightKeys.MyLearningLearningPath, out _);
        }

        /// <summary>
        /// View My Learning Community Page.
        /// </summary>
        /// <returns>Can view My Learning Community Page.</returns>
        public static bool GetPermissionForMyLearningCommunity()
        {
            Dictionary<string, bool> accessRights = Services.Identity.IdentityService.AccessRights;
            return accessRights.TryGetValue(AccessRightKeys.MyLearningCommunity, out _);
        }

        /// <summary>
        /// View My Learning Bookmark Page.
        /// </summary>
        /// <returns>Can view My Learning Bookmark Page.</returns>
        public static bool GetPermissionForMyLearningBookmark()
        {
            Dictionary<string, bool> accessRights = Services.Identity.IdentityService.AccessRights;
            return accessRights.TryGetValue(AccessRightKeys.MyLearningBookmark, out _);
        }

        /// <summary>
        /// Create, edit, delete learning path.
        /// </summary>
        /// <returns>Can Create, edit, delete learning path.</returns>
        public static bool GetPermissionForLearningPathCUD()
        {
            Dictionary<string, bool> accessRights = Services.Identity.IdentityService.AccessRights;
            return accessRights.TryGetValue(AccessRightKeys.MyLearningLearningPathCUD, out _);
        }

        /// <summary>
        /// Customize homepage widgets.
        /// </summary>
        /// <returns>Visbile Customize homepage widgets.</returns>
        public static bool GetPermissionForHomeSetting()
        {
            Dictionary<string, bool> accessRights = Services.Identity.IdentityService.AccessRights;
            return accessRights.TryGetValue(AccessRightKeys.HomeSetting, out _);
        }

        /// <summary>
        /// Add course to PD Plan, remove course from PD Plan.
        /// </summary>
        /// <returns>Can add course to PD Plan, remove course from PD Plan.</returns>
        public static bool GetPermissionForAddRemovePlan()
        {
            Dictionary<string, bool> accessRights = Services.Identity.IdentityService.AccessRights;
            return accessRights.TryGetValue(AccessRightKeys.ActionPlan, out _);
        }

        /// <summary>
        /// Apply course, change class run, withdraw from class run.
        /// </summary>
        /// <returns>Can apply course, change class run, withdraw from class run.</returns>
        public static bool GetPermissionForClassRun()
        {
            Dictionary<string, bool> accessRights = Services.Identity.IdentityService.AccessRights;
            return accessRights.TryGetValue(AccessRightKeys.ActionClassRun, out _);
        }

        /// <summary>
        /// Start Learning course, microlearning, digital content.
        /// </summary>
        /// <returns>Can start Learning course, microlearning, digital content.</returns>
        public static bool GetPermissionForStartLearning()
        {
            Dictionary<string, bool> accessRights = Services.Identity.IdentityService.AccessRights;
            return accessRights.TryGetValue(AccessRightKeys.ActionStartLearning, out _);
        }

        /// <summary>
        /// Check-in, do assignment, download content, do post-course evaluation.
        /// </summary>
        /// <returns>Can check-in, do assignment, download content, do post-course evaluation.</returns>
        public static bool GetPermissionForCheckinDoAssignmentDownloadContentDoPostCourse()
        {
            Dictionary<string, bool> accessRights = Services.Identity.IdentityService.AccessRights;
            return accessRights.TryGetValue(AccessRightKeys.ActionCheckinDoAssignmentDownloadContentDoPostCourse, out _);
        }
    }
}
