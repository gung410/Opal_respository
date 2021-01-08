using System;
using System.Threading.Tasks;
using Microservice.Badge.Attributes;
using Microservice.Badge.Domain.Entities;
using Microservice.Badge.Domain.ValueObjects;
using Microservice.Badge.Infrastructure;
using Microservice.Badge.Infrastructure.Extensions;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.Badge.Application.BusinessLogic
{
    public class GetBadgeCriteriaLogic<T> : IGetBadgeCriteriaLogic<T> where T : BaseBadgeCriteria
    {
        private readonly BadgeDbContext _dbContext;

        public GetBadgeCriteriaLogic(BadgeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<T> ExecuteAsync()
        {
            var badgeCriteriaAttributes = (BadgeCriteriaForAttribute[])Attribute.GetCustomAttributes(typeof(T), typeof(BadgeCriteriaForAttribute));

            if (badgeCriteriaAttributes.Length == 0)
            {
                throw new ArgumentNullException($"Need to define {typeof(BadgeCriteriaForAttribute)} for the class {typeof(T)}");
            }

            var badgeId = badgeCriteriaAttributes[0].BadgeId;

            var badge = await _dbContext
                .GetBadgeCriteriaCollection<T>()
                .FirstOrDefaultAsync(p => p.Id == badgeId);

            if (badge == null)
            {
                throw new EntityNotFoundException(typeof(BadgeEntity), badgeId);
            }

            return badge.Criteria;
        }
    }
}
