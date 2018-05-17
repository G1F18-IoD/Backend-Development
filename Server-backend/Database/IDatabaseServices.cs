﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server_backend.FlightplanNS;
using Server_backend.RPiConnectionNS;

namespace Server_backend.Database
{
    public interface IAuthenticationDatabaseService
    {
        int Login(string username, string password);
        int Register(string username, string password);
    }

    public interface ICommandDatabaseService : ICommandCommon
    {

    }

    public interface IFlightplanDatabaseService
    {
        Flightplan GetFlightplanInfo(int flightplanId);
        List<Flightplan> GetFlightplans();
        Flightplan CreateFlightplan(int authorId, string name);
    }

    public interface IRPiConnectionDatabaseService : IRPiConnectionCommon
    {
        bool StartFlight(int rpiConnectionId, int flightplanId);
    }


}
