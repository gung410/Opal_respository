using cxOrganization.Domain.BaseEnums;
using cxOrganization.Domain.Dtos.Users;
using cxPlatform.Client.ConexusBase;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace cxOrganization.Domain.HttpClients
{
    public interface IIdentityServerClientService
    {
        Task<string> GetAccessToken();
        Task<UserIdentityResponseDto> UpdateUserAsync(long userInternalId,
            UserGenericDto userGenericDto,
            bool keepUserLogin,
            bool isRejectingUser = false);
        Task<UserIdentityResponseDto> UpdateUserStatusAsync(string userInternalId, IdmUserStatus userStatus);
        Task<UserIdentityResponseDto> CreateUserAsync(UserGenericDto userGenericDto);
        Task DeleteUserAsync(int userId);
        Task ChangeArchiveUserStatusAsync(int userId, EntityStatusEnum? lastEntityStatusId);
        Task<List<UserIdentityDto>> GetUsersAsync(UserFilterParams userFilterParams);
    }
}