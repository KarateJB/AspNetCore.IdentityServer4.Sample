using IdentityServer4.Models;

namespace AspNetCore.IdentityServer4.Auth.Models.ViewModels
{
    /// <summary>
    /// Error View model
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ErrorViewModel()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="error">Error</param>
        public ErrorViewModel(string error)
        {
            Error = new ErrorMessage { Error = error };
        }

        /// <summary>
        /// Error
        /// </summary>
        public ErrorMessage Error { get; set; }
    }
}
