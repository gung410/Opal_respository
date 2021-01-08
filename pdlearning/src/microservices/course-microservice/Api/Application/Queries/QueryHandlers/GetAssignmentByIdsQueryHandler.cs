using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.Queries.Abstracts;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Course.Application.Queries.QueryHandlers
{
    public class GetAssignmentByIdsQueryHandler : BaseQueryHandler<GetAssignmentByIdsQuery, IEnumerable<AssignmentModel>>
    {
        private readonly IReadOnlyRepository<Assignment> _readAssignmentRepository;
        private readonly GetAggregatedContentSharedQuery _aggregatedContentSharedQuery;

        public GetAssignmentByIdsQueryHandler(
            IReadOnlyRepository<Assignment> readAssignmentRepository,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager,
            GetAggregatedContentSharedQuery aggregatedContentSharedQuery) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readAssignmentRepository = readAssignmentRepository;
            _aggregatedContentSharedQuery = aggregatedContentSharedQuery;
        }

        protected override async Task<IEnumerable<AssignmentModel>> HandleAsync(GetAssignmentByIdsQuery query, CancellationToken cancellationToken)
        {
            var assignmentQuery = _readAssignmentRepository.GetAll().Where(x => query.Ids.Contains(x.Id));
            var aggregatedAssignments = await _aggregatedContentSharedQuery.AssignmentByQuery(
                assignmentQuery,
                query.IncludeQuizForm,
                false,
                cancellationToken);

            return aggregatedAssignments
                .Select(x => new AssignmentModel(
                    x.Assignment,
                    x.QuizForm == null ? null : new QuizAssignmentFormModel(x.QuizForm, x.QuizFormQuestions, false)));
        }
    }
}
