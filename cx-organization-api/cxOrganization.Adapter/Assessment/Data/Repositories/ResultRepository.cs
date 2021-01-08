using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cxOrganization.Adapter.Assessment.Data.Entities;
using cxOrganization.Adapter.Shared.Common;
using cxOrganization.Adapter.Shared.Entity;
using cxPlatform.Core;
using cxPlatform.Core.Extentions;
using Microsoft.EntityFrameworkCore;

namespace cxOrganization.Adapter.Assessment.Data.Repositories
{
    /// <summary>
    /// ActivityRepository
    /// </summary>
    public class ResultRepository : RepositoryBase<ResultEntity>, IResultRepository
    {
        const int MaximumRecordsReturn = 100000;
        private readonly AssessmentContext _assessmentContext;
        /// <summary>
        /// ActivityRepository
        /// </summary>
        /// <param name="unitOfWork"></param>
        public ResultRepository(AssessmentContext assessmentContext)
            : base(assessmentContext)
        {
            _assessmentContext = assessmentContext;
        }

        public List<ResultEntity> GetListResultByUserAndSurvey(int userId, bool includeAllAnswers, bool includeInActiveStatus = false)
        {
            var query = GetAll().Where(x => x.UserID == userId);
            if (!includeInActiveStatus)
            {
                query = query.Where(x => x.EntityStatusId == (int)EntityStatusEnum.Active);
            }
            if (includeAllAnswers)
            {
                query = query.Include(x => x.Answers);
            }
            return query.OrderByDescending(p => p.Created).ToList();
        }

        public ResultEntity GetResultById(long resultId,
            bool isIncludeAnswers = false,
            bool isIncludeSoftDeletedResult = false,
            bool isChangeConnectionString = true)
        {
            var query = isIncludeSoftDeletedResult ? GetAll().Where(x => x.ResultID == resultId)
                : GetAll().Where(x => x.ResultID == resultId && x.EntityStatusId == (short)EntityStatusEnum.Active);

            if (isIncludeAnswers)
                query = query.Include(x => x.Answers);
            return query.FirstOrDefault();
        }

        public ResultEntity GetResultByUserAndBatch(int userId, int batchId, bool includeAllAnswers, bool includeActiveStatus = false)
        {
            var query = GetAll().Where(x => x.UserID == userId && x.BatchID == batchId);
            if (includeAllAnswers)
                query = query.Include(x => x.Answers);
            if (!includeActiveStatus)
                query = query.Where(x => x.EntityStatusId == (int)EntityStatusEnum.Active);
            return query.FirstOrDefault();
        }

        public ResultEntity GetResultByUserAndSurvey(int userId, int surveyId, bool includeAllAnswers,
            bool includeInActiveStatus = false)
        {
            var query = GetAll().Where(x => x.UserID == userId && x.SurveyID == surveyId);
            if (!includeInActiveStatus)
            {
                query = query.Where(x => x.EntityStatusId == (int)EntityStatusEnum.Active);
            }
            if (includeAllAnswers)
            {
                query = query.Include(x => x.Answers);
            }
            return query.OrderByDescending(p => p.Created).FirstOrDefault();
        }
        public List<ResultEntity> GetResults(List<int> departmentIds, List<int> surveyIds, bool includeAnswers)
        {
            var query = GetAll().Include(p => p.Answers).Where(p => departmentIds.Contains(p.DepartmentID) && surveyIds.Contains(p.SurveyID) && p.EntityStatusId == (short)EntityStatusEnum.Active);
            if (includeAnswers)
            {
                query.Include(p => p.Answers);
            }
            return query.ToList();
        }

        public PaginatedList<ResultEntity> GetResults(int ownerId,
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
            string orderBy = "")
        {

            var query = BuildGetResultsQuery(customerIds, departmentIds, statusIds, surveyIds, statusTypeIds, userIds, resultIds, batchIds, startDateBefore, startDateAfter, endDateBefore, endDateAfter, lastUpdatedBefore, lastUpdatedAfter, includeAllAnswers, orderBy);

            var hasMoreData = false;
            //Build paging from IQueryable
            var totalItem = 0;
            var items = query.ToPaging(pageIndex, pageSize, out hasMoreData, out totalItem);
            return new PaginatedList<ResultEntity>(items, pageIndex, pageSize, hasMoreData);
        }

        public async Task<PaginatedList<ResultEntity>> GetResultsAsync(int ownerId,
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
            string orderBy = "")
        {
            var query = BuildGetResultsQuery(customerIds, departmentIds, statusIds, surveyIds, statusTypeIds, userIds, resultIds, batchIds, startDateBefore, startDateAfter, endDateBefore, endDateAfter, lastUpdatedBefore, lastUpdatedAfter, includeAllAnswers, orderBy);

            //Build paging from IQueryable
            var paginatedResult = await query.ToPagingAsync(pageIndex, pageSize, false);
            var paginatedEntities = new PaginatedList<ResultEntity>(paginatedResult.Items, pageIndex, pageSize, paginatedResult.HasMoreData);
            return paginatedEntities;
        }

