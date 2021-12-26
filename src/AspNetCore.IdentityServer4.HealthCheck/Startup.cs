using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
            //services.AddHealthChecks();
            services.AddHealthChecksUI().AddInMemoryStorage();
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
