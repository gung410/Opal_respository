using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.Microservice.Tagging.DataProviders;
using Conexus.Opal.Microservice.Tagging.Domain.Entities;
using Conexus.Opal.Microservice.Tagging.Dtos;
using Conexus.Opal.Microservice.Tagging.Events.ResourceSavedEvent;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Cqrs;
using Thunder.Service.Authentication;

namespace Conexus.Opal.Microservice.Tagging.Services
{
    public class ResourceService : BaseApplicationService
    {
        private readonly ITaggingDataProvider _dataProvider;
        private readonly IUserContext _userContext;

        public ResourceService(IThunderCqrs thunderCqrs, ITaggingDataProvider taggingDataProvider, IUserContext userContext) : base(thunderCqrs)
        {
            _dataProvider = taggingDataProvider;
            _userContext = userContext;
        }

        public async Task SaveResourceMetadata(Guid resourceId, ResourceType resourceType, SaveResourceMetadataRequest request, Guid userId)
        {
            var resource = await _dataProvider.GetResourceById(resourceId, userId);
            if (resource == null)
            {
                resource = new Resource
                {
                    ResourceId = resourceId,
                    ResourceType = resourceType,
                    CreatedBy = userId,
                    MainSubjectAreaTagId = request.MainSubjectAreaTagId,
                    PreRequisties = request.PreRequisties,
                    ObjectivesOutCome = request.ObjectivesOutCome,
                    Tags = BuildToSaveResourceTags(request),
                    DynamicMetaData = request.DynamicMetaData,
                    SearchTags = request.SearchTags
                };
            }
            else
            {
                resource.ResourceType = resourceType;
                resource.MainSubjectAreaTagId = request.MainSubjectAreaTagId;
                resource.PreRequisties = request.PreRequisties;
                resource.ObjectivesOutCome = request.ObjectivesOutCome;
                resource.Tags = BuildToSaveResourceTags(request);
                resource.DynamicMetaData = request.DynamicMetaData;
                resource.SearchTags = request.SearchTags;
            }

            _dataProvider.SaveResourceMetadata(resource, userId);
            await ThunderCqrs.SendEvent(new SaveResourceEvent(resource));
        }

        public async Task SendResourceToQueue(IEnumerable<ResourceType> resourceTypes)
        {
            var resources = await _dataProvider.GetResourceByResourceType(resourceTypes);

            await ThunderCqrs.SendEvents(resources.Select(p => new SaveResourceEvent(p)));
        }

        public async Task<Resource> CloneResource(Guid resourceId, Guid clonedFromResourceId, Guid userId)
        {
            var clonedResource = await _dataProvider.CloneResource(resourceId, clonedFromResourceId, userId);
            await ThunderCqrs.SendEvent(new SaveResourceEvent(clonedResource));
            return clonedResource;
        }

        public async Task MigrateTagsNotification(List<Guid> listIds, Guid userId)
        {
            if (!_userContext.IsSysAdministrator())
            {
                throw new EntityNotFoundException();
            }

            var getResourceTasks = listIds.Select(i => _dataProvider.GetResourceById(i, userId)).ToArray();

            var resources = await Task.WhenAll(getResourceTasks);
            await ThunderCqrs.SendEvents(resources.Where(p => p != null).Select(r => new SaveResourceEvent(r)));
        }

        private static IEnumerable<Guid> BuildToSaveResourceTags(SaveResourceMetadataRequest request)
        {
            return request.MainSubjectAreaTagId != null ? request.TagIds?.Concat(new List<Guid> { request.MainSubjectAreaTagId.Value }).Distinct() : request.TagIds;
        }
    }
}
