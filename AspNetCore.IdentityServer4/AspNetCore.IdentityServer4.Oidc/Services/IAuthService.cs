using System;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace AspNetCore.IdentityServer4.Oidc.Services
{
    public interface IAuthService : IDisposable
    {
        Task<TokenResponse> SignInAsync(string userName, string password);
        Task<UserInfoResponse> GetUserInfoAsync(string accessToken);
    }
}
