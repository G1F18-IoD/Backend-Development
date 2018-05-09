﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;

namespace Server_backend.utility
{
    public interface IAuthenticationService
    {
        void Login(string username, string password);
		bool ValidateToken(string token);
		void Register(string username, string password);
    }
}
