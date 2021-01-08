using System;
using System.Collections.Generic;
using System.Linq;
using cxOrganization.Adapter.Assessment.Data.Entities;
using cxPlatform.Core;

namespace cxOrganization.Adapter.Assessment.Data.Repositories
{
    /// <summary>
    /// ActivityRepository
    /// </summary>
    public class ActivityStatusTypeRepository : RepositoryBase<ActivityStatusTypeEntity>, IActivityStatusTypeRepository
    {
        const int MaximumRecordsReturn = 5000;
        private readonly AssessmentConfigContext _assessmentConfigContext;
        //private readonly ICacheProvider _memoryCacheProvider;
        /// <summary>
        /// ActivityRepository
        /// </summary>
        /// <param name="unitOfWork"></param>
        public ActivityStatusTypeRepository(AssessmentConfigContext assessmentConfigContext)//, Func<string, ICacheProvider> memoryCacheProvider)
            : base(assessmentConfigContext)
        {
            _assessmentConfigContext = assessmentConfigContext;
            //_memoryCacheProvider = memoryCacheProvider(CacheType.MEMORY_CACHE);
        }

        public List<ActivityStatusTypeEntity> GetActivityStatusByActivityId(int activityID)
        {
            var result = new List<ActivityStatusTypeEntity>();
            //TODO enable cache
            //var cacheKey = new CacheKey("AssessmentDomainGetActivityStatusByActivityId", new { ActivityId = activityID });
            //if (_memoryCacheProvider.TryGet(cacheKey, out result))
            //    return result;
            result = GetAllAsNoTracking().Where(t => t.ActivityID == activityID).ToList();
            //_memoryCacheProvider.Add(cacheKey, result);
            return result;
        }
    }
}