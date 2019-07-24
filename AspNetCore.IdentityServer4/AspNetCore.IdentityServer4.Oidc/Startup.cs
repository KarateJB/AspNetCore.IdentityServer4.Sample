using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.IdentityServer4.Oidc.Controllers;
using AspNetCore.IdentityServer4.Oidc.Models;
using AspNetCore.IdentityServer4.Oidc.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AspNetCore.IdentityServer4.Oidc
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            // Enable Authentication
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
            })
            .AddCookie("Cookies")
            .AddOpenIdConnect("oidc", options =>
            {
                options.SignInScheme = "Cookies";

                options.Authority = "https://localhost:6001";
                options.RequireHttpsMetadata = true;

                options.ClientId = "OidcBackend";
                options.ClientSecret = "secret";
                options.ResponseType = "code id_token";

                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;

                //options.Scope.Add("api1");
                options.Scope.Add("offline_access");
                options.ClaimActions.MapJsonKey("website", "website");
            });
            
            // Inject AppSetting configuration
            services.Configure<AppSettings>(this.Configuration);

            // Inject HttpClient
            services.AddHttpClient<IAuthService, AuthService>().SetHandlerLifetime(TimeSpan.FromMinutes(2)); // HttpMessageHandler lifetime = 2 min
            services.AddHttpClient();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // Authentication
            app.UseAuthentication();

            app.UseHttpsRedirection();

            app.UseMvc();
        }
    }
}
