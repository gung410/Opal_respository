using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cxOrganization.Adapter.Assessment.Data.Entities;
using cxOrganization.Adapter.Shared.Common;
using cxOrganization.Adapter.Shared.Entity;
using cxPlatform.Core;
using cxPlatform.Core.Extentions;

namespace cxOrganization.Adapter.Assessment.Data.Repositories
{
    /// <summary>
    /// AnswerRepository
    /// </summary>
    public class AnswerRepository : RepositoryBase<AnswerEntity>, IAnswerRepository
    {
        const int MaximumRecordsReturn = 5000;
        private readonly AssessmentContext _assessmentContext;
        /// <summary>
        /// AnswerRepository
        /// </summary>
        /// <param name="unitOfWork"></param>
        public AnswerRepository(AssessmentContext unitOfWork)
            : base(unitOfWork)
        {
            _assessmentContext = unitOfWork;
        }

        public int CountAnswerByResultIdQuestionId(long resultId, int questionId, int? itemId)
        {
            return GetAllAsNoTracking().Count(t => t.ResultID == resultId && t.QuestionID == questionId && (itemId == null || t.ItemId == itemId));

        }

        public int CountAnswerByResultIdQuestionIdAlternativeId(long resultId, int questionId, int? alternativeId, int? itemId)
        {
            return GetAllAsNoTracking().Count(t => t.ResultID == resultId && t.QuestionID == questionId &&
                                       (t.AlternativeID == alternativeId || alternativeId == null) && (itemId == null || t.ItemId == itemId));

        }

        public AnswerEntity GetAnswerByResultIdQuestionIdAlternativeId(long resultId, int questionId, int? alternativeId, int? itemId = default(int?), int? no = default(int?))
        {
            return GetAll().FirstOrDefault(t => t.ResultID == resultId && t.QuestionID == questionId &&
                (alternativeId == null || t.AlternativeID == alternativeId) &&
                (itemId == null || t.ItemId == itemId) &&
                (!no.HasValue || t.No == no.Value));
        }

        public PaginatedList<AnswerEntity> GetAnswers(
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
            string orderBy = "")
        {
            var query = BuildGetAnswersQuery(customerIds, answerIds, resultIds, userIds, departmentIds, surveyIds, alternativeIds, questionIds, itemIds, resultStatusTypeIds, batchIds, lastUpdatedBefore, lastUpdatedAfter, orderBy);

            var hasMoreData = false;
            //Build paging from IQueryable
            var totalItem = 0;
            var items = query.ToPaging(pageIndex, pageSize, out hasMoreData, out totalItem);
            return new PaginatedList<AnswerEntity>(items, pageIndex, pageSize, hasMoreData);
        }

        public async Task<PaginatedList<AnswerEntity>> GetAnswersAsync(
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
            string orderBy = "")
        {
            var query = BuildGetAnswersQuery(customerIds, answerIds, resultIds, userIds, departmentIds, surveyIds, alternativeIds, questionIds, itemIds, resultStatusTypeIds, batchIds, lastUpdatedBefore, lastUpdatedAfter, orderBy);

            //Build paging from IQueryable
            var paginatedResult = await query.ToPagingAsync(pageIndex, pageSize, false);
            var paginatedEntities = new PaginatedList<AnswerEntity>(paginatedResult.Items, pageIndex, pageSize, paginatedResult.HasMoreData);
            return paginatedEntities;
        }

