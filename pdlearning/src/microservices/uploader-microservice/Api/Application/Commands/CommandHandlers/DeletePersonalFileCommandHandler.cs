using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Uploader.Application.Models;
using Microservice.Uploader.Domain.Entities;
using Microservice.Uploader.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Cqrs;

namespace Microservice.Uploader.Application.Commands.CommandHandlers
{
    public class DeletePersonalFileCommandHandler : BaseCommandHandler<DeletePersonalFileCommand>
    {
        private readonly IRepository<PersonalFile> _personalFileRepository;
        private readonly IRepository<PersonalSpace> _personalSpaceRepository;
        private readonly IThunderCqrs _thunderCqrs;

        public DeletePersonalFileCommandHandler(
            IRepository<PersonalFile> personalFileRepository,
            IRepository<PersonalSpace> personalSpaceRepository,
            IThunderCqrs thunderCqrs,
            IUnitOfWorkManager unitOfWorkManager,
            IAccessControlContext accessControlContext) : base(unitOfWorkManager, accessControlContext)
        {
            _personalFileRepository = personalFileRepository;
            _personalSpaceRepository = personalSpaceRepository;
            _thunderCqrs = thunderCqrs;
        }

        protected override async Task HandleAsync(DeletePersonalFileCommand command, CancellationToken cancellationToken)
        {
            var exisitedPersonalFile = await _personalFileRepository
                .GetAll()
                .FirstOrDefaultAsync(file => file.Id == command.Id, cancellationToken);

            var existedPersonalSpace = _personalSpaceRepository
                .GetAll()
                .Where(PersonalSpaceExpressions.HasPermissionToSeePersonalSpace(command.UserId))
                .FirstOrDefault(x => x.Id == exisitedPersonalFile.PersonalSpaceId);

            if (existedPersonalSpace == null || exisitedPersonalFile == null)
            {
                throw new PersonalSpaceAccessDeniedException();
            }

            await _personalFileRepository.DeleteAsync(command.Id);

            existedPersonalSpace.TotalUsed -= exisitedPersonalFile.FileSize;
            await _personalSpaceRepository.UpdateAsync(existedPersonalSpace);
        }
    }
}
