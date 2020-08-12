namespace AspNetCore.IdentityServer4.Mvc.OpenApiSpec
{
    /// <summary>
    /// Authentication Scheme
    /// </summary>
    public static class AuthenticationScheme
    {
        /// <summary>
        /// Bearer-token authentication
        /// </summary>
        /// <remarks>Equals JwtBearerDefaults.AuthenticationScheme</remarks>
        public static string Bearer = "bearer";

        /// <summary>
        /// Basic (Id/Pwd) authentication
        /// </summary>
        public static string Basic = "basic";
    }
}
