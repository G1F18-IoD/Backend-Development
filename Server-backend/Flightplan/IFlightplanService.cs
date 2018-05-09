using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server_backend.Flightplan
{
    interface IFlightplanService
    {
        List<Command> GetPossibleCommands();
        List<Command> GetCommands(int flightplanId);

		Flightplan GetFlightplan(int flightplanId);
		void SaveFlightplan(Flightplan flightplan);
    }
}
