using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microservice.Form.Application.Commands;
using Microservice.Form.Application.Models;
using Microservice.Form.Application.Queries;
using Thunder.Platform.Cqrs;

namespace Microservice.Form.Application.Services
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
