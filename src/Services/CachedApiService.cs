using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using MMAEvents.ApiClients;
using MMAEvents.ApiClients.Models;

namespace MMAEvents.TelegramBot.Services
{
    public class CachedApiService : IEventsApiClient
    {
        private readonly IMemoryCache cache;
        private readonly EventsApiClient apiClient;

        public CachedApiService(IMemoryCache cache, EventsApiClient apiClient)
        {
            this.cache = cache;
            this.apiClient = apiClient;
        }

        public async Task<EventDTO> GetEventByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            var cacheKey = CreateCacheKey(id);

            return await cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromSeconds(30);
                return await apiClient.GetEventByIdAsync(id);
            });
        }

        public async Task<ICollection<EventDTO>> GetEventsAsync(string eventName, DateTimeOffset? date, int? quantity, CancellationToken cancellationToken = default)
        {
            var events = await apiClient.GetEventsAsync(eventName, date, quantity, cancellationToken);

            foreach (var e in events)
            {
                var cacheKey = CreateCacheKey(e.Id);
                cache.Set(cacheKey, e, TimeSpan.FromSeconds(30));
            }

            return events;
        }

        private string CreateCacheKey(long id) => $"event-{id}";
    }
}