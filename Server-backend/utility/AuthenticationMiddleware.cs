using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using System.Security.Claims;
using System.Security.Principal;
using System.Security.Authentication;
using System.Threading;
using System.Web.Http.Filters;
using System.Net.Http;
using System.Web.Http.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Web.Http;
using System.Net;


// https://andrewlock.net/adding-default-security-headers-in-asp-net-core/
namespace Server_backend.utility
{
    
    
    public class SampleAsyncActionFilter : IAsyncActionFilter
    {
        private readonly IAuthenticationService auth;

        public SampleAsyncActionFilter(IAuthenticationService _auth)
        {
            this.auth = _auth;
        }

        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            // do something before the action executes
            StringValues jwtToken;
            context.HttpContext.Request.Headers.TryGetValue("AuthToken", out jwtToken);
            this.auth.SetToken(jwtToken.ToString());
            var resultContext = await next();
            // do something after the action executes; resultContext.Result will be set
        }
    }

    public class SaveAuthenticationHeader : Microsoft.AspNetCore.Mvc.Filters.IActionFilter
    {
        private readonly IAuthenticationService auth;

        public SaveAuthenticationHeader(IAuthenticationService _auth)
        {
            this.auth = _auth;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // do something before the action executes
            StringValues jwtToken;
            context.HttpContext.Request.Headers.TryGetValue("AuthToken", out jwtToken);
            this.auth.SetToken(jwtToken.ToString());
            
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // do something after the action executes
        }
    }

    public class ValidateToken : Microsoft.AspNetCore.Mvc.Filters.IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            context.HttpContext.Response.StatusCode = 418;
            Console.WriteLine("lawl");
            //context.Result = new AuthenticationFailureResult("Douche");
        }
    }

    public class JwtAuthenticationAttribute : Attribute, IAuthenticationFilter
    {
        public string Realm { get; set; }
        public bool AllowMultiple => false;

        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            Console.WriteLine("In AUTH");
            var request = context.Request;
            var authorization = request.Headers.Authorization;

            if (authorization == null || authorization.Scheme != "Bearer")
                return;

            if (string.IsNullOrEmpty(authorization.Parameter))
            {
                Console.WriteLine("failed");
                //context.ErrorResult = new AuthenticationFailureResult("Missing Jwt Token", request);r
                return;
            }

            var token = authorization.Parameter;
            var principal = await AuthenticateJwtToken(token);

            if (principal == null)
                Console.WriteLine("failed");
            //context.ErrorResult = new AuthenticationFailureResult("Invalid token", request);

            else
                context.Principal = principal;
        }



        private bool ValidateToken(string token, out string username)
        {
            username = null;

            //var simplePrinciple = JwtManager.GetPrincipal(token);
            var simplePrinciple = AuthenticationService.GetPrincipal(token);
            var identity = simplePrinciple?.Identity as ClaimsIdentity;

            if (identity == null)
                return false;

            if (!identity.IsAuthenticated)
                return false;

            var usernameClaim = identity.FindFirst(ClaimTypes.Name);
            username = usernameClaim?.Value;

            if (string.IsNullOrEmpty(username))
                return false;

            // More validate to check whether username exists in system

            return true;
        }

        protected Task<IPrincipal> AuthenticateJwtToken(string token)
        {
            string username;

            if (ValidateToken(token, out username))
            {
                // based on username to get more information from database in order to build local identity
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username)
                    // Add more claims if needed: Roles, ...
                };

                var identity = new ClaimsIdentity(claims, "Jwt");
                IPrincipal user = new ClaimsPrincipal(identity);

                return Task.FromResult(user);
            }

            return Task.FromResult<IPrincipal>(null);
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            Console.WriteLine("LAWLING");
            Challenge(context);
            return Task.FromResult(0);
        }

        private void Challenge(HttpAuthenticationChallengeContext context)
        {
            Console.WriteLine("NAAAAR");
            string parameter = null;

            if (!string.IsNullOrEmpty(Realm))
                parameter = "realm=\"" + Realm + "\"";

            //context.ChallengeWith("Bearer", parameter);
        }
    }

    public class AuthenticationFailureResult : IHttpActionResult
    {
        public AuthenticationFailureResult(string reasonPhrase)
        {
            ReasonPhrase = reasonPhrase;
            //Request = request;
        }

        public string ReasonPhrase { get; }

        //public HttpRequestMessage Request { get; }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute());
        }

        private HttpResponseMessage Execute()
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Unauthorized)
            {
                ReasonPhrase = ReasonPhrase
            };
                
                /*new HttpResponseMessage(HttpStatusCode.Unauthorized)
            {
                RequestMessage = Request,
                ReasonPhrase = ReasonPhrase
            };*/

            return response;
        }
    }

    public class AuthFailureResult : IActionResult
    {
        public Task ExecuteResultAsync(ActionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
