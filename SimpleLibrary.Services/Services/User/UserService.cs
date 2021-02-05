using Microsoft.EntityFrameworkCore;
using SimpleLibrary.Models.Common.Responses;
using SimpleLibrary.Models.Consts;
using SimpleLibrary.Models.Enums;
using SimpleLibrary.Models.Models.User;
using SimpleLibraryDbContext.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleLibrary.Services.Services.User
{
    public class UserService
    {
        private readonly LibraryDb_DevContext _DbMan;

        public UserService(LibraryDb_DevContext dbContext)
        {
            this._DbMan = dbContext;
        }

        public async Task<DataResponse<List<UserModel>>> GetUsers()
        {
            var res = new DataResponse<List<UserModel>>();
            res.Data = new List<UserModel>();
            var users = await this._DbMan.Users
               .Select(Q => new UserModel
               {
                   UserId = Q.UserId,
                   UserName = Q.UserName,
                   Roles = Q.UserRole
               })
               .ToListAsync();
            if (users != null)
            {
                res.Data = users;
                res.SetSuccessStatsus();
            }
            else
            {
                res.SetFailedtatsus();
                res.SetMessage("Users not found", (int)MessageTypeEnum.Warning);
            }
            return res;
        }

        public async Task<DataResponse<UserModel>> GetUser(int id)
        {
            var res = new DataResponse<UserModel>();
            res.Data = new UserModel();
            var user = await this._DbMan.Users
                .Where(Q => Q.UserId == id)
                .Select(Q => new UserModel
                {
                    UserId = Q.UserId,
                    UserName = Q.UserName,
                    Roles = Q.UserRole
                })
                .FirstOrDefaultAsync();
            if (user != null)
            {
                res.Data = user;
                res.SetSuccessStatsus();
            }
            else
            {
                res.SetFailedtatsus();
                res.SetMessage("User not found", (int)MessageTypeEnum.Warning);
            }
            return res;
        }

        public async Task<BaseResponse> CreateUser(UserRegistrationModel user)
        {
            var res = new BaseResponse();
            if (await IsUserExistsAsync(user.UserName))
            {
                res.SetMessage("User sudah terdaftar!", (int)MessageTypeEnum.Warning);
                res.SetFailedtatsus();
                return res;
            }

            try
            {
                this._DbMan.Users.Add(new SimpleLibraryDbContext.Entities.User
                {
                    UserName = user.UserName,
                    UserPassword = user.UserPassword,
                    UserRole = "User",
                    CreatedAt = DateTime.Now,
                    CreateBy = "SYSTEM"
                });
                await this._DbMan.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                //ex should be loggeg to logging services
                res.SetMessage(CommonMessage.CommonErrorMessage, (int)MessageTypeEnum.Exception);
                res.SetFailedtatsus();
                return res;
            }
            res.SetSuccessStatsus();
            res.SetMessage("Registrasi user berhasil!", (int)MessageTypeEnum.Success);
            return res;
        }

        private async Task<bool> IsUserExistsAsync(string userName)
        {
            var existedUser = await this._DbMan.Users
                .Where(Q => Q.UserName == userName)
                .FirstOrDefaultAsync();

            if (existedUser != null)
            {
                return true;
            }
            return false;
        }
    }
}
