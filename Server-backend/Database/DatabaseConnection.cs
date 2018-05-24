using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;

namespace Server_backend.Database
{
    public interface INpgSqlConnection
    {
        NpgsqlConnection GetCon();
    }

    public class DatabaseConnection : INpgSqlConnection
    {
        private static NpgsqlConnection npgSqlCon;

        public DatabaseConnection()
        {
            //this.StartConnection();
        }

        private void StartConnection()
        {
            //var connString = "Host=tek-uas-stud0b.stud-srv.sdu.dk;Port=12013;Username=agger;Password=22215145;Database=agger";
            var connString = "Host=localhost;Port=5432;Username=postgres;Password=admin;Database=postgres";

            DatabaseConnection.npgSqlCon = new NpgsqlConnection(connString);
            DatabaseConnection.npgSqlCon.Open();

            // Insert some data
            /*using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "INSERT INTO data (some_field) VALUES (@p)";
                cmd.Parameters.AddWithValue("p", "Hello world");
                cmd.ExecuteNonQuery();
            }*/

            // Retrieve all rows
            /*
            using (var cmd = new NpgsqlCommand("SELECT * FROM public.test ORDER BY id ASC", conn))
            using (var reader = cmd.ExecuteReader())
                while (reader.Read())
                    Console.WriteLine(reader.GetString(1));*/
        }

        public static NpgsqlConnection GetStaticCon()
        {
            INpgSqlConnection conService = new DatabaseConnection();
            return conService.GetCon();
        }

        public NpgsqlConnection GetCon()
        {
            NpgsqlConnection tempCon;
            var connString = "Host=localhost;Port=5432;Username=postgres;Password=admin;Database=postgres;Pooling=false;Minimum Pool Size=1;Maximum Pool Size=500;Connection Idle Lifetime=1;Connection Pruning Interval=1";

            tempCon = new NpgsqlConnection(connString);
            tempCon.Open();
            //this.StartConnection();
            return tempCon;
        }
    }
}
