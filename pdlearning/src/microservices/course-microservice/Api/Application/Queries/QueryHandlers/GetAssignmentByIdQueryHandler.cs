using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.Constants;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.Queries.Abstracts;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Exceptions;

namespace Microservice.Course.Application.Queries.QueryHandlers
{
    public class GetAssignmentByIdQueryHandler : BaseQueryHandler<GetAssignmentByIdQuery, AssignmentModel>
    {
        private readonly IReadOnlyRepository<Assignment> _readAssignmentRepository;
        private readonly GetAggregatedContentSharedQuery _aggregatedContentSharedQuery;

        public GetAssignmentByIdQueryHandler(
            IReadOnlyRepository<Assignment> readAssignmentRepository,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager,
            GetAggregatedContentSharedQuery aggregatedContentSharedQuery) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readAssignmentRepository = readAssignmentRepository;
            _aggregatedContentSharedQuery = aggregatedContentSharedQuery;
        }

        protected override async Task<AssignmentModel> HandleAsync(GetAssignmentByIdQuery query, CancellationToken cancellationToken)
        {
            var aggregatedAssignment =
                (await _aggregatedContentSharedQuery.AssignmentByQuery(
                    _readAssignmentRepository.GetAll().Where(x => x.Id == query.Id),
                    true,
                    false,
                    cancellationToken))
                .FirstOrDefault();

            aggregatedAssignment = EnsureEntityFound(aggregatedAssignment);

            var assignmentQuizForm =
                aggregatedAssignment.QuizForm == null
                    ? null
                    : new QuizAssignmentFormModel(
                        aggregatedAssignment.QuizForm,
                        aggregatedAssignment.QuizFormQuestions,
                        query.ForLearnerAnswer);
            return new AssignmentModel(
                aggregatedAssignment.Assignment, assignmentQuizForm);
        }
    }
}
