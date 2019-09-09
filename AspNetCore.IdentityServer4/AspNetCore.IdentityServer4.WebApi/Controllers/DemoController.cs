using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AspNetCore.IdentityServer4.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore.IdentityServer4.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DemoController : ControllerBase
    {
        private readonly UserV1 fooUserV1 = null;
        private readonly UserV2 fooUserV2 = null;

        public DemoController()
        {
            this.fooUserV1 = new UserV1
            {
                Id = "11111",
                Name = "JB1",
                Password = "123456",
                Email = "jb1@foo.com",
                Birthday = new DateTime(year: 2000, month: 12, day: 31),
                Age = 30,
                Metadata = new Metadata()
                {
                    Id = 1,
                    CreateOn = DateTime.Now,
                    Description = "I am JB1",
                }
            };

            this.fooUserV2 = new UserV2
            {
                Id = "22222",
                Name = "JB2",
                Password = "123456",
                Email = "jb2@foo.com",
                Age = 31,
                Metadata = new Metadata()
                {
                    Id = 2,
                    CreateOn = DateTime.Now,
                    Description = "I am JB2",
                }
            };
        }

        [HttpGet("GetV1")]
        public ActionResult<UserV1> GetV1()
        {
            return this.fooUserV1;
        }

        [HttpGet("GetV2")]
        public ActionResult<UserV2> GetV2()
        {
            return this.fooUserV2;
        }

        [HttpPost("CreateV1")]
        public ActionResult<UserV1> Post([FromBody] UserV1 user)
        {
            Debug.WriteLine($"Id validate : {!string.IsNullOrEmpty(user.Id)}");
            Debug.WriteLine($"Name validate : {string.IsNullOrEmpty(user.Name)}");
            Debug.WriteLine($"Password validate : {!string.IsNullOrEmpty(user.Password)}");
            Debug.WriteLine($"Email validate : {!string.IsNullOrEmpty(user.Email)}");
            Debug.WriteLine($"Birthday validate : {user.Birthday!=null}");
            Debug.WriteLine($"Age validate : {!user.Age.HasValue}");
            Debug.WriteLine($"Metadata validate : {user.Metadata == null}");

            return this.fooUserV1;
        }

        [HttpPost("CreateV2")]
        public ActionResult<UserV2> Post([FromBody] UserV2 user)
        {
            Debug.WriteLine($"Id validate : {!string.IsNullOrEmpty(user.Id)}");
            Debug.WriteLine($"Name validate : {!string.IsNullOrEmpty(user.Name)}");
            Debug.WriteLine($"Password validate : {!string.IsNullOrEmpty(user.Password)}");
            Debug.WriteLine($"Email validate : {!string.IsNullOrEmpty(user.Email)}");
            Debug.WriteLine($"Age validate : {user.Age.HasValue}");
            Debug.WriteLine($"Metadata validate : {user.Metadata == null}");

            return this.fooUserV2;
        }

        [HttpPut("UpdateV1")]
        public ActionResult<UserV1> Put([FromBody] UserV1 user)
        {
            Debug.WriteLine($"Id validate : {string.IsNullOrEmpty(user.Id)}");
            Debug.WriteLine($"Name validate : {string.IsNullOrEmpty(user.Name)}");
            Debug.WriteLine($"Password validate : {string.IsNullOrEmpty(user.Password)}");
            Debug.WriteLine($"Email validate : {!string.IsNullOrEmpty(user.Email)}");
            Debug.WriteLine($"Birthday validate : {user.Birthday != null}");
            Debug.WriteLine($"Age validate : {!user.Age.HasValue}");
            Debug.WriteLine($"Metadata validate : {user.Metadata == null}");

            return this.fooUserV1;
        }

        [HttpPut("UpdateV2")]
        public ActionResult<UserV2> Put([FromBody] UserV2 user)
        {
            Debug.WriteLine($"Id validate : {!string.IsNullOrEmpty(user.Id)}");
            Debug.WriteLine($"Name validate : {!string.IsNullOrEmpty(user.Name)}");
            Debug.WriteLine($"Password validate : {!string.IsNullOrEmpty(user.Password)}");
            Debug.WriteLine($"Email validate : {!string.IsNullOrEmpty(user.Email)}");
            Debug.WriteLine($"Age validate : {user.Age.HasValue}");
            Debug.WriteLine($"Metadata validate : {user.Metadata == null}");

            return this.fooUserV2;
        }

        [HttpDelete("DeleteV1")]
        public ActionResult<UserV1> Delete([FromBody] UserV1 user)
        {
            Debug.WriteLine($"Id validate : {string.IsNullOrEmpty(user.Id)}");
            Debug.WriteLine($"Name validate : {string.IsNullOrEmpty(user.Name)}");
            Debug.WriteLine($"Password validate : {string.IsNullOrEmpty(user.Password)}");
            Debug.WriteLine($"Email validate : {string.IsNullOrEmpty(user.Email)}");
            Debug.WriteLine($"Birthday validate : {user.Birthday != null}");
            Debug.WriteLine($"Age validate : {!user.Age.HasValue}");
            Debug.WriteLine($"Metadata validate : {user.Metadata == null}");

            return this.fooUserV1;
        }

        [HttpDelete("DeleteV2")]
        public ActionResult<UserV2> Delete([FromBody] UserV2 user)
        {
            Debug.WriteLine($"Id validate : {!string.IsNullOrEmpty(user.Id)}");
            Debug.WriteLine($"Name validate : {!string.IsNullOrEmpty(user.Name)}");
            Debug.WriteLine($"Password validate : {string.IsNullOrEmpty(user.Password)}");
            Debug.WriteLine($"Email validate : {!string.IsNullOrEmpty(user.Email)}");
            Debug.WriteLine($"Age validate : {user.Age.HasValue}");
            Debug.WriteLine($"Metadata validate : {user.Metadata == null}");

            return this.fooUserV2;
        }
    }
}
