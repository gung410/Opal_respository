using Thunder.Platform.Caching;

namespace Microservice.WebinarProxy.Infrastructure.Caches
{
    public class TicketStoreCacheKey : CacheKey
    {
        public TicketStoreCacheKey(string id) : base(nameof(TicketStoreCacheKey), id, false)
        {
        }
    }
}
