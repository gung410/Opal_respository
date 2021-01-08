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
    public class DeleteBookmarkByIdCommandHandler : BaseCommandHandler<DeleteBookmarkByIdCommand>
    {
        private readonly IWriteUserBookmarkLogic _writeUserBookmarkLogic;
        private readonly IReadOnlyRepository<UserBookmark> _readUserBookmarkRepository;

        public DeleteBookmarkByIdCommandHandler(
            IUserContext userContext,
            IUnitOfWorkManager unitOfWorkManager,
            IWriteUserBookmarkLogic writeUserBookmarkLogic,
            IReadOnlyRepository<UserBookmark> readUserBookmarkRepository) : base(unitOfWorkManager, userContext)
        {
            _writeUserBookmarkLogic = writeUserBookmarkLogic;
            _readUserBookmarkRepository = readUserBookmarkRepository;
        }

        protected override async Task HandleAsync(DeleteBookmarkByIdCommand command, CancellationToken cancellationToken)
        {
            var bookmark = await _readUserBookmarkRepository
                .GetAll()
                .Where(p => p.UserId == CurrentUserId)
                .Where(p => p.Id == command.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (bookmark == null)
            {
                throw new EntityNotFoundException();
            }

            await _writeUserBookmarkLogic.Delete(bookmark);
        }
    }
}
