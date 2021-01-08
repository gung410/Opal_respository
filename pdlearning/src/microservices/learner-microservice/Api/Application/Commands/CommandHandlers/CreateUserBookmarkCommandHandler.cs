using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Learner.Application.BusinessLogic.Abstractions;
using Microservice.Learner.Application.Exceptions;
using Microservice.Learner.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Learner.Application.Commands.CommandHandlers
{
    public class CreateUserBookmarkCommandHandler : BaseCommandHandler<CreateUserBookmarkCommand>
    {
        private readonly IWriteUserBookmarkLogic _writeUserBookmarkLogic;
        private readonly IReadOnlyRepository<UserBookmark> _readUserBookmarkRepository;

        public CreateUserBookmarkCommandHandler(
            IUserContext userContext,
            IUnitOfWorkManager unitOfWorkManager,
            IWriteUserBookmarkLogic writeUserBookmarkLogic,
            IReadOnlyRepository<UserBookmark> readUserBookmarkRepository) : base(unitOfWorkManager, userContext)
        {
            _writeUserBookmarkLogic = writeUserBookmarkLogic;
            _readUserBookmarkRepository = readUserBookmarkRepository;
        }

        protected override async Task HandleAsync(CreateUserBookmarkCommand command, CancellationToken cancellationToken)
        {
            var bookmark = await _readUserBookmarkRepository
                .GetAll()
                .Where(p => p.UserId == CurrentUserId)
                .Where(p => p.ItemId == command.ItemId)
                .Where(p => p.ItemType == command.ItemType)
                .FirstOrDefaultAsync(cancellationToken);

            if (bookmark != null)
            {
                throw new UserBookmarkExistedException();
            }

            var userBookmark = new UserBookmark
            {
                Id = command.Id,
                CreatedBy = CurrentUserIdOrDefault,
                UserId = CurrentUserIdOrDefault,
                ItemType = command.ItemType,
                ItemId = command.ItemId
            };

            await _writeUserBookmarkLogic.Insert(userBookmark);
        }
    }
}
