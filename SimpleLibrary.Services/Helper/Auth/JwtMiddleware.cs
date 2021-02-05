using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using SimpleLibrary.Services.Services.Auth;

namespace SimpleLibrary.Services.Helper.Auth
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AppSettings _appSettings;

        public JwtMiddleware(RequestDelegate next, IOptions<AppSettings> appSettings)
        {
            _next = next;
            _appSettings = appSettings.Value;
        }

        public async Task Invoke(HttpContext context, IAuthService authService)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
            {
                await attachUserToContext(context, authService, token);
            }

            await _next(context);
        }

        private async Task attachUserToContext(HttpContext context, IAuthService authService, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                // attach user to context on successful jwt validation
                context.Items[ClaimTypes.NameIdentifier] = jwtToken.Claims.First(Q => Q.Type == "nameid").Value;
                context.Items[ClaimTypes.Role] =  await CheckToken(authService, jwtToken.Claims.First(Q => Q.Type == "nameid").Value, 
                    jwtToken.Claims.FirstOrDefault(Q => Q.Type == "role").Value);
                    //jwtToken.Claims.FirstOrDefault(Q => Q.Type == "role").Value;
            }
            catch
            {

            }
        }

        private async Task<string> CheckToken(IAuthService auth, string id, string userRole)
        {
            var isValid = int.TryParse(id, out var userId);
            var role = await auth.GetUserRole(userId);
            if(userRole == role)
            {
                return userRole;
            }
            return role;
        }
    }
}
