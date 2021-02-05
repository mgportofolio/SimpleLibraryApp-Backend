using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SimpleLibrary.Models.Common.Responses;
using SimpleLibrary.Models.Consts;
using SimpleLibrary.Models.Enums;
using SimpleLibrary.Models.Models.Auth;
using SimpleLibrary.Services.Helper;
using SimpleLibraryDbContext.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SimpleLibrary.Services.Services.Auth
{
    public interface IAuthService
    {
        Task<DataResponse<AuthModel>> Login(AuthLoginModel auth);
        Task<BaseResponse> MakeAdmin(int userId);
        Task<string> GetUserRole(int userId);
    }

    public class AuthService : IAuthService
    {
        private readonly LibraryDb_DevContext _DbMan;
        private readonly AppSettings _SettingMan;

        public AuthService(LibraryDb_DevContext dbContext, IOptions<AppSettings> appSettings)
        {
            this._DbMan = dbContext;
            this._SettingMan = appSettings.Value;
        }

        public async Task<DataResponse<AuthModel>> Login(AuthLoginModel auth)
        {
            var res = new DataResponse<AuthModel>();
            res.Data = new AuthModel();
            res.SetSuccessStatsus();

            var user = await this._DbMan.Users
                .Where(Q => Q.UserName == auth.UserName)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                res.SetFailedtatsus();
                res.SetMessage(AuthMessage.AuthError, (int)MessageTypeEnum.Error);
                res.Data = null;
                return res;
            }

            var IsPasswordValid = BCrypt.Net.BCrypt.EnhancedVerify(auth.UserPassword, user.UserPassword, BCrypt.Net.HashType.SHA256);
            if (IsPasswordValid == false)
            {
                res.SetFailedtatsus();
                res.SetMessage(AuthMessage.AuthError, (int)MessageTypeEnum.Error);
                res.Data = null;
                return res;
            }

            try
            {
                res.Data.Token = GenerateJwtToken(user);
                res.Data.UserId = user.UserId;
                res.Data.UserRole = user.UserRole;
            }
            catch (Exception ex)
            {
                var msg = ex.Message; //TODO: Insert Log
                res.SetFailedtatsus();
                res.SetMessage(CommonMessage.CommonErrorMessage, (int)MessageTypeEnum.Exception);
            }

            res.SetMessage(AuthMessage.AuthSuccess, (int)MessageTypeEnum.Success);
            return res;
        }

        private string GenerateJwtToken(SimpleLibraryDbContext.Entities.User user)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_SettingMan.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Role, user.UserRole)
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<BaseResponse> MakeAdmin(int userId)
        {
            var res = new BaseResponse();
            var user = await this._DbMan.Users.Where(Q => Q.UserId == userId)
                .FirstOrDefaultAsync();
            res.SetMessage("Membuat User " + user.UserName + " menjadi admin " + CommonMessage.CommonSuccessMessage, (int)MessageTypeEnum.Success);
            res.SetSuccessStatsus();
            try
            {
                user.UserRole = "Admin";
                this._DbMan.Update(user);
                await this._DbMan.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                //ex should be loggeg to logging services
                res.SetMessage(CommonMessage.CommonErrorMessage, (int)MessageTypeEnum.Exception);
                res.SetFailedtatsus();
            }
            return res;
        }

        public async Task<string> GetUserRole(int userId)
        {
            var role = "";
            try
            {
               role = await this._DbMan.Users
                   .Where(Q => Q.UserId == userId)
                   .Select(Q => Q.UserRole)
                   .FirstOrDefaultAsync();
            }
            catch(Exception ex)
            {
                var err = ex.ToString();
            }

            return role;
        }
    }
}
