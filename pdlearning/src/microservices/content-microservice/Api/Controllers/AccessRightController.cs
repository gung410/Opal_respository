using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microservice.Content.Application.Commands;
using Microservice.Content.Application.Models;
using Microservice.Content.Application.Queries;
using Microservice.Content.Application.RequestDtos;
using Microsoft.AspNetCore.Mvc;
using Thunder.Platform.AspNetCore.Controllers;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Cqrs;

namespace Microservice.Content.Controllers
{
    [Route("api/content/collaboration")]
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
            var command = new CreateAccessRightCommand(
                id: request.Id,
                originalObjectId: request.OriginalObjectId,
                userId: CurrentUserId,
                userIds: request.UserIds);

            await this._thunderCqrs.SendCommand(command);
        }

        [HttpPost("search")]
        public async Task<PagedResultDto<AccessRightModel>> SearchAccessRight([FromBody] SearchAccessRightRequest request)
        {
            var query = new SearchAccessRightQuery(
                originalObjectId: request.OriginalObjectId,
                pagedInfo: request.PagedInfo);

            return await this._thunderCqrs.SendQuery(query);
        }

        [HttpPost("getAllIds")]
        public async Task<IEnumerable<Guid>> GetAllCollaboratorsId([FromBody] GetAllCollaboratorIdsRequest request)
        {
            var query = new GetAllCollaboratorsIdQuery(originalObjectId: request.OriginalObjectId);

            return await this._thunderCqrs.SendQuery(query);
        }

        [HttpDelete("{accessRightId:guid}")]
        public async Task DeleteAccessRight(Guid accessRightId)
        {
            var command = new DeleteAccessRightCommand(accessRightId: accessRightId);

            await this._thunderCqrs.SendCommand(command);
        }
    }
}
