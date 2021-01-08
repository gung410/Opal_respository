using System.Threading.Tasks;
using LearnerApp.Models;
using LearnerApp.Models.Achievement;
using Refit;

namespace LearnerApp.Services.Backend
{
    public interface IBadgeBackendService
    {
        [Get("/api/badges/currentUser/GetAwardedBadges?SkipCount={skipCount}&MaxResultCount={maxResultCount}")]
        Task<ListResultDto<AchievementUserBadgeDto>> GetUserBadges(int skipCount = 0, int maxResultCount = GlobalSettings.MaxResultPerPage);

        [Get("/api/me/badges")]
        Task<AchievementBadgeInfoDto[]> GetBadgeInfo();
    }
}
