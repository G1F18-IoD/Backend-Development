﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Server_backend.FlightplanNS;
using Server_backend.utility;

namespace Server_backend.Controllers
{
    [Route("api/[controller]")]
    public class FlightplanController : Controller
    {
        private readonly IFlightplanService flightplanService;
        private readonly ICommandService commandService;
        private readonly IAuthenticationService auth;
		
		public FlightplanController(IFlightplanService _flightplanService, IAuthenticationService _auth, ICommandService _commandService)
        {
            this.flightplanService = _flightplanService;
            this.commandService = _commandService;
            this.auth = _auth;
		}
		
        // GET api/values
        [HttpGet]
        public List<Flightplan> Get()
        {
            return this.flightplanService.GetFlightplans();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public Flightplan Get(int id)
        {
            return this.flightplanService.GetFlightplan(id);
        }

        // POST api/values
        [HttpPost]
        [ServiceFilter(typeof(SaveAuthenticationHeader))]
        public Flightplan Post()
        {
            return this.flightplanService.CreateFlightplan();
        }
        
        [HttpPost("cmd/{id}")]
        public Command Put(int id, [FromBody]CommandModel cmd)
        {
            Command command = new Command();
            command.CmdString = cmd.cmd;
            command.Message = cmd.message;
            command.Params.AddRange(cmd.parameters);
            command.Order = cmd.order;
            command.FlightplanId = id;
            return this.commandService.SaveCommand(command);
        }

        /*
        // DELETE api/values/5
        [HttpPost("delete/{id}")]
        public void Delete(int id)
        {
        }*/
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
        public Command Get(int id)
        {
            return this.commandService.GetCommand(id);
        }
    }

    public class FlightplanModel
    {

    }

    public class CommandModel
    {
        public string cmd { get; set; }
        public string message { get; set; }
        public List<string> parameters { get; set; }
        public int order { get; set; }
    }
}

// https://stackoverflow.com/questions/46744561/how-to-do-simple-header-authorization-in-net-core-2-0
// https://andrewlock.net/adding-default-security-headers-in-asp-net-core/