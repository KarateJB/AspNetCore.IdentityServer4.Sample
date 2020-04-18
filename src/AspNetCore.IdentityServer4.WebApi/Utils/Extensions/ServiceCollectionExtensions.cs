using AspNetCore.IdentityServer4.Core;
using AspNetCore.IdentityServer4.Service.Cache;
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
            services.AddScoped<ICacheService, RedisService>();
            return services;
        }

        /// <summary>
        /// Add other custom services, utils ...etc
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <returns>Self</returns>
        public static IServiceCollection AddCustomServices(this IServiceCollection services)
        {
            services.AddSingleton<AccessTokenValidator>();
            return services;
        }
    }
}
