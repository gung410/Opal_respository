using System;
using System.Collections.Generic;
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
    public class AssignmentService : BaseApplicationService
    {
        public AssignmentService(IThunderCqrs thunderCqrs, IUnitOfWorkManager unitOfWork) : base(thunderCqrs, unitOfWork)
        {
        }

        public async Task<AssignmentModel> SaveAssignment(SaveAssignmentRequest request)
        {
            var createOrUpdateAssignmentCommand = request.Data.ToCreateOrUpdateAssignmentCommand();
            await ThunderCqrs.SendCommand(createOrUpdateAssignmentCommand);

            var extractContentUrlCommand = new ExtractContentUrlCommand { AssignmentId = createOrUpdateAssignmentCommand.Id };
            await ThunderCqrs.SendCommand(extractContentUrlCommand);

            return await ThunderCqrs.SendQuery(new GetAssignmentByIdQuery { Id = createOrUpdateAssignmentCommand.Id, ForLearnerAnswer = false });
        }

        public Task<AssignmentModel> GetAssignmentById(GetAssignmentByIdRequest request)
        {
            return ThunderCqrs.SendQuery(new GetAssignmentByIdQuery { Id = request.Id, ForLearnerAnswer = request.ForLearnerAnswer });
        }

        public Task DeleteAssignment(Guid assignmentId)
        {
            return ThunderCqrs.SendCommand(new DeleteAssignmentCommand
            {
                Id = assignmentId
            });
        }

        public Task<IEnumerable<AssignmentModel>> GetAssignmentByIds(GetAssignmentsByIdsRequest request)
        {
            return ThunderCqrs.SendQuery(new GetAssignmentByIdsQuery
            {
                Ids = request.Ids,
                IncludeQuizForm = request.IncludeQuizForm
            });
        }

        public Task<PagedResultDto<AssignmentModel>> GetAssignments(GetAssignmentsRequest request)
        {
            return ThunderCqrs.SendQuery(new GetAssignmentsQuery
            {
                CourseId = request.CourseId,
                ClassRunId = request.ClassRunId,
                FilterType = request.FilterType,
                PageInfo = new PagedResultRequestDto
                {
                    MaxResultCount = request.MaxResultCount,
                    SkipCount = request.SkipCount
                },
                IncludeQuizForm = request.IncludeQuizForm
            });
        }

        public Task<IEnumerable<NoOfAssignmentDoneInfoModel>> GetNoOfAssignmentDones(GetNoOfAssignmentDonesRequest request)
        {
            return ThunderCqrs.SendQuery(new GetNoOfAssignmentDonesQuery
            {
               RegistrationIds = request.RegistrationIds,
               ClassRunId = request.ClassRunId
            });
        }

        public async Task SetupPeerAssessment(SetupPeerAssessmentRequest request)
        {
            await ThunderCqrs.SendCommand(new SetupPeerAssessmentCommand
            {
                AssignmentId = request.AssignmentId,
                ClassrunId = request.ClassrunId,
                NumberAutoAssessor = request.NumberAutoAssessor
            });
        }
    }
}
