using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Server_backend.FlightplanNS;
using Server_backend.utility;

namespace Server_backend.Controllers
{
    /**
     * The API controller to handle the flightplans.
     * URL: api/flightplan/...
     * Produces: JSON encoded data.
     */
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class FlightplanController : Controller
    {
        private readonly IFlightplanService flightplanService;
        private readonly ICommandService commandService;
        private readonly IAuthenticationService auth;
		
        /**
         * Constructor for dependency injection.
         */
		public FlightplanController(IFlightplanService _flightplanService, IAuthenticationService _auth, ICommandService _commandService)
        {
            this.flightplanService = _flightplanService;
            this.commandService = _commandService;
            this.auth = _auth;
		}

        /**
         * HTTP GET method for fetching all the possible flightplans.
         * Filter: Saving the authentication token.
         */
        [HttpGet]
        [ServiceFilter(typeof(SaveAuthenticationHeader))]
        public JsonResult Get()
        {
            if (!this.auth.ValidateToken(this.auth.GetToken(), out string authResponse))
            {
                this.HttpContext.Response.StatusCode = 401;
                return Json(new string[] { authResponse });
            }
            return Json(this.flightplanService.GetFlightplans());
        }

        /**
         * HTTP GET method for getting hardcoded settings.
         * Filter: Saving the authentication token.
         */
        [HttpGet("settings")]
        [ServiceFilter(typeof(SaveAuthenticationHeader))]
        public JsonResult GetSettings()
        {
            if (!this.auth.ValidateToken(this.auth.GetToken(), out string authResponse))
            {
                this.HttpContext.Response.StatusCode = 401;
                return Json(new string[] { authResponse });
            }
            return Json(new SettingsModel());
        }

        /**
         * HTTP GET method for fetching a specific flightplan by id.
         * Filter: Saving the authentication token.
         */
        [HttpGet("{id}")]
        [ServiceFilter(typeof(SaveAuthenticationHeader))]
        public JsonResult Get(int id)
        {
            if (!this.auth.ValidateToken(this.auth.GetToken(), out string authResponse))
            {
                this.HttpContext.Response.StatusCode = 401;
                return Json(new string[] { authResponse });
            }
            return Json(this.flightplanService.GetFlightplan(id));
        }

        /**
         * HTTP POST method for creating a flightplan.
         * Filter: Saving the authentication token.
         */
        [HttpPost]
        [ServiceFilter(typeof(SaveAuthenticationHeader))]
        public JsonResult Post([FromBody]FlightplanModel fpModel)
        {
            if (!this.auth.ValidateToken(this.auth.GetToken(), out string authResponse))
            {
                this.HttpContext.Response.StatusCode = 401;
                return Json(new string[] { authResponse });
            }
            return Json(this.flightplanService.CreateFlightplan(fpModel.name));
        }

        /**
         * HTTP POST method for adding a command to a flightplan based on the id of the flightplan.
         * Filter: Saving the authentication token.
         */
        [HttpPost("cmd/{id}")]
        [ServiceFilter(typeof(SaveAuthenticationHeader))]
        public JsonResult PostCmd(int id, [FromBody]CommandModel cmd)
        {
            if (!this.auth.ValidateToken(this.auth.GetToken(), out string authResponse))
            {
                this.HttpContext.Response.StatusCode = 401;
                return Json(new string[] { authResponse });
            }
            Command command = new Command();
            command.CmdString = cmd.cmd;
            command.Message = cmd.message;
            command.Params.AddRange(cmd.parameters);
            command.Order = cmd.order;
            command.FlightplanId = id;
            return Json(this.commandService.SaveCommand(command));
        }
    }

    /**
     * The API controller to handling commands.
     * URL: api/flightplan/...
     * Produces: JSON encoded data.
     */
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class CommandController : Controller
    {
        private readonly ICommandService commandService;
        private readonly IAuthenticationService auth;

        /**
         * Constructor for dependency injection.
         */
        public CommandController(ICommandService _commandService, IAuthenticationService _auth)
        {
            this.commandService = _commandService;
            this.auth = _auth;
        }

        /**
         * HTTP GET method for fetching a specific command based on the row id of the command.
         * Filter: Saving the authentication token.
         */
        [HttpGet("{id}")]
        [ServiceFilter(typeof(SaveAuthenticationHeader))]
        public JsonResult Get(int id)
        {
            if (!this.auth.ValidateToken(this.auth.GetToken(), out string authResponse))
            {
                this.HttpContext.Response.StatusCode = 401;
                return Json(new string[] { authResponse });
            }
            return Json(this.commandService.GetCommand(id));
        }

        /**
         * HTTP GET method for fetching all the different commands possible.
         * Filter: Saving the authentication token.
         */
        [HttpGet]
        [ServiceFilter(typeof(SaveAuthenticationHeader))]
        public JsonResult GetPossibleCommands()
        {
            if (!this.auth.ValidateToken(this.auth.GetToken(), out string authResponse))
            {
                this.HttpContext.Response.StatusCode = 401;
                return Json(new string[] { authResponse });
            }
            return Json(this.commandService.GetPossibleCommands());
        }
    }

    /**
     * Class containing the required info for creating a flightplan.
     */
    public class FlightplanModel
    {
        public string name { get; set; }
    }

    /**
     * Class containing the required info for adding a command to a flightplan.
     */
    public class CommandModel
    {
        public string cmd { get; set; }
        public string message { get; set; }
        public List<int> parameters { get; set; }
        public int order { get; set; }
    }

    /**
     * Class containing the hardcoded settings. This should probably be fetched from the service instead.
     */
    public class SettingsModel
    {
        public int CommandPerFlightplanCount = 25;
    }
}