﻿using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server_backend.Database
{
    public interface INpgSqlConnection
    {
        NpgsqlConnection GetCon();
    }
}