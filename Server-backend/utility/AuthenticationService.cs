using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Server_backend.Database;

namespace Server_backend.utility
{
    /**
     * Class for handling everything related to authentication.
     * For authentication, a JWT token is used.
     */
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IAuthenticationDatabaseService authDBService;
        private static JwtSecurityToken jwtToken;
        private static string jwtTokenStr;

        /**
         * Constructor for dependency injection.
         */
        public AuthenticationService(IAuthenticationDatabaseService _authDBService)
        {
            this.authDBService = _authDBService;
        }

        /**
         * Will return a authentication token if successfully logged in. It will return a label if not.
         */
        public string Login(string username, string password)
        {
            int id = this.authDBService.Login(username, password);
            if(id <= 0)
            {
                return "lblFailedLogin";
            }
            return this.GenerateToken(username, id);
        }

        /**
         * Will return true if succesfully validated the token. This validation uses the JWT token concept that is implemented by the .Net Core.
         * It was planned to return a string, through an out parameter, that contained the error message, although, not enough work was put into this yet.
         */
        public bool ValidateToken(string token, out string responseStr)
        {
            responseStr = "Could not validate token!";
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                if (jwtToken == null)
                {
                    return false;
                }

                var symmetricKey = Convert.FromBase64String(Secret);

                var validationParameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(symmetricKey)
                };

                SecurityToken securityToken;
                var principal = tokenHandler.ValidateToken(token, validationParameters, out securityToken);

                return true;
            }

            catch (Exception e)
            {
                return false;
            }
        }

        /**
         * Will return a authentication token if successfully registering a new user. It will return a label if not.
         */
        public string Register(string username, string password)
		{
            int count = this.authDBService.Register(username, password);
            if (count <= 0)
            {
                return "lblFailedRegistration";
            }
            return this.Login(username, password);
        }

        public void SetToken(string token)
        {
            AuthenticationService.jwtTokenStr = token;
        }
        
        public string GetToken()
        {
            return AuthenticationService.jwtTokenStr;
        }

        /**
         * This gets a value from the token, like the user id.
         */
        public string GetTokenClaim(string attribute)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            AuthenticationService.jwtToken = tokenHandler.ReadToken(AuthenticationService.jwtTokenStr) as JwtSecurityToken;

            string retVal;
            switch(attribute.ToLower())
            {
                case "user_id":
                case "userid":
                    retVal = AuthenticationService.jwtToken.Payload.Claims.First(claim => claim.Type == "primarysid").Value;
                    break;
                default:
                    retVal = "";
                    break;
            }
            return retVal;
        }

        private const string Secret = "db3OIsj+BXE9NZDy0t8W3TcNekrF+2d/1sFnWG4HnV8TZY30iTOdtVWJG8abWvB1GlOgJuQZdcF2Luqm/hccMw==";

        /**
         * This will use the JWT token implementation by the .Net to created a token with a name and a primary sid, which is the user id.
         * The Secret will be used to hash the signature, so we can compare that signature in the future to confirm the validity of the token.
         */
        private string GenerateToken(string username, int id, int expireMinutes = 20)
        {
            var symmetricKey = Convert.FromBase64String(Secret);
            var tokenHandler = new JwtSecurityTokenHandler();

            var now = DateTime.UtcNow;
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                        {
                        new Claim(ClaimTypes.Name, username),
                        new Claim(ClaimTypes.PrimarySid, id.ToString())
                    }),

                Expires = now.AddMinutes(Convert.ToInt32(expireMinutes)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256Signature)
            };

            var stoken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(stoken);

            return token;
        }
    }
}
