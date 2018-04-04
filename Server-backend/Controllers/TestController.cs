using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Server_backend.Controllers
{
    [Produces("application/json")]
    [Route("api/Test")]
    public class TestController : Controller
    {
        [HttpGet]
        public StatusCodeResult Get()
        {
            return StatusCode(201);
        }

        [HttpGet("{name}")]
        public string Get(string name)
        {
            return "Hello " + name + ".";
        }

        [HttpGet("{name}/{id}/{msg}")]
        public IEnumerable<string> Get(string name, int id, string msg)
        {
            return new string[] { name, id + "", msg };
        }
    }
}