        private IQueryable<ResultEntity> BuildGetResultsQuery(List<int> customerIds, List<int> departmentIds, List<EntityStatusEnum> statusIds, List<int> surveyIds, List<int> statusTypeIds, List<int> userIds, List<long> resultIds, List<int> batchIds, DateTime? startDateBefore, DateTime? startDateAfter, DateTime? endDateBefore, DateTime? endDateAfter, DateTime? lastUpdatedBefore, DateTime? lastUpdatedAfter, bool includeAllAnswers, string orderBy)
        {
            var query = GetAllAsNoTracking();
            if (customerIds != null && customerIds.Any())
            {
                query = query.Where(p => customerIds.Contains(p.CustomerID.Value));
            }
            if (userIds != null && userIds.Any())
            {
                query = query.Where(t => userIds.Contains((int)t.UserID));
            }
            if (departmentIds != null && departmentIds.Any())
            {
                query = query.Where(p => departmentIds.Contains(p.DepartmentID));
            }
            if (surveyIds != null && surveyIds.Any())
            {
                query = query.Where(p => surveyIds.Contains(p.SurveyID));
            }
            if (statusTypeIds != null && statusTypeIds.Any())
            {
                query = query.Where(p => statusTypeIds.Contains(p.StatusTypeID.Value));
            }
            if (statusIds == null || !statusIds.Any())
            {
                query = query.Where(p => p.EntityStatusId.Value == (int)EntityStatusEnum.Active);
            }
            else
                if (!statusIds.Contains(EntityStatusEnum.All))
            {
                query = query.Where(p => statusIds.Contains((EntityStatusEnum)p.EntityStatusId.Value));
            }
            if (lastUpdatedBefore.HasValue)
            {
                query = query.Where(p => p.LastUpdated <= lastUpdatedBefore);
            }
            if (lastUpdatedAfter.HasValue)
            {
                query = query.Where(p => p.LastUpdated >= lastUpdatedAfter);
            }
            if (resultIds != null && resultIds.Any())
            {
                query = query.Where(p => resultIds.Contains(p.ResultID));
            }
            if (batchIds != null && batchIds.Any())
            {
                query = query.Where(p => batchIds.Contains(p.BatchID));
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

            // if having any filter on AssessmentEntry
            if (includeAllAnswers)
            {
                query = query.Include(t => t.Answers);
            }
            //Query must be ordered before apply paging
            return query.ApplyOrderBy(p => (int)p.ResultID, orderBy);
        }

        public List<ResultEntity> GetResultsForCalculating(
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
            DateTime? startDateAfter = null)
        {
            //TODO : Skip calculate with result have deleted date
            int MaximumResults = 50001;
            var query = GetAll();
            if (customerIds != null && customerIds.Any())
            {
                query = query.Where(x => x.CustomerID.HasValue && customerIds.Contains(x.CustomerID.Value));
            }
            if (surveyIds != null && surveyIds.Any())
            {
                query = query.Where(x => surveyIds.Contains(x.SurveyID));
            }
            if (departmentIds != null && departmentIds.Any())
            {
                query = query.Where(x => departmentIds.Contains(x.DepartmentID));
            }
            if (userIds != null && userIds.Any())
            {
                query = query.Where(x => x.UserID.HasValue && userIds.Contains(x.UserID.Value));
            }
            if (resultIds != null && resultIds.Any())
            {
                query = query.Where(x => resultIds.Contains(x.ResultID));
            }
            if (batchIds != null && batchIds.Any())
            {
                query = query.Where(x => batchIds.Contains(x.BatchID));
            }
            if (statusTypeIds != null && statusTypeIds.Any())
            {
                query = query.Where(x => x.StatusTypeID.HasValue && statusTypeIds.Contains(x.StatusTypeID.Value));
            }
            if (endDateAfter.HasValue)
            {
                query = query.Where(x => x.EndDate.HasValue && x.EndDate >= endDateAfter);
            }
            if (endDateBefore.HasValue)
            {
                query = query.Where(x => x.EndDate.HasValue && x.EndDate <= endDateBefore);
            }
            if (startDateAfter.HasValue)
            {
                query = query.Where(x => x.StartDate.HasValue && x.StartDate >= startDateAfter);
            }
            if (startDateBefore.HasValue)
            {
                query = query.Where(x => x.StartDate.HasValue && x.StartDate <= startDateBefore);
            }
            return query.Take(MaximumResults).ToList();

        }

    }
}