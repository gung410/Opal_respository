using System.Threading.Tasks;
using LearnerApp.Models;
using Refit;

namespace LearnerApp.Services.Backend
{
    public interface IUserManagementBackendService
    {
        [Get("/users?extIds={userId}")]
        Task<UserOnBoardingState<UserManagement>> GetUserManagement(string userId);
    }
}
