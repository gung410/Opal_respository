using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Conexus.Opal.Microservice.Metadata.DataProviders;
using Conexus.Opal.Microservice.Metadata.Entities;
using Conexus.Opal.Microservice.Tagging.Domain.Entities;
using Conexus.Opal.Microservice.Tagging.Dtos;
using Thunder.Platform.Core.Application.Dtos;

namespace Conexus.Opal.Microservice.Tagging.DataProviders
{
    public interface ITaggingDataProvider : IMetadataDataProvider
    {
        void SaveResourceMetadata(Resource resource, Guid userId);

        Task<PagedResultDto<SearchTag>> QuerySearchTags(QuerySearchTagRequest request);

        Task<IEnumerable<SearchTag>> GetSearchTagByNames(IEnumerable<string> searchTagNames);

        Task<GetResourceWithMetadataResult> GetResourceWidthMetadata(Guid resourceId, Guid userId);

        Task<IEnumerable<MetadataTag>> GetAllMetadataTags();

        Task<IEnumerable<MetadataTag>> GetMetadataTagsByIds(IEnumerable<Guid> tagIds);

        Task<IEnumerable<Guid>> GetSuggestedResourcesByTags(IEnumerable<Guid> tagIds, ResourceType resourceType);

        Task<Resource> GetResourceById(Guid resourceId, Guid userId);

        Task<List<Resource>> GetResourceByResourceType(IEnumerable<ResourceType> resourceTypes);

        Task<Resource> CloneResource(Guid resourceId, Guid clonedFromResourceId, Guid userId);

        Task<GetResourceMetadataListResult> GetResourceMetadataList(IEnumerable<Guid> resourceIds);
    }
}
