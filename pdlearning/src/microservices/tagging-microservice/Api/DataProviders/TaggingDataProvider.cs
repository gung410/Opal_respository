using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Conexus.Opal.Microservice.Metadata.Common;
using Conexus.Opal.Microservice.Metadata.Constants;
using Conexus.Opal.Microservice.Metadata.DataProviders;
using Conexus.Opal.Microservice.Metadata.Entities;
using Conexus.Opal.Microservice.Tagging.Domain.Entities;
using Conexus.Opal.Microservice.Tagging.Dtos;
using Conexus.Opal.Microservice.Tagging.Infrastructure;
using Conexus.Opal.Microservice.Tagging.Models;
using Microsoft.Extensions.Logging;
using Thunder.Platform.Core.Application.Dtos;

namespace Conexus.Opal.Microservice.Tagging.DataProviders
{
    public class ResourceMetadataTagModel : MetadataTag
    {
        public Guid ResourceId { get; set; }
    }

    public class ResourceSearchTagModel : SearchTag
    {
        public Guid ResourceId { get; set; }
    }

    public class ResourceDynamicMetadataItem
    {
        public Guid ResourceId { get; set; }

        public string Key { get; set; }

        public string JsonValue { get; set; }
    }

    public class TaggingDataProvider : BaseMetadataDataProvider, ITaggingDataProvider
    {
        public TaggingDataProvider(TaggingDbContext taggingDbContext, ILoggerFactory loggerFactory) : base(taggingDbContext, loggerFactory)
        {
        }

        public async Task<GetResourceWithMetadataResult> GetResourceWidthMetadata(Guid resourceId, Guid userId)
        {
            var resource = await GetResourceById(resourceId, userId);
            if (resource == null)
            {
                return null;
            }

            var metadataTagsReader = GetDataReader("uspGetMetadataTagsRecursiveByTagIds", new List<DbParameter>
                {
                    GetParameter("TagIds", SqlXmlDataTypeHelper.SerializeToXml(resource.Tags.Select(p => p.ToString()).ToArray()))
                });
            var metadataTags = SqlDataReaderHelper.CreateList<MetadataTag>(metadataTagsReader);

            return await Task.FromResult(new GetResourceWithMetadataResult
            {
                MetadataTags = metadataTags,
                Resource = resource
            });
        }

        public void SaveResourceMetadata(Resource resource, Guid userId)
        {
            var dynamicMetaDataItems = resource.DynamicMetaData != null
                ? resource.DynamicMetaData.Select(p => new ResourceDynamicMetadataItem
                {
                    ResourceId = resource.ResourceId,
                    Key = p.Key,
                    JsonValue = p.Value != null ? JsonSerializer.Serialize(p.Value) : null
                })
                : new List<ResourceDynamicMetadataItem>();

            ExecuteNonQuery(
                "uspSaveResourceMetadata",
                new List<DbParameter>
                {
                    GetParameter("ResourceId", resource.ResourceId),
                    GetParameter("ResourceType", resource.ResourceType.ToString()),
                    GetParameter("TagIds", SqlXmlDataTypeHelper.SerializeToXml(resource.Tags.Distinct().Select(p => p.ToString()).ToArray())),
                    GetParameter("DynamicMetaDataItemsJson", JsonSerializer.Serialize(dynamicMetaDataItems)),
                    GetParameter("MainSubjectAreaTagId", resource.MainSubjectAreaTagId.HasValue ? resource.MainSubjectAreaTagId.ToString() : null),
                    GetParameter("PreRequisties", resource.PreRequisties),
                    GetParameter("ObjectivesOutCome", resource.ObjectivesOutCome),
                    GetParameter("UserId", userId)
                });
            ExecuteNonQuery(
                "uspSaveSearchTag",
                new List<DbParameter>
                {
                    GetParameter("ResourceId", resource.ResourceId),
                    GetParameter("SearchTagNames", SqlXmlDataTypeHelper.SerializeToXml(resource.SearchTags != null ? resource.SearchTags.ToArray() : null))
                });
        }

        public async Task<IEnumerable<SearchTag>> GetSearchTagByNames(IEnumerable<string> searchTagNames)
        {
            var searchTagDataReader = ExecuteSqlReader(BuildGetSearchTagByNames(searchTagNames));
            var searchTag = SqlDataReaderHelper.CreateList<SearchTag>(searchTagDataReader);
            return await Task.FromResult(searchTag);
        }

