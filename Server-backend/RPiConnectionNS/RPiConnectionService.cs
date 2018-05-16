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
        RPiConnection OfferRPiConnection(string ip, int port, string password);
    }

    public interface IRPiConnectionService : IRPiConnectionCommon
    {
        bool HandFlightplanToRPiConnection(int receiverRPiConnectionId, int flightplanId, int priority);
    }

    public class RPiConnection
    {
        public int rowId { get; set; }
        public Dictionary<int, string> userRights { get; set; }
        public string ip { get; set; }
        public int port { get; set; }
        public string status { get; set; }
        public string password { get; set; }

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
        private readonly ISendHttpService sendHttpService;

        public RPiConnectionService(IAuthenticationService _auth, IRPiConnectionDatabaseService _rpiConDbService, IFlightplanService _fpService, ISendHttpService _sendHttpService)
        {
            this.auth = _auth;
            this.rpiConDbService = _rpiConDbService;
            this.fpService = _fpService;
            this.sendHttpService = _sendHttpService;
        }
        public RPiConnection GetRPiConnection(int rpiConnectionId)
        {
            return this.rpiConDbService.GetRPiConnection(rpiConnectionId);
        }

        public List<RPiConnection> GetRPiConnections()
        {
            return this.rpiConDbService.GetRPiConnections();
        }

        public RPiConnection OfferRPiConnection(string ip, int port, string password)
        {
            return this.rpiConDbService.OfferRPiConnection(ip, port, password);
        }

        public RPiConnection SetRPiConnectionStatus(int rpiConnectionId, string status)
        {
            return this.rpiConDbService.SetRPiConnectionStatus(rpiConnectionId, status);
        }

        public bool HandFlightplanToRPiConnection(int receiverRPiConnectionId, int flightplanId, int priority)
        {
            Flightplan fpToSend = this.fpService.GetFlightplan(flightplanId);
            RPiConnection receiverRPiConnection = this.GetRPiConnection(receiverRPiConnectionId);

            FlightplanModelForJava flightplanModelForJava = new FlightplanModelForJava();
            flightplanModelForJava.auth_token = receiverRPiConnection.password;
            flightplanModelForJava.author_id = fpToSend.authorId;
            flightplanModelForJava.created_at = fpToSend.createdAt;
            flightplanModelForJava.priority = priority;
            flightplanModelForJava.commands = new List<CommandModelForJava>();
            foreach (KeyValuePair<int, Command> entry in fpToSend.commands)
            {
                CommandModelForJava commandModelForJava = new CommandModelForJava();
                commandModelForJava.cmd_id = entry.Value.CmdId;
                commandModelForJava.parameters = entry.Value.Params;
                flightplanModelForJava.commands.Insert(entry.Key, commandModelForJava);
            }

            this.sendHttpService.SendPost("", ref flightplanModelForJava);
            return true;
        }
    }

    public class FlightplanModelForJava
    {
        public string auth_token { get; set; }
        public int author_id { get; set; }
        public int created_at { get; set; }
        public int priority { get; set; }
        public List<CommandModelForJava> commands { get; set; }
    }

    public class CommandModelForJava
    {
        public int cmd_id { get; set; }
        public List<int> parameters { get; set; }
    }
}
