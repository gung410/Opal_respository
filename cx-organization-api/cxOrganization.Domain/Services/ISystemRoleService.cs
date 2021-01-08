using cxOrganization.Client.UserTypes;
using cxOrganization.Domain.Dtos.UserTypes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace cxOrganization.Domain.Services
{
    public interface ISystemRoleService
    {
        public Task<List<SystemRoleSubjects>> GetSystemRolesInfoAsync(GetSystemRolesInfoRequest getSystemRolesInfoRequest, string token);
        public Task<List<UserTypeDto>> GetSystemRolesConvertedToUserTypesModel(GetSystemRolesInfoRequest getSystemRolesInfoRequest, string token);
    }
}
