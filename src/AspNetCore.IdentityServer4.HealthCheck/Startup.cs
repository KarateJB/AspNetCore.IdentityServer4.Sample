using System;
using System.Net.Http;
using AspNetCore.IdentityServer4.Core.Models.Config.HealthCheck;
using AspNetCore.IdentityServer4.HealthCheck.HealthChecks;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace AspNetCore.IdentityServer4.HealthCheck
{
    public class Startup
    {
        private readonly AppSettings appSettings;
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
            this.appSettings = new AppSettings();
            this.Configuration.Bind(this.appSettings);
        }

        public IConfiguration Configuration { get; }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            #region Health Check
            var healthCheckService = this.appSettings
                .HealthChecks.Service;
            string redisConnectionStr = $"{healthCheckService.Redis.Host}:{healthCheckService.Redis.Port}";
            string idsrvUrl = $"https://{healthCheckService.IdentityServer.Host}:{healthCheckService.IdentityServer.Port}/"; // Must ends with "/"

            services.AddHealthChecks()
               .AddRedis(redisConnectionStr, name: "Redis", tags: new string[] { "redis" })
               //.AddIdentityServer(new Uri(idsrvUrl), name: "Idsrv: Discovery Document", tags: new string[] { "identity server", "auth" }) // Not work on docker env, so I write the custom HealthCheck: IdsrvHealthCheck 
               .AddCheck("Idsrv", new IdsrvHealthCheck(this.appSettings), HealthStatus.Unhealthy, new string[] { "idsrv" });

            #endregion

            #region Health Check UI
            services.AddHealthChecksUI(setup =>
                {
                    setup.SetEvaluationTimeInSeconds(10);
                    setup.MaximumHistoryEntriesPerEndpoint(10);
                    setup.SetMinimumSecondsBetweenFailureNotifications(60);

                // Ignore certificate check
                setup.UseApiEndpointHttpMessageHandler(sp =>
                    {
                        return new HttpClientHandler
                        {
                            ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, certChain, policyErrors) =>
                            {
                                return true;
                            }
                        };
                    });

                // Health check endpoints
                this.appSettings.HealthChecks.Endpoints.ForEach(e =>
                       {
                           setup.AddHealthCheckEndpoint(e.Name, e.Url);
                       });

                // Healthe check webhooks
                //setup.AddWebhookNotification()
            }).AddInMemoryStorage();
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseHealthChecksUI(options =>
            {
                options.ApiPath = "/healthchecks-ui-api";
                options.UIPath = "/healthchecks-ui";
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecksUI();
                //endpoints.MapHealthChecks("/health", new HealthCheckOptions()
                //{
                //    Predicate = _ => true,
                //    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                //});
            });
        }
    }
}