        public async Task<IEnumerable<MetadataTag>> GetAllMetadataTags()
        {
            var tagsDataReader = ExecuteSqlReader(BuildGetAllMetadataTags());
            var tags = SqlDataReaderHelper.CreateList<MetadataTag>(tagsDataReader);
            return await Task.FromResult(tags);
        }

        public async Task<IEnumerable<MetadataTag>> GetMetadataTagsByIds(IEnumerable<Guid> tagIds)
        {
            var tagsDataReader = ExecuteSqlReader(BuildGetMetadataTagsByIds(tagIds));
            var tags = SqlDataReaderHelper.CreateList<MetadataTag>(tagsDataReader);
            return await Task.FromResult(tags);
        }

        public async Task<IEnumerable<Guid>> GetSuggestedResourcesByTags(IEnumerable<Guid> tagIds, ResourceType resourceType)
        {
            var resultReader = GetDataReader("uspGetSuggestedResources", new List<DbParameter>
                {
                    GetParameter("ResourceType", resourceType.ToString()),
                    GetParameter("TagIds", SqlXmlDataTypeHelper.SerializeToXml(tagIds.Select(p => p.ToString()).ToArray())),
                    GetParameter("ExcludedGroupCodes", SqlXmlDataTypeHelper.SerializeToXml(
                        MetadataTagGroupCodes.ServiceSchemes,
                        MetadataTagGroupCodes.PdoTypes,
                        MetadataTagGroupCodes.PdoCategories,
                        MetadataTagGroupCodes.PdoModes,
                        MetadataTagGroupCodes.PdoNatures,
                        MetadataTagGroupCodes.CourseLevels))
                });
            var result = SqlDataReaderHelper.CreateSingleValueList<Guid>(resultReader);
            return await Task.FromResult(result.Distinct());
        }

        public async Task<Resource> GetResourceById(Guid resourceId, Guid userId)
        {
            var resourceDataReader = ExecuteSqlReader(BuildGetResourceByIdSQL(resourceId));
            var resource = SqlDataReaderHelper.CreateList<Resource>(resourceDataReader).FirstOrDefault();
            if (resource == null)
            {
                return await Task.FromResult(resource);
            }

            var tagIdsDataReader = ExecuteSqlReader(BuildGetTagIdsByResourceIdSQL(resourceId));
            var tagIds = SqlDataReaderHelper.CreateSingleValueList<Guid>(tagIdsDataReader);
            resource.Tags = tagIds;

            var resourceDynamicMetadataItemsDataReader = ExecuteSqlReader(BuildGetResourceDynamicMetadataItemsByResourceIdSQL(resourceId));
            var resourceDynamicMetadataItems = SqlDataReaderHelper.CreateList<ResourceDynamicMetadataItem>(resourceDynamicMetadataItemsDataReader);
            resource.DynamicMetaData = resourceDynamicMetadataItems.ToDictionary(p => p.Key, p => p.JsonValue != null ? JsonSerializer.Deserialize<object>(p.JsonValue) : null);

            var searchTagsDataReader = ExecuteSqlReader(BuildGetSearchTagsByResourceId(resourceId));
            var searchTags = SqlDataReaderHelper.CreateSingleValueList<string>(searchTagsDataReader);
            resource.SearchTags = searchTags;

            return await Task.FromResult(resource);
        }

