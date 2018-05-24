using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Server_backend.utility;

namespace Server_backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //dbCon.startConnection();
            //CURL cURL = new CURL();
            //cURL.testCurl();
            SendHttpService test = new SendHttpService();
            
            //test.PostResult();
            //test.testCurl();
            //IAuthenticationService auth = new AuthenticationService();
            //auth.Login("cunt", "cunt");
            BuildWebHost(args).Run();
            // RPi static ip: 192.168.1.155
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
            .UseUrls("http://192.168.0.19:5021")
                .Build();
    }
}
