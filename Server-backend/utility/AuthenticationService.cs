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
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IAuthenticationDatabaseService authDBService;

        public AuthenticationService(IAuthenticationDatabaseService _authDBService)
        {
            this.authDBService = _authDBService;
        }

        public string Login(string username, string password)
        {
            int id = this.authDBService.Login(username, password);
            if(id <= 0)
            {
                return "lblFailedLogin";
            }
            return this.GenerateToken(username, id);
        }

        public bool ValidateToken(string token)
        {
            throw new NotImplementedException();
        }
		
		public string Register(string username, string password)
		{
            int count = this.authDBService.Register(username, password);
            if (count <= 0)
            {
                return "lblFailedRegistration";
            }
            return this.Login(username, password);
        }

        private const string Secret = "db3OIsj+BXE9NZDy0t8W3TcNekrF+2d/1sFnWG4HnV8TZY30iTOdtVWJG8abWvB1GlOgJuQZdcF2Luqm/hccMw==";
        //private const string Secret = "fiskenfiskerfisk";

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
