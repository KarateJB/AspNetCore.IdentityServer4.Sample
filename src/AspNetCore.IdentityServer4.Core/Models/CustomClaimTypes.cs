using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace AspNetCore.IdentityServer4.Core.Models
{
    /// <summary>
    /// Custom Claim Types
    /// </summary>
    /// <remarks>
    /// Reference:  System.Security.Claims.ClaimTypes
    /// </remarks>
    public static class CustomClaimTypes
    {
        public const string Department = "department";
    }
}
