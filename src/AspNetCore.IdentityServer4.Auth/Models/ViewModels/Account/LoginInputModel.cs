using System.ComponentModel.DataAnnotations;

namespace AspNetCore.IdentityServer4.Auth.Models.ViewModels
{
    /// <summary>
    /// Login input model
    /// </summary>
    public class LoginInputModel
    {
        /// <summary>
        /// User name
        /// </summary>
        [Required]
        public string Username { get; set; }

        /// <summary>
        /// Pwd
        /// </summary>
        [Required]
        public string Password { get; set; }

        /// <summary>
        /// Is remember login
        /// </summary>
        public bool RememberLogin { get; set; }

        /// <summary>
        /// Return url
        /// </summary>
        public string ReturnUrl { get; set; }
    }
}