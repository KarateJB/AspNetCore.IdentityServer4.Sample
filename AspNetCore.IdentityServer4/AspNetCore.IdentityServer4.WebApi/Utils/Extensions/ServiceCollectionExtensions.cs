using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.IdentityServer4.WebApi.Utils.Extensions
{
    /// <summary>
    /// ServiceCollections extensions
    /// </summary>
    public class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add Cache service
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <returns>Self</returns>
        public static IServiceCollection AddCacheServices(this IServiceCollection services)
        {
            services.AddSingleton<ICacheKeyFactory, RedisKeyFactory>();
            services.AddScoped<ICacheService, RedisService>();
            return services;
        }
    }
}
