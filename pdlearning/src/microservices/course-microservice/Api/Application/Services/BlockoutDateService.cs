using System;
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
    public class BlockoutDateService : BaseApplicationService
    {
        public BlockoutDateService(IThunderCqrs thunderCqrs, IUnitOfWorkManager unitOfWorkManager) : base(thunderCqrs, unitOfWorkManager)
        {
        }

        public Task<BlockoutDateModel> GetBlockoutDateById(Guid blockoutDateId)
        {
            return ThunderCqrs.SendQuery(new GetBlockoutDateByIdQuery { Id = blockoutDateId });
        }

        public Task<PagedResultDto<BlockoutDateModel>> SearchBlockoutDates(SearchBlockoutDatesRequest request)
        {
            return ThunderCqrs.SendQuery(
                new SearchBlockoutDatesQuery
                {
                    SearchText = request.SearchText,
                    PageInfo = new PagedResultRequestDto()
                    {
                        SkipCount = request.SkipCount,
                        MaxResultCount = request.MaxResultCount
                    },
                    CoursePlanningCycleId = request.CoursePlanningCycleId,
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

        public Task<GetBlockoutDateDependenciesModel> GetBlockoutDateDependencies(GetBlockoutDateDependenciesRequest request)
        {
            return ThunderCqrs.SendQuery(request.ToQuery());
        }

        public async Task<BlockoutDateModel> SaveBlockoutDate(SaveBlockoutDateRequest request)
        {
            var command = request.Data.ToSaveBlockoutDateCommand();
            await ThunderCqrs.SendCommand(command);
            return await ThunderCqrs.SendQuery(new GetBlockoutDateByIdQuery { Id = command.Id });
        }

        public Task ConfirmBlockoutDate(ConfirmBlockoutDateRequest request)
        {
            return ThunderCqrs.SendCommand(new ConfirmBlockoutDateCommand
            {
                CoursePlanningCycleId = request.CoursePlanningCycleId
            });
        }

        public Task DeleteBlockoutDate(Guid blockoutDateId)
        {
            return ThunderCqrs.SendCommand(new DeleteBlockoutDateCommand { BlockoutDateId = blockoutDateId });
        }
    }
}