        public async Task<List<Resource>> GetResourceByResourceType(IEnumerable<ResourceType> resourceTypes)
        {
            var resourceDataReader = ExecuteSqlReader(BuildGetResourceByResourceTypesSQL(resourceTypes));
            var resourceList = SqlDataReaderHelper.CreateList<Resource>(resourceDataReader);

            if (resourceList == null)
            {
                return await Task.FromResult(resourceList);
            }

            var resourceTagDataReader = await ExecuteSqlReaderAsync(BuildGetResourcesWithTagsByResourceTypeSQL(resourceTypes));
            var tagByResourceId = SqlDataReaderHelper.CreateList<ResourceMetadataTagModel>(resourceTagDataReader)
                                                    .GroupBy(tag => tag.ResourceId)
                                                    .ToDictionary(tagGrouped => tagGrouped.Key, tagGrouped => tagGrouped.Select(tag => tag.TagId));

            var resourceDynamicMetadataDataReader = await ExecuteSqlReaderAsync(BuildGetResourceDynamicMetadataByResourceTypeSQL(resourceTypes));
            var dynamicMetadataByResourceId = SqlDataReaderHelper.CreateList<ResourceDynamicMetadataItem>(resourceDynamicMetadataDataReader)
                                                                .GroupBy(metadata => metadata.ResourceId)
                                                                .ToDictionary(
                                                                    metadataGrouped => metadataGrouped.Key,
                                                                    metadataGrouped => metadataGrouped.ToDictionary(p => p.Key, p => p.JsonValue != null ? JsonSerializer.Deserialize<object>(p.JsonValue) : null));

            var resourceSearchTagDataReader = await ExecuteSqlReaderAsync(BuildGetResourceSearchTagByResourceTypeSQL(resourceTypes));
            var searchTagByResourceId = SqlDataReaderHelper.CreateList<ResourceSearchTagModel>(resourceSearchTagDataReader)
                                                            .GroupBy(tag => tag.ResourceId)
                                                            .ToDictionary(tagGrouped => tagGrouped.Key, tagGrouped => tagGrouped.Select(tag => tag.Name));

            resourceList.ForEach(resource =>
            {
                IEnumerable<Guid> tags = new List<Guid>();
                if (tagByResourceId.TryGetValue(resource.ResourceId, out tags))
                {
                    resource.Tags = tags;
                }

                Dictionary<string, object> dynamicMetaData = new Dictionary<string, object>();
                if (dynamicMetadataByResourceId.TryGetValue(resource.ResourceId, out dynamicMetaData))
                {
                    resource.DynamicMetaData = dynamicMetaData;
                }

                IEnumerable<string> searchTag = new List<string>();
                if (searchTagByResourceId.TryGetValue(resource.ResourceId, out searchTag))
                {
                    resource.SearchTags = searchTag.ToList();
                }
            });
            return await Task.FromResult(resourceList);
        }

        public async Task<Resource> CloneResource(Guid resourceId, Guid clonedFromResourceId, Guid userId)
        {
            var clonedFromResource = await GetResourceById(clonedFromResourceId, userId);
            if (clonedFromResource == null)
            {
                return null;
            }

            var clonedResource = clonedFromResource.Clone(resourceId, userId);

            SaveResourceMetadata(clonedResource, userId);
            return await GetResourceById(resourceId, userId);
        }

        public async Task<GetResourceMetadataListResult> GetResourceMetadataList(IEnumerable<Guid> resourceIds)
        {
            var resourceMetadaReader = ExecuteSqlReader(BuildGetMetadataByResourceIdsSQL(resourceIds));
            var queryResult = SqlDataReaderHelper.CreateList<ResourceMetadataTagModel>(resourceMetadaReader);

            return await Task.FromResult(new GetResourceMetadataListResult()
            {
                Items = queryResult.GroupBy(x => x.ResourceId).Select(x => new ResourceMetadatasModel
                {
                    ResourceId = x.Key,
                    Metadatas = x.ToList()
                })
            });
        }

        public async Task<PagedResultDto<SearchTag>> QuerySearchTags(QuerySearchTagRequest request)
        {
            var countSearchTagsDataReader = ExecuteSqlReader(BuildCountAllSearchTag());
            var totalCount = SqlDataReaderHelper.Count(countSearchTagsDataReader);
            var querySearchTagsDataReader = ExecuteSqlReader(BuildGetPagingSearchTag(request));
            var searchTags = SqlDataReaderHelper.CreateList<SearchTag>(querySearchTagsDataReader);
            return await Task.FromResult(new PagedResultDto<SearchTag>(totalCount, searchTags));
        }

        private static string BuildGetResourceByIdSQL(Guid resourceId)
        {
            return $"SELECT TOP 1 * FROM Resources WHERE ResourceId = '{resourceId.ToString()}'";
        }

        private static string BuildGetTagIdsByResourceIdSQL(Guid resourceId)
        {
            return $"SELECT MetadataTags1.TagId " +
            "FROM Resources Resources1, TaggedWith, MetadataTags MetadataTags1 " +
            "WHERE MATCH(Resources1-(TaggedWith)->MetadataTags1) " +
            $"AND Resources1.ResourceId = '{resourceId.ToString()}' AND MetadataTags1.TagId IS NOT NULL";
        }

        private static string BuildGetMetadataByResourceIdsSQL(IEnumerable<Guid> resourceIds)
        {
            var resourceIdsParam = string.Join(",", resourceIds.Select(resourceId => $"'{resourceId.ToString()}'"));

            return $@"SELECT Resources.ResourceId,MetadataTags.*
                    FROM Resources, TaggedWith, MetadataTags
                    WHERE MATCH(Resources-(TaggedWith)->MetadataTags)
                    AND Resources.ResourceId IN ({resourceIdsParam})";
        }

