using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Server_backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            DatabaseConnection dbCon = new DatabaseConnection();
            //dbCon.startConnection();
            CURL cURL = new CURL();
            cURL.testCurl();
            //cURL.PostResult();
            BuildWebHost(args).Run();
            // RPi static ip: 192.168.1.155
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
