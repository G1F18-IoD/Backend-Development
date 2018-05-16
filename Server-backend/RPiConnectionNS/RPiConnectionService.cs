using Server_backend.Database;
using Server_backend.utility;
using Server_backend.FlightplanNS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server_backend.RPiConnectionNS
{
    public interface IRPiConnectionCommon
    {
        List<RPiConnection> GetRPiConnections();
        RPiConnection GetRPiConnection(int rpiConnectionId);
        RPiConnection SetRPiConnectionStatus(int rpiConnectionId, string status);
        RPiConnection OfferRPiConnection(string ip, int port);
    }

    public interface IRPiConnectionService : IRPiConnectionCommon
    {
        bool HandFlightplanToRPiConnection(int receiverRPiConnectionId, int flightplanId);
    }

    public class RPiConnection
    {
        public int rowId { get; set; }
        public Dictionary<int, string> userRights { get; set; }
        public string ip { get; set; }
        public int port { get; set; }
        public string status { get; set; }

        public RPiConnection()
        {
            this.userRights = new Dictionary<int, string>();
        }
    }

    public class RPiConnectionService : IRPiConnectionService
    {
        private readonly IAuthenticationService auth;
        private readonly IRPiConnectionDatabaseService rpiConDbService;
        private readonly IFlightplanService fpService;

        public RPiConnectionService(IAuthenticationService _auth, IRPiConnectionDatabaseService _rpiConDbService, IFlightplanService _fpService)
        {
            this.auth = _auth;
            this.rpiConDbService = _rpiConDbService;
            this.fpService = _fpService;
        }
        public RPiConnection GetRPiConnection(int rpiConnectionId)
        {
            return this.rpiConDbService.GetRPiConnection(rpiConnectionId);
        }

        public List<RPiConnection> GetRPiConnections()
        {
            return this.rpiConDbService.GetRPiConnections();
        }

        public RPiConnection OfferRPiConnection(string ip, int port)
        {
            return this.rpiConDbService.OfferRPiConnection(ip, port);
        }

        public RPiConnection SetRPiConnectionStatus(int rpiConnectionId, string status)
        {
            return this.rpiConDbService.SetRPiConnectionStatus(rpiConnectionId, status);
        }

        public bool HandFlightplanToRPiConnection(int receiverRPiConnectionId, int flightplanId)
        {
            Flightplan fpToSend = this.fpService.GetFlightplan(flightplanId);
            RPiConnection receiverRPiConnection = this.GetRPiConnection(receiverRPiConnectionId);
            return true;
        }
    }
}
