using Server_backend.Database;
using Server_backend.utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server_backend.FlightplanNS
{
    public class Flightplan
    {
        public Dictionary<int, Command> commands { get; set; }
        public int authorId { get; set; }
        public int createdAt { get; set; }
        public int rowId { get; set; }

        public Flightplan()
        {
            this.commands = new Dictionary<int, Command>();
        }
    }

    public interface IFlightplanService
    {
        List<Flightplan> GetFlightplans();
        Flightplan GetFlightplan(int flightplanId);
        Flightplan CreateFlightplan();
    }

    public class FlightplanService : IFlightplanService
    {
        private readonly ICommandService cmdService;
        private readonly IFlightplanDatabaseService fpDbService;
        private readonly IAuthenticationService auth;

        public FlightplanService(ICommandService _cmdService, IFlightplanDatabaseService _fpDbService, IAuthenticationService _auth)
        {
            this.cmdService = _cmdService;
            this.fpDbService = _fpDbService;
            this.auth = _auth;
        }

        public List<Flightplan> GetFlightplans()
        {
            List<Flightplan> flightplans = this.fpDbService.GetFlightplans();
            foreach(Flightplan flightplan in flightplans)
            {
                flightplan.commands = this.cmdService.GetCommands(flightplan.rowId);
            }
            return flightplans;
        }

		public Flightplan GetFlightplan(int flightplanId)
		{
            return this.fpDbService.GetFlightplanInfo(flightplanId);
		}

        public Flightplan CreateFlightplan()
        {
            string user_id = this.auth.GetTokenClaim("user_id");
            int uid = 0;
            if(!Int32.TryParse(user_id, out uid))
            {
                throw new FormatException("Could not get user_id from token!");
            }
            return this.fpDbService.CreateFlightplan(uid);
        }
    }
}
