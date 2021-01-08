using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microservice.StandaloneSurvey.Application.Commands.SaveFormAnswer;
using Microservice.StandaloneSurvey.Application.Models;
using Microservice.StandaloneSurvey.Application.Queries;
using Microservice.StandaloneSurvey.Application.RequestDtos;
using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Application.Services
{
    public class SurveyAnswerApplicationService : BaseApplicationService
    {
        public SurveyAnswerApplicationService(IThunderCqrs thunderCqrs) : base(thunderCqrs)
        {
        }

        public async Task<SurveyAnswerModel> SaveFormAnswer(SaveSurveyAnswerRequestDto dto, Guid userId)
        {
            var newformAnswerId = Guid.NewGuid();
            var command = new SaveSurveyAnswerCommand
            {
                IsCreation = true,
                SurveyId = dto.FormId,
                ResourceId = dto.ResourceId,
                SurveyAnswerId = newformAnswerId,
                UserId = userId
            };
            await ThunderCqrs.SendCommand(command);
            return await ThunderCqrs.SendQuery(new GetSurveyAnswerByIdQuery { SurveyAnswerId = newformAnswerId, UserId = userId });
        }

        public async Task<SurveyAnswerModel> UpdateFormAnswer(UpdateSurveyAnswerRequestDto dto, Guid userId)
        {
            await ThunderCqrs.SendCommand(new SaveSurveyAnswerCommand
            {
                IsCreation = false,
                SurveyAnswerId = dto.FormAnswerId,
                UserId = userId,
                UpdateFormAnswerInfo = new UpdateInfo
                {
                    IsSubmit = dto.IsSubmit,
                    QuestionAnswers = dto.QuestionAnswers?.Select(p => p.ToSaveFormAnswerCommandUpdateInfoQuestionAnswer())
                }
            });
            var result = await ThunderCqrs.SendQuery(new GetSurveyAnswerByIdQuery { SurveyAnswerId = dto.FormAnswerId, UserId = userId });
            return result;
        }

        public async Task<IEnumerable<SurveyAnswerModel>> GetBySurveyId(Guid formId, Guid? resourceId, Guid userId)
        {
            var searchFormAnswersQueryResult = await ThunderCqrs.SendQuery(new SearchSurveyAnswersQuery
            {
                SurveyId = formId,
                ResourceId = resourceId,
                UserId = userId
            });
            return searchFormAnswersQueryResult.Items;
        }
    }
}
