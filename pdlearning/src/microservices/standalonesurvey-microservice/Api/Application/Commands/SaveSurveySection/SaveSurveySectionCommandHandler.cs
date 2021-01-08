using System;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.StandaloneSurvey.Domain.Entities;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.StandaloneSurvey.Application.Commands
{
    public class SaveSurveySectionCommandHandler : BaseCommandHandler<SaveSurveySectionCommand>
    {
        private readonly IRepository<SurveySection> _formSectionRepository;

        public SaveSurveySectionCommandHandler(
            IAccessControlContext accessControlContext,
            IRepository<SurveySection> formSectionRepository,
            IUnitOfWorkManager unitOfWorkManager) : base(unitOfWorkManager, accessControlContext)
        {
            _formSectionRepository = formSectionRepository;
        }

        protected override async Task HandleAsync(SaveSurveySectionCommand command, CancellationToken cancellationToken)
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

        private async Task Update(SaveSurveySectionCommand command)
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

        private async Task Create(SaveSurveySectionCommand command)
        {
            var formSection = new SurveySection
            {
                Id = command.CreationRequest.Id ?? Guid.NewGuid(),
                MainDescription = command.CreationRequest.MainDescription,
                AdditionalDescription = command.CreationRequest.AdditionalDescription,
            };
            await _formSectionRepository.InsertAsync(formSection);
        }
    }
}
