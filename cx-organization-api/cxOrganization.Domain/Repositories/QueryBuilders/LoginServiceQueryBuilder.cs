using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace cxOrganization.Domain.Repositories.QueryBuilders
{
    public class LoginServiceQueryBuilder
    {
        private IQueryable<LoginServiceEntity> _query;

        public LoginServiceQueryBuilder(IQueryable<LoginServiceEntity> query)
        {
            _query = query;
        }

        public IQueryable<LoginServiceEntity> Build()
        {
            return _query;
        }

        public static LoginServiceQueryBuilder InitQueryBuilder(IQueryable<LoginServiceEntity> query)
        {
            return new LoginServiceQueryBuilder(query);
        }

        public LoginServiceQueryBuilder FilterWithIds(List<int> loginServiceIds)
        {
            if (loginServiceIds.IsNotNullOrEmpty())
            {
                if (loginServiceIds.Count == 1)
                {
                    int value = loginServiceIds[0];
                    _query = _query.Where(t => t.LoginServiceID == value);
                }
                else
                {
                    _query = _query.Where(t => loginServiceIds.Contains(t.LoginServiceID));
                }
            }
            return this;
        }

        public LoginServiceQueryBuilder FilterWithSecondaryClaimTypes(List<string> secondaryClaimTypes)
        {
            if (secondaryClaimTypes.IsNotNullOrEmpty())
            {
                if (secondaryClaimTypes.Count == 1)
                {
                    string value = secondaryClaimTypes[0];
                    _query = _query.Where(t => t.SecondaryClaimType == value);
                }
                else
                {
                    _query = _query.Where(t => secondaryClaimTypes.Contains(t.SecondaryClaimType));
                }
            }
            return this;
        }

        public LoginServiceQueryBuilder FilterWithSiteIds(List<int> siteIds)
        {
            if (siteIds.IsNotNullOrEmpty())
            {
                if (siteIds.Count == 1)
                {
                    int value = siteIds[0];
                    _query = _query.Where(t => t.SiteID == null || t.SiteID == value);
                }
                else
                {
                    _query = _query.Where(t => t.SiteID == null || siteIds.Contains(t.SiteID.Value));
                }
            }
            return this;
        }

        public LoginServiceQueryBuilder FilterWithPrimaryClaimTypes(List<string> primaryClaimTypes)
        {
            if (primaryClaimTypes.IsNotNullOrEmpty())
            {
                if (primaryClaimTypes.Count == 1)
                {
                    string value = primaryClaimTypes[0];
                    _query = _query.Where(t => t.PrimaryClaimType == value);
                }
                else
                {
                    _query = _query.Where(t => primaryClaimTypes.Contains(t.PrimaryClaimType));
                }
            }
            return this;
        }

        public LoginServiceQueryBuilder FilterWithIssuers(List<string> issuers)
        {
            if (issuers.IsNotNullOrEmpty())
            {
                if (issuers.Count == 1)
                {
                    string value = issuers[0];
                    _query = _query.Where(t => t.Authority == value);
                }
                else
                {
                    _query = _query.Where(t => issuers.Contains(t.Authority));
                }
            }
            return this;
        }
    }
}
