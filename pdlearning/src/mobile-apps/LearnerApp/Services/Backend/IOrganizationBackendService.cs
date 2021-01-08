using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LearnerApp.Models;
using LearnerApp.Models.UserOnBoarding;
using Refit;

namespace LearnerApp.Services.Backend
{
    public interface IOrganizationBackendService
    {
        [Get("/userinfo/{userid}")]
        Task<UserTag> GetMyTaggingInformation(string userId);

        [Post("/userinfo/public")]
        Task<List<UserInformation>> GetUserInfomation([Body] object userCxIds, CancellationToken ctoken = default(CancellationToken));

        [Get("/departments/1/hierarchydepartmentidentifiers/v2?departmentId=1&includeChildren=true&includeDepartmentType=true&{departmentTypeIds}&departmentEntityStatuses=Active&pageSize=0")]
        Task<UserOnBoardingState<Department>> GetDepartmentInfomation(string departmentTypeIds = "departmentTypeIds=12&departmentTypeIds=13&departmentTypeIds=14&departmentTypeIds=15");
    }
}
