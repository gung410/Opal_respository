using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microservice.Course.Application.Commands;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.Queries;
using Microservice.Course.Application.RequestDtos;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Services
{
    public class AssessmentAnswerService : BaseApplicationService
    {
        public AssessmentAnswerService(IThunderCqrs thunderCqrs, IUnitOfWorkManager unitOfWork) : base(thunderCqrs, unitOfWork)
        {
        }

        public Task<AssessmentAnswerModel> GetAssessmentAnswerByIdOrUser(GetAssessmentAnswerByIdOrUserRequest request)
        {
            return ThunderCqrs.SendQuery(new GetAssessmentAnswerByIdOrUserQuery
            {
                Id = request.Id,
                ParticipantAssignmentTrackId = request.ParticipantAssignmentTrackId,
                UserId = request.UserId
            });
        }

        public async Task<AssessmentAnswerModel> SaveAssessmentAnswer(SaveAssessmentAnswerRequest request)
        {
            await ThunderCqrs.SendCommand(new SaveAssessmentAnswerCommand
            {
                Id = request.Id,
                CriteriaAnswers = request.CriteriaAnswers,
                IsSubmit = request.IsSubmit
            });

            return await ThunderCqrs.SendQuery(new GetAssessmentAnswerByIdOrUserQuery
            {
                Id = request.Id
            });
        }

        public Task<PagedResultDto<AssessmentAnswerModel>> SearchAssessmentAnswer(SearchAssessmentAnswerRequest request)
        {
            return ThunderCqrs.SendQuery(new SearchAssessmentAnswerQuery
            {
                ParticipantAssignmentTrackId = request.ParticipantAssignmentTrackId,
                UserId = request.UserId,
                SearchText = request.SearchText,
                PageInfo = new PagedResultRequestDto
                {
                    SkipCount = request.SkipCount,
                    MaxResultCount = request.MaxResultCount
                },
                Filter = request.Filter != null
                ? new CommonFilter
                {
                    ContainFilters = request.Filter.ContainFilters?
                    .Select(p => new ContainFilter { Field = p.Field, Values = p.Values, NotContain = p.NotContain })
                    .ToList(),
                    FromToFilters = request.Filter.FromToFilters?
                    .Select(p => new FromToFilter
                    {
                        Field = p.Field,
                        FromValue = p.FromValue,
                        ToValue = p.ToValue,
                        EqualFrom = p.EqualFrom,
                        EqualTo = p.EqualTo
                    })
                    .ToList()
                }
                : null
            });
        }

        public async Task<AssessmentAnswerModel> CreateAssessmentAnswer(CreateAssessmentAnswerRequest request)
        {
            var command = request.Data.ToCommand();
            await ThunderCqrs.SendCommand(command);
            return await ThunderCqrs.SendQuery(new GetAssessmentAnswerByIdOrUserQuery { Id = command.Id });
        }

        public async Task DeleteAssessmentAnswer(Guid id)
        {
            await this.ThunderCqrs.SendCommand(new DeleteAssessmentAnswerCommand { Id = id });
        }

        public Task<IEnumerable<NoOfAssessmentDoneInfoModel>> GetNoOfAssessmentDones(GetNoOfAssessmentDonesRequest request)
        {
            return ThunderCqrs.SendQuery(new GetNoOfAssessmentDonesQuery
            {
                ParticipantAssignmentTrackIds = request.ParticipantAssignmentTrackIds
            });
        }
    }
}
