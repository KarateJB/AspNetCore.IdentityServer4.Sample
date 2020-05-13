using System;
using System.Threading.Tasks;
using IdentityServer4.Validation;

namespace AspNetCore.IdentityServer4.Auth.Utils.Validator
{
    /// <summary>
    /// Custom ResourceOwnerPassword validator
    /// </summary>
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        //private readonly IUserService _userService;

        //public ResourceOwnerPasswordValidator(IUserService userService)
        //{
        //    _userService = userService;
        //}

        /// <summary>
        /// Validate
        /// </summary>
        /// <param name="context">ResourceOwnerPasswordValidationContext</param>
        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            throw new NotImplementedException();
            //var user = await _userService.Login(context.UserName, context.Password);
            //if (user != null)
            //{
            //    var claims = new List<Claim>() { new Claim(JwtClaimTypes.Role, "admin") };
            //    context.Result = new GrantValidationResult(user.UserId.ToString(), OidcConstants.AuthenticationMethods.Password, claims.AsEnumerable());
            //}
        }
    }
}