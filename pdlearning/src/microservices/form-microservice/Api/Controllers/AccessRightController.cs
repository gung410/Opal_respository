using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microservice.Form.Application.Commands;
using Microservice.Form.Application.Models;
using Microservice.Form.Application.Queries;
using Microservice.Form.Application.RequestDtos;
using Microsoft.AspNetCore.Mvc;
using Thunder.Platform.AspNetCore.Controllers;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Cqrs;

namespace Microservice.Form.Controllers
{
    [Route("api/form/collaboration")]
    public class AccessRightController : ApplicationApiController
    {
        private readonly IThunderCqrs _thunderCqrs;

        public AccessRightController(IThunderCqrs thunderCqrs, IUserContext userContext) : base(userContext)
        {
            _thunderCqrs = thunderCqrs;
        }

        [HttpPost("create")]
        public async Task CreateAccessRight([FromBody] CreateAccessRightRequest request)
        {
            var saveCommand = new CreateAccessRightCommand
            {
                CreationRequest = request,
                UserId = CurrentUserId
            };

            await this._thunderCqrs.SendCommand(saveCommand);
        }

        [HttpPost("search")]
        public async Task<PagedResultDto<AccessRightModel>> SearchAccessRight([FromBody] SearchAccessRightRequest request)
        {
            var searchQuery = new SearchAccessRightQuery
            {
                Request = request
            };

            return await this._thunderCqrs.SendQuery(searchQuery);
        }

        [HttpPost("getAllIds")]
        public async Task<List<Guid>> GetAllCollaboratorsId([FromBody] GetAllAccessRightIdsRequest request)
        {
            var getQuery = new GetAllCollaboratorsIdQuery
            {
                Request = request
            };

            return await this._thunderCqrs.SendQuery(getQuery);
        }

        [HttpDelete("{accessRightId:guid}")]
        public async Task DeleteAccessRight(Guid accessRightId)
        {
            var deleteCommand = new DeleteAccessRightCommand
            {
                Id = accessRightId
            };

            await this._thunderCqrs.SendCommand(deleteCommand);
        }
    }
}
