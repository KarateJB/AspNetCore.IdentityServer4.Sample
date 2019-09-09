using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace AspNetCore.IdentityServer4.WebApi.Utils
{
    /// <summary>
    /// Custom JsonOption Wrapper
    /// </summary>
    public class CustomJsonOptionWrapper : IConfigureOptions<MvcJsonOptions>
    {
        private readonly IServiceProvider serviceProvider = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serviceProvider"></param>
        public CustomJsonOptionWrapper(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Configure
        /// </summary>
        /// <param name="options"></param>
        public void Configure(MvcJsonOptions options)
        {
            options.SerializerSettings.ContractResolver = new ApiContractResolver(this.serviceProvider);
            options.SerializerSettings.Formatting = Formatting.None;
            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        }
    }
}
