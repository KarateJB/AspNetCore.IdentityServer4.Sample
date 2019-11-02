using AspNetCore.IdentityServer4.Core;
using JB.Infra.Service.Redis;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.IdentityServer4.Auth.Utils.Extensions
{
    /// <summary>
    /// ServiceCollections extensions
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add Cache service
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <returns>Self</returns>
        public static IServiceCollection AddCacheServices(this IServiceCollection services)
        {
            services.AddSingleton<CacheKeyFactory, CacheKeyFactory>();
            services.AddScoped<ICacheService, RedisService>();
            return services;
        }
    }
}
