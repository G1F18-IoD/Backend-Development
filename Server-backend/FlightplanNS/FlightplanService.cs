using Server_backend.Database;
using Server_backend.utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server_backend.FlightplanNS
{
    /**
     * Holds the properties of what a Flightplan needs.
     */
    public class Flightplan
    {
        public Dictionary<int, Command> commands { get; set; }
        public int authorId { get; set; }
        public int createdAt { get; set; }
        public int rowId { get; set; }
        public string name { get; set; }

        public Flightplan()
        {
            this.commands = new Dictionary<int, Command>();
        }
    }

    public interface IFlightplanService
    {
        List<Flightplan> GetFlightplans();
        Flightplan GetFlightplan(int flightplanId);
        Flightplan GetFlightplan(string flightplanName);
        Flightplan CreateFlightplan(string name);
    }

    public class FlightplanService : IFlightplanService
    {
        private readonly ICommandService cmdService;
        private readonly IFlightplanDatabaseService fpDbService;
        private readonly IAuthenticationService auth;

        /**
         * Constructor for dependency injection.
         */
        public FlightplanService(ICommandService _cmdService, IFlightplanDatabaseService _fpDbService, IAuthenticationService _auth)
        {
            this.cmdService = _cmdService;
            this.fpDbService = _fpDbService;
            this.auth = _auth;
        }

        /**
         * Gets all the flightplans, along with getting the commands for each flightplan.
         */
        public List<Flightplan> GetFlightplans()
        {
            List<Flightplan> flightplans = this.fpDbService.GetFlightplans();
            foreach(Flightplan flightplan in flightplans)
            {
                flightplan.commands = this.cmdService.GetCommands(flightplan.rowId);
            }
            return flightplans;
        }

        /**
         * Gets a specific flightplan by id.
         */
        public Flightplan GetFlightplan(int flightplanId)
        {
            Flightplan flightplan = this.fpDbService.GetFlightplanInfo(flightplanId);
            flightplan.commands = this.cmdService.GetCommands(flightplan.rowId);
            return flightplan;
        }

        /**
         * Gets a specific flightplan by name.
         */
        public Flightplan GetFlightplan(string flightplanName)
        {
            Flightplan flightplan = this.fpDbService.GetFlightplanInfo(flightplanName);
            flightplan.commands = this.cmdService.GetCommands(flightplan.rowId);
            return flightplan;
        }

        /**
         * Finds the user's id and creates a new flightplan using it and a name given.
         */
        public Flightplan CreateFlightplan(string name)
        {
            string user_id = this.auth.GetTokenClaim("user_id");
            int uid = 0;
            if(!Int32.TryParse(user_id, out uid))
            {
                throw new FormatException("Could not get user_id from token!");
            }
            return this.fpDbService.CreateFlightplan(uid, name);
        }
    }
}
