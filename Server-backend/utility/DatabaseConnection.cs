using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;

namespace Server_backend
{
    public class DatabaseConnection
    {
        private NpgsqlConnection con;

        public void startConnection()
        {
            var connString = "Host=tek-uas-stud0b.stud-srv.sdu.dk;Port=12013;Username=agger;Password=22215145;Database=agger";

            using (var conn = new NpgsqlConnection(connString))
            {
                this.con = conn;
                conn.Open();

                // Insert some data
                /*using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "INSERT INTO data (some_field) VALUES (@p)";
                    cmd.Parameters.AddWithValue("p", "Hello world");
                    cmd.ExecuteNonQuery();
                }*/

                // Retrieve all rows
                using (var cmd = new NpgsqlCommand("SELECT * FROM public.test ORDER BY id ASC", conn))
                using (var reader = cmd.ExecuteReader())
                    while (reader.Read())
                        Console.WriteLine(reader.GetString(1));
            }
        }
    }
}
