using System;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Content.Domain.ValueObject;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Timing;
using DigitalContentEntity = Microservice.Content.Domain.Entities.DigitalContent;
using VideoCommentEntity = Microservice.Content.Domain.Entities.VideoComment;

namespace Microservice.Content.Application.Commands.CommandHandlers
{
    public class SaveVideoCommentCommandHandler : BaseCommandHandler<SaveVideoCommentCommand>
    {
        private readonly IRepository<DigitalContentEntity> _digitalContentRepository;
        private readonly IRepository<VideoCommentEntity> _videoCommentRepository;

        public SaveVideoCommentCommandHandler(
            IRepository<VideoCommentEntity> videoCommentRepository,
            IRepository<DigitalContentEntity> digitalContentRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IUserContext userContext,
            IAccessControlContext accessControlContext) : base(unitOfWorkManager, userContext, accessControlContext)
        {
            _digitalContentRepository = digitalContentRepository;
            _videoCommentRepository = videoCommentRepository;
        }

        protected override async Task HandleAsync(SaveVideoCommentCommand command, CancellationToken cancellationToken)
        {
            if (command.IsCreation)
            {
                await Create(command);
            }
            else
            {
                await Update(command);
            }
        }

        private async Task Create(SaveVideoCommentCommand command)
        {
            var objectId = command.CreationRequest.ObjectId ?? Guid.Empty;
            var originalObjectId = command.CreationRequest.OriginalObjectId ?? Guid.Empty;
            var noted = $"{command.CreationRequest.SourceType} ";
            switch (command.CreationRequest.SourceType)
            {
                case VideoSourceType.CCPM:
                    var digitalContent = await _digitalContentRepository.GetAsync(objectId);
                    originalObjectId = digitalContent.OriginalObjectId;
                    noted += $"/ ObjectId: [DigitalContentId] / OriginalObjectId: [OriginalDigitalContentId]";
                    break;
                case VideoSourceType.LMM:
                    noted += $"/ ObjectId: [ClassrunId] / OriginalObjectId: [LectureId]";
                    break;
                case VideoSourceType.CSL:
                    noted += $"/"; // [CSL] if using same video, will upload same video again and get new videoId
                    break;
                default:
                    return;
            }

            var videoComment = new VideoCommentEntity
            {
                Id = command.Id,
                UserId = command.UserId,
                ObjectId = objectId,
                OriginalObjectId = originalObjectId,
                SourceType = command.CreationRequest.SourceType,
                Content = command.CreationRequest.Content,
                VideoId = command.CreationRequest.VideoId,
                VideoTime = command.CreationRequest.VideoTime,
                Note = noted,
            };

            await _videoCommentRepository.InsertAsync(videoComment);
        }

        private async Task Update(SaveVideoCommentCommand command)
        {
            var existedComment = await _videoCommentRepository.GetAsync(command.UpdateRequest.Id);
            if (!Guid.Equals(command.UserId, existedComment.UserId))
            {
                return;
            }

            existedComment.Content = command.UpdateRequest.Content;
            existedComment.VideoTime = command.UpdateRequest.VideoTime;
            await _videoCommentRepository.UpdateAsync(existedComment);
        }
    }
}
