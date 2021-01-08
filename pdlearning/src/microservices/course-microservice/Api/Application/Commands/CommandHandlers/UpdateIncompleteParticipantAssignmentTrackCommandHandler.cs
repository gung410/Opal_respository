using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Application.Commands;
using Microservice.Course.Application.Commands.Abstracts;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microservice.Course.Settings;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Timing;

namespace Microservice.Course.Application.Queries.QueryHandlers
{
    public class UpdateIncompleteParticipantAssignmentTrackCommandHandler : BaseCommandHandler<UpdateIncompleteParticipantAssignmentTrackCommand>
    {
        private readonly OpalSettingsOption _opalSettingsOption;
        private readonly GetAggregatedParticipantAssignmentTrackSharedQuery _getAggregatedParticipantAssignmentTrackSharedQuery;
        private readonly ParticipantAssignmentTrackCudLogic _participantAssignmentTrackCudLogic;

        public UpdateIncompleteParticipantAssignmentTrackCommandHandler(
            GetAggregatedParticipantAssignmentTrackSharedQuery getAggregatedParticipantAssignmentTrackSharedQuery,
            ParticipantAssignmentTrackCudLogic participantAssignmentTrackCudLogic,
            IOptions<OpalSettingsOption> opalSettingsOption,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _getAggregatedParticipantAssignmentTrackSharedQuery = getAggregatedParticipantAssignmentTrackSharedQuery;
            _participantAssignmentTrackCudLogic = participantAssignmentTrackCudLogic;
            _opalSettingsOption = opalSettingsOption.Value;
        }

        protected override async Task HandleAsync(
            UpdateIncompleteParticipantAssignmentTrackCommand command,
            CancellationToken cancellationToken)
        {
            var assignmentExtendedDays = _opalSettingsOption.AssignmentExtendedDays;

            var aggregatedParticipantAssignmentTracks = await _getAggregatedParticipantAssignmentTrackSharedQuery.All(x => x.EndDate < Clock.Now, cancellationToken);

            EnsureBusinessLogicValid(aggregatedParticipantAssignmentTracks, p => p.Course.ValidateNotArchived());

            var participantAssignmentIncompletePendingSubmission = aggregatedParticipantAssignmentTracks
                .Where(x => x.ParticipantAssignmentTrack.SubmittedDate == null &&
                            x.ParticipantAssignmentTrack.EndDate < Clock.Now &&
                            x.ParticipantAssignmentTrack.EndDate >= Clock.Now.AddDays(-assignmentExtendedDays))
                .Select(x => x.ParticipantAssignmentTrack)
                .ToList();

            var participantAssignmentIncomplete = aggregatedParticipantAssignmentTracks
                .Where(x => !x.ParticipantAssignmentTrack.IsDone())
                .Where(x => x.ParticipantAssignmentTrack.EndDate < Clock.Now.AddDays(-assignmentExtendedDays))
                .Select(x => x.ParticipantAssignmentTrack)
                .ToList();

            participantAssignmentIncompletePendingSubmission.ForEach(x =>
            {
                x.Status = ParticipantAssignmentTrackStatus.IncompletePendingSubmission;
                x.ChangedDate = Clock.Now;
            });

            participantAssignmentIncomplete.ForEach(x =>
            {
                x.Status = ParticipantAssignmentTrackStatus.Incomplete;
                x.ChangedDate = Clock.Now;
            });

            await _participantAssignmentTrackCudLogic.UpdateMany(participantAssignmentIncompletePendingSubmission);
            await _participantAssignmentTrackCudLogic.UpdateMany(participantAssignmentIncomplete);
        }
    }
}
