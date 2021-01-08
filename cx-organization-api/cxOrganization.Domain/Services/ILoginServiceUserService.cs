using System;
using System.Collections.Generic;
using cxOrganization.Domain.Dtos.Users;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Domain.Services
{
    public interface ILoginServiceUserService
    {
        LoginServiceUserDto Insert(LoginServiceUserDto insertingLoginServiceUserDto);
        LoginServiceUserDto Update(LoginServiceUserDto updatingLoginServiceUserDto);
        LoginServiceUserDto Delete(LoginServiceUserDto deletingLoginServiceUserDto);
        LoginServiceUserDto InsertOrUpdate(LoginServiceUserDto insertingLoginServiceUserDto);
        void Delete(int loginServiceId, int userId);

        List<LoginServiceUserDto> Get(List<int> userIds = null,
            List<string> userExtIds = null,
            List<ArchetypeEnum> userArchetypes = null,
            List<int> loginServiceIds = null,
            List<string> primaryClaimTypes = null,
            List<EntityStatusEnum> userEntityStatuses = null,
            List<int> siteIds = null,
            bool? includeLoginServiceHasNullSiteId = null,
            List<string> claimValues = null,
            DateTime? createdAfter = null,
            DateTime? createdBefore = null
        );
    }
}