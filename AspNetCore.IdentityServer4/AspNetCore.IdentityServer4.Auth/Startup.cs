using AspNetCore.IdentityServer4.Auth.Events;
using AspNetCore.IdentityServer4.Auth.Utils.Config;
using AspNetCore.IdentityServer4.Auth.Utils.Extensions;
using AspNetCore.IdentityServer4.Auth.Utils.Service;
using AspNetCore.IdentityServer4.Core;
using IdentityServer.LdapExtension.Extensions;
using IdentityServer.LdapExtension.UserModel;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.IdentityServer4.Auth
{
    public class Startup {
        public Startup (IConfiguration configuration) {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices (IServiceCollection services) 
        {
            services.AddMvc ().SetCompatibilityVersion (CompatibilityVersion.Version_2_2);

            services.AddSession();

            #region IISOptions
            services.Configure<IISOptions>(options =>
            {
                options.AutomaticAuthentication = false;
                options.AuthenticationDisplayName = "Windows";
            });
            #endregion

            #region Identity Server

            var builder = services.AddIdentityServer (options => {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
            });

            // Signing credential
            builder.AddDeveloperSigningCredential ();

            // Set in-memory, code config
            builder.AddInMemoryIdentityResources (InMemoryInitConfig.GetIdentityResources ());
            builder.AddInMemoryApiResources (InMemoryInitConfig.GetApiResources ());
            builder.AddInMemoryClients (InMemoryInitConfig.GetClients ());
            builder.AddLdapUsers<OpenLdapAppUser> (this.Configuration.GetSection ("LdapServer"), UserStore.InMemory); // OpenLDAP
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
        public void Configure (IApplicationBuilder app, IHostingEnvironment env) 
        {
            if (env.IsDevelopment ()) {
                app.UseDeveloperExceptionPage ();
            }

            app.UseIdentityServer ();

            app.UseSession();

            app.UseHttpsRedirection ();

            app.UseMvc ();
        }
    }
}