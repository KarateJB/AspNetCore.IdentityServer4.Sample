using System;
using System.Threading.Tasks;
using AspNetCore.IdentityServer4.WebApi.Models;
using AspNetCore.IdentityServer4.WebApi.Services;
using AspNetCore.IdentityServer4.WebApi.Utils;
using AspNetCore.IdentityServer4.WebApi.Utils.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AspNetCore.IdentityServer4.WebApi
{
    public class Startup
    {
        private readonly IHostingEnvironment env = null;
        private readonly ILogger<Startup> logger = null;

        public Startup(IConfiguration configuration, IHostingEnvironment env, ILogger<Startup> logger)
        {
            this.Configuration = configuration;
            this.env = env;
            this.logger = logger;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IConfigureOptions<MvcJsonOptions>, CustomJsonOptionWrapper>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // Enable Authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.Authority = "https://localhost:6001"; // Base-address of your identityserver
                options.RequireHttpsMetadata = true;
                options.Audience = "MyBackendApi2"; // API Resource name
                options.TokenValidationParameters.ClockSkew = TimeSpan.Zero; // The JWT security token handler allows for 5 min clock skew in default
                options.Events = new JwtBearerEvents()
                {
                    OnAuthenticationFailed = (e) =>
                    {
                        this.logger.LogError(e.Exception.Message);
                        return Task.CompletedTask;
                    }
                };
            });

            // Inject AppSetting configuration
            services.Configure<AppSettings>(this.Configuration);

            // Inject HttpClient
            services.AddHttpClient<IIdentityClient, IdentityClient>().SetHandlerLifetime(TimeSpan.FromMinutes(2)); // HttpMessageHandler lifetime = 2 min

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            // Custom Token expired response
            app.UseTokenExpiredResponse();

            // Authentication
            app.UseAuthentication();

            // Use ExceptionHandler
            app.ConfigureExceptionHandler(loggerFactory);

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
