using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microservice.StandaloneSurvey.Application.Commands;
using Microservice.StandaloneSurvey.Application.Models;
using Microservice.StandaloneSurvey.Application.Queries;
using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Application.Services
{
    public class SurveySectionApplicationService : BaseApplicationService
    {
        public SurveySectionApplicationService(
            IThunderCqrs thunderCqrs) : base(thunderCqrs)
        {
        }

        public async Task<SurveySectionModel> CreateSurveySection(CreateSurveySectionRequestDto request, Guid userId)
        {
            if (!request.Id.HasValue)
            {
                request.Id = Guid.NewGuid();
            }

            var saveCommand = new SaveSurveySectionCommand
            {
                CreationRequest = request,
                UserId = userId,
                IsCreation = true
            };
            await this.ThunderCqrs.SendCommand(saveCommand);

            return await this.ThunderCqrs.SendQuery(new GetFormSectionByIdQuery { Id = request.Id.Value });
        }

        public Task<List<SurveySectionModel>> GetFormSectionsBySurveyId(Guid formId)
        {
            return this.ThunderCqrs.SendQuery(new GetFormSectionsByFormIdQuery() { FormId = formId });
        }
    }
}
