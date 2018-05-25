using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Server_backend.utility;

namespace Server_backend.Controllers
{
    /**
     * The API controller to handle authentication like loging in and registering.
     * URL: api/auth/...
     * Produces: JSON encoded data.
     */
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthenticationService auth;
        
        /**
         * Constructor for dependency injection.
         */
        public AuthController(IAuthenticationService _auth)
        {
            this.auth = _auth;
        }

        /**
         * HTTP POST method for loging in.
         */
        [HttpPost]
        public JsonResult Post([FromBody]LoginModel loginModel)
        {
            //Console.WriteLine(loginModel.Username + "..." + loginModel.Password);
            //this.auth = new AuthenticationService();
            //return this.auth.Login(loginModel.Username, loginModel.Password);
            return Json(new string[] { this.auth.Login(loginModel.Username, loginModel.Password) });
        }

        /**
         * HTTP POST method for registering a user.
         */
        [HttpPost("register")]
        public JsonResult RegisterPost([FromBody]LoginModel loginModel)
        {
            return Json(new string[] { this.auth.Register(loginModel.Username, loginModel.Password) });
        }
    }

    /**
     * Class containing the required info for loging in.
     */
    public class LoginModel
    {
        public string Username { set; get; }
        public string Password { set; get; }
    }
}
