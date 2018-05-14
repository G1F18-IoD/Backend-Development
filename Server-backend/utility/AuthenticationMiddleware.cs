using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;


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

    public class SaveAuthenticationHeader : IActionFilter
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

}
