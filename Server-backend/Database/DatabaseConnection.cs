using Npgsql;

namespace Server_backend.Database
{
    public interface INpgSqlConnection
    {
        NpgsqlConnection GetCon();
    }

    /**
     * This class will be the one providing the database connection. Where an interface can be used to dependency inject the correct connection required.
     * However, this connection is not working properly, either because something is not setup properly, or that the Npgsql is not handling pooling connections correct.
     * After around 20 API calls to this system, it will throw an exception stating that too many clients are already connected, or worse,
     * the Npgsql component will crash, leaving the system running, but unable to return any data from the database.
     * No solution were found to fix this problem. Various connection string settings have been attempted, and various uses of the connection in the DatabaseService
     * class have been tried as well. Some give you a few more calls than 20 before crashing, and some give you less. The current implementation of the
     * connection was found to be the most stable and provide the most stable amount of calls before crashing.
     * To try this crash out yourself, simply start the server and make a GET on .../api/flightplan/. Consider "disable" authentication by simply making "validateToken" return true.
     */
    public class DatabaseConnection : INpgSqlConnection
    {
        private static NpgsqlConnection npgSqlCon;

        public DatabaseConnection()
        {
            //this.StartConnection();
        }

        private void StartConnection()
        {
            // Below is the connection string for the test environment on VPN.
            //var connString = "Host=tek-uas-stud0b.stud-srv.sdu.dk;Port=12013;Username=agger;Password=22215145;Database=agger";
            var connString = "Host=localhost;Port=5432;Username=postgres;Password=admin;Database=postgres";

            DatabaseConnection.npgSqlCon = new NpgsqlConnection(connString);
            DatabaseConnection.npgSqlCon.Open();
        }

        /**
         * A method to created to test other scenarios.
         */
        public static NpgsqlConnection GetStaticCon()
        {
            INpgSqlConnection conService = new DatabaseConnection();
            return conService.GetCon();
        }

        /**
         * The actual method that should be used from the dependency injection. The implementation is not correct, but changed to make the connection more stable before crashing.
         */
        public NpgsqlConnection GetCon()
        {
            NpgsqlConnection tempCon;
            var connString = "Host=localhost;Port=5432;Username=postgres;Password=admin;Database=postgres;Pooling=false;Minimum Pool Size=1;Maximum Pool Size=500;Connection Idle Lifetime=1;Connection Pruning Interval=1";

            tempCon = new NpgsqlConnection(connString);
            tempCon.Open();

            //Use this below in case you want to test the "original" connection string
            //tempCon.Close();
            //this.StartConnection();
            //tempcon = DatabaseConnection.npgSqlCon;

            return tempCon;
        }
    }
}