        private IQueryable<AnswerEntity> BuildGetAnswersQuery(List<int> customerIds, List<long> answerIds, List<long> resultIds, List<int> userIds, List<int> departmentIds, List<int> surveyIds, List<int> alternativeIds, List<int> questionIds, List<int> itemIds, List<int> resultStatusTypeIds, List<int> batchIds, DateTime? lastUpdatedBefore, DateTime? lastUpdatedAfter, string orderBy)
        {
            var query = GetAll();
            if (customerIds != null && customerIds.Any())
                query = query.Where(t => customerIds.Contains(t.CustomerID.Value));

            if (answerIds != null && answerIds.Any())
                query = query.Where(t => answerIds.Contains(t.AnswerID));

            if (resultIds != null && resultIds.Any())
                query = query.Where(t => resultIds.Contains(t.ResultID));

            if (resultStatusTypeIds != null && resultStatusTypeIds.Any())
                query = query.Where(t => resultStatusTypeIds.Contains(t.Result.StatusTypeID.Value));

            if (batchIds != null && batchIds.Any())
                query = query.Where(t => batchIds.Contains(t.Result.BatchID));

            if (userIds != null && userIds.Any())
                query = query.Where(t => userIds.Contains(t.Result.UserID.Value));

            if (departmentIds != null && departmentIds.Any())
                query = query.Where(t => departmentIds.Contains(t.Result.DepartmentID));

            if (surveyIds != null && surveyIds.Any())
                query = query.Where(t => surveyIds.Contains(t.Result.SurveyID));

            if (alternativeIds != null && alternativeIds.Any())
                query = query.Where(t => alternativeIds.Contains(t.AlternativeID));

            if (questionIds != null && questionIds.Any())
                query = query.Where(t => questionIds.Contains(t.QuestionID));

            if (lastUpdatedBefore.HasValue)
                query = query.Where(p => p.LastUpdated <= lastUpdatedBefore);

            if (lastUpdatedAfter.HasValue)
                query = query.Where(p => p.LastUpdated >= lastUpdatedAfter);

            if (itemIds != null && itemIds.Any())
                query = query.Where(p => itemIds.Contains(p.ItemId.Value));

            //Query must be ordered before apply paging
            return query.ApplyOrderBy(p => (int)p.AnswerID, orderBy);
        }

        public List<AnswerEntity> GetAnswersOrigin(
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
            List<EntityStatusEnum> statusIds = null)
        {
            var query = GetAll();
            if (customerIds != null && customerIds.Any())
                query = query.Where(t => customerIds.Contains(t.CustomerID.Value));

            if (answerIds != null && answerIds.Any())
                query = query.Where(t => answerIds.Contains(t.AnswerID));

            if (resultIds != null && resultIds.Any())
                query = query.Where(t => resultIds.Contains(t.ResultID));

            if (resultStatusTypeIds != null && resultStatusTypeIds.Any())
                query = query.Where(t => resultStatusTypeIds.Contains(t.Result.StatusTypeID.Value));

            if (batchIds != null && batchIds.Any())
                query = query.Where(t => batchIds.Contains(t.Result.BatchID));

            if (userIds != null && userIds.Any())
                query = query.Where(t => userIds.Contains(t.Result.UserID.Value));

            if (departmentIds != null && departmentIds.Any())
                query = query.Where(t => departmentIds.Contains(t.Result.DepartmentID));

            if (surveyIds != null && surveyIds.Any())
                query = query.Where(t => surveyIds.Contains(t.Result.SurveyID));

            if (alternativeIds != null && alternativeIds.Any())
                query = query.Where(t => alternativeIds.Contains(t.AlternativeID));

            if (questionIds != null && questionIds.Any())
                query = query.Where(t => questionIds.Contains(t.QuestionID));

            if (lastUpdatedBefore.HasValue)
                query = query.Where(p => p.LastUpdated <= lastUpdatedBefore);

            if (lastUpdatedAfter.HasValue)
                query = query.Where(p => p.LastUpdated >= lastUpdatedAfter);

            if (itemIds != null && itemIds.Any())
                query = query.Where(p => itemIds.Contains(p.ItemId.Value));

            return query.ToList();
        }

        public List<AnswerEntity> GetAnswersByResultIdQuestionIdAlternativeId(long resultId, int questionId, int? alternativeId, int? itemId = null)
        {
            return GetAll().Where(t => t.ResultID == resultId && t.QuestionID == questionId &&
                (alternativeId == null || t.AlternativeID == alternativeId) && (itemId == null || t.ItemId == itemId)).ToList();
        }

        public List<AnswerEntity> GetResultAnswers(long resultId)
        {
            return GetAll().Where(t => t.ResultID == resultId).ToList();
        }
    }
}