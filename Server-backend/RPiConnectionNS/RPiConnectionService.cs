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
        RPiConnection OfferRPiConnection(string ip, int port, string password);
    }

    public interface IRPiConnectionService : IRPiConnectionCommon
    {
        bool HandFlightplanToRPiConnection(int receiverRPiConnectionId, string flightplanName, int priority);
        RPiConnection SetRPiConnectionStatus(int rpiConnectionId, string status);

    }

    public class RPiConnection
    {
        public int rowId { get; set; }
        public Dictionary<int, string> userRights { get; set; }
        public string ip { get; set; }
        public int port { get; set; }
        public string status { get; set; }
        public string password { get; set; }
        public int userId { get; set; }
        public int lastTouch { get; set; }

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

        private int GetUserId()
        {
            //return 1;
            string user_id = this.auth.GetTokenClaim("user_id");
            int uid = 0;
            if (!Int32.TryParse(user_id, out uid))
            {
                throw new FormatException("Could not get user_id from token!");
            }
            return uid;
        }

        /**
         * Update last touch whenever the user_id is used, so we know the user is still online.
         */
        private void UpdateLastTouch()
        {
            this.rpiConDbService.UpdateLastTouch(this.GetUserId());
        }

        public RPiConnection GetRPiConnection(int rpiConnectionId)
        {
            this.rpiConDbService.DisconnectOldRPiConnections();
            this.UpdateLastTouch();
            this.rpiConDbService.DisconnectOldRPiConnections();
            RPiConnection rpiConnection = this.rpiConDbService.GetRPiConnection(rpiConnectionId);
            if(rpiConnection.userId == this.GetUserId())
            {
                rpiConnection.status = "connected";
            }
            return rpiConnection;
        }

        public List<RPiConnection> GetRPiConnections()
        {
            this.rpiConDbService.DisconnectOldRPiConnections();
            this.UpdateLastTouch();
            List<RPiConnection> rpiConnections = this.rpiConDbService.GetRPiConnections();
            rpiConnections.ForEach(rpiCon =>
            {
                if(rpiCon.userId == this.GetUserId())
                {
                    rpiCon.status = "connected";
                }
            });
            return rpiConnections;
        }

        public RPiConnection OfferRPiConnection(string ip, int port, string password)
        {
            return this.rpiConDbService.OfferRPiConnection(ip, port, password);
        }

        public RPiConnection SetRPiConnectionStatus(int rpiConnectionId, string status)
        {
            this.rpiConDbService.DisconnectOldRPiConnections();
            this.UpdateLastTouch();
            RPiConnection rpiConnection = this.rpiConDbService.SetRPiConnectionStatus(rpiConnectionId, status, this.GetUserId());
            if (rpiConnection.userId == this.GetUserId())
            {
                rpiConnection.status = "connected";
            }
            return rpiConnection;
        }

        public bool HandFlightplanToRPiConnection(int receiverRPiConnectionId, string flightplanName, int priority)
        {
            this.rpiConDbService.DisconnectOldRPiConnections();
            this.UpdateLastTouch();
            Flightplan fpToSend = this.fpService.GetFlightplan(flightplanName);
            RPiConnection receiverRPiConnection = this.GetRPiConnection(receiverRPiConnectionId);
            
            int uid = this.GetUserId();

            if (receiverRPiConnection.status == "disconnected")
            {
                Console.WriteLine("RPi not connected");
                return false;
            } else if(receiverRPiConnection.userId != uid)
            {
                Console.WriteLine("You have no connections!");
                return false;
            }

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

            string sendFlightplanResult = this.sendHttpService.SendPost("", ref flightplanModelForJava, receiverRPiConnection.password);
            if(!this.sendHttpService.DeserializeJsonString<bool>(sendFlightplanResult))
            {
                throw new TimeoutException("Could not send flightplan to RPi!");
            }

            string executeFlightplan = this.sendHttpService.SendGet("", receiverRPiConnection.password);
            if (!this.sendHttpService.DeserializeJsonString<bool>(executeFlightplan))
            {
                throw new TimeoutException("Could not execute flightplan at RPi!");
            }

            this.rpiConDbService.StartFlight(receiverRPiConnection.rowId, fpToSend.rowId, uid);
            
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
