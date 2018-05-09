using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;

namespace Server_backend.utility
{
    public class AuthenticationService : IAuthenticationService
    {
        public void Login(string username, string password)
        {
            throw new NotImplementedException();
        }

        public bool ValidateToken(string token)
        {
            throw new NotImplementedException();
        }
		
		public void Register(string username, string password)
		{
			throw new NotImplementedException();
		}
    }
}
