using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.Queries.Abstracts;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Course.Application.Queries.QueryHandlers
{
    public class GetAssessmentAnswerByIdOrUserQueryHandler : BaseQueryHandler<GetAssessmentAnswerByIdOrUserQuery, AssessmentAnswerModel>
    {
        private readonly IReadOnlyRepository<AssessmentAnswer> _readAssessmentAnswerRepository;

        public GetAssessmentAnswerByIdOrUserQueryHandler(
            IReadOnlyRepository<AssessmentAnswer> readAssessmentAnswerRepository,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readAssessmentAnswerRepository = readAssessmentAnswerRepository;
        }

        protected override async Task<AssessmentAnswerModel> HandleAsync(
            GetAssessmentAnswerByIdOrUserQuery query,
            CancellationToken cancellationToken)
        {
            if (query.Id.HasValue)
            {
                var assessmentAnswer = await _readAssessmentAnswerRepository.GetAsync(query.Id.Value);
                return new AssessmentAnswerModel(assessmentAnswer);
            }
            else
            {
                var assessmentAnswer = await _readAssessmentAnswerRepository.FirstOrDefaultAsync(x => x.ParticipantAssignmentTrackId == query.ParticipantAssignmentTrackId && x.UserId == query.UserId);

                EnsureEntityFound(assessmentAnswer);

                return new AssessmentAnswerModel(assessmentAnswer);
            }
        }
    }
}
