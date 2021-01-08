using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cxOrganization.Adapter.Assessment.Data.Entities;
using cxPlatform.Core;
using cxPlatform.Core.Cache;
using cxPlatform.Core.Caching;
using Microsoft.EntityFrameworkCore;

namespace cxOrganization.Adapter.Assessment.Data.Repositories
{
    /// <summary>
    /// ActivityRepository
    /// </summary>
    public class ActivityRepository : RepositoryBase<ActivityEntity>, IActivityRepository
    {
        const int MaximumRecordsReturn = 5000;
        private readonly AssessmentConfigContext _assessmentConfigContext;
        private readonly ICacheProvider _memoryCacheProvider;
        /// <summary>
        /// ActivityRepository
        /// </summary>
        /// <param name="unitOfWork"></param>
        public ActivityRepository(AssessmentConfigContext assessmentConfigContext, ICacheProvider memoryCacheProvider)
            : base(assessmentConfigContext)
        {
            _assessmentConfigContext = assessmentConfigContext;
            _memoryCacheProvider = memoryCacheProvider;
        }

        public List<ActivityEntity> GetActivityByExternalId(string activityExtId)
        {
            return GetAllAsNoTracking().Where(a => a.ExtID.ToLower() == activityExtId.ToLower()).ToList();
        }
        /// <summary>
        /// Get list activity by owner
        /// </summary>
        /// <param name="ownerid"></param>
        /// <returns></returns>
        public List<ActivityEntity> GetActivitiesByOwner(int ownerid)
        {
            return GetAllAsNoTracking().Where(x => x.OwnerID == ownerid).ToList();
        }
        public List<ActivityEntity> GetActivities(int ownerId = 0,
            List<int> surveyIds = null,
            List<int> activityIds = null,
            List<string> extIds = null,
            DateTime? createdBefore = null,
            DateTime? createdAfter = null)
        {
            var query = BuildGetActivitiesQuery(ownerId, surveyIds, activityIds, extIds, createdBefore, createdAfter);
            return query.ToList();
        }

        public Task<List<ActivityEntity>> GetActivitiesAsync(int ownerId = 0,
            List<int> surveyIds = null,
            List<int> activityIds = null,
            List<string> extIds = null,
            DateTime? createdBefore = null,
            DateTime? createdAfter = null)
        {
            var query = BuildGetActivitiesQuery(ownerId, surveyIds, activityIds, extIds, createdBefore, createdAfter);

            return query.ToListAsync();
        }

        private IQueryable<ActivityEntity> BuildGetActivitiesQuery(int ownerId, List<int> surveyIds, List<int> activityIds, List<string> extIds, DateTime? createdBefore, DateTime? createdAfter)
        {
            var query = GetAllAsNoTracking();
            if (ownerId > 0)
            {
                query = query.Where(t => t.OwnerID == ownerId);
            }
            if (activityIds != null && activityIds.Any())
            {
                query = query.Where(t => activityIds.Contains(t.ActivityID));
            }
            if (surveyIds != null && surveyIds.Any())
            {
                query = query.Where(t => t.Surveys.Any(s => surveyIds.Contains(s.SurveyID)));
            }
            if (extIds != null && extIds.Any())
            {
                query = query.Where(t => extIds.Contains(t.ExtID));
            }
            if (createdBefore.HasValue)
            {
                query = query.Where(p => p.Created <= createdBefore);
            }
            if (createdAfter.HasValue)
            {
                query = query.Where(p => p.Created >= createdAfter);
            }

            return query.Include(t => t.LtActivities).Take(10000).OrderBy(x => x.No);
        }

        public ActivityEntity GetActivityById(int activityId)
        {
            ActivityEntity result = null;
            var cacheKey = new CacheKey("AssessmentDomainGetSurveyById", new { activityId = activityId });
            if (_memoryCacheProvider.TryGet(cacheKey, out result))
                return result;
            result = base.GetById(activityId);
            _memoryCacheProvider.Add(cacheKey, result);
            return result;
        }
    }
}