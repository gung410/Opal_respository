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
    public class GetSingleParticipantAssignmentTrackQueryHandler : BaseQueryHandler<GetSingleParticipantAssignmentTrackQuery, ParticipantAssignmentTrackModel>
    {
        private readonly IReadOnlyRepository<ParticipantAssignmentTrack> _readParticipantAssignmentTrackRepository;

        public GetSingleParticipantAssignmentTrackQueryHandler(
            IReadOnlyRepository<ParticipantAssignmentTrack> readParticipantAssignmentTrackRepository,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readParticipantAssignmentTrackRepository = readParticipantAssignmentTrackRepository;
        }

        protected override async Task<ParticipantAssignmentTrackModel> HandleAsync(GetSingleParticipantAssignmentTrackQuery query, CancellationToken cancellationToken)
        {
            var participantAssignmentTrack = EnsureEntityFound(
                await _readParticipantAssignmentTrackRepository
                    .FirstOrDefaultAsync(p => p.AssignmentId == query.AssignmentId && p.RegistrationId == query.RegistrationId));

            return new ParticipantAssignmentTrackModel(
                participantAssignmentTrack,
                participantAssignmentTrack.QuizAnswer);
        }
    }
}
