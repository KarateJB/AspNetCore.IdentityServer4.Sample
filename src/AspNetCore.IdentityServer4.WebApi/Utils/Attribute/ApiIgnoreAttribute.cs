using System;

namespace AspNetCore.IdentityServer4.WebApi.Utils
{
    /// <summary>
    /// Ignore Attrbutr for API Deserialization(Model binding) and Serialization(Response)
    /// </summary>
    /// <remarks>
    /// Exclude(Ignore) property on defined Http method(s) when doing serialization or deserialization.
    /// If you want to ignore on all cases, just use JsonIgnoreAttribute!
    /// </remarks>
    public class ApiIgnoreAttribute : Attribute
    {
        /// <summary>
        /// Enable the property on what Http methods when doing deserialization
        /// </summary>
        /// <remarks>
        /// Default: Enable on all Http methods.
        /// Notice that the IgnoreDeserializeOn priority is higher than EnableDeserializeOn.
        /// </remarks>
        public string EnableDeserializeOn { get; set; } = "*";

        /// <summary>
        /// Ignore the property on what Http methods when doing deserialization
        /// </summary>
        public string IgnoreDeserializeOn { get; set; }

        /// <summary>
        /// Enable the property on what Http methods when doing deserialization
        /// </summary>
        /// <remarks>
        /// Default: Enable on all Http methods.
        /// Notice that the IgnoreSerializeOn priority is higher than EnableSerializeOn.
        /// </remarks>
        public string EnableSerializeOn { get; set; } = "*";

        /// <summary>
        /// Ignore the property on what Http methods when doing serialization
        /// </summary>
        public string IgnoreSerializeOn { get; set; }
    }
}
