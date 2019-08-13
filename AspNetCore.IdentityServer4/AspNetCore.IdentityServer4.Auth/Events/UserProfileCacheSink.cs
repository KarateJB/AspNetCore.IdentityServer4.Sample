using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.IdentityServer4.Auth.Utils.Cache;
using IdentityServer4.Events;
using IdentityServer4.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace AspNetCore.IdentityServer4.Auth.Events
{
    public class UserProfileCacheSink : IEventSink
    {
        private readonly ICacheKeyFactory _cacheKeyFactory = null;
        private readonly IMemoryCache _cache = null;
        private readonly ILogger<UserProfileCacheSink> _logger = null;

        public UserProfileCacheSink(ICacheKeyFactory cacheKeyFactory, IMemoryCache cache, ILogger<UserProfileCacheSink> logger)
        {
            this._cacheKeyFactory = cacheKeyFactory;
            this._cache = cache;
            this._logger = logger;
        }

        public async Task PersistAsync(Event evt)
        {
            if (evt.Id.Equals(EventIds.UserLoginSuccess))
            {
                if (evt.EventType == EventTypes.Success || evt.EventType == EventTypes.Information)
                {
                    this._logger.LogInformation($"{evt.Name} ({evt.Id}), Details: {evt.Message}");
                }
                else
                {
                    this._logger.LogError($"{evt.Name} ({evt.Id}), Details: {evt.Message}");
                }
            }
        }
    }
}
