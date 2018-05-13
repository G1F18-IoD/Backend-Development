using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server_backend.Flightplan;

namespace Server_backend.Database
{
    public interface IAuthenticationDatabaseService
    {
        int Login(string username, string password);
        int Register(string username, string password);
    }

    public interface ICommandDatabaseService : ICommandCommon
    {

    }


}
