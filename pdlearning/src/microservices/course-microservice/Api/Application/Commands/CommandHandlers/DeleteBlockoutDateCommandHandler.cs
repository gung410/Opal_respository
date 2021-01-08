using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Application.Commands.Abstracts;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class DeleteBlockoutDateCommandHandler : BaseCommandHandler<DeleteBlockoutDateCommand>
    {
        private readonly BlockoutDateCudLogic _blockoutDateCudLogic;
        private readonly IReadOnlyRepository<BlockoutDate> _readBlockoutDateRepository;

        public DeleteBlockoutDateCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            BlockoutDateCudLogic blockoutDateCudLogic,
            IReadOnlyRepository<BlockoutDate> readBlockoutDateRepository) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _blockoutDateCudLogic = blockoutDateCudLogic;
            _readBlockoutDateRepository = readBlockoutDateRepository;
        }

        protected override async Task HandleAsync(DeleteBlockoutDateCommand command, CancellationToken cancellationToken)
        {
            EnsureValidPermission(BlockoutDate.HasCrudPermission(CurrentUserId, CurrentUserRoles));

            var blockoutDate = await _readBlockoutDateRepository.GetAsync(command.BlockoutDateId);

            EnsureBusinessLogicValid(blockoutDate.ValidateCanBeDeleted());

            await _blockoutDateCudLogic.Delete(blockoutDate, cancellationToken);
        }
    }
}
