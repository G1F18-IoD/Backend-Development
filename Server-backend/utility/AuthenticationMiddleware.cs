using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;

namespace Server_backend.utility
{
    /**
     * This class is a filter, which means it gets called before and after the request is received by the kestrel (servlet component) and the actual implementation by the controller.
     */
    public class SaveAuthenticationHeader : IActionFilter
    {
        private readonly IAuthenticationService auth;

        /**
         * Constructor for dependency injection.
         */
        public SaveAuthenticationHeader(IAuthenticationService _auth)
        {
            this.auth = _auth;
        }

        /**
         * The method that gets called before the implementation of the controller. This basically saves then header string called AuthToken.
         */
        public void OnActionExecuting(ActionExecutingContext context)
        {
            StringValues jwtToken;
            context.HttpContext.Request.Headers.TryGetValue("AuthToken", out jwtToken);
            this.auth.SetToken(jwtToken.ToString());
        }

        /**
         * The method that gets called after the implementation of the controller. This has no implementation in this system.
         */
        public void OnActionExecuted(ActionExecutedContext context)
        {

        }
    }

}
