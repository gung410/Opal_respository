using System.Threading.Tasks;
using Conexus.Opal.Microservice.Metadata.Entities;

namespace Conexus.Opal.Microservice.Metadata.DataSync
{
    public interface IMetadataSynchronizer
    {
        Task Sync(bool force = false);

        Task Sync<TMetadataTag>(bool force = false) where TMetadataTag : IMetadataTag;
    }
}
