using System.Threading.Tasks;
using LearnerApp.Models;
using Refit;

namespace LearnerApp.Services.Backend
{
    public interface ICommunityBackendService
    {
        [Get("/api/v1/user/space/{userId}?page={page}&limit={itemPerPage}")]
        Task<ListCSLResultDto<Community>> GetAllCommunity(string userId, int page, int itemPerPage = GlobalSettings.MaxResultPerPage);

        [Get("/api/v1/user/space/created/{userId}?page={page}&limit={itemPerPage}")]
        Task<ListCSLResultDto<Community>> GetMyCreatedCommunity(string userId, int page, int itemPerPage = GlobalSettings.MaxResultPerPage);

        [Get("/api/v1/user/space/joined/{userId}?page={page}&limit={itemPerPage}")]
        Task<ListCSLResultDto<Community>> GetMyCommunity(string userId, int page, int itemPerPage = GlobalSettings.MaxResultPerPage);

        [Post("/api/v1/space/list")]
        Task<ListCSLResultDto<Community>> GetCommunityByIds([Body] GetCommunityByIdRequestModel body);
    }
}
