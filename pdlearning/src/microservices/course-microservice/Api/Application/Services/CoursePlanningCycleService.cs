using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.Queries;
using Microservice.Course.Application.RequestDtos;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Services
{
    public class CoursePlanningCycleService : BaseApplicationService
    {
        public CoursePlanningCycleService(IThunderCqrs thunderCqrs, IUnitOfWorkManager unitOfWork) : base(thunderCqrs, unitOfWork)
        {
        }

        public Task<CoursePlanningCycleModel> GetCoursePlanningCycleById(Guid coursePlanningCycleId)
        {
            return ThunderCqrs.SendQuery(new GetCoursePlanningCycleByIdQuery { Id = coursePlanningCycleId });
        }

        public async Task<CoursePlanningCycleModel> SaveCoursePlanningCycle(SaveCoursePlanningCycleRequest request)
        {
            var command = request.Data.ToCommand();
            await ThunderCqrs.SendCommand(command);
            return await ThunderCqrs.SendQuery(new GetCoursePlanningCycleByIdQuery { Id = command.Id });
        }

        public Task<PagedResultDto<CoursePlanningCycleModel>> SearchCoursePlanningCycles(SearchCoursesPlanningCyclesRequest request)
        {
            return ThunderCqrs.SendQuery(
                new SearchCoursePlanningCyclesQuery
                {
                    SearchText = request.SearchText,
                    PageInfo = new PagedResultRequestDto()
                    {
                        SkipCount = request.SkipCount,
                        MaxResultCount = request.MaxResultCount
                    }
                });
        }

        public Task<List<CoursePlanningCycleModel>> GetCoursePlanningCyclesByIds(
            List<Guid> coursePlanningCycleIds)
        {
            return ThunderCqrs.SendQuery(new GetCoursePlanningCyclesByIdsQuery
            {
                Ids = coursePlanningCycleIds
            });
        }
    }
}
