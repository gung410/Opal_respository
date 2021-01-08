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
    public class LearningPathService : BaseApplicationService
    {
        public LearningPathService(IThunderCqrs thunderCqrs, IUnitOfWorkManager unitOfWork) : base(thunderCqrs, unitOfWork)
        {
        }

        public Task<PagedResultDto<LearningPathModel>> SearchLearningPaths(SearchLearningPathRequest request)
        {
            return ThunderCqrs.SendQuery(new SearchLearningPathsQuery
            {
                SearchText = request.SearchText,
                PageInfo = new PagedResultRequestDto()
                {
                    SkipCount = request.SkipCount,
                    MaxResultCount = request.MaxResultCount
                }
            });
        }

        public Task<LearningPathModel> GetLearningPathById(Guid learningPathId)
        {
            return ThunderCqrs.SendQuery(new GetLearningPathByIdQuery
            {
                Id = learningPathId
            });
        }

        public Task<List<LearningPathModel>> GetLearningPathByIds(Guid[] learningPathIds)
        {
            return ThunderCqrs.SendQuery(new GetLearningPathByIdsQuery
            {
                Ids = learningPathIds
            });
        }

        public async Task<LearningPathModel> SaveLearningPath(SaveLearningPathRequest request)
        {
            var command = request.Data.ToCommand();
            await ThunderCqrs.SendCommand(command);

            return await ThunderCqrs.SendQuery(new GetLearningPathByIdQuery { Id = command.Id });
        }
    }
}
