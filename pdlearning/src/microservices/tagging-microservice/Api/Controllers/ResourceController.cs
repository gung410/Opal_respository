using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Conexus.Opal.Microservice.Tagging.DataProviders;
using Conexus.Opal.Microservice.Tagging.Domain.Entities;
using Conexus.Opal.Microservice.Tagging.Dtos;
using Conexus.Opal.Microservice.Tagging.Services;
using Microsoft.AspNetCore.Mvc;
using Thunder.Platform.AspNetCore.Controllers;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;

namespace Conexus.Opal.Microservice.Tagging.Controllers
{
    [Route("api")]
    public class ResourceController : ApplicationApiController
    {
        private readonly ITaggingDataProvider dataProvider;
        private readonly ResourceService resourceService;

        public ResourceController(IUserContext userContext, ITaggingDataProvider taggingDataProvider, ResourceService resourceService) : base(userContext)
        {
            dataProvider = taggingDataProvider;
            this.resourceService = resourceService;
        }

        [HttpGet("resource/{resourceId:guid}")]
        public async Task<Resource> GetResourceById(Guid resourceId)
        {
            return await dataProvider.GetResourceById(resourceId, CurrentUserId);
        }

        [HttpPost("resource/{resourceId:guid}/cloned-from/{clonedFromResourceId:guid}")]
        public async Task<Resource> CloneResource(Guid resourceId, Guid clonedFromResourceId)
        {
            return await resourceService.CloneResource(resourceId, clonedFromResourceId, CurrentUserId);
        }

        [HttpPost("resource/resourceMetadataList")]
        public async Task<GetResourceMetadataListResult> GetResourceMetadataList([FromBody] IEnumerable<Guid> resourceIds)
        {
            return await dataProvider.GetResourceMetadataList(resourceIds);
        }

        [HttpPost("search-tag/search")]
        public async Task<PagedResultDto<SearchTag>> QuerySearchTag([FromBody] QuerySearchTagRequest request)
        {
            return await dataProvider.QuerySearchTags(request);
        }

        [HttpPost("search-tag/get-by-names")]
        public async Task<IEnumerable<SearchTag>> GetSearchTagByNames([FromBody] IEnumerable<string> searchTagNames)
        {
            return await dataProvider.GetSearchTagByNames(searchTagNames);
        }
    }
}
