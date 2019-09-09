using System;
using AspNetCore.IdentityServer4.WebApi.Utils;
using Newtonsoft.Json;

namespace AspNetCore.IdentityServer4.WebApi.Models
{
    public class UserV1
    {
        [ApiIgnore(EnableDeserializeOn = "POST")]
        public string Id { get; set; }

        [ApiIgnore(IgnoreDeserializeOn = "*")]
        public string Name { get; set; }

        [ApiIgnore(EnableDeserializeOn = "POST", IgnoreSerializeOn = "*")]
        public string Password { get; set; }

        [ApiIgnore(EnableDeserializeOn = "POST,PUT")]
        public string Email { get; set; }

        [ApiIgnore(IgnoreSerializeOn = "GET")]
        public DateTime? Birthday { get; set; }

        [ApiIgnore(IgnoreDeserializeOn = "*")]
        public int? Age { get; set; }

        [ApiIgnore(IgnoreDeserializeOn = "*")]
        public Metadata Metadata { get; set; }
    }

    public class UserV2
    {
        [ApiIgnore(EnableDeserializeOn = "POST,PUT,DELETE")]
        public string Id { get; set; }

        public string Name { get; set; }

        [ApiIgnore(EnableDeserializeOn = "POST,PUT", IgnoreSerializeOn = "*")]
        public string Password { get; set; }

        public string Email { get; set; }

        //public DateTime Birthday { get; set; }

        public int? Age { get; set; }

        [ApiIgnore(IgnoreDeserializeOn = "*", IgnoreSerializeOn = "*")]
        public Metadata Metadata { get; set; }
    }

    public class Metadata
    {
        [JsonIgnore]
        public int Id { get; set; }

        [ApiIgnore(IgnoreDeserializeOn = "*", EnableSerializeOn = "GET")]
        public DateTime CreateOn { get; set; } = DateTime.Now;

        [ApiIgnore(IgnoreDeserializeOn = "*", EnableSerializeOn = "DELETE")]
        public string Description { get; set; }
    }
}
