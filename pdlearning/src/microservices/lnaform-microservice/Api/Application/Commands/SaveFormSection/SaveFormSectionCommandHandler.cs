using System;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.LnaForm.Domain.Entities;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.LnaForm.Application.Commands
{
    public class SaveFormSectionCommandHandler : BaseCommandHandler<SaveFormSectionCommand>
    {
        private readonly IRepository<FormSection> _formSectionRepository;

        public SaveFormSectionCommandHandler(
            IAccessControlContext accessControlContext,
            IRepository<FormSection> formSectionRepository,
            IUnitOfWorkManager unitOfWorkManager) : base(unitOfWorkManager, accessControlContext)
        {
            _formSectionRepository = formSectionRepository;
        }

        protected override async Task HandleAsync(SaveFormSectionCommand command, CancellationToken cancellationToken)
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

        private async Task Update(SaveFormSectionCommand command)
        {
            var existingFormSection = await _formSectionRepository.GetAsync(command.UpdateRequest.Id);
            if (existingFormSection is null)
            {
                throw new EntityNotFoundException($"{nameof(existingFormSection)} not found");
            }

            existingFormSection.MainDescription = command.UpdateRequest.MainDescription;
            existingFormSection.AdditionalDescription = command.UpdateRequest.AdditionalDescription;

            await _formSectionRepository.UpdateAsync(existingFormSection);
        }

        private async Task Create(SaveFormSectionCommand command)
        {
            var formSection = new FormSection
            {
                Id = command.CreationRequest.Id ?? Guid.NewGuid(),
                MainDescription = command.CreationRequest.MainDescription,
                AdditionalDescription = command.CreationRequest.AdditionalDescription,
            };
            await _formSectionRepository.InsertAsync(formSection);
        }
    }
}
