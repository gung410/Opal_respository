using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.Queries.Abstracts;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Course.Application.Queries.QueryHandlers
{
    public class GetParticipantAssignmentTrackByIdQueryHandler : BaseQueryHandler<GetParticipantAssignmentTrackByIdQuery, ParticipantAssignmentTrackModel>
    {
        private readonly IReadOnlyRepository<ParticipantAssignmentTrack> _readParticipantAssignmentTrackRepository;

        public GetParticipantAssignmentTrackByIdQueryHandler(
            IReadOnlyRepository<ParticipantAssignmentTrack> readParticipantAssignmentTrackRepository,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readParticipantAssignmentTrackRepository = readParticipantAssignmentTrackRepository;
        }

        protected override async Task<ParticipantAssignmentTrackModel> HandleAsync(GetParticipantAssignmentTrackByIdQuery query, CancellationToken cancellationToken)
        {
            var participantAssignmentTrack = await _readParticipantAssignmentTrackRepository.GetAsync(query.Id);

            return new ParticipantAssignmentTrackModel(
                participantAssignmentTrack,
                participantAssignmentTrack.QuizAnswer);
        }
    }
}
