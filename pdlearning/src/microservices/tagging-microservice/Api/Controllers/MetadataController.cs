using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.Microservice.Metadata.Entities;
using Conexus.Opal.Microservice.Tagging.Cache;
using Conexus.Opal.Microservice.Tagging.DataProviders;
using Conexus.Opal.Microservice.Tagging.Domain.Entities;
using Conexus.Opal.Microservice.Tagging.Dtos;
using Conexus.Opal.Microservice.Tagging.Services;
using Microsoft.AspNetCore.Mvc;
using Thunder.Platform.AspNetCore.Controllers;
using Thunder.Platform.Caching;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Exceptions;
using Thunder.Service.Authentication;

namespace Conexus.Opal.Microservice.Tagging.Controllers
{
    [Route("api")]
    public class MetadataController : ApplicationApiController
    {
        private readonly ICacheDataProvider _cacheDataProvider;
        private readonly ITaggingDataProvider _dataProvider;
        private readonly ResourceService _resourceService;

        public MetadataController(
            ICacheDataProvider cacheDataProvider,
            ITaggingDataProvider taggingDataProvider,
            IUserContext userContext,
            ResourceService resourceService) : base(userContext)
        {
            _cacheDataProvider = cacheDataProvider;
            _dataProvider = taggingDataProvider;
            _resourceService = resourceService;
        }

        [HttpGet("metadataTag")]
        public async Task<IEnumerable<MetadataTag>> GetAllMetadataTags()
        {
            var cacheKey = MetadataCacheKey.ForAll();
            var dataFromCache = _cacheDataProvider.GetCachedData<IEnumerable<MetadataTag>>(cacheKey);
            if (dataFromCache != null)
            {
                return dataFromCache;
            }

            var allMetadataTags = (await _dataProvider.GetAllMetadataTags()).ToList();

            // Should we move FromDays(1) to the config file?
            _cacheDataProvider.AddCachedData(cacheKey, allMetadataTags, TimeSpan.FromDays(1));

            return allMetadataTags;
        }

        [HttpPost("metadataTagByIds")]
        public async Task<IEnumerable<MetadataTag>> GetMetadataTagsByIds([FromBody] IEnumerable<Guid> tagIds)
        {
            return await _dataProvider.GetMetadataTagsByIds(tagIds);
        }

        [HttpGet("courses/{courseId}/metadata")]
        public async Task<GetResourceWithMetadataResult> GetCourseMetaData(Guid courseId)
        {
            return await _dataProvider.GetResourceWidthMetadata(courseId, CurrentUserId);
        }

        [HttpPost("courses/{courseId}/metadata")]
        public async Task SaveCourseMetaData(Guid courseId, [FromBody] SaveResourceMetadataRequest request)
        {
            await _resourceService.SaveResourceMetadata(
               courseId,
               ResourceType.Course,
               request,
               CurrentUserId);
        }

        [HttpPost("learningPaths/{learningPathId}/metadata")]
        public async Task SaveLearningPathMetaData(Guid learningPathId, [FromBody] SaveResourceMetadataRequest request)
        {
            await _resourceService.SaveResourceMetadata(
               learningPathId,
               ResourceType.LearningPath,
               request,
               CurrentUserId);
        }

        [HttpGet("contents/{contentId}/metadata")]
        public async Task<GetResourceWithMetadataResult> GetContentMetaData(Guid contentId)
        {
            return await _dataProvider.GetResourceWidthMetadata(contentId, CurrentUserId);
        }

        [HttpPost("contents/{contentId}/metadata")]
        public async Task SaveContentMetaData(Guid contentId, [FromBody] SaveResourceMetadataRequest request)
        {
            await _resourceService.SaveResourceMetadata(
               contentId,
               ResourceType.Content,
               request,
               CurrentUserId);
        }

        [HttpPost("forms/{formId}/metadata")]
        public async Task SaveFormMetaData(Guid formId, [FromBody] SaveResourceMetadataRequest request)
        {
            await _resourceService.SaveResourceMetadata(
               formId,
               ResourceType.Form,
               request,
               CurrentUserId);
        }

        [HttpGet("community/{communityId}/metadata")]
        public async Task<GetResourceWithMetadataResult> GetCommunityMetaData(Guid communityId)
        {
            return await _dataProvider.GetResourceWidthMetadata(communityId, CurrentUserId);
        }

        [HttpPost("community/{communityId}/metadata")]
        public async Task SaveCommunityMetaData(Guid communityId, [FromBody] SaveResourceMetadataRequest request)
        {
            await _resourceService.SaveResourceMetadata(
               communityId,
               ResourceType.Community,
               request,
               CurrentUserId);
        }

        [HttpPost("send-resource-to-queue")]
        public async Task SendResourceToQueue([FromBody] IEnumerable<ResourceType> resourceTypes)
        {
            if (!UserContext.IsSysAdministrator())
            {
                throw new BusinessLogicException("You do not have permission to access this endpoint");
            }

            await _resourceService.SendResourceToQueue(resourceTypes);
        }
    }
}
