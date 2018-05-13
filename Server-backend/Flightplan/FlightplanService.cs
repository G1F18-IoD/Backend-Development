using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server_backend.Flightplan
{
    public class Flightplan
    {
        Dictionary<int, Command> commands = new Dictionary<int, Command>();

        Flightplan()
        {

        }
    }

    public interface IFlightplanService
    {
        List<Command> GetPossibleCommands();
        List<Command> GetCommands(int flightplanId);

        Flightplan GetFlightplan(int flightplanId);
        void SaveFlightplan(Flightplan flightplan);
    }

    public class FlightplanService : IFlightplanService
    {
        public List<Command> GetCommands(int flightplanId)
        {
            throw new NotImplementedException();
        }

        public List<Command> GetPossibleCommands()
        {
            throw new NotImplementedException();
        }
		
		public Flightplan GetFlightplan(int flightplanId)
		{
			throw new NotImplementedException();
		}
		
		public void SaveFlightplan(Flightplan flightplan)
		{
			throw new NotImplementedException();
		}
    }
}
