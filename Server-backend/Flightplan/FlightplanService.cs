using Server_backend.Database;
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
        void SaveFlightplan(Flightplan flightplan);
    }

    public class FlightplanService : IFlightplanService
    {
        private readonly ICommandService cmdService;
        private readonly IFlightplanDatabaseService fpDbService;

        public FlightplanService(ICommandService _cmdService, IFlightplanDatabaseService _fpDbService)
        {
            this.cmdService = _cmdService;
            this.fpDbService = _fpDbService;
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
		
		public void SaveFlightplan(Flightplan flightplan)
		{
			throw new NotImplementedException();
		}
    }
}
