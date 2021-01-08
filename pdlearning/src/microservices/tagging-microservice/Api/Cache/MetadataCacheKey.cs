using System;
using Thunder.Platform.Caching;

namespace Conexus.Opal.Microservice.Tagging.Cache
{
    public class MetadataCacheKey : CacheKey
    {
        public MetadataCacheKey(string repositoryName, string key, bool isGlobal) : base(repositoryName, key, isGlobal)
        {
        }

        public static MetadataCacheKey ForAll()
        {
            return new MetadataCacheKey(nameof(MetadataCacheKey), "All", false);
        }
    }
}
