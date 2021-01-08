using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Uploader.Domain.Entities;
using Microservice.Uploader.Infrastructure;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Timing;

namespace Microservice.Uploader.Application.Commands
{
    public class SavePersonalSpaceCommandHandler : BaseCommandHandler<SavePersonalSpaceCommand>
    {
        private readonly IRepository<PersonalSpace> _personalSpaceRepository;

        public SavePersonalSpaceCommandHandler(
            IRepository<PersonalSpace> personalSpaceRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IAccessControlContext accessControlContext) : base(unitOfWorkManager, accessControlContext)
        {
            _personalSpaceRepository = personalSpaceRepository;
        }

        // ******** IMPORTANT **********
        // This command is use for internal update with data received from SAM Database
        // In Update Function, if the UserId does not exist in PersonalSpace data,
        // that mean we got some mistake with synchronization, so need create a new Record
        protected override async Task HandleAsync(SavePersonalSpaceCommand command, CancellationToken cancellationToken)
        {
            if (command.IsCreation)
            {
                await CreateNewPersonalSpace(command);
            }
            else
            {
                await UpdatePersonalSpace(command);
            }
        }

        protected async Task<PersonalSpace> CreateNewPersonalSpace(SavePersonalSpaceCommand command)
        {
            var personalSpace = _personalSpaceRepository
                .GetAll()
                .FirstOrDefault(x => x.UserId == command.UserId);

            if (personalSpace == null)
            {
                personalSpace = new PersonalSpace
                {
                    TotalUsed = 0,
                    Id = Guid.NewGuid(),
                    UserId = command.UserId,
                    TotalSpace = command.TotalSpace,
                    IsStorageUnlimited = command.IsStorageUnlimited,
                    CreatedDate = Clock.Now,
                    ChangedDate = Clock.Now
                };

                await _personalSpaceRepository.InsertAsync(personalSpace);
            }

            return personalSpace;
        }

        protected async Task UpdatePersonalSpace(SavePersonalSpaceCommand command)
        {
            var existedPersonalSpace = _personalSpaceRepository
                .GetAll()
                .Where(PersonalSpaceExpressions.HasPermissionToSeePersonalSpace(command.UserId))
                .FirstOrDefault(x => x.Id == command.Id);

            if (existedPersonalSpace == null)
            {
                existedPersonalSpace = await CreateNewPersonalSpace(command);
            }

            existedPersonalSpace.ChangedDate = Clock.Now;
            existedPersonalSpace.TotalUsed = command.TotalUsed;
            existedPersonalSpace.TotalSpace = command.TotalSpace;
            existedPersonalSpace.IsStorageUnlimited = command.IsStorageUnlimited;

            await _personalSpaceRepository.UpdateAsync(existedPersonalSpace);
        }
    }
}
