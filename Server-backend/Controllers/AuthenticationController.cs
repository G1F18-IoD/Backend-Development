using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Server_backend.utility;

namespace Server_backend.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthenticationService auth;
        
        public AuthController(IAuthenticationService _auth)
        {
            this.auth = _auth;
        }

        // POST api/values
        [HttpPost]
        public string Post([FromBody]LoginModel loginModel)
        {
            //Console.WriteLine(loginModel.Username + "..." + loginModel.Password);
            //this.auth = new AuthenticationService();
            return this.auth.Login(loginModel.Username, loginModel.Password);
        }

        // PUT api/values/5
        [HttpPost("register")]
        public string RegisterPost([FromBody]LoginModel loginModel)
        {
            return this.auth.Register(loginModel.Username, loginModel.Password);
        }
    }

    public class LoginModel
    {
        public string Username { set; get; }
        public string Password { set; get; }
    }
}
