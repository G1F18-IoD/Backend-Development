using Server_backend.Database;
using Server_backend.utility;
using Server_backend.FlightplanNS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server_backend.RPiConnectionNS
{
    /**
     * An interface to contain what both the service and database service should be able to do. This removes duplicates in the interfaces, and actually helps ensure data integrity between the layers.
     */
    public interface IRPiConnectionCommon
    {
        List<RPiConnection> GetRPiConnections();
        RPiConnection GetRPiConnection(int rpiConnectionId);
        RPiConnection OfferRPiConnection(string ip, int port, string password);
    }

    /**
     * 
     */
    public interface IRPiConnectionService : IRPiConnectionCommon
    {
        bool HandFlightplanToRPiConnection(int receiverRPiConnectionId, string flightplanName, int priority);
        RPiConnection SetRPiConnectionStatus(int rpiConnectionId, string status);

    }

    /**
     * Holds the properties of what an RPi connection needs.
     */
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

        /**
         * Since this piece of code was used several places in the class, it was decided to put it into its own method.
         * It basically just asks the authentication service for the user id in the token.
         */
        private int GetUserId()
        {
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
         * Along with first deleting too old RPi connections. This makes sure that the connections expire for whenever a user requests the status of a RPi connection.
         * This is instead of having some automatic clean up that would run every so often to remove expired connections.
         */
        private void UpdateLastTouch()
        {
            this.rpiConDbService.DisconnectOldRPiConnections();
            this.rpiConDbService.UpdateLastTouch(this.GetUserId());
        }

        /**
         * Gets a specific RPi connection, and changes its status to connected if you are connected to it.
         */
        public RPiConnection GetRPiConnection(int rpiConnectionId)
        {
            this.UpdateLastTouch();
            RPiConnection rpiConnection = this.rpiConDbService.GetRPiConnection(rpiConnectionId);
            if(rpiConnection.userId == this.GetUserId())
            {
                rpiConnection.status = "connected";
            }
            return rpiConnection;
        }

        /**
         * Gets all of the RPi connections and tells you which the user is currently connected to.
         */
        public List<RPiConnection> GetRPiConnections()
        {
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

        /**
         * Basically creating a new RPi connection for people to use.
         */
        public RPiConnection OfferRPiConnection(string ip, int port, string password)
        {
            return this.rpiConDbService.OfferRPiConnection(ip, port, password);
        }

        /**
         * Sets the status of a specific RPi, and returns it's information afterwards.
         */
        public RPiConnection SetRPiConnectionStatus(int rpiConnectionId, string status)
        {
            this.UpdateLastTouch();
            RPiConnection rpiConnection = this.rpiConDbService.SetRPiConnectionStatus(rpiConnectionId, status, this.GetUserId());
            if (rpiConnection.userId == this.GetUserId())
            {
                rpiConnection.status = "connected";
            }
            return rpiConnection;
        }

        /**
         * This method executes a flightplan at a RPi connection. This basically means that the method does:
         * Get the proper information needed, like the RPi connection object and the Flightplan object.
         * Then it validates that you are connected to the RPi connection.
         * Then it formats the classes (found below this class), so that it is readable by the RPi backend.
         * It then starts a flight in this system.
         * Then it sends the flightplan to the RPi backend.
         * It then asks the RPi connection to execute the flightplan.
         */
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

            // Start flight
            this.rpiConDbService.StartFlight(receiverRPiConnection.rowId, fpToSend.rowId, uid);

            string urlIpPort = "http://" + receiverRPiConnection.ip + ":" + receiverRPiConnection.port;

            string urlStoreFlightplan = urlIpPort + "/api/flightplan/store/";
            string sendFlightplanResult = this.sendHttpService.SendPost(urlStoreFlightplan, ref flightplanModelForJava, receiverRPiConnection.password);
            if(!this.sendHttpService.DeserializeJsonString<bool>(sendFlightplanResult))
            {
                throw new TimeoutException("Could not send flightplan to RPi!");
            }

            // TODO: Log to flight that flightplan has been sent

            string urlExecuteFlightplan = urlIpPort + "/api/flightplan/execute/";
            EmptyModel emptyModel = new EmptyModel();
            string executeFlightplan = this.sendHttpService.SendPost(urlExecuteFlightplan, ref emptyModel, receiverRPiConnection.password);
            if (!this.sendHttpService.DeserializeJsonString<bool>(executeFlightplan))
            {
                throw new TimeoutException("Could not execute flightplan at RPi!");
            }

            // TODO: Log to flight that flightplan has been executed


            return true;
        }
    }

    /**
     * A class that holds the correct formatting of what the RPi backend requires in its POST requests.
     */
    public class FlightplanModelForJava
    {
        public int created_at { get; set; }
        public int priority { get; set; }
        private int cmd_delay = 60000;
        public List<CommandModelForJava> commands { get; set; }
    }

    /**
     * A class that holds the correct formatting of what the RPi backend requires in its POST requests.
     */
    public class CommandModelForJava
    {
        public int cmd_id { get; set; }
        public List<int> parameters { get; set; }
    }

    /**
     * Since our SendPost method requires a class to put in the body, this class lets us send no data at all, but still send a class to put in the body.
     */
    public class EmptyModel
    {

    }
}
