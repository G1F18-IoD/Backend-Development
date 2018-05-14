using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Web.Http.Cors;
using Server_backend.utility;

namespace Server_backend.Controllers
{
    [Route("api/[controller]")]
    //[EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ValuesController : Controller
    {
        // GET api/values
        [HttpGet]
        
        //[ServiceFilter(typeof(SampleAsyncActionFilter))]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public string Post([FromBody]string value)
        {
            return "agger er nub";
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
