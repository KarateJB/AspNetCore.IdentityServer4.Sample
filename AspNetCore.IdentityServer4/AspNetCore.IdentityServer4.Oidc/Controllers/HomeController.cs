using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using AspNetCore.IdentityServer4.Auth.Models;
using AspNetCore.IdentityServer4.Oidc.Models;
using AspNetCore.IdentityServer4.Oidc.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AspNetCore.IdentityServer4.Oidc.Controllers
{
    [Route("[controller]")]
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly AppSettings configuration = null;
        private readonly IHttpClientFactory httpClientFactory = null;
        private readonly string remoteServiceBaseUrl = string.Empty;

        public HomeController(
            IOptions<AppSettings> configuration,
            IHttpClientFactory httpClientFactory)
        {
            this.configuration = configuration.Value;
            this.httpClientFactory = httpClientFactory;
            this.remoteServiceBaseUrl = this.configuration.Host.AuthServer;
        }

        // GET: Home
        [HttpGet]
        [Route("Login")]
        public async Task<ActionResult> Login()
        {
            var user = new LdapUser()
            {
                Username = "jblin",
                Password = "123456"
            };
            return await Task.FromResult(this.View(user));
        }

        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult> Login(LdapUser user)
        {
            //var token1 = await this.getToken(user);
            string signInUri = string.Concat(this.remoteServiceBaseUrl, "/api/Ldap/SignIn");
            var http = this.httpClientFactory.CreateClient();
            var response = await http.PostAsJsonAsync<LdapUser>(signInUri, user);
            //var token2 = await this.getToken(user);

            //var token = await this.HttpContext.GetTokenAsync("access_token");
            //Debug.WriteLine($"Token = {token}");

            return await Task.FromResult(this.View(user));
        }

        private async Task<string> getToken(LdapUser user)
        {
            string getTokenUri = string.Concat(this.remoteServiceBaseUrl, "/api/Ldap/GetToken");
            var http = this.httpClientFactory.CreateClient();
            var response = await http.GetAsync(getTokenUri);
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }
    }
}