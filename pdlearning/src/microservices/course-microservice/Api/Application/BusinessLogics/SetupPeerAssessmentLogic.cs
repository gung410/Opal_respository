using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Course.Application.AggregatedEntityModels;
using Microservice.Course.Application.BusinessLogics.Abstractions;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Common.Helpers;
using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Extensions;
using Thunder.Platform.Core.Validation;

namespace Microservice.Course.Application.BusinessLogics
{
    public class SetupPeerAssessmentLogic : BaseBusinessLogic
    {
        private readonly IReadOnlyRepository<ParticipantAssignmentTrack> _readParticipantAssignmentTrackRepository;
        private readonly IReadOnlyRepository<Registration> _readRegistrationRepository;
        private readonly ParticipantAssignmentTrackCudLogic _participantAssignmentTrackCudLogic;
        private readonly GetAggregatedParticipantAssignmentTrackSharedQuery _participantAssignmentTrackSharedQuery;

        public SetupPeerAssessmentLogic(
            IReadOnlyRepository<ParticipantAssignmentTrack> readParticipantAssignmentTrackRepository,
            IReadOnlyRepository<Registration> readRegistrationRepository,
            ParticipantAssignmentTrackCudLogic participantAssignmentTrackCudLogic,
            GetAggregatedParticipantAssignmentTrackSharedQuery participantAssignmentTrackSharedQuery,
            IUserContext userContext) : base(userContext)
        {
            _readParticipantAssignmentTrackRepository = readParticipantAssignmentTrackRepository;
            _readRegistrationRepository = readRegistrationRepository;
            _participantAssignmentTrackCudLogic = participantAssignmentTrackCudLogic;
            _participantAssignmentTrackSharedQuery = participantAssignmentTrackSharedQuery;
        }

        public async Task<IEnumerable<AssessmentAnswer>> Execute(
            List<Guid> participantAssignmentTrackIds,
            Assignment assignment,
            CancellationToken cancellationToken)
        {
            var aggregatedParticipantAssignmentTracks = await _participantAssignmentTrackSharedQuery
                .WithRegistrationByQuery(
                    _readParticipantAssignmentTrackRepository
                    .GetAll()
                    .Where(p => participantAssignmentTrackIds.Contains(p.Id))
                    .Where(ParticipantAssignmentTrack.CanSetupPeerAssessmentValidator().IsValidExpression));

            var classRunId = EnsureBusinessLogicValid(ValidateAllAssignmentTrackSameClassRun(aggregatedParticipantAssignmentTracks));

            var allParticipantInClass = await _readRegistrationRepository
                .GetAll()
                .Where(p => p.ClassRunId == classRunId)
                .Where(Registration.IsParticipantExpr())
                .ToListAsync(cancellationToken);

            EnsureBusinessLogicValid(Validation.ValidIf(
                assignment.AssessmentConfig.NumberAutoAssessor < allParticipantInClass.Count,
                "Number of random peer assessment is invalid. It must be less than total assignees in class."));

            var allPeerAssessments = aggregatedParticipantAssignmentTracks
                .Select(p => p.ParticipantAssignmentTrack)
                .SelectList(participantAssignmentTrack =>
                    ListHelper.RandomPick(
                        items: allParticipantInClass.SelectList(p => p.UserId),
                        pickAmount: assignment.AssessmentConfig.NumberAutoAssessor,
                        ignoreItems: participantAssignmentTrack.UserId)
                    .SelectList(randomizedOtherLeanerId => new AssessmentAnswer
                    {
                        Id = Guid.NewGuid(),
                        AssessmentId = assignment.AssessmentConfig.AssessmentId,
                        ParticipantAssignmentTrackId = participantAssignmentTrack.Id,
                        UserId = randomizedOtherLeanerId,
                        CriteriaAnswers = new List<AssessmentCriteriaAnswer>(),
                        CreatedBy = CurrentUserIdOrDefault
                    }))
                .SelectMany(randomizedOtherLearners => randomizedOtherLearners);

            aggregatedParticipantAssignmentTracks.ForEach(p => p.ParticipantAssignmentTrack.IsAutoAssignedOnce = true);

            await _participantAssignmentTrackCudLogic.UpdateMany(aggregatedParticipantAssignmentTracks.Select(p => p.ParticipantAssignmentTrack).ToList());

            return allPeerAssessments;
        }

        public async Task<ParticipantAssignmentTrack> UpdateAssignedAssessmentManuallyOnceTrue(Guid participantAssignmentTrackId)
        {
            var participantAssignmentTrack =
                await _readParticipantAssignmentTrackRepository.GetAsync(participantAssignmentTrackId);

            if (participantAssignmentTrack.IsAssignedAssessmentManuallyOnce == false)
            {
                participantAssignmentTrack.IsAssignedAssessmentManuallyOnce = true;

                await _participantAssignmentTrackCudLogic.Update(participantAssignmentTrack);
            }

            return participantAssignmentTrack;
        }

        private Validation<Guid> ValidateAllAssignmentTrackSameClassRun(
            List<ParticipantAssignmentTrackAggregatedEntityModel> aggregatedParticipantAssignmentTracks)
        {
            var classRunIds = aggregatedParticipantAssignmentTracks
                .SelectList(p => p.Registration.ClassRunId)
                .Distinct()
                .ToList();
            return Validation.ValidIf(
                classRunIds.FirstOrDefault(),
                classRunIds.Count() == 1,
                "All assignees must be in same class.");
        }
    }
}
