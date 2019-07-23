using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace AspNetCore.IdentityServer4.WebApi.Services
{
    public interface IAuthService : IDisposable
    {
        Task<TokenResponse> SignInAsync(string userName, string password);
        Task<UserInfoResponse> GetUserInfoAsync(string accessToken);
    }
}
