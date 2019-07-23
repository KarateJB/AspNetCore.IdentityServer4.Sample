namespace AspNetCore.IdentityServer4.WebApi.Models
{
    public class User
    {
        public string Uid { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string AccessToken { get; set; }
    }
}
