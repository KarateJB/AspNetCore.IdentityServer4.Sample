﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AspNetCore.IdentityServer4.WebApi.Models;
using IdentityModel.Client;
using Microsoft.Extensions.Options;

namespace AspNetCore.IdentityServer4.WebApi.Services
{
    public class AuthService : IAuthService
    {
        public void Dispose()
        {
        }

        private const string SECRETKEY = "secret";
        private readonly AppSettings configuration = null;
        private readonly HttpClient httpClient = null;
        private readonly string remoteServiceBaseUrl = string.Empty;

        public AuthService(
            IOptions<AppSettings> configuration,
            HttpClient httpClient)
        {
            this.configuration = configuration.Value;
            this.httpClient = httpClient;
            this.remoteServiceBaseUrl = this.configuration.Host.AuthServer;
        }

        public async Task<TokenResponse> SignInAsync(string userName, string password)
        {
            var discoResponse = await this.httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = this.remoteServiceBaseUrl,
                Policy =
                {
                    RequireHttps = true // default: true
                }
            });

            if (discoResponse.IsError)
            {
                throw new Exception(discoResponse.Error);
            }

            TokenResponse tokenResponse = await this.httpClient.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = discoResponse.TokenEndpoint,
                ClientId = "MyBackend",
                ClientSecret = SECRETKEY,
                UserName = userName,
                Password = password,
            });

            //await this.httpClient.Req

            return tokenResponse;
        }
    }
}