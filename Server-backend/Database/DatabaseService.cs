using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server_backend.Database
{
    public class DatabaseService : IAuthenticationDatabaseService
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
    }
}
