using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Application.Commands.Abstracts;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Timing;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class SaveCommentTrackCommandHandler : BaseCommandHandler<SaveCommentTrackCommand>
    {
        private readonly IReadOnlyRepository<CommentTrack> _readCommentTrackRepository;
        private readonly CommentCudLogic _commentCudLogic;

        public SaveCommentTrackCommandHandler(
            IReadOnlyRepository<CommentTrack> readCommentTrackRepository,
            CommentCudLogic commentCudLogic,
            IAccessControlContext<CourseUser> accessControlContext,
            IUserContext userContext,
            IUnitOfWorkManager unitOfWorkManager) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readCommentTrackRepository = readCommentTrackRepository;
            _commentCudLogic = commentCudLogic;
        }

        protected override async Task HandleAsync(SaveCommentTrackCommand command, CancellationToken cancellationToken)
        {
            var commentTrackeds = await _readCommentTrackRepository.GetAllListAsync(x => x.UserId == CurrentUserId);

            var trackedCommentIds = commentTrackeds.Select(x => x.CommentId);

            var addCommentTracks = command.CommentIds
                .Where(x => !trackedCommentIds.Contains(x))
                .Select(x => new CommentTrack
                {
                    Id = Guid.NewGuid(),
                    CommentId = x,
                    UserId = CurrentUserIdOrDefault,
                    CreatedDate = Clock.Now
                });

            await _commentCudLogic.InsertManyCommentTrack(addCommentTracks);
        }
    }
}
