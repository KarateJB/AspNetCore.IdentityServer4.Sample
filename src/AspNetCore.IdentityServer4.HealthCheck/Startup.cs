using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using HealthChecks.IdSvr;
using System;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace AspNetCore.IdentityServer4.HealthCheck
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public IConfiguration configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            #region Health Check
            services.AddHealthChecks()
               .AddRedis(configuration["Host:Redis"], name: "Redis HealthCheck")
               .AddIdentityServer(new Uri("https://localhost:6001/"));

            #endregion
            #region Health Check UI
            //services.AddHealthChecks()
            //    .AddRedis(Configuration["Host:Redis"], name: "Redis HealthCheck");

            services.AddHealthChecksUI(setup =>
            {
                setup.SetEvaluationTimeInSeconds(10);
                setup.MaximumHistoryEntriesPerEndpoint(10);
                setup.SetMinimumSecondsBetweenFailureNotifications(60);

                // Health check endpoints
                setup.AddHealthCheckEndpoint("Backend", "https://localhost:5001/health");
                setup.AddHealthCheckEndpoint("Auth", "https://localhost:6001/health");
                setup.AddHealthCheckEndpoint("Services", "https://localhost:7001/health");

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
                endpoints.MapHealthChecks("/health", new HealthCheckOptions()
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });

            });
        }
    }
}
