using System.Linq;
using System.Threading.Tasks;
using LearnerApp.Common;
using LearnerApp.Models.UserOnBoarding;
using LearnerApp.Services.Backend;
using LearnerApp.ViewModels.Base;

namespace LearnerApp.ViewModels.MyLearning
{
    public class VerticleCourseListViewModel : BaseViewModel
    {
        private readonly ICourseBackendService _courseBackendService;
        private readonly IOrganizationBackendService _organizationBackendService;

        public VerticleCourseListViewModel()
        {
            _courseBackendService = CreateRestClientFor<ICourseBackendService>(GlobalSettings.BackendServiceCourse);
            _organizationBackendService = CreateRestClientFor<IOrganizationBackendService>(GlobalSettings.BackendServiceOrganization);
        }

        public async Task ChangeStatusByCourseClassRun(string myClassRunId, string courseId, RegistrationStatus offer)
        {
            await ExecuteBackendService(() => _courseBackendService.ChangeStatusByCourseClassRun(new { ClassRunId = myClassRunId, CourseId = courseId, Status = offer.ToString() }));
        }

        public async Task<UserInformation> GetUserInfo(string userId)
        {
            var userResponse = await ExecuteBackendService(() => _organizationBackendService.GetUserInfomation(new { UserCxIds = new string[] { userId } }));

            if (userResponse.HasEmptyResult())
            {
                return null;
            }

            return userResponse.Payload.FirstOrDefault();
        }
    }
}
