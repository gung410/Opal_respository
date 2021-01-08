using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Domain.Repositories.QueryBuilders
{
    public class LoginServiceUserQueryBuilder
    {
        private IQueryable<LoginServiceUserEntity> _query;

        public LoginServiceUserQueryBuilder(IQueryable<LoginServiceUserEntity> query)
        {
            _query = query;
        }

        public IQueryable<LoginServiceUserEntity> Build()
        {
            return _query;
        }

        public static LoginServiceUserQueryBuilder InitQueryBuilder(IQueryable<LoginServiceUserEntity> query)
        {
            return new LoginServiceUserQueryBuilder(query);
        }
        public LoginServiceUserQueryBuilder FilterWithOwnerId(int ownerId)
        {
            if (ownerId > 0)
            {
                _query = _query.Where(p => p.User.OwnerId == ownerId);
            }
            return this;
        }
        public LoginServiceUserQueryBuilder FilterWithCustomerIds(List<int> customerIds)
        {
            if (customerIds.IsNotNullOrEmpty())
            {
                if (customerIds.Count == 1)
                {
                    int value = customerIds[0];
                    _query = _query.Where(t => t.User.CustomerId == value);
                }
                else
                {
                    _query = _query.Where(t => t.User.CustomerId != null && customerIds.Contains(t.User.CustomerId.Value));
                }
            }
            return this;
        }
        public LoginServiceUserQueryBuilder FilterWithUserIds(List<int> userIds)
        {
            if (userIds.IsNotNullOrEmpty())
            {
                if (userIds.Count == 1)
                {
                    int value = userIds[0];
                    _query = _query.Where(t => t.UserId == value);
                }
                else
                {
                    _query = _query.Where(t => userIds.Contains(t.UserId));
                }
            }
            return this;
        }
        public LoginServiceUserQueryBuilder FilterWithUserExtIds(List<string> userArchetypeIds)
        {
            if (userArchetypeIds.IsNotNullOrEmpty())
            {
                if (userArchetypeIds.Count == 1)
                {
                    string value = userArchetypeIds[0];
                    _query = _query.Where(t => t.User.ExtId == value);
                }
                else
                {
                    _query = _query.Where(t => userArchetypeIds.Contains(t.User.ExtId));
                }
            }
            return this;
        }
        public LoginServiceUserQueryBuilder FilterWithUserArchetypeIds(List<int> userArchetypeIds)
        {
            if (userArchetypeIds.IsNotNullOrEmpty())
            {
                if (userArchetypeIds.Count == 1)
                {
                    int value = userArchetypeIds[0];
                    _query = _query.Where(t => t.User.ArchetypeId == value);
                }
                else
                {
                    _query = _query.Where(t => t.User.ArchetypeId != null && userArchetypeIds.Contains(t.User.ArchetypeId.Value));
                }
            }
            return this;
        }
        public LoginServiceUserQueryBuilder FilterWithUserEntityStatues(List<int> userEntityStatues)
        {
            if (userEntityStatues.IsNotNullOrEmpty())
            {
                if (userEntityStatues.Contains((int)EntityStatusEnum.All))
                    return this;
                if (userEntityStatues.Count == 1)
                {
                    int value = userEntityStatues[0];
                    _query = _query.Where(t => t.User.EntityStatusId == value);
                }
                else
                {
                    _query = _query.Where(t => t.User.EntityStatusId != null && userEntityStatues.Contains(t.User.EntityStatusId.Value));
                }
            }
            else
            {
                var activeStatusId = (int)EntityStatusEnum.Active;
                _query = _query.Where(t => t.User.EntityStatusId == activeStatusId);
            }
            return this;
        }

        public LoginServiceUserQueryBuilder FilterWithPrimaryClaimValues(List<string> primaryClaimValues)
        {
            if (primaryClaimValues.IsNotNullOrEmpty())
            {
                if (primaryClaimValues.Count == 1)
                {
                    string value = primaryClaimValues[0];
                    _query = _query.Where(t => t.PrimaryClaimValue == value);
                }
                else
                {
                    _query = _query.Where(t => primaryClaimValues.Contains(t.PrimaryClaimValue));
                }
            }
            return this;
        }
        public LoginServiceUserQueryBuilder FilterWithCreatedAfter(DateTime? createdAfter)
        {
            if (createdAfter.HasValue)
            {
                _query = _query.Where(t => createdAfter.Value <= t.Created);
            }
            return this;
        }
        public LoginServiceUserQueryBuilder FilterWithCreatedBefore(DateTime? createdBefore)
        {
            if (createdBefore.HasValue)
            {
                _query = _query.Where(t => t.Created <= createdBefore.Value);
            }
            return this;
        }
        public LoginServiceUserQueryBuilder FilterWithClaimValues(List<string> claimValues)
        {
            if (claimValues.IsNotNullOrEmpty())
            {
                if (claimValues.Count == 1)
                {
                    string value = claimValues[0];
                    _query = _query.Where(t => t.PrimaryClaimValue == value);
                }
                else
                {
                    _query = _query.Where(t => claimValues.Contains(t.PrimaryClaimValue));
                }
            }
            return this;
        }
        public LoginServiceUserQueryBuilder FilterWithLoginServiceIds(List<int> loginServiceIds)
        {
            if (loginServiceIds.IsNotNullOrEmpty())
            {
                if (loginServiceIds.Count == 1)
                {
                    int value = loginServiceIds[0];
                    _query = _query.Where(t => t.LoginServiceId == value);
                }
                else
                {
                    _query = _query.Where(t => loginServiceIds.Contains(t.LoginServiceId));
                }
            }
            return this;
        }
        public LoginServiceUserQueryBuilder FilterWithPrimaryClaimTypes(List<string> primaryClaimTypes)
        {
            if (primaryClaimTypes.IsNotNullOrEmpty())
            {
                if (primaryClaimTypes.Count == 1)
                {
                    string value = primaryClaimTypes[0];
                    _query = _query.Where(t => t.LoginService.PrimaryClaimType == value);
                }
                else
                {
                    _query = _query.Where(t => primaryClaimTypes.Contains(t.LoginService.PrimaryClaimType));
                }
            }
            return this;
        }


        public LoginServiceUserQueryBuilder FilterWithSiteIds(List<int> siteIds, bool? includeNullSiteId)
        {
            if (siteIds.IsNotNullOrEmpty())
            {
                if (includeNullSiteId == true)
                {
                    if (siteIds.Count == 1)
                    {
                        int value = siteIds[0];
                        _query = _query.Where(t => t.LoginService.SiteID == null || t.LoginService.SiteID == value);
                    }
                    else
                    {
                        _query = _query.Where(t => t.LoginService.SiteID == null || siteIds.Contains(t.LoginService.SiteID.Value));
                    }
                }
                else
                {
                    if (siteIds.Count == 1)
                    {
                        int value = siteIds[0];
                        _query = _query.Where(t => t.LoginService.SiteID == value);
                    }
                    else
                    {
                        _query = _query.Where(t => t.LoginService.SiteID != null && siteIds.Contains(t.LoginService.SiteID.Value));
                    }
                }
            }
            else if (includeNullSiteId == false)
            {
                _query = _query.Where(t => t.LoginService.SiteID != null);
            }
            return this;
        }



    }
}
