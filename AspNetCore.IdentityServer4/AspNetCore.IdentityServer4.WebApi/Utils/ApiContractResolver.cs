using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AspNetCore.IdentityServer4.WebApi.Utils
{
    /// <summary>
    /// API ContractResolver
    /// </summary>
    public class ApiContractResolver : CamelCasePropertyNamesContractResolver
    {
        private readonly Hashtable contractCache = null;
        private readonly IServiceProvider serviceProvider = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serviceProvider"></param>
        public ApiContractResolver(IServiceProvider serviceProvider)
            : base()
        {
            this.contractCache = new Hashtable();
            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Resolve contract
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public override JsonContract ResolveContract(Type type)
        {
            JsonContract contract = null;
            var contextAccessor = this.serviceProvider.GetRequiredService<IHttpContextAccessor>();
            var httpMethod = contextAccessor?.HttpContext?.Request?.Method;

            if (string.IsNullOrEmpty(httpMethod))
            {
                contract = this.CreateContract(type);
            }
            else
            {
                string key = string.Format("{0}-{1}", type.ToString(), httpMethod);

                if (!this.contractCache.ContainsKey(key))
                {
                    contract = this.CreateContract(type);
                    this.contractCache.Add(key, contract);
                }
                else
                {
                    contract = this.contractCache[key] as JsonContract;
                }
            }

            return contract;
        }

        /// <summary>
        /// Check if the prop should be ignored by enable/ignore settings
        /// </summary>
        /// <param name="ignoreOn">Ignore methods</param>
        /// <param name="enableOn">Enable methods</param>
        /// <param name="method">Target Http method</param>
        /// <returns>true(Should be ignored)|false(Should not be ignored)</returns>
        internal virtual bool CheckIfIgnore(string ignoreOn, string enableOn, string method)
        {
            if (string.IsNullOrEmpty(ignoreOn) && string.IsNullOrEmpty(enableOn))
            {
                // If set nothing, ignore it
                return true;
            }
            else if (string.IsNullOrEmpty(ignoreOn) && enableOn.Equals("*"))
            {
                // Enable: * and Ignore nothing, so dont ignore it
                return false;
            }
            else if (!string.IsNullOrEmpty(ignoreOn) && (string.IsNullOrEmpty(enableOn) || enableOn.Equals("*")))
            {
                // IgnoreOn had been set, but not with EnableOn or EnableOn is *
                return ignoreOn.Equals("*") || ignoreOn.Contains(method);
            }
            else if (!string.IsNullOrEmpty(ignoreOn) && !string.IsNullOrEmpty(enableOn))
            {
                // Check IgnoreOn first
                if (ignoreOn.Equals("*") || ignoreOn.Contains(method, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
                else
                {
                    return !enableOn.Contains(method, StringComparison.OrdinalIgnoreCase);
                }
            }
            else
            {
                // Just check EnableOn
                return !enableOn.Contains(method, StringComparison.OrdinalIgnoreCase);
            }
        }

        /// <summary>
        /// Create Property
        /// </summary>
        /// <param name="member">MemberInfo</param>
        /// <param name="memberSerialization">MemberSerialization</param>
        /// <returns></returns>
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            // 1. Check [JsonIgnore]
            if (member.GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Length > 0)
            {
                property.ShouldSerialize = instance => { return false; };
                property.ShouldDeserialize = instance => { return false; };
            }
            else
            {
                // 2. Check [ApiJsonIgnore]
                var customAttr = member.GetCustomAttributes(typeof(ApiIgnoreAttribute), true).FirstOrDefault() as ApiIgnoreAttribute;
                if (customAttr != null)
                {
                    var contextAccessor = this.serviceProvider.GetRequiredService<IHttpContextAccessor>();
                    var httpMethod = contextAccessor?.HttpContext?.Request?.Method;

                    property.ShouldSerialize = instance =>
                    {
                        return string.IsNullOrEmpty(httpMethod) ? true : !this.CheckIfIgnore(customAttr.IgnoreSerializeOn, customAttr.EnableSerializeOn, httpMethod);
                    };

                    property.ShouldDeserialize = instance =>
                    {
                        return string.IsNullOrEmpty(httpMethod) ? true : !this.CheckIfIgnore(customAttr.IgnoreDeserializeOn, customAttr.EnableDeserializeOn, httpMethod);
                    };
                }
            }

            return property;
        }
    }
}
