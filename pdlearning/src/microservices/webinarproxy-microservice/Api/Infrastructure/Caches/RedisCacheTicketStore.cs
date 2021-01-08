using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Thunder.Platform.Caching;

namespace Microservice.WebinarProxy.Infrastructure.Caches
{
    public class RedisCacheTicketStore : ITicketStore
    {
        private readonly ICacheDataProvider _cacheDataProvider;

        public RedisCacheTicketStore(ICacheDataProvider cacheDataProvider)
        {
            _cacheDataProvider = cacheDataProvider;
        }

        public async Task<string> StoreAsync(AuthenticationTicket ticket)
        {
            var key = new TicketStoreCacheKey(Guid.NewGuid().ToString());

            await RenewAsync(key.FullKey, ticket);

            return key.FullKey;
        }

        public Task RenewAsync(string key, AuthenticationTicket ticket)
        {
            TimeSpan expireTime = ticket.Properties.ExpiresUtc?.TimeOfDay ?? TimeSpan.Zero;

            byte[] serializedTicket = TicketSerializer.Default.Serialize(ticket);

            _cacheDataProvider.AddCachedData(
                key: new TicketStoreCacheKey(key),
                data: serializedTicket,
                expireIn: expireTime);

            return Task.CompletedTask;
        }

        public Task<AuthenticationTicket> RetrieveAsync(string key)
        {
            var storedBytes = _cacheDataProvider.GetCachedData<byte[]>(new TicketStoreCacheKey(key));
            var ticket = storedBytes == null ? null : TicketSerializer.Default.Deserialize(storedBytes);
            return Task.FromResult(ticket);
        }

        public Task RemoveAsync(string key)
        {
            _cacheDataProvider.RemoveCacheData(new TicketStoreCacheKey(key));
            return Task.CompletedTask;
        }
    }
}
