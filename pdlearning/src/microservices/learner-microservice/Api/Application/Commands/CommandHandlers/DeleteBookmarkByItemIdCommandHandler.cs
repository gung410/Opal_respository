using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Learner.Application.BusinessLogic.Abstractions;
using Microservice.Learner.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Learner.Application.Commands.CommandHandlers
{
    public class DeleteBookmarkByItemIdCommandHandler : BaseCommandHandler<DeleteBookmarkByItemIdCommand>
    {
        private readonly IWriteUserBookmarkLogic _writeUserBookmarkLogic;
        private readonly IReadOnlyRepository<UserBookmark> _readUserBookmarkRepository;

        public DeleteBookmarkByItemIdCommandHandler(
            IUserContext userContext,
            IUnitOfWorkManager unitOfWorkManager,
            IWriteUserBookmarkLogic writeUserBookmarkLogic,
            IReadOnlyRepository<UserBookmark> readUserBookmarkRepository) : base(unitOfWorkManager, userContext)
        {
            _writeUserBookmarkLogic = writeUserBookmarkLogic;
            _readUserBookmarkRepository = readUserBookmarkRepository;
        }

        protected override async Task HandleAsync(DeleteBookmarkByItemIdCommand command, CancellationToken cancellationToken)
        {
            var userBookmark = await _readUserBookmarkRepository
                .GetAll()
                .Where(p => p.UserId == CurrentUserId)
                .Where(p => p.ItemId == command.ItemId)
                .FirstOrDefaultAsync(cancellationToken);

            if (userBookmark == null)
            {
                throw new EntityNotFoundException();
            }

            await _writeUserBookmarkLogic.Delete(userBookmark);
        }
    }
}
