﻿using Newtonsoft.Json.Linq;
using Npgsql;
using Server_backend.FlightplanNS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using System.Web.Script.Serialization;

namespace Server_backend.Database
{
    public class DatabaseService : IAuthenticationDatabaseService, ICommandDatabaseService, IFlightplanDatabaseService
    {
        private readonly NpgsqlConnection npgSqlCon;

        public DatabaseService(INpgSqlConnection connection)
        {
            this.npgSqlCon = connection.GetCon();
        }

        public int Login(string username, string password)
        {
            int id = -1;
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = this.npgSqlCon;
                //cmd.CommandText = "SELECT id FROM account";
                cmd.CommandText = "SELECT id FROM public.account WHERE username=(@u) AND password=(@p)";
                cmd.Parameters.AddWithValue("@u", username);
                cmd.Parameters.AddWithValue("@p", password);
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    id = reader.GetInt32(0);
                }
                //id = (int)cmd.ExecuteScalar();
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
                //cmd.CommandText = "SELECT id FROM account";
                cmd.CommandText = "INSERT INTO public.account (username, password) VALUES ((@u), (@p))";
                cmd.Parameters.AddWithValue("@u", username);
                cmd.Parameters.AddWithValue("@p", password);
                count = cmd.ExecuteNonQuery();
                //id = (int)cmd.ExecuteScalar();
            }
            return count;
        }

        public Command GetCommand(int CommandId)
        {
            Command retObj = new Command();
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = this.npgSqlCon;
                //cmd.CommandText = "SELECT id FROM account";
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

                    //JavaScriptSerializer js = new JavaScriptSerializer();
                    //string[] Params = js.Deserialize<string[]>(reader.GetString(4));
                    JArray ja = JArray.Parse(reader.GetString(4));
                    string[] Params = ja.Select(jv => (string)jv).ToArray();
                    int count = 0;
                    foreach (string Param in Params)
                    {
                        retObj.Params.Insert(count++, Param);
                    }
                }
            }
            return retObj;
        }

        public Dictionary<int, Command> GetCommands(int flightplanId)
        {
            Dictionary<int, Command> cmds = new Dictionary<int, Command>();
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = this.npgSqlCon;
                //cmd.CommandText = "SELECT id FROM account";
                cmd.CommandText = "SELECT id, flightplan_id, cmd, message, payload, \"order\" FROM public.flightplan_commands WHERE flightplan_id=(@fpid)";
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

                        //JavaScriptSerializer js = new JavaScriptSerializer();
                        //string[] Params = js.Deserialize<string[]>(reader.GetString(4));
                        JArray ja = JArray.Parse(reader.GetString(4));
                        string[] Params = ja.Select(jv => (string)jv).ToArray();
                        int count = 0;
                        foreach (string Param in Params)
                        {
                            command.Params.Insert(count++, Param);
                        }
                        cmds.Add(command.Order, command);
                    }
                }
                //id = (int)cmd.ExecuteScalar();
            }
            return cmds;
        }

        public Command SaveCommand(Command command)
        {
            int cmdId = -1;
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = this.npgSqlCon;
                //cmd.CommandText = "SELECT id FROM account";
                cmd.CommandText = "INSERT INTO public.flightplan_commands (flightplan_id, cmd, message, payload, \"order\") VALUES (@fpid, @cmd::possible_commands, @message, array_to_json(@params), @order) RETURNING id;";
                cmd.Parameters.AddWithValue("@fpid", command.FlightplanId);
                cmd.Parameters.AddWithValue("@cmd", command.CmdString);
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
                //id = (int)cmd.ExecuteScalar();
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
                //cmd.CommandText = "SELECT id FROM account";
                cmd.CommandText = "SELECT id, author, created_at FROM public.flightplan WHERE id=(@fpid)";
                cmd.Parameters.AddWithValue("@fpid", flightplanId);
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();

                    flightplan.rowId = reader.GetInt32(0);
                    flightplan.authorId = reader.GetInt32(1);
                    flightplan.createdAt = reader.GetInt32(2);
                }
                //id = (int)cmd.ExecuteScalar();
            }
            return flightplan;
        }

        public List<Flightplan> GetFlightplans()
        {
            List<Flightplan> flightplans = new List<Flightplan>();
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = this.npgSqlCon;
                //cmd.CommandText = "SELECT id FROM account";
                cmd.CommandText = "SELECT id, author, created_at FROM public.flightplan";
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Flightplan flightplan = new Flightplan();
                        flightplan.rowId = reader.GetInt32(0);
                        flightplan.authorId = reader.GetInt32(1);
                        flightplan.createdAt = reader.GetInt32(2);
                        flightplans.Add(flightplan);
                    }
                }
                //id = (int)cmd.ExecuteScalar();
            }
            return flightplans;
        }

        public Flightplan CreateFlightplan(int authorId)
        {
            int flightplanId = -1;
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = this.npgSqlCon;
                //cmd.CommandText = "SELECT id FROM account";
                cmd.CommandText = "INSERT INTO public.flightplan (author) VALUES (@authorId) RETURNING id;";
                cmd.Parameters.AddWithValue("@authorId", authorId);
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    flightplanId = reader.GetInt32(0);
                }
                //id = (int)cmd.ExecuteScalar();
            }
            if (flightplanId <= 0)
            {
                throw new NpgsqlException("Something went wrong with inserting a flightplan!");
            }
            return this.GetFlightplanInfo(flightplanId);
        }
    }
}
