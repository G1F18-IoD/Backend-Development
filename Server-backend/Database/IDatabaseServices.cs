using System.Collections.Generic;
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
        Flightplan GetFlightplanInfo(string flightplanName);
        List<Flightplan> GetFlightplans();
        Flightplan CreateFlightplan(int authorId, string name);
    }

    public interface IRPiConnectionDatabaseService : IRPiConnectionCommon
    {
        bool StartFlight(int rpiConnectionId, int flightplanId, int userId);
        RPiConnection SetRPiConnectionStatus(int rpiConnectionId, string status, int userId);
        void DisconnectOldRPiConnections();
        void UpdateLastTouch(int userId);
    }


}
