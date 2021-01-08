using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microservice.Form.Application.Commands;
using Microservice.Form.Application.Models;
using Microservice.Form.Application.Queries;
using Microservice.Form.Application.RequestDtos;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Form.Application.Services
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
                CourseId = dto.CourceId,
                MyCourseId = dto.MyCourseId,
                ClassRunId = dto.ClassRunId,
                AssignmentId = dto.AssignmentId,
                FormAnswerId = newformAnswerId,
                UserId = userId
            };
            await ThunderCqrs.SendCommand(command);
            return await ThunderCqrs.SendQuery(new GetFormAnswerByIdQuery { FormAnswerId = newformAnswerId, UserId = userId });
        }

        public async Task<PagedResultDto<FormAnswerModel>> SearchFormAnswer(SearchFormAnswerRequestDto dto)
        {
            var query = new SearchFormAnswersQuery
            {
                SearchText = dto.SearchText,
                FormId = dto.FormId,
                CourseId = dto.CourseId,
                MyCourseId = dto.MyCourseId,
                ClassRunId = dto.ClassRunId,
                AssignmentId = dto.AssignmentId,
                BeforeDueDate = dto.BeforeDueDate,
                BeforeTimeLimit = dto.BeforeTimeLimit,
                IsCompleted = dto.IsCompleted,
                IsSubmitted = dto.IsSubmitted,
                PagedInfo = dto.PagedInfo,
                UserId = dto.UserId
            };
            return await ThunderCqrs.SendQuery(query);
        }

        public async Task<FormAnswerModel> UpdateFormAnswer(UpdateFormAnswerRequestDto dto, Guid userId)
        {
            await ThunderCqrs.SendCommand(new SaveFormAnswerCommand
            {
                IsCreation = false,
                FormAnswerId = dto.FormAnswerId,
                MyCourseId = dto.MyCourseId,
                UserId = userId,
                UpdateFormAnswerInfo = new SaveFormAnswerCommand.UpdateInfo
                {
                    IsSubmit = dto.IsSubmit,
                    QuestionAnswers = dto.QuestionAnswers?.Select(p => p.ToSaveFormAnswerCommandUpdateInfoQuestionAnswer())
                }
            });
            var result = await ThunderCqrs.SendQuery(new GetFormAnswerByIdQuery { FormAnswerId = dto.FormAnswerId, UserId = userId });
            return result;
        }

        public async Task<FormAnswerModel> UpdateFormAnswerScore(UpdateFormAnswerScoreRequestDto dto, Guid userId)
        {
            await ThunderCqrs.SendCommand(new SaveFormAnswerCommand
            {
                IsCreation = false,
                IsMarking = true,
                FormAnswerId = dto.FormAnswerId,
                MyCourseId = dto.MyCourseId,
                UserId = userId,
                UpdateFormAnswerInfo = new SaveFormAnswerCommand.UpdateInfo
                {
                    QuestionAnswers = dto.QuestionAnswers?.Select(p => p.ToSaveFormAnswerCommandUpdateInfoQuestionAnswer())
                }
            });
            var result = await ThunderCqrs.SendQuery(new GetFormAnswerByIdQuery { FormAnswerId = dto.FormAnswerId, UserId = userId });
            return result;
        }

        public async Task<IEnumerable<FormAnswerModel>> GetByFormId(Guid formId, Guid? resourceId, Guid? myCourseId, Guid? classRunId, Guid? assignmentId, Guid userId)
        {
            var searchFormAnswersQueryResult = await ThunderCqrs.SendQuery(new SearchFormAnswersQuery
            {
                FormId = formId,
                CourseId = resourceId,
                MyCourseId = myCourseId,
                ClassRunId = classRunId,
                AssignmentId = assignmentId,
                UserId = userId
            });
            return searchFormAnswersQueryResult.Items;
        }
    }
}
