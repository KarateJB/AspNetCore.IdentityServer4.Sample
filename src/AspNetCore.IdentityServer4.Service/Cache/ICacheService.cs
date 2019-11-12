using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.IdentityServer4.Service.Cache
{
    /// <summary>
    /// Interface: Cache service
    /// </summary>
    public interface ICacheService : IDisposable
    {
        /// <summary>
        /// Search Redis keys with pattern
        /// </summary>
        /// <param name="pattern">Pattern, such as "xxxx*"</param>
        /// <returns>Keys collection</returns>
        IEnumerable<string> SearchKeys(string pattern = "");

        /// <summary>
        /// Save cache
        /// </summary>
        /// <typeparam name="T">Generic type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="data">The data which will be cached</param>
        void SaveCache<T>(string key, T data);

        /// <summary>
        /// Save cache with expire time
        /// </summary>
        /// <typeparam name="T">Generic type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="expire">Expire timespan</param>
        /// <param name="data">The data which will be cached</param>
        void SaveCache<T>(string key, TimeSpan? expire, T data);

        /// <summary>
        /// Save cache (async)
        /// </summary>
        /// <typeparam name="T">Generic type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="data">The data which will be cached</param>
        /// <remarks>
        /// Use this async call for only async befavior! It will hang while a SYNC method calls.
        /// <seealso cref="https://github.com/StackExchange/StackExchange.Redis/issues/88"/>
        /// <seealso cref="https://github.com/StackExchange/StackExchange.Redis/issues/131"/>
        /// </remarks>
        Task SaveCacheAsync<T>(string key, T data);

        /// <summary>
        /// Save cache with expire time (async)
        /// </summary>
        /// <typeparam name="T">Generic type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="expire">Expire timespan</param>
        /// <param name="data">The data which will be cached</param>
        /// <remarks>
        /// Use this async call for only async befavior! It will hang while a SYNC method calls.
        /// <seealso cref="https://github.com/StackExchange/StackExchange.Redis/issues/88"/>
        /// <seealso cref="https://github.com/StackExchange/StackExchange.Redis/issues/131"/>
        /// </remarks>
        Task SaveCacheAsync<T>(string key, TimeSpan? expire, T data);

        /// <summary>
        /// Increase the key's value
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="by">Increment</param>
        Task IncreaseAsyc(string key, TimeSpan? expire = null, long by = 1);

        /// <summary>
        /// Decrease the key's value
        /// <summary>
        /// <param name="key">Key</param>
        /// <param name="by">Decrement</param>
        Task DecreaseAsyc(string key, long by = 1);

        /// <summary>
        /// Get cache by key
        /// </summary>
        /// <typeparam name="T">Generic type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="rtn">T</param>
        /// <returns>Boolean</returns>
        bool GetCache<T>(string key, out T rtn) where T : new();

        /// <summary>
        /// Get Cache (async)
        /// </summary>
        /// <typeparam name="T">Generic type</typeparam>
        /// <param name="key">Key</param>
        /// <returns>Tuple for T and boolean</returns>
        /// <remarks>
        /// Use this async call for only async befavior! It will hang while a SYNC method calls.
        /// <seealso cref="https://github.com/StackExchange/StackExchange.Redis/issues/88"/>
        /// <seealso cref="https://github.com/StackExchange/StackExchange.Redis/issues/131"/>
        /// </remarks>
        Task<Tuple<T, bool>> GetCacheAsync<T>(string key) where T : new();

        /// <summary>
        /// Clear cache with key
        /// </summary>
        /// <param name="key">Key</param>
        void ClearCache(string key);

        /// <summary>
        /// Clear cache with key
        /// </summary>
        /// <param name="key">Key</param>
        /// <remarks>
        /// Use this async call for only async befavior! It will hang while a SYNC method calls.
        /// <seealso cref="https://github.com/StackExchange/StackExchange.Redis/issues/88"/>
        /// <seealso cref="https://github.com/StackExchange/StackExchange.Redis/issues/131"/>
        /// </remarks>
        Task ClearCacheAsync(string key);
    }
}
