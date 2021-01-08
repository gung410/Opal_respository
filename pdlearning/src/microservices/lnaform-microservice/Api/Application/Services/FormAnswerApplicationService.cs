using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microservice.LnaForm.Application.Commands.SaveFormAnswer;
using Microservice.LnaForm.Application.Models;
using Microservice.LnaForm.Application.Queries;
using Microservice.LnaForm.Application.RequestDtos;
using Thunder.Platform.Cqrs;

namespace Microservice.LnaForm.Application.Services
{
    public class FormAnswerApplicationService : BaseApplicationService
    {
        public FormAnswerApplicationService(IThunderCqrs thunderCqrs) : base(thunderCqrs)
        {
        }

        public async Task<FormAnswerModel> SaveFormAnswer(SaveFormAnswerRequestDto dto, Guid userId)
        {
            var newformAnswerId = Guid.NewGuid();
            var command = new SaveFormAnswerCommand
            {
                IsCreation = true,
                FormId = dto.FormId,
                ResourceId = dto.ResourceId,
                FormAnswerId = newformAnswerId,
                UserId = userId
            };
            await ThunderCqrs.SendCommand(command);
            return await ThunderCqrs.SendQuery(new GetFormAnswerByIdQuery { FormAnswerId = newformAnswerId, UserId = userId });
        }

        public async Task<FormAnswerModel> UpdateFormAnswer(UpdateFormAnswerRequestDto dto, Guid userId)
        {
            await ThunderCqrs.SendCommand(new SaveFormAnswerCommand
            {
                IsCreation = false,
                FormAnswerId = dto.FormAnswerId,
                UserId = userId,
                UpdateFormAnswerInfo = new UpdateInfo
                {
                    IsSubmit = dto.IsSubmit,
                    QuestionAnswers = dto.QuestionAnswers?.Select(p => p.ToSaveFormAnswerCommandUpdateInfoQuestionAnswer())
                }
            });
            var result = await ThunderCqrs.SendQuery(new GetFormAnswerByIdQuery { FormAnswerId = dto.FormAnswerId, UserId = userId });
            return result;
        }

        public async Task<IEnumerable<FormAnswerModel>> GetByFormId(Guid formId, Guid? resourceId, Guid userId)
        {
            var searchFormAnswersQueryResult = await ThunderCqrs.SendQuery(new SearchFormAnswersQuery
            {
                FormId = formId,
                ResourceId = resourceId,
                UserId = userId
            });
            return searchFormAnswersQueryResult.Items;
        }
    }
}
