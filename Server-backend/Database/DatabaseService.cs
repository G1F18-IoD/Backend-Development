using Newtonsoft.Json.Linq;
using Npgsql;
using Server_backend.FlightplanNS;
using Server_backend.RPiConnectionNS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using System.Web.Script.Serialization;

namespace Server_backend.Database
{
    public class DatabaseService : IAuthenticationDatabaseService, ICommandDatabaseService, IFlightplanDatabaseService, IRPiConnectionDatabaseService
    {
        private readonly NpgsqlConnection npgSqlCon;
        private readonly INpgSqlConnection npgSqlConnectionService;

        public DatabaseService(INpgSqlConnection connection)
        {
            this.npgSqlCon = connection.GetCon();
            this.npgSqlConnectionService = connection;
        }

        public int Login(string username, string password)
        {
            int id = -1;
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = this.npgSqlCon;
                cmd.CommandText = "SELECT id FROM public.account WHERE lower(username)=lower(@u) AND password=(@p)";
                cmd.Parameters.AddWithValue("@u", username);
                cmd.Parameters.AddWithValue("@p", password);
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    id = reader.GetInt32(0);
                }
            }
            return id;
        }

        public int Register(string username, string password)
        {
            int count = 0;
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = this.npgSqlCon;
                Console.WriteLine("Con:" + this.npgSqlCon == null);
                Console.WriteLine("CMD:" + cmd == null);
                cmd.CommandText = "INSERT INTO public.account (username, password) VALUES ((@u), (@p))";
                cmd.Parameters.AddWithValue("@u", username);
                cmd.Parameters.AddWithValue("@p", password);
                count = cmd.ExecuteNonQuery();
            }
            return count;
        }

        public Command GetCommand(int CommandId)
        {
            Command retObj = new Command();
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = this.npgSqlCon;
                cmd.CommandText = "SELECT id, flightplan_id, cmd, message, payload, \"order\" FROM public.flightplan_commands WHERE id=(@cmdId)";
                cmd.Parameters.AddWithValue("@cmdId", CommandId);
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    retObj.RowId = reader.GetInt32(0);
                    retObj.FlightplanId = reader.GetInt32(1);
                    retObj.CmdString = reader.GetString(2);
                    retObj.Message = reader.IsDBNull(3) ? "" : reader.GetString(3);
                    retObj.Order = reader.GetInt32(5);
                    
                    // Turns the JSON encoded array from the DB into an array readable by C#
                    JArray ja = JArray.Parse(reader.GetString(4));
                    string[] Params = ja.Select(jv => (string)jv).ToArray();
                    int count = 0;
                    foreach (string Param in Params)
                    {
                        retObj.Params.Insert(count++, Int32.Parse(Param));
                    }
                }
            }
            return retObj;
        }

        public Dictionary<int, Command> GetCommands(int flightplanId)
        {
            Dictionary<int, Command> cmds = new Dictionary<int, Command>();
            using (NpgsqlConnection npgsqlConnection = DatabaseConnection.GetStaticCon())
            {
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = npgsqlConnection;
                    cmd.CommandText = "SELECT id, flightplan_id, cmd::varchar, message, payload, \"order\" FROM public.flightplan_commands WHERE flightplan_id=(@fpid) ORDER BY \"order\" ASC, id ASC";
                    cmd.Parameters.AddWithValue("@fpid", flightplanId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Command command = new Command();
                            command.RowId = reader.GetInt32(0);
                            command.FlightplanId = reader.GetInt32(1);
                            command.CmdString = reader.GetString(2);
                            command.Message = reader.IsDBNull(3) ? "" : reader.GetString(3);
                            command.Order = reader.GetInt32(5);

                            // Turns the JSON encoded array from the DB into an array readable by C#
                            JArray ja = JArray.Parse(reader.GetString(4));
                            string[] Params = ja.Select(jv => (string)jv).ToArray();
                            int count = 0;
                            foreach (string Param in Params)
                            {
                                int item = 0;
                                if (!Int32.TryParse(Param, out item))
                                {
                                    item = 0;
                                }
                                command.Params.Insert(count++, item);
                            }
                            cmds.Add(cmds.Count, command);
                        }
                    }
                }
            }
            return cmds;
        }

        public Command SaveCommand(Command command)
        {
            int cmdId = -1;
            using (NpgsqlConnection npgsqlConnection = DatabaseConnection.GetStaticCon())
            {
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = npgsqlConnection;
                    cmd.CommandText = "INSERT INTO public.flightplan_commands (flightplan_id, cmd, message, payload, \"order\") VALUES (@fpid, @cmd::possible_commands, @message, array_to_json(@params), @order) RETURNING id;";
                    cmd.Parameters.AddWithValue("@fpid", command.FlightplanId);
                    cmd.Parameters.AddWithValue("@cmd", command.CmdString);
                    // Inserts NULL into the message if the command's message is empty.
                    if (command.Message.Length <= 0)
                    {
                        cmd.Parameters.AddWithValue("@message", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@message", command.Message);
                    }

                    cmd.Parameters.AddWithValue("@params", command.Params);
                    cmd.Parameters.AddWithValue("@order", command.Order);
                    using (var reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        cmdId = reader.GetInt32(0);
                    }
                }

            }
            if (cmdId <= 0)
            {
                throw new NpgsqlException("Something went wrong with inserting a command!");
            }
            return this.GetCommand(cmdId);
        }

        public Flightplan GetFlightplanInfo(int flightplanId)
        {
            Flightplan flightplan = new Flightplan();
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = this.npgSqlCon;
                cmd.CommandText = "SELECT id, \"name\", author, created_at FROM public.flightplan WHERE id=(@fpid)";
                cmd.Parameters.AddWithValue("@fpid", flightplanId);
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();

                    flightplan.rowId = reader.GetInt32(0);
                    flightplan.name = reader.GetString(1);
                    flightplan.authorId = reader.GetInt32(2);
                    flightplan.createdAt = reader.GetInt32(3);
                }
            }
            return flightplan;
        }

        public Flightplan GetFlightplanInfo(string flightplanName)
        {
            Flightplan flightplan = new Flightplan();
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = this.npgSqlCon;
                cmd.CommandText = "SELECT id, \"name\", author, created_at FROM public.flightplan WHERE name=(@fpname)";
                cmd.Parameters.AddWithValue("@fpname", flightplanName);
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();

                    flightplan.rowId = reader.GetInt32(0);
                    flightplan.name = reader.GetString(1);
                    flightplan.authorId = reader.GetInt32(2);
                    flightplan.createdAt = reader.GetInt32(3);
                }
            }
            return flightplan;
        }

        public List<Flightplan> GetFlightplans()
        {
            List<Flightplan> flightplans = new List<Flightplan>();
            using (NpgsqlConnection npgsqlConnection = DatabaseConnection.GetStaticCon())
            {
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = npgsqlConnection;
                    cmd.CommandText = "SELECT id, author, created_at, \"name\" FROM public.flightplan";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Flightplan flightplan = new Flightplan();
                            flightplan.rowId = reader.GetInt32(0);
                            flightplan.authorId = reader.GetInt32(1);
                            flightplan.createdAt = reader.GetInt32(2);
                            flightplan.name = reader.GetString(3);
                            flightplans.Add(flightplan);
                        }
                    }
                }
                npgsqlConnection.Close();
            }
            return flightplans;
        }

        public Flightplan CreateFlightplan(int authorId, string name)
        {
            int flightplanId = -1;

            using (NpgsqlConnection npgsqlConnection = DatabaseConnection.GetStaticCon())
            {
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = npgsqlConnection;
                    cmd.CommandText = "INSERT INTO public.flightplan (\"name\",author) VALUES (@name, @authorId) RETURNING id;";
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@authorId", authorId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        flightplanId = reader.GetInt32(0);
                    }
                }
                if (flightplanId <= 0)
                {
                    throw new NpgsqlException("Something went wrong with inserting a flightplan!");
                }
            }
            return this.GetFlightplanInfo(flightplanId);
        }

        public void DisconnectOldRPiConnections()
        {
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = this.npgSqlCon;
                cmd.CommandText = "UPDATE public.rpi_connection SET user_id_connected = NULL WHERE last_touch < (select extract(epoch from now()))-600";
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateLastTouch(int userid)
        {
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = this.npgSqlCon;
                cmd.CommandText = "UPDATE public.rpi_connection SET last_touch = (select extract(epoch from now())) WHERE user_id_connected = (@userid)";
                cmd.Parameters.AddWithValue("@userid", userid);
                cmd.ExecuteNonQuery();
            }
        }

        public List<RPiConnection> GetRPiConnections()
        {
            List<RPiConnection> rpiConnections = new List<RPiConnection>();
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = this.npgSqlCon;
                cmd.CommandText = "SELECT id, ip, port, password, user_id_connected, last_touch FROM public.rpi_connection ORDER BY id";
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        RPiConnection rpiConnection = new RPiConnection();
                        rpiConnection.rowId = reader.GetInt32(0);
                        rpiConnection.ip = reader.GetString(1);
                        rpiConnection.port = reader.GetInt32(2);
                        rpiConnection.status = "disconnected";
                        rpiConnection.password = reader.GetString(3);
                        rpiConnection.userId = reader.IsDBNull(4) ? -1 : reader.GetInt32(4);
                        rpiConnection.lastTouch = reader.GetInt32(5);
                        rpiConnections.Add(rpiConnection);
                    }
                }
            }
            rpiConnections.ForEach(rpiCon =>
            {
                rpiCon.userRights = this.GetRPiConnectionUserRights(rpiCon.rowId);
            });
            return rpiConnections;
        }

        private Dictionary<int, string> GetRPiConnectionUserRights(int rpiConnectionId)
        {
            Dictionary<int, string> rpiConnectionUserRights = new Dictionary<int, string>();
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = this.npgSqlCon;
                cmd.CommandText = "SELECT user_id, user_right::varchar FROM public.user_has_rpi_connection WHERE rpi_connection_id=(@rpiconid)";
                cmd.Parameters.AddWithValue("@rpiconid", rpiConnectionId);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        rpiConnectionUserRights.Add(reader.GetInt32(0), reader.GetString(1));
                    }
                }
            }
            return rpiConnectionUserRights;
        }

        public RPiConnection GetRPiConnection(int rpiConnectionId)
        {
            RPiConnection rpiConnection = new RPiConnection();
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = this.npgSqlCon;
                cmd.CommandText = "SELECT id, ip, port, password, user_id_connected, last_touch FROM public.rpi_connection WHERE id=(@rpiconid)";
                cmd.Parameters.AddWithValue("@rpiconid", rpiConnectionId);
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();

                    rpiConnection.rowId = reader.GetInt32(0);
                    rpiConnection.ip = reader.GetString(1);
                    rpiConnection.port = reader.GetInt32(2);
                    rpiConnection.status = "disconnected";
                    rpiConnection.password = reader.GetString(3);
                    rpiConnection.userId = reader.IsDBNull(4) ? -1 : reader.GetInt32(4);
                    rpiConnection.lastTouch = reader.GetInt32(5);
                }
            }
            rpiConnection.userRights = this.GetRPiConnectionUserRights(rpiConnection.rowId);
            return rpiConnection;
        }

        public RPiConnection SetRPiConnectionStatus(int rpiConnectionId, string status, int userId)
        {
            Console.WriteLine(rpiConnectionId + "." + status + "." + userId);
            int insertedId = -1;
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = this.npgSqlCon;
                cmd.CommandText = "WITH selectCount AS(SELECT COUNT(*) FROM public.rpi_connection WHERE user_id_connected = (@userid) AND 'connected' = (@status))" +
                    "UPDATE public.rpi_connection SET user_id_connected = (@useridconnected), last_touch = (select extract(epoch from now())) WHERE id = (@rpiconid) AND(user_id_connected = (@userid) OR user_id_connected IS NULL) AND 0 = (SELECT \"count\" FROM selectCount) RETURNING id;";
                cmd.Parameters.AddWithValue("@rpiconid", rpiConnectionId);
                cmd.Parameters.AddWithValue("@status", status);
                cmd.Parameters.AddWithValue("@userid", userId);
                if (status == "connected")
                {
                    cmd.Parameters.AddWithValue("@useridconnected", userId);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@useridconnected", DBNull.Value);
                }
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        insertedId = reader.GetInt32(0);
                    }

                }
            }
            if (insertedId != rpiConnectionId)
            {
                throw new NpgsqlException("Something went wrong with updating a RPi Connection!");
            }
            return this.GetRPiConnection(rpiConnectionId);
        }

        public RPiConnection OfferRPiConnection(string ip, int port, string password)
        {
            int rpiConId = -1;
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = this.npgSqlCon;
                cmd.CommandText = "INSERT INTO public.rpi_connection (ip, port, password) VALUES (@ip, @port, @password) RETURNING id;";
                cmd.Parameters.AddWithValue("@ip", ip);
                cmd.Parameters.AddWithValue("@port", port);
                cmd.Parameters.AddWithValue("@password", password);
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    rpiConId = reader.GetInt32(0);
                }
            }
            if (rpiConId <= 0)
            {
                throw new NpgsqlException("Something went wrong with inserting a RPi Connection!");
            }
            return this.GetRPiConnection(rpiConId);
        }

        public bool StartFlight(int rpiConnectionId, int flightplanId, int userId)
        {
            int flightId = -1;
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = this.npgSqlCon;
                cmd.CommandText = "INSERT INTO public.flight (rpi_connection_id, flightplan_id, user_id) VALUES (@rpiconid, @fpid, @userid) RETURNING id;";
                cmd.Parameters.AddWithValue("@rpiconid", rpiConnectionId);
                cmd.Parameters.AddWithValue("@fpid", flightplanId);
                cmd.Parameters.AddWithValue("@userid", userId);
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    flightId = reader.GetInt32(0);
                }
            }
            if (flightId <= 0)
            {
                throw new NpgsqlException("Something went wrong with inserting a Flight!");
            }
            return true;
        }
    }
}
