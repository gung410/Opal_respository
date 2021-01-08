using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using cxOrganization.Adapter.Assessment.Data.Entities;
using cxOrganization.Adapter.Shared.Common;
using cxOrganization.Adapter.Shared.Entity;
using cxPlatform.Core;

namespace cxOrganization.Adapter.Assessment.Data.Repositories
{
    /// <summary>
    /// IEventRepository
    /// </summary>
    public interface IResultRepository : IRepository<ResultEntity>
    {
        ResultEntity GetResultByUserAndBatch(int userId, int batchId, bool includeAllAnswers, bool includeActiveStatus = false);
        ResultEntity GetResultById(long resultId, bool isIncludeAnswers = false, bool isIncludeSoftDeletedResult = false, bool isChangeConnectionString = true);
        List<ResultEntity> GetListResultByUserAndSurvey(int userId, bool includeAllAnswers, bool includeInActiveStatus = false);
        PaginatedList<ResultEntity> GetResults(int ownerId,
            List<int> customerIds = null,
            List<int> departmentIds = null,
            List<EntityStatusEnum> statusIds = null,
            List<int> surveyIds = null,
            List<int> statusTypeIds = null,
            List<int> userIds = null,
            List<long> resultIds = null,
            List<long> answerIds = null,
            List<int> alternativeIds = null,
            List<int> questionIds = null,
            List<int> batchIds = null,
            DateTime? startDateBefore = null,
            DateTime? startDateAfter = null,
            DateTime? endDateBefore = null,
            DateTime? endDateAfter = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? lastUpdatedAfter = null,
            bool includeAllAnswers = true,
            int pageIndex = 0,
            int pageSize = 0,
            string orderBy = "");
        Task<PaginatedList<ResultEntity>> GetResultsAsync(int ownerId,
            List<int> customerIds = null,
            List<int> departmentIds = null,
            List<EntityStatusEnum> statusIds = null,
            List<int> surveyIds = null,
            List<int> statusTypeIds = null,
            List<int> userIds = null,
            List<long> resultIds = null,
            List<long> answerIds = null,
            List<int> alternativeIds = null,
            List<int> questionIds = null,
            List<int> batchIds = null,
            DateTime? startDateBefore = null,
            DateTime? startDateAfter = null,
            DateTime? endDateBefore = null,
            DateTime? endDateAfter = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? lastUpdatedAfter = null,
            bool includeAllAnswers = true,
            int pageIndex = 0,
            int pageSize = 0,
            string orderBy = "");
        List<ResultEntity> GetResults(List<int> departmentIds, List<int> surveyIds, bool includeAnswers);
        ResultEntity GetResultByUserAndSurvey(int userId, int surveyID, bool isIncludeAnswers, bool includeResultDeletedBecauseUserBeDeleted);

        List<ResultEntity> GetResultsForCalculating(
            List<int> surveyIds = null,
            List<int> departmentIds = null,
            List<int> userIds = null,
            List<long> resultIds = null,
            List<int> customerIds = null,
            List<int> batchIds = null,
            List<int> statusTypeIds = null,
            DateTime? endDateBefore = null,
            DateTime? endDateAfter = null,
            DateTime? startDateBefore = null,
            DateTime? startDateAfter = null);
    }
}
