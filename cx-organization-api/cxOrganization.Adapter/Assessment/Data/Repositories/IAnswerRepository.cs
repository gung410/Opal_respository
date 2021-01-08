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
    public interface IAnswerRepository : IRepository<AnswerEntity>
    {
        int CountAnswerByResultIdQuestionIdAlternativeId(long resultId, int questionId, int? alternativeId, int? itemId);
        int CountAnswerByResultIdQuestionId(long resultId, int questionId, int? itemId);
        AnswerEntity GetAnswerByResultIdQuestionIdAlternativeId(long resultId, int questionId,
            int? alternativeId, int? itemId = null, int? no = null);
        List<AnswerEntity> GetAnswersByResultIdQuestionIdAlternativeId(long resultId, int questionId,
            int? alternativeId, int? itemId = null);
        PaginatedList<AnswerEntity> GetAnswers(
            int ownerId = 0,
            List<int> customerIds = null,
            List<long> answerIds = null,
            List<long> resultIds = null,
            List<int> userIds = null,
            List<int> departmentIds = null,
            List<int> surveyIds = null,
            List<int> alternativeIds = null,
            List<int> questionIds = null,
            List<int> itemIds = null,
            List<int> resultStatusTypeIds = null,
            List<int> batchIds = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? lastUpdatedAfter = null,
            List<EntityStatusEnum> statusIds = null,
            int pageIndex = 0,
            int pageSize = 0,
            string orderBy = "");
        Task<PaginatedList<AnswerEntity>> GetAnswersAsync(
            int ownerId = 0,
            List<int> customerIds = null,
            List<long> answerIds = null,
            List<long> resultIds = null,
            List<int> userIds = null,
            List<int> departmentIds = null,
            List<int> surveyIds = null,
            List<int> alternativeIds = null,
            List<int> questionIds = null,
            List<int> itemIds = null,
            List<int> resultStatusTypeIds = null,
            List<int> batchIds = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? lastUpdatedAfter = null,
            List<EntityStatusEnum> statusIds = null,
            int pageIndex = 0,
            int pageSize = 0,
            string orderBy = "");

        List<AnswerEntity> GetAnswersOrigin(
            int ownerId = 0,
            List<int> customerIds = null,
            List<long> answerIds = null,
            List<long> resultIds = null,
            List<int> userIds = null,
            List<int> departmentIds = null,
            List<int> surveyIds = null,
            List<int> alternativeIds = null,
            List<int> questionIds = null,
            List<int> itemIds = null,
            List<int> resultStatusTypeIds = null,
            List<int> batchIds = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? lastUpdatedAfter = null,
            List<EntityStatusEnum> statusIds = null);

        List<AnswerEntity> GetResultAnswers(long resultId);

    }

}
