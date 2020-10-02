using System.ComponentModel;

namespace AspNetCore.IdentityServer4.Core.Models.Enum
{
    /// <summary>
    /// ClientID enum
    /// </summary>
    public enum AuthClientEnum
    {
        [Description("MyBackend Client")]
        MyBackend = 1,

        [Description("PolicyBasedBackend Client")]
        PolicyBasedBackend,

        [Description("Resource Owners")]
        Resources,

        [Description("PKCE Authorization code Client")]
        PkceCodeBackend
    }
}
