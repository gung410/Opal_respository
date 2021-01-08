using System;
using System.Linq;
using Microservice.StandaloneSurvey.Domain.Entities;
using Microservice.StandaloneSurvey.Domain.ValueObjects;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Extensions;

namespace Microservice.StandaloneSurvey.Infrastructure
{
    public interface ICslAccessControlContext
    {
        IQueryable<Guid> GetCommunityIds(CommunityMembershipRole[] filterRoles);
    }

    public class CslAccessControlContext : ICslAccessControlContext
    {
        private readonly IUserContext _userContext;
        private readonly IRepository<SyncedCslCommunityMember> _communityMemberRepo;

        public CslAccessControlContext(IUserContext userContext, IRepository<SyncedCslCommunityMember> communityMemberRepo)
        {
            _userContext = userContext;
            _communityMemberRepo = communityMemberRepo;
        }

        private Guid? UserId
        {
            get
            {
                var idString = _userContext.GetValue<string>(CommonUserContextKeys.UserId);
                return string.IsNullOrEmpty(idString) ? null : (Guid?)Guid.Parse(idString);
            }
        }

        public IQueryable<Guid> GetCommunityIds(CommunityMembershipRole[] filterRoles)
        {
            return _communityMemberRepo
                .GetAll()
                .Where(_ => _.UserId == UserId)
                .WhereIf(filterRoles != null, _ => filterRoles.Contains(_.Role))
                .Select(_ => _.CommunityId);
        }
    }
}