        private static string BuildGetAllMetadataTags()
        {
            return $"SELECT * FROM MetadataTags";
        }

        private static string BuildGetMetadataTagsByIds(IEnumerable<Guid> tagIds)
        {
            var tagIdsParam = string.Join(",", tagIds.Select(tagid => $"'{tagid.ToString()}'"));
            return $"SELECT * FROM MetadataTags WHERE TagId IN ({tagIdsParam})";
        }

        private static string BuildGetSearchTagByNames(IEnumerable<string> searchTagNames)
        {
            var searchTagIdsParam = string.Join(",", searchTagNames.Select(searchTagId => $"'{searchTagId.ToString()}'"));
            return $"SELECT * FROM SearchTags WHERE [Name] IN ({searchTagIdsParam.ToString()})";
        }

        private static string BuildCountAllSearchTag()
        {
            return "SELECT COUNT(SearchTags.Id) FROM SearchTags INNER JOIN ResourceSearchTags ON SearchTags.Id = ResourceSearchTags.SearchTagId";
        }

        private static string BuildGetSearchTagsByResourceId(Guid resourceId)
        {
            return $@"SELECT st.[Name]
                FROM [dbo].SearchTags st
                INNER JOIN (
                    SELECT *
                    FROM [dbo].ResourceSearchTags
                    WHERE ResourceId = '{resourceId.ToString()}') rst
                ON ST.Id = rst.SearchTagId";
        }

        private static string BuildGetPagingSearchTag(QuerySearchTagRequest request)
        {
            return $@"SELECT DISTINCT SearchTags.*
                    FROM SearchTags INNER JOIN ResourceSearchTags ON SearchTags.Id = ResourceSearchTags.SearchTagId
                    {(string.IsNullOrWhiteSpace(request.SearchText) ? string.Empty : $"WHERE CONTAINS([Name],'\"{request.SearchText}*\"')")}
                    ORDER BY [Name]
                    {(request.PagedInfo.MaxResultCount == 0 ? string.Empty : $"OFFSET {request.PagedInfo.SkipCount} ROWS FETCH NEXT {request.PagedInfo.MaxResultCount} ROWS ONLY")}";
        }

        private static string BuildGetResourceByResourceTypesSQL(IEnumerable<ResourceType> resourceTypes)
        {
            var resourceTypesParam = string.Join(",", resourceTypes.Select(type => $"'{type.ToString()}'"));
            return $"SELECT * FROM Resources WHERE ResourceType IN ({resourceTypesParam})";
        }

        private static string BuildGetResourcesWithTagsByResourceTypeSQL(IEnumerable<ResourceType> resourceTypes)
        {
            var resourceTypesParam = string.Join(",", resourceTypes.Select(type => $"'{type.ToString()}'"));

            return $@"SELECT Resources.ResourceId, MetadataTags.*
		        FROM Resources,TaggedWith,MetadataTags
		        WHERE MATCH(Resources-(TaggedWith)->MetadataTags) AND Resources.ResourceType IN ({resourceTypesParam})
                AND MetadataTags.TagId IS NOT NULL";
        }

        private string BuildGetResourceDynamicMetadataByResourceTypeSQL(IEnumerable<ResourceType> resourceTypes)
        {
            var resourceTypesParam = string.Join(",", resourceTypes.Select(type => $"'{type.ToString()}'"));

            return $@"SELECT *
                        FROM ResourceDynamicMetadata
                        WHERE ResourceId IN (SELECT ResourceId FROM Resources WHERE ResourceType IN ({resourceTypesParam}))";
        }

        private string BuildGetResourceSearchTagByResourceTypeSQL(IEnumerable<ResourceType> resourceTypes)
        {
            var resourceTypesParam = string.Join(",", resourceTypes.Select(type => $"'{type.ToString()}'"));

            return $@"SELECT rst.ResourceId,st.*
                        FROM ResourceSearchTags rst INNER JOIN SearchTags st ON rst.SearchTagId = st.Id
                        WHERE rst.ResourceId IN (SELECT ResourceId FROM Resources WHERE ResourceType IN ({resourceTypesParam}))";
        }

        private string BuildGetResourceDynamicMetadataItemsByResourceIdSQL(Guid resourceId)
        {
            return $"SELECT * FROM ResourceDynamicMetadata WHERE ResourceId='{resourceId.ToString()}'";
        }
    }
}
