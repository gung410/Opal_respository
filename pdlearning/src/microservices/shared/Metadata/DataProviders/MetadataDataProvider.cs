using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.Microservice.Infrastructure;
using Conexus.Opal.Microservice.Metadata.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Conexus.Opal.Microservice.Metadata.DataProviders
{
    public abstract class BaseMetadataDataProvider : BaseDataProvider, IMetadataDataProvider
    {
        private readonly ILogger<BaseMetadataDataProvider> _logger;

        protected BaseMetadataDataProvider(DbContext dbContext, ILoggerFactory loggerFactory) : base(dbContext.Database.GetDbConnection())
        {
            _logger = loggerFactory.CreateLogger<BaseMetadataDataProvider>();
        }

        protected virtual string TagIdColumn => "TagId";

        public void SaveMetadataTag(MetadataTag metadataTag)
        {
            ExecuteNonQuery(
                "uspSaveMetadataTag",
                new List<DbParameter>
                {
                    GetParameter("TagId", metadataTag.TagId.ToString()),
                    GetParameter("ParentTagId", metadataTag.ParentTagId?.ToString()),
                    GetParameter("FullStatement", metadataTag.FullStatement),
                    GetParameter("DisplayText", metadataTag.DisplayText),
                    GetParameter("GroupCode", metadataTag.GroupCode),
                    GetParameter("CodingScheme", metadataTag.CodingScheme),
                    GetParameter("Note", metadataTag.Note),
                    GetParameter("Type", metadataTag.Type)
                });
        }

        public Task<bool> HasAnyMetadataTags()
        {
            var resourceDataReader = ExecuteSqlReader("SELECT TOP 1 Id FROM MetadataTags");
            var result = resourceDataReader.Read();
            resourceDataReader.Close();
            return Task.FromResult(result);
        }

        public async Task DeleteNotExistedMetadataInListIds<TMetadataTag>(IEnumerable<Guid> ids) where TMetadataTag : IMetadataTag
        {
            try
            {
                await ExecuteSqlReaderAsync(BuildDeleteMetadataTagsNotInTagIdsSQL(ids.ToList()));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to delete invalid metadata");
                throw;
            }
        }

        private string BuildDeleteMetadataTagsNotInTagIdsSQL(IEnumerable<Guid> ids)
        {
            return $"DELETE [dbo].[MetadataTags] WHERE {TagIdColumn} not in ({string.Join(',', ids.Select(p => $"'{p.ToString()}'"))})";
        }
    }
}
