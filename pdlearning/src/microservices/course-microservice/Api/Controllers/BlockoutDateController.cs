using System;
using System.Threading.Tasks;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.RequestDtos;
using Microservice.Course.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;

namespace Microservice.Course.Controllers
{
    [Route("api/blockoutDate")]
    public class BlockoutDateController : BaseController<BlockoutDateService>
    {
        public BlockoutDateController(IUserContext userContext, BlockoutDateService appService) : base(userContext, appService)
        {
        }

        [HttpPost("getBlockoutDateDependencies")]
        public async Task<GetBlockoutDateDependenciesModel> GetBlockoutDateDependencies([FromBody] GetBlockoutDateDependenciesRequest request)
        {
            return await AppService.GetBlockoutDateDependencies(request);
        }

        [HttpPost("search")]
        public async Task<PagedResultDto<BlockoutDateModel>> SearchBlockoutDates([FromBody] SearchBlockoutDatesRequest request)
        {
            return await AppService.SearchBlockoutDates(request);
        }

        [HttpPost("save")]
        public async Task<BlockoutDateModel> SaveBlockoutDate([FromBody] SaveBlockoutDateRequest request)
        {
            return await AppService.SaveBlockoutDate(request);
        }

        [HttpGet("{blockoutDateId:guid}")]
        public async Task<BlockoutDateModel> GetBlockoutDateById(Guid blockoutDateId)
        {
            return await AppService.GetBlockoutDateById(blockoutDateId);
        }

        [HttpPut("confirmBlockoutDate")]
        public async Task ConfirmBlockoutDate([FromBody] ConfirmBlockoutDateRequest request)
        {
            await AppService.ConfirmBlockoutDate(request);
        }

        [HttpDelete("{blockoutDateId:guid}")]
        public async Task DeleteBlockoutDate(Guid blockoutDateId)
        {
            await AppService.DeleteBlockoutDate(blockoutDateId);
        }
    }
}
