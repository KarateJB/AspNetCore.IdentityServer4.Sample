using AspNetCore.IdentityServer4.WebApi.Utils.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.IdentityServer4.WebApi.Utils.Extensions
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
            services.AddSingleton<RedisKeyFactory, RedisKeyFactory>();
            services.AddScoped<ICacheService, RedisService>();
            return services;
        }
    }
}
