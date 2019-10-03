using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.IdentityServer4.Core;
using IdentityServer4.Events;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace AspNetCore.IdentityServer4.Auth.Events
{
    public class UserProfileCacheSink : IEventSink
    {
        private IHttpContextAccessor httpContextAccessor = null;
        private readonly CacheKeyFactory cacheKeyFactory = null;
        private readonly IMemoryCache cache = null;
        private readonly ILogger<UserProfileCacheSink> logger = null;

        public UserProfileCacheSink(IHttpContextAccessor httpContextAccessor, CacheKeyFactory cacheKeyFactory, IMemoryCache cache, ILogger<UserProfileCacheSink> logger)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.cacheKeyFactory = cacheKeyFactory;
            this.cache = cache;
            this.logger = logger;
        }

        public async Task PersistAsync(Event evt)
        {
            if (evt.Id.Equals(EventIds.UserLoginSuccess))
            {
                if (evt.EventType == EventTypes.Success || evt.EventType == EventTypes.Information)
                {
                    var httpContext = this.httpContextAccessor.HttpContext;

                    try
                    {
                        if (this.httpContextAccessor.HttpContext.Session.IsAvailable)
                        {
                            var session = this.httpContextAccessor.HttpContext.Session;
                            var user = this.httpContextAccessor.HttpContext.User;
                            var subject = user.Claims.Where(x => x.Type == "sub").FirstOrDefault()?.Value;
                            var token = session.GetString("AccessToken");
                            string cacheKey = this.cacheKeyFactory.UserProfile(subject);
                            _ = await this.cache.GetOrCreateAsync<JObject>(cacheKey, async entry =>
                            {
                                entry.SlidingExpiration = TimeSpan.FromSeconds(600);
                                string jsonStr = $"{{\"{subject}\":\"{token}\"}}";
                                return JObject.Parse(jsonStr);
                            });

                            // Check if the cache exist
                            if (this.cache.TryGetValue<JObject>(cacheKey, out JObject tokenInfo))
                            {
                               Debug.WriteLine($"Cached: {tokenInfo.ToString()}");
                            }
                            
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                else
                {
                    this.logger.LogError($"{evt.Name} ({evt.Id}), Details: {evt.Message}");
                }
            }

            
        }
    }
}
