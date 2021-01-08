using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.Queries.Abstracts;
using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Course.Application.Queries.QueryHandlers
{
    public class GetParticipantAssignmentTrackByIdsQueryHandler : BaseQueryHandler<GetParticipantAssignmentTrackByIdsQuery, List<ParticipantAssignmentTrackModel>>
    {
        private readonly IReadOnlyRepository<ParticipantAssignmentTrack> _readParticipantAssignmentTrackRepository;

        public GetParticipantAssignmentTrackByIdsQueryHandler(
            IReadOnlyRepository<ParticipantAssignmentTrack> readParticipantAssignmentTrackRepository,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readParticipantAssignmentTrackRepository = readParticipantAssignmentTrackRepository;
        }

        protected override async Task<List<ParticipantAssignmentTrackModel>> HandleAsync(GetParticipantAssignmentTrackByIdsQuery query, CancellationToken cancellationToken)
        {
            var participantAssignmentTracks = await _readParticipantAssignmentTrackRepository.GetAll().Where(x => query.Ids.Contains(x.Id)).ToListAsync(cancellationToken);

            return participantAssignmentTracks
                .Select(participantAssignmentTrack => new ParticipantAssignmentTrackModel(
                    participantAssignmentTrack,
                    participantAssignmentTrack.QuizAnswer))
                .ToList();
        }
    }
}
