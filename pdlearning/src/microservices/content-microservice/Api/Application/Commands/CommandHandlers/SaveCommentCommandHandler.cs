using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using CommentEntity = Microservice.Content.Domain.Entities.Comment;

namespace Microservice.Content.Application.Commands.CommandHandlers
{
    public class SaveCommentCommandHandler : BaseCommandHandler<SaveCommentCommand>
    {
        private readonly IRepository<CommentEntity> _commentRepository;

        public SaveCommentCommandHandler(
            IRepository<CommentEntity> commentRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IUserContext userContext,
            IAccessControlContext accessControlContext) : base(unitOfWorkManager, userContext, accessControlContext)
        {
            _commentRepository = commentRepository;
        }

        protected override async Task HandleAsync(SaveCommentCommand command, CancellationToken cancellationToken)
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

        private async Task Update(SaveCommentCommand command)
        {
            var existedComment = await _commentRepository.GetAsync(command.UpdateRequest.Id);
            existedComment.Content = command.UpdateRequest.Content;
            await _commentRepository.UpdateAsync(existedComment);
        }

        private async Task Create(SaveCommentCommand command)
        {
            var comment = new CommentEntity
            {
                Id = command.Id,
                Content = command.CreationRequest.Content,
                UserId = command.UserId,
                ObjectId = command.CreationRequest.ObjectId,
            };

            await _commentRepository.InsertAsync(comment);
        }
    }
}