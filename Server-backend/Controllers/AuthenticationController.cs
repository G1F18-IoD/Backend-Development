using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Server_backend.utility;

namespace Server_backend.Controllers
{
    [Produces("application/json")]
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
        public JsonResult Post([FromBody]LoginModel loginModel)
        {
            //Console.WriteLine(loginModel.Username + "..." + loginModel.Password);
            //this.auth = new AuthenticationService();
            //return this.auth.Login(loginModel.Username, loginModel.Password);
            return Json(new string[] { this.auth.Login(loginModel.Username, loginModel.Password) });
        }

        // PUT api/values/5
        [HttpPost("register")]
        public JsonResult RegisterPost([FromBody]LoginModel loginModel)
        {
            return Json(new string[] { this.auth.Register(loginModel.Username, loginModel.Password) });
        }
    }

    public class LoginModel
    {
        public string Username { set; get; }
        public string Password { set; get; }
    }

    public class ValidateTokenModel
    {
        public string token { set; get; }
    }
}
