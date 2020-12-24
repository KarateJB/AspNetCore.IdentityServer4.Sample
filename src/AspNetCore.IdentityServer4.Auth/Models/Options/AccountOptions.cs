using System;

namespace AspNetCore.IdentityServer4.Auth.Models
{
    /// <summary>
    /// Account options
    /// </summary>
    public class AccountOptions
    {
        /// <summary>
        /// Does allow local login
        /// </summary>
        public static bool AllowLocalLogin = true;

        /// <summary>
        /// Does allow remember-login
        /// </summary>
        public static bool AllowRememberLogin = true;

        /// <summary>
        /// If remember-login is allowed, how long will be the cookie stays in browser
        /// </summary>
        public static TimeSpan RememberMeLoginDuration = TimeSpan.FromDays(30);

        /// <summary>
        /// Does show logout prompt
        /// </summary>
        public static bool ShowLogoutPrompt = true;

        /// <summary>
        /// Does enable automatically redirect after signout
        /// </summary>
        public static bool AutomaticRedirectAfterSignOut = false;

        /// <summary>
        /// Error msg for invalid credential
        /// </summary>
        public static string InvalidCredentialsErrorMessage = "Invalid username or password";
    }
}
