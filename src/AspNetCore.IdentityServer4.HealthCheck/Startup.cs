using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using HealthChecks.IdSvr;

namespace AspNetCore.IdentityServer4.HealthCheck
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
            services.AddControllers();

            #region Health Check UI
            //services.AddHealthChecks()
            //    .AddRedis(Configuration["Host:Redis"], name: "Redis HealthCheck");
            services.AddHealthChecksUI(setup => {
                setup.SetEvaluationTimeInSeconds(10);
                setup.MaximumHistoryEntriesPerEndpoint(10);
                setup.SetMinimumSecondsBetweenFailureNotifications(60);

                setup.AddHealthCheckEndpoint("Backend", "https://localhost:5001/health");
                setup.AddHealthCheckEndpoint("Auth", "https://localhost:6001/health");
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
                //endpoints.MapHealthChecks("/health");
                endpoints.MapHealthChecksUI();
            });
        }
    }
}
