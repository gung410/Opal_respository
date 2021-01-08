using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Learner.Application.Models;
using Microservice.Learner.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Extensions;

namespace Microservice.Learner.Application.Queries.QueryHandlers
{
    public class GetAllUserPreferencesQueryHandler : BaseQueryHandler<GetAllUserPreferencesQuery, List<UserPreferenceModel>>
    {
        private readonly IRepository<UserPreference> _userPreferenceRepository;

        public GetAllUserPreferencesQueryHandler(
            IRepository<UserPreference> userPreferenceRepository,
            IUserContext userContext) : base(userContext)
        {
            _userPreferenceRepository = userPreferenceRepository;
        }

        protected override async Task<List<UserPreferenceModel>> HandleAsync(GetAllUserPreferencesQuery query, CancellationToken cancellationToken)
        {
            var preferencesOfUser = await _userPreferenceRepository
                .GetAll()
                .Where(_ => _.UserId == CurrentUserId)
                .WhereIf(!query.Keys.IsNullOrEmpty(), _ => query.Keys.Contains(_.Key))
                .Select(_ => new UserPreferenceModel
                {
                    Id = _.Id,
                    UserId = _.UserId,
                    Key = _.Key,
                    Value = _.Value
                }).ToListAsync(cancellationToken);

            return preferencesOfUser;
        }
    }
}
