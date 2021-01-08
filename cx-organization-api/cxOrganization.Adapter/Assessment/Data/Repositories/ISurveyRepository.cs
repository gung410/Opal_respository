using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using cxOrganization.Adapter.Assessment.Data.Entities;
using cxPlatform.Core;

namespace cxOrganization.Adapter.Assessment.Data.Repositories
{
    /// <summary>
    /// IEventRepository
    /// </summary>
    public interface ISurveyRepository : IRepository<SurveyEntity>
    {
        List<SurveyEntity> FindSurveysByActivityIds(List<int> activityIds);
        SurveyEntity GetCurrentSurvey(int activityID, int currentPeriodId, DateTime? activeDate = null);
        List<SurveyEntity> GetSurveysByActivityId(int activityid);
        List<SurveyEntity> GetSurveys(List<int> surveyIds = null,
            List<int> activityIds = null,
            List<string> extIds = null,
            DateTime? createdBefore = null,
            DateTime? createdAfter = null,
            DateTime? startDateBefore = null,
            DateTime? startDateAfter = null,
            DateTime? endDateBefore = null,
            DateTime? endDateAfter = null);
        Task<List<SurveyEntity>> GetSurveysAsync(List<int> surveyIds = null,
            List<int> activityIds = null,
            List<string> extIds = null,
            DateTime? createdBefore = null,
            DateTime? createdAfter = null,
            DateTime? startDateBefore = null,
            DateTime? startDateAfter = null,
            DateTime? endDateBefore = null,
            DateTime? endDateAfter = null);
        SurveyEntity GetSurveyById(int surveyId);
        List<SurveyEntity> GetSurveyByIds(List<int> surveyIds);
    }
}
