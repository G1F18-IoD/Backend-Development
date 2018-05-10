using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;

namespace Server_backend.Database
{
    public class DatabaseConnection : INpgSqlConnection
    {
        private static NpgsqlConnection npgSqlCon;

        private void StartConnection()
        {
            var connString = "Host=tek-uas-stud0b.stud-srv.sdu.dk;Port=12013;Username=agger;Password=22215145;Database=agger";
            
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

        public NpgsqlConnection GetCon()
        {
            if (DatabaseConnection.npgSqlCon == null)
            {
                this.StartConnection();
            }
            return DatabaseConnection.npgSqlCon;
        }
    }
}
