using AspNetCore.IdentityServer4.Auth.Events;
using AspNetCore.IdentityServer4.Auth.Utils.Config;
using AspNetCore.IdentityServer4.Auth.Utils.Extensions;
using AspNetCore.IdentityServer4.Auth.Utils.Service;
using AspNetCore.IdentityServer4.Core.Models.Config.Auth;
using IdentityServer.LdapExtension.Extensions;
using IdentityServer.LdapExtension.UserModel;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AspNetCore.IdentityServer4.Auth
{
    public class Startup
    {
        private readonly AppSettings appSettings = null;

        private IConfiguration configuration { get; }
        private IWebHostEnvironment env { get; }

        public Startup(
            IConfiguration configuration,
            IWebHostEnvironment env)
        {
            this.configuration = configuration;
            this.env = env;
            this.appSettings = new AppSettings();
            this.configuration.Bind(this.appSettings);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                 .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddSession();

            #region IISOptions
            services.Configure<IISOptions>(options =>
            {
                options.AutomaticAuthentication = false;
                options.AuthenticationDisplayName = "Windows";
            });
            #endregion

            #region Identity Server

            var builder = services.AddIdentityServer(options =>
            {
                // options.PublicOrigin = "https://localhost:6001";
                // options.IssuerUri = "https://localhost:6001";
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;

                options.Discovery.ResponseCacheInterval = 60;
            });

            // Signing credential
            if (this.env.IsDevelopment())
            {
                builder.AddDeveloperSigningCredential();
            }
            else
            {
                // 1. Store in file (Support renew manually)
                // builder.AddSigningCredentialsByFile(this.appSettings);

                // 2. Store in Redis (Support renew automatically)
                builder.AddSigningCredentialByRedis(this.appSettings);

                // 3. Use cert
                // builder.AddSigningCredentialByCert(this.appSettings, isFromWindowsCertStore: true);
            }

            // Set in-memory, code config
            builder.AddInMemoryIdentityResources(InMemoryInitConfig.GetIdentityResources());
            builder.AddInMemoryApiResources(InMemoryInitConfig.GetApiResources());
            builder.AddInMemoryClients(InMemoryInitConfig.GetClients());
            builder.AddLdapUsers<OpenLdapAppUser>(this.configuration.GetSection("LdapServer"), UserStore.InMemory); // OpenLDAP
                                                                                                                    // builder.AddLdapUsers<ActiveDirectoryAppUser>(this.Configuration.GetSection("LdapServer"), UserStore.InMemory); // ActiveDirectory

            builder.AddProfileService<ProfileService>();

            #endregion

            #region  Inject Cache service
            services.AddMemoryCache();
            services.AddCacheServices();
            #endregion

            #region Custom sinks
            services.AddScoped<IEventSink, UserProfileCacheSink>();
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseIdentityServer();

            app.UseSession();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}