using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;

namespace Server_backend.utility
{
    public interface IAuthenticationService
    {
        string Login(string username, string password);
		bool ValidateToken(string token);
		string Register(string username, string password);
        void SetToken(string token);
        string GetToken();
        string GetTokenClaim(string attribute);
    }
}
