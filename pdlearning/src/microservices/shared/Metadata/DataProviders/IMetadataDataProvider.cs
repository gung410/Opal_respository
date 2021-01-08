using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Conexus.Opal.Microservice.Metadata.Entities;

namespace Conexus.Opal.Microservice.Metadata.DataProviders
{
    public interface IMetadataDataProvider
    {
        void SaveMetadataTag(MetadataTag metadataTag);

        Task DeleteNotExistedMetadataInListIds<TMetadataTag>(IEnumerable<Guid> ids) where TMetadataTag : IMetadataTag;

        Task<bool> HasAnyMetadataTags();
    }
}
