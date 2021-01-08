using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Application.Commands.Abstracts;
using Microservice.Course.Application.Constants;
using Microservice.Course.Application.Enums;
using Microservice.Course.Application.Events.Todos;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Settings;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class SaveCommentCommandHandler : BaseCommandHandler<SaveCommentCommand>
    {
        private readonly IReadOnlyRepository<Comment> _readCommentRepository;
        private readonly IThunderCqrs _thunderCqrs;
        private readonly WebAppLinkBuilder _webAppLinkBuilder;
        private readonly CommentCudLogic _commentCudLogic;
        private readonly GetAggregatedParticipantAssignmentTrackSharedQuery
          _getAggregatedParticipantAssignmentTrackSharedQuery;

        public SaveCommentCommandHandler(
            IAccessControlContext<CourseUser> accessControlContext,
            IReadOnlyRepository<Comment> readCommentRepository,
            IThunderCqrs thunderCqrs,
            IUserContext userContext,
            IUnitOfWorkManager unitOfWorkManager,
            WebAppLinkBuilder webAppLinkBuilder,
            GetAggregatedParticipantAssignmentTrackSharedQuery getAggregatedParticipantAssignmentTrackSharedQuery,
            CommentCudLogic commentCudLogic) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readCommentRepository = readCommentRepository;
            _thunderCqrs = thunderCqrs;
            _webAppLinkBuilder = webAppLinkBuilder;
            _commentCudLogic = commentCudLogic;
            _getAggregatedParticipantAssignmentTrackSharedQuery = getAggregatedParticipantAssignmentTrackSharedQuery;
        }

        protected override async Task HandleAsync(SaveCommentCommand command, CancellationToken cancellationToken)
        {
            if (command.IsCreate)
            {
                await Create(command, cancellationToken);
            }
            else
            {
                await Update(command, cancellationToken);
            }

            if (command.EntityCommentType == EntityCommentType.ParticipantAssignmentTrackQuizAnswer)
            {
                await SendAssignmentFeedback(command);
            }
        }

        private async Task SendAssignmentFeedback(SaveCommentCommand command)
        {
            switch (command.Notification)
            {
                case CommentNotification.AssignmentFeedbackToLearner:
                    await SendAssignmentFeedbackToLearner(command);
                    break;
                case CommentNotification.AssignmentFeedbackToCF:
                    await SendAssignmentFeedbackToCF(command);
                    break;
            }
        }

        private async Task SendAssignmentFeedbackToLearner(SaveCommentCommand command)
        {
            var aggregatedParticipantAssignmentTrack = await _getAggregatedParticipantAssignmentTrackSharedQuery.ByParticipantAssignmentTrackId(command.ObjectId);

            await _thunderCqrs.SendEvent(new AssignmentFeedbackNotifyLearnerEvent(
                CurrentUserIdOrDefault,
                new AssignmentFeedbackNotifyLearnerPayload
                {
                    CourseTitle = aggregatedParticipantAssignmentTrack.Course.CourseName,
                    ClassRunTitle = aggregatedParticipantAssignmentTrack.ClassRun.ClassTitle,
                    ActionUrl = _webAppLinkBuilder.GetCourseDetailLearnerLinkForCAMModule(aggregatedParticipantAssignmentTrack.Course.Id)
                },
                new List<Guid> { aggregatedParticipantAssignmentTrack.Learner.Id }));
        }

        private async Task SendAssignmentFeedbackToCF(SaveCommentCommand command)
        {
            var aggregatedParticipantAssignmentTrack = await _getAggregatedParticipantAssignmentTrackSharedQuery.ByParticipantAssignmentTrackId(command.ObjectId);
            var receiverIds = new List<Guid>();
            receiverIds.AddRange(aggregatedParticipantAssignmentTrack.Course.GetFacilitatorIds().Concat(aggregatedParticipantAssignmentTrack.Course.CollaborativeContentCreatorIds));
            receiverIds.Add(aggregatedParticipantAssignmentTrack.Course.CreatedBy);

            await _thunderCqrs.SendEvent(new AssignmentFeedbackNotifyCFEvent(
                CurrentUserIdOrDefault,
                new AssignmentFeedbackNotifyCFPayload
                {
                    LearnerName = aggregatedParticipantAssignmentTrack.Learner.FullName(),
                    CourseTitle = aggregatedParticipantAssignmentTrack.Course.CourseName,
                    ClassrunTitle = aggregatedParticipantAssignmentTrack.ClassRun.ClassTitle,
                    ActionUrl = _webAppLinkBuilder.GetAssignmentDetailLinkForLMMModule(
                        LMMTabConfigurationConstant.CoursesTab,
                        LMMTabConfigurationConstant.CourseInfoTab,
                        LMMTabConfigurationConstant.AllClassRunsTab,
                        CourseDetailModeConstant.View,
                        LMMTabConfigurationConstant.ClassRunInfoTab,
                        ClassRunDetailModeConstant.View,
                        LMMTabConfigurationConstant.AssigneesTab,
                        AssignmentDetailModeConstant.View,
                        aggregatedParticipantAssignmentTrack.Course.Id,
                        aggregatedParticipantAssignmentTrack.ClassRun.Id,
                        aggregatedParticipantAssignmentTrack.ParticipantAssignmentTrack.AssignmentId)
                },
                receiverIds));
        }

        private async Task Update(SaveCommentCommand command, CancellationToken cancellationToken)
        {
            if (command.Id.HasValue)
            {
                var existedComment = await _readCommentRepository.GetAsync(command.Id.Value);
                existedComment.Content = command.Content;
                await _commentCudLogic.Update(existedComment, cancellationToken);
            }
        }

        private async Task Create(SaveCommentCommand command, CancellationToken cancellationToken)
        {
            if (command.ObjectIds != null && command.ObjectIds.Any())
            {
                var comments = command.ObjectIds.
                    Select(objectId => new Comment
                    {
                        Id = Guid.NewGuid(),
                        Content = command.Content,
                        UserId = CurrentUserIdOrDefault,
                        ObjectId = objectId,
                        Action = GetAction(command.EntityCommentType, command.StatusEnum)
                    })
                    .ToList();
                await _commentCudLogic.InsertMany(comments, cancellationToken);
            }
            else
            {
                var comment = new Comment
                {
                    Id = command.Id ?? Guid.NewGuid(),
                    Content = command.Content,
                    UserId = CurrentUserIdOrDefault,
                    ObjectId = command.ObjectId,
                    Action = GetAction(command.EntityCommentType, command.StatusEnum)
                };
                await _commentCudLogic.Insert(comment, cancellationToken);
            }
        }

        private string GetAction(EntityCommentType entityCommentType, Enum statusEnum)
        {
            statusEnum ??= DefaultEnum.None;

            return CommentActionConstant.EntityActionDict.ContainsKey(entityCommentType) &&
                   CommentActionConstant.EntityActionDict[entityCommentType].ContainsKey(statusEnum)
                ? CommentActionConstant.EntityActionDict[entityCommentType][statusEnum]
                : string.Empty;
        }
    }
}
