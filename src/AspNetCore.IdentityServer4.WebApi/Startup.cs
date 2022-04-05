using System;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using AspNetCore.IdentityServer4.Core.Models;
using AspNetCore.IdentityServer4.Core.Models.Config.WebApi;
using AspNetCore.IdentityServer4.Core.Utils.Factory;
using AspNetCore.IdentityServer4.Mvc.OpenApiSpec;
using AspNetCore.IdentityServer4.WebApi.Handlers;
using AspNetCore.IdentityServer4.WebApi.Models.AuthorizationRequirement;
using AspNetCore.IdentityServer4.WebApi.Services;
using AspNetCore.IdentityServer4.WebApi.Utils.Config;
using AspNetCore.IdentityServer4.WebApi.Utils.Extensions;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Prometheus;

namespace AspNetCore.IdentityServer4.WebApi
{
    public class Startup
    {
        private readonly IWebHostEnvironment env = null;
        private readonly AppSettings appSettings = null;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            this.Configuration = configuration;
            this.env = env;
            this.appSettings = new AppSettings();
            this.Configuration.Bind(this.appSettings);
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();

            // services.AddControllers()
            services.AddControllersWithViews()
                .AddRazorOptions(
                 options =>
                 {
                     //{2} is area, {1} is controller,{0} is the action
                     options.ViewLocationFormats.Add("/Areas/{1}/Views/{0}.cshtml");
                 })
                .AddNewtonsoftJson()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            #region OpenAPI specification (Swagger)
            services.AddOpenApiSpec<CustomSwaggerConfig>();
            #endregion

            #region Enable Authentication
            services.AddJwtAuthentication(this.appSettings);
            services.AddOpenIdAuthentication(this.appSettings);
            #endregion

            #region Enable policy-based authorization

            // Required: Role "admin"
            services.AddAuthorization(options => options.AddPolicy("AdminPolicy", policy => policy.RequireRole("admin")));
            // Required: Role "user"
            services.AddAuthorization(options => options.AddPolicy("UserPolicy", policy => policy.RequireRole("user")));
            // Required: Role "sit"
            services.AddAuthorization(options => options.AddPolicy("SitPolicy", policy => policy.RequireRole("sit")));
            // Required: Role "admin" OR "user"
            services.AddAuthorization(options => options.AddPolicy("AdminOrUserPolicy", policy => policy.RequireRole("admin", "user")));
            // Required: Department "Sales"
            services.AddAuthorization(options => options.AddPolicy("SalesDepartmentPolicy", policy => policy.RequireClaim(CustomClaimTypes.Department, "Sales")));
            // Required: Department "CRM"
            services.AddAuthorization(options => options.AddPolicy("CrmDepartmentPolicy", policy => policy.RequireClaim(CustomClaimTypes.Department, "CRM")));
            // Required: Department "Sales" AND Role "admin"
            services.AddAuthorization(options => options.AddPolicy("SalesDepartmentAndAdminPolicy",
                policy => policy.RequireClaim(CustomClaimTypes.Department, "Sales").RequireRole("admin")));
            // Required: Department "Sales" AND Role "admin" or "user"
            services.AddAuthorization(options => options.AddPolicy("SalesDepartmentAndAdminOrUserPolicy",
                            policy => policy.RequireClaim(CustomClaimTypes.Department, "Sales").RequireRole("admin", "user")));
            // Required: Department "Sales" OR Role "admin"
            services.AddAuthorization(options => options.AddPolicy("SalesDepartmentOrAdminPolicy", policy => policy.RequireAssertion(
                context => context.User.Claims.Any(
                    x => (x.Type.Equals(CustomClaimTypes.Department) && x.Value.Equals("Sales")) || (x.Type.Equals(ClaimTypes.Role) && x.Value.Equals("admin"))))));
            #endregion

            #region Enable custom Authorization Handlers (The registration order matters!)
            services.AddSingleton<IAuthorizationHandler, EmailDomainAuthHandler>();
            services.AddSingleton<IAuthorizationHandler, UserNameAuthHandler>();

            services.AddAuthorization(options =>
            {
                var emailDomainRequirement = new EmailDomainRequirement("xxx.com");
                var userNameRequirement = new UserNameRequirement("jblin");

                // options.InvokeHandlersAfterFailure = false; // Default: true
                options.AddPolicy("DoaminAndUsernamePolicy", policy =>
                         policy.AddRequirements(emailDomainRequirement, userNameRequirement));
            });
            #endregion

            #region Inject AppSetting configuration

            services.Configure<AppSettings>(this.Configuration);
            #endregion

            #region HttpClient Factory
            services.AddHttpClient(HttpClientNameFactory.AuthHttpClient,
                config =>
                {
                    config.Timeout = TimeSpan.FromMinutes(5);
                    // config.BaseAddress = new Uri("https://localhost:6001/");
                    config.DefaultRequestHeaders.Add("Accept", "application/json");
                })
                .ConfigurePrimaryHttpMessageHandler(h =>
                {
                    var handler = new HttpClientHandler();

                    // Enable sending request to server with untrusted SSL cert 
                    handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                    return handler;
                })
                .SetHandlerLifetime(TimeSpan.FromMinutes(5)); // HttpMessageHandler lifetime = 2 min

            // services.AddHttpClient<IIdentityClient, IdentityClient>().SetHandlerLifetime(TimeSpan.FromMinutes(2)) // HttpMessageHandler default lifetime = 2 min
            // .ConfigurePrimaryHttpMessageHandler(h =>
            // {
            //   var handler = new HttpClientHandler();
            //   if (this.env.IsDevelopment())
            //   {
            //       //Allow untrusted Https connection
            //       handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            //   }
            //   return handler;
            // });
            #endregion

            #region Identity Client
            services.AddSingleton<IIdentityClient, IdentityClient>();
            #endregion

            #region Inject Cache service
            services.AddCacheServices();
            #endregion

            #region Inject other custom services/utils...etc
            services.AddCustomServices();
            #endregion

            #region Health check
            services.AddHealthChecks();
            #endregion
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">IApplicationBuilder</param>
        /// <param name="loggerFactory">Logger factory</param>
        /// <param name="env">IWebHostEnvironment</param>
        /// <param name="provider">IApiVersionDescriptionProvider</param>
        public void Configure(
            IApplicationBuilder app,
            ILoggerFactory loggerFactory,
            IWebHostEnvironment env,
            IApiVersionDescriptionProvider provider)
        {
            // HACK: For testing
            app.Use(async (context, next) =>
            {
                // Logging
                var logger = loggerFactory.CreateLogger("Intercepter Logging");
                logger.LogDebug($"Requesting {context.Request.Path}...");
                // Do work that doesn't write to the Response.
                await next.Invoke();
                // Do logging or other work that doesn't write to the Response.
            });

            // Use static files
            app.UseStaticFiles();

            // Health check
            //app.UseHealthChecks("/health"); // Disable this line when set route on app.UseEndpoints

            // Enable Swagger and Swagger UI
            app.UseCustomSwagger(provider);

            // Custom Invalid Token response/handling
            app.UseInvalidTokenResponse();

            // Use ExceptionHandler
            app.ConfigureExceptionHandler(loggerFactory);

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication(); // Must after app.UseRouting()
            app.UseAuthorization(); // Must after app.UseRouting()

            // Enable Prometheus metrics
            app.UseMetricServer();

            // Custom Middleware
            app.UseRequestCounter();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health", new HealthCheckOptions()
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });

            });
        }
    }
}
