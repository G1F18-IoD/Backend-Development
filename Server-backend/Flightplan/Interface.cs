using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server_backend.Flightplan
{
    interface IFlightplan
    {
        List<Command> GetPossibleCommands();
        List<Command> GetCommands(int flightplanId);
        void AddCommand(Command cmd);
    }
}
