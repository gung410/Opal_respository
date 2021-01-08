using System;
using System.Collections.Generic;
using System.Linq;
using cxOrganization.Adapter.Assessment.Data.Entities;
using cxPlatform.Core;
using Microsoft.EntityFrameworkCore;

namespace cxOrganization.Adapter.Assessment.Data.Repositories
{
    /// <summary>
    /// AlternativeRepository
    /// </summary>
    public class AlternativeRepository : RepositoryBase<AlternativeEntity>, IAlternativeRepository
    {
        const int MaximumRecordsReturn = 5000;
        private readonly AssessmentConfigContext _assessmentConfigContext;
        /// <summary>
        /// AlternativeRepository
        /// </summary>
        /// <param name="unitOfWork"></param>
        public AlternativeRepository(AssessmentConfigContext assessmentConfigContext)
            : base(assessmentConfigContext)
        {
            _assessmentConfigContext = assessmentConfigContext;
        }

        public List<AlternativeEntity> GetAlternatives(
            List<int> alternativeIds = null,
            List<int> scaleIds = null,
            List<string> extIds = null,
            List<int> activityIds = null,
            DateTime? createdAfter = null,
            DateTime? createdBefore = null)
        {
            var query = GetAllAsNoTracking();
            if (activityIds!=null && activityIds.Count>0)
            {
                query = query.Where(p => activityIds.Any(id => id == p.Scale.ActivityID));
            }
            if (alternativeIds != null && alternativeIds.Any())
            {
                query = query.Where(e => alternativeIds.Contains(e.AlternativeID));
            }
            if (scaleIds != null && scaleIds.Any())
            {
                query = query.Where(e => scaleIds.Contains(e.ScaleID));
            }
            if (extIds != null && extIds.Any())
            {
                query = query.Where(e => extIds.Contains(e.ExtID));
            }

            if (createdBefore.HasValue)
            {
                query = query.Where(p => p.Created <= createdBefore);
            }

            if (createdAfter.HasValue)
            {
                query = query.Where(p => p.Created >= createdAfter);
            }
            return query.Include(p => p.LtAlternativeEntities).Take(10000).ToList();
        }
    }
}