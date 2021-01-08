using System.Text;

namespace LearnerApp.Common
{
    public static class GetDeepLink
    {
        public static string GetCourseDeepLink(string courseId)
        {
            var deepLink = new StringBuilder(GlobalSettings.BackendBaseUrl).Append("/app/learner/detail/course/").Append(courseId);

            return deepLink.ToString();
        }

        public static string GetDigitalContentDeepLink(string digitalContentId)
        {
            var deepLink = new StringBuilder(GlobalSettings.BackendBaseUrl).Append("/app/learner/detail/digitalcontent/").Append(digitalContentId);

            return deepLink.ToString();
        }
    }
}
