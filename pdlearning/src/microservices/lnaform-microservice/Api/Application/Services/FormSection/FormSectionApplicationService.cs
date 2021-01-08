using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microservice.LnaForm.Application.Commands;
using Microservice.LnaForm.Application.Models;
using Microservice.LnaForm.Application.Queries;
using Thunder.Platform.Cqrs;

namespace Microservice.LnaForm.Application.Services
{
    public class FormSectionApplicationService : BaseApplicationService
    {
        public FormSectionApplicationService(
            IThunderCqrs thunderCqrs) : base(thunderCqrs)
        {
        }

        public async Task<FormSectionModel> CreateFormSection(CreateFormSectionRequestDto request, Guid userId)
        {
            if (!request.Id.HasValue)
            {
                request.Id = Guid.NewGuid();
            }

            var saveCommand = new SaveFormSectionCommand
            {
                CreationRequest = request,
                UserId = userId,
                IsCreation = true
            };
            await this.ThunderCqrs.SendCommand(saveCommand);

            return await this.ThunderCqrs.SendQuery(new GetFormSectionByIdQuery { Id = request.Id.Value });
        }

        public Task<List<FormSectionModel>> GetFormSectionsByFormId(Guid formId)
        {
            return this.ThunderCqrs.SendQuery(new GetFormSectionsByFormIdQuery() { FormId = formId });
        }
    }
}
