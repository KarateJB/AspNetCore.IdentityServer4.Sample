using AspNetCore.IdentityServer4.Core;
using AspNetCore.IdentityServer4.Service.Cache;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.IdentityServer4.Auth.Utils.Extensions
{
    /// <summary>
    /// ServiceCollections extensions
    /// </summary>
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Add Cache service
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <returns>Self</returns>
        public static IServiceCollection AddCacheServices(this IServiceCollection services)
        {
            services.AddSingleton<ICacheService, RedisService>();
            return services;
        }

        /// <summary>
        /// Add CORS
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <param name="corsPolicyName">CORS policy name</param>
        /// <param name="allowedCrossDomains">Allowed cross domains</param>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection AddCustomCors(this IServiceCollection services, string corsPolicyName, string[] allowedCrossDomains)
        {
            services.AddCors(options =>
                       {
                           options.AddPolicy(
                               corsPolicyName,
                               builder =>
                               builder.WithOrigins(allowedCrossDomains) // or builder.AllowAnyOrigin()
                               .AllowAnyHeader()
                               .AllowCredentials()
                               .AllowAnyMethod());
                       });

            return services;
        }

    }
}
