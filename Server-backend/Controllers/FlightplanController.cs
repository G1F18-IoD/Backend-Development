using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Server_backend.Flightplan;
using Server_backend.utility;

namespace Server_backend.Controllers
{
    [Route("api/[controller]")]
    public class FlightplanController : Controller
    {
		private readonly IFlightplanService flightplanService;
        private readonly IAuthenticationService auth;
		
		public FlightplanController(IFlightplanService _flightplanService, IAuthenticationService _auth)
		{
			this.flightplanService = _flightplanService;
            this.auth = _auth;
		}
		
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            this.auth.ValidateToken(".");
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
        public void Post([FromBody]string value)
        {
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

    [Route("api/[controller]")]
    public class CommandController : Controller
    {
        private readonly ICommandService commandService;
        private readonly IAuthenticationService auth;

        public CommandController(ICommandService _commandService, IAuthenticationService _auth)
        {
            this.commandService = _commandService;
            this.auth = _auth;
        }

        [HttpGet("{id}")]
        public string Get(int id)
        {
            return this.commandService.GetCommand(id).CmdString;
        }
    }
}
