using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using AspNetCore.IdentityServer4.Auth.Events;
using AspNetCore.IdentityServer4.Auth.Utils.Config;
using AspNetCore.IdentityServer4.Auth.Utils.Extensions;
using AspNetCore.IdentityServer4.Auth.Utils.Service;
using AspNetCore.IdentityServer4.Service.Crypto;
using IdentityServer.LdapExtension.Extensions;
using IdentityServer.LdapExtension.UserModel;
using IdentityServer4;
using IdentityServer4.Configuration;
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
        private IConfiguration configuration { get; }

        private IWebHostEnvironment env { get; }

        public Startup(
            IConfiguration configuration,
            IWebHostEnvironment env)
        {
            this.configuration = configuration;
            this.env = env;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
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
            });

            // Signing credential
            if (!this.env.IsDevelopment())
            {
                builder.AddDeveloperSigningCredential();
            }
            else
            {
                var key = CryptoHelper.CreateRsaSecurityKey();

                RSAParameters parameters;

                if (key.Rsa != null)
                {
                    parameters = key.Rsa.ExportParameters(includePrivateParameters: true);
                }
                else
                {
                    parameters = key.Parameters;
                }

                // var tempKey = new CryptoHelper.TemporaryRsaKey
                // {
                //     Parameters = parameters,
                //     KeyId = key.KeyId
                // };

                // if (persistKey)
                // {
                //     File.WriteAllText(filename, JsonConvert.SerializeObject(tempKey, new JsonSerializerSettings { ContractResolver = new CryptoHelper.RsaKeyContractResolver() }));
                // }

                builder.AddSigningCredential(key, IdentityServerConstants.RsaSigningAlgorithm.RS256);

                // Or Use self-signed cert
                // var rootPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Certs");
                // var cert = new X509Certificate2(Path.Combine(rootPath, "Docker.pfx"), string.Empty);
                // builder.AddSigningCredential(cert);
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