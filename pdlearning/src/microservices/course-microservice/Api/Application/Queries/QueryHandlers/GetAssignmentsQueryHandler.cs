using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.Enums;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.Queries.Abstracts;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Course.Application.Queries.QueryHandlers
{
    public class GetAssignmentsQueryHandler : BaseQueryHandler<GetAssignmentsQuery, PagedResultDto<AssignmentModel>>
    {
        private readonly IReadOnlyRepository<Assignment> _readAssignmentRepository;
        private readonly GetAggregatedContentSharedQuery _aggregatedContentSharedQuery;

        public GetAssignmentsQueryHandler(
            IReadOnlyRepository<Assignment> readAssignmentRepository,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager,
            GetAggregatedContentSharedQuery aggregatedContentSharedQuery) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readAssignmentRepository = readAssignmentRepository;
            _aggregatedContentSharedQuery = aggregatedContentSharedQuery;
        }

        protected override async Task<PagedResultDto<AssignmentModel>> HandleAsync(GetAssignmentsQuery query, CancellationToken cancellationToken)
        {
            var dbQuery = _readAssignmentRepository.GetAll().Where(_ => _.CourseId == query.CourseId && _.ClassRunId == query.ClassRunId);
            switch (query.FilterType)
            {
                case AssignmentsFilterType.Quiz:
                    {
                        dbQuery = dbQuery.Where(x => x.Type == AssignmentType.Quiz);
                        break;
                    }
            }

            var totalCount = await dbQuery.CountAsync(cancellationToken);

            dbQuery = dbQuery.OrderBy(p => p.CreatedDate);

            dbQuery = ApplyPaging(dbQuery, query.PageInfo);

            var aggregatedAssignments = await _aggregatedContentSharedQuery.AssignmentByQuery(
                dbQuery,
                query.IncludeQuizForm,
                false,
                cancellationToken);

            return new PagedResultDto<AssignmentModel>(
                totalCount,
                aggregatedAssignments
                    .Select(x => new AssignmentModel(
                        x.Assignment,
                        x.QuizForm == null ? null : new QuizAssignmentFormModel(x.QuizForm, x.QuizFormQuestions, false)))
                    .ToList());
        }
    }
}
