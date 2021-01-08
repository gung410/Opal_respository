using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cxOrganization.Adapter.Assessment.Data.Entities;
using cxPlatform.Core;
using Microsoft.EntityFrameworkCore;

namespace cxOrganization.Adapter.Assessment.Data.Repositories
{
    /// <summary>
    /// ActivityRepository
    /// </summary>
    public class SurveyRepository : RepositoryBase<SurveyEntity>, ISurveyRepository
    {
        const int MaximumRecordsReturn = 5000;
        private readonly AssessmentConfigContext _assessmentConfigContext;
        private readonly Dictionary<int, SurveyEntity> _survey = new Dictionary<int, SurveyEntity>();
        /// <summary>
        /// ActivityRepository
        /// </summary>
        /// <param name="unitOfWork"></param>
        public SurveyRepository(AssessmentConfigContext assessmentConfigContext)
            : base(assessmentConfigContext)
        {
            _assessmentConfigContext = assessmentConfigContext;
        }

        public List<SurveyEntity> FindSurveysByActivityIds(List<int> activityIds)
        {
            return GetAllAsNoTracking()
              .Include(p => p.LtSurvey)
              //.Include(p => p.Period)
              .Where(p => activityIds.Contains(p.ActivityID)).ToList();
        }

        public SurveyEntity GetCurrentSurvey(int activityID, int currentPeriodId, DateTime? activeDate = default(DateTime?))
        {
            return GetAllAsNoTracking().FirstOrDefault(x => x.ActivityID == activityID && x.PeriodID == currentPeriodId
                    && x.Status > 0 && x.EndDate.Date >= activeDate.Value && x.StartDate.Date <= activeDate.Value);
        }

        public List<SurveyEntity> GetSurveysByActivityId(int activityid)
        {
            return GetAllAsNoTracking().Where(x => x.ActivityID == activityid).ToList();
        }
        public List<SurveyEntity> GetSurveys(List<int> surveyIds = null,
            List<int> activityIds = null,
            List<string> extIds = null,
            DateTime? createdBefore = null,
            DateTime? createdAfter = null,
            DateTime? startDateBefore = null,
            DateTime? startDateAfter = null,
            DateTime? endDateBefore = null,
            DateTime? endDateAfter = null)
        {
            var query = BuildGetSurveysQuery(surveyIds, activityIds, extIds, createdBefore, createdAfter, startDateBefore, startDateAfter, endDateBefore, endDateAfter);
            return query.ToList();
        }

        public Task<List<SurveyEntity>> GetSurveysAsync(List<int> surveyIds = null,
            List<int> activityIds = null,
            List<string> extIds = null,
            DateTime? createdBefore = null,
            DateTime? createdAfter = null,
            DateTime? startDateBefore = null,
            DateTime? startDateAfter = null,
            DateTime? endDateBefore = null,
            DateTime? endDateAfter = null)
        {
            var query = BuildGetSurveysQuery(surveyIds, activityIds, extIds, createdBefore, createdAfter, startDateBefore, startDateAfter, endDateBefore, endDateAfter);
            return query.ToListAsync();
        }

        private IQueryable<SurveyEntity> BuildGetSurveysQuery(List<int> surveyIds, List<int> activityIds, List<string> extIds, DateTime? createdBefore, DateTime? createdAfter, DateTime? startDateBefore, DateTime? startDateAfter, DateTime? endDateBefore, DateTime? endDateAfter)
        {
            var query = GetAllAsNoTracking();
            if (surveyIds != null && surveyIds.Any())
            {
                query = query.Where(t => surveyIds.Contains(t.SurveyID));
            }
            if (activityIds != null && activityIds.Any())
            {
                query = query.Where(t => activityIds.Contains(t.ActivityID));
            }
            if (extIds != null && extIds.Any())
            {
                query = query.Where(t => extIds.Contains(t.ExtId));
            }
            if (createdBefore.HasValue)
            {
                query = query.Where(p => p.Created <= createdBefore);
            }
            if (createdAfter.HasValue)
            {
                query = query.Where(p => p.Created >= createdAfter);
            }
            if (startDateBefore.HasValue)
            {
                query = query.Where(p => p.StartDate <= startDateBefore);
            }
            if (startDateAfter.HasValue)
            {
                query = query.Where(p => p.StartDate >= startDateAfter);
            }
            if (endDateBefore.HasValue)
            {
                query = query.Where(p => p.EndDate <= endDateBefore);
            }
            if (endDateAfter.HasValue)
            {
                query = query.Where(p => p.EndDate >= endDateAfter);
            }
            return query.Include(t => t.Period).Include(t => t.LtSurvey).Take(10000);
        }

        /// <summary>
        /// Gets the survey by identifier.
        /// </summary>
        /// <param name="surveyId">The survey identifier.</param>
        /// <returns>Survey.</returns>
        public SurveyEntity GetSurveyById(int surveyId)
        {
            SurveyEntity result = null;
            //TODO
            //var cacheKey = new CacheKey("AssessmentDomainGetSurveyById", new { surveyId = surveyId });
            //if (_memoryCacheProvider.TryGet(cacheKey, out result))
            //    return result;
            result = base.GetById(surveyId);
            return result;
        }
        public List<SurveyEntity> GetSurveyByIds(List<int> surveyIds)
        {
            return GetAllAsNoTracking().Include(x => x.LtSurvey).Where(x => surveyIds.Contains(x.SurveyID)).ToList();
        }
    }
}