using System;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Content.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Content.Application.Commands.CommandHandlers
{
    public class DeleteVideoCommentCommandHandler : BaseCommandHandler<DeleteVideoCommentCommand>
    {
        private readonly IRepository<VideoComment> _videoCommentRepository;

        public DeleteVideoCommentCommandHandler(
            IRepository<VideoComment> videoCommentRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IAccessControlContext accessControlContext,
            IUserContext userContext) : base(unitOfWorkManager, userContext, accessControlContext)
        {
            _videoCommentRepository = videoCommentRepository;
        }

        protected override async Task HandleAsync(DeleteVideoCommentCommand command, CancellationToken cancellationToken)
        {
            var existedComment = await _videoCommentRepository.GetAsync(command.Id);
            if (!Guid.Equals(command.UserId, existedComment.UserId))
            {
                return;
            }

            await _videoCommentRepository.DeleteAsync(existedComment);
        }
    }
}
