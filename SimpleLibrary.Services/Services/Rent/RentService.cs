using SimpleLibrary.Models.Common.Responses;
using SimpleLibrary.Models.Models.Rent;
using SimpleLibraryDbContext.Entities;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SimpleLibrary.Models.Enums;
using System.Security.Claims;
using SimpleLibrary.Models.Consts;

namespace SimpleLibrary.Services.Services.Rent
{
    public class RentService
    {
        private readonly LibraryDb_DevContext _DbMan;

        public RentService(LibraryDb_DevContext dbContext)
        {
            this._DbMan = dbContext;
        }

        public async Task<DataResponse<List<RentModel>>> GetRentHistory()
        {
            var res = new DataResponse<List<RentModel>>();
            res.Data = new List<RentModel>();
            var query = from rent in _DbMan.Rents
                        join book in _DbMan.Books on rent.BookId equals book.BookId 
                        join user in _DbMan.Users on rent.UserId equals user.UserId
                        select new RentModel
                        {
                            BookId = book.BookId,
                            BookName = book.BookName,
                            UserName = user.UserName,
                            RentAt = rent.RentAt.ToShortDateString(),
                            ReturnedAt = rent.ReturnedAt.HasValue ? rent.ReturnedAt.GetValueOrDefault(DateTime.Now).ToShortDateString() : "Not Returned Yet",
                            Status = rent.ReturnedAt.HasValue ? true : false,
                            RentId = rent.RentId,
                            UserId = user.UserId,
                        };
            var rentalData = await query.ToListAsync().ConfigureAwait(false);
            if (rentalData.Count > 0)
            {
                res.Data = rentalData;
                res.SetSuccessStatsus();
            }
            else
            {
                res.SetFailedtatsus();
                res.SetMessage("Rental data not found", (int)MessageTypeEnum.Warning);
            }
            return res;
        }

        public async Task<DataResponse<List<RentModel>>> GetRentHistory(int userId)
        {
            var res = new DataResponse<List<RentModel>>();
            res.Data = new List<RentModel>();
            var query = from rent in _DbMan.Rents
                        join book in _DbMan.Books on rent.BookId equals book.BookId
                        join user in _DbMan.Users on rent.UserId equals user.UserId
                        where(user.UserId == userId)
                        select new RentModel
                        {
                            BookId = book.BookId,
                            BookName = book.BookName,
                            UserName = user.UserName,
                            RentAt = rent.RentAt.ToShortDateString(),
                            ReturnedAt = rent.ReturnedAt.HasValue ? rent.ReturnedAt.GetValueOrDefault(DateTime.Now).ToShortDateString() : "Not Returned Yet",
                            Status = rent.ReturnedAt.HasValue ? true : false,
                            RentId = rent.RentId,
                            UserId = user.UserId,
                        };
            var rentalData = await query.ToListAsync().ConfigureAwait(false);
            if (rentalData.Count > 0)
            {
                res.Data = rentalData;
                res.SetSuccessStatsus();
            }
            else
            {
                res.SetFailedtatsus();
                res.SetMessage("Rental data not found", (int)MessageTypeEnum.Warning);
            }
            return res;
        }

        public async Task<BaseResponse> RentBook(int bookId, string userId)
        {
            var res = new BaseResponse();

            var isUserIdValid = int.TryParse(userId, out var id);
            if(isUserIdValid == false)
            {
                res.SetMessage(CommonMessage.CommonErrorMessage, (int)MessageTypeEnum.Error);
                res.SetFailedtatsus();
                return res;
            }

            if (await IsRentExisted(bookId, id))
            {
                res.SetMessage("Buku sedang dipinjam", (int)MessageTypeEnum.Warning);
                res.SetFailedtatsus();
                return res;
            }

            try
            {
                this._DbMan.Rents.Add(new SimpleLibraryDbContext.Entities.Rent
                {
                    UserId = id,
                    BookId = bookId,
                    RentAt = DateTime.Now,
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
            res.SetMessage("Buku berhasil dipinjam", (int)MessageTypeEnum.Success);
            return res;

        }

        public async Task<BaseResponse> ReturnBook(int rentId, string userId)
        {
            var res = new BaseResponse();

            var isUserIdValid = int.TryParse(userId, out var id);
            if (isUserIdValid == false)
            {
                res.SetMessage(CommonMessage.CommonErrorMessage, (int)MessageTypeEnum.Error);
                res.SetFailedtatsus();
                return res;
            }

            var rentedBook = await this._DbMan.Rents
                    .Where(Q => Q.RentId == rentId && Q.ReturnedAt == null && Q.UserId == id)
                    .FirstOrDefaultAsync();

            if (rentedBook == null)
            {
                res.SetMessage("Buku sedang tidak dipinjam atau dipinjam oleh orang lain", (int)MessageTypeEnum.Warning);
                res.SetFailedtatsus();
                return res;
            }

            try
            {
                rentedBook.ReturnedAt = DateTime.Now;
                this._DbMan.Update(rentedBook);
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
            res.SetMessage("Buku berhasil dikembalikan", (int)MessageTypeEnum.Success);
            return res;

        }

        private async Task<bool> IsRentExisted(int bookId, int userId)
        {
            var rentedBook = await this._DbMan.Rents
                .Where(Q => Q.BookId == bookId && Q.ReturnedAt == null && Q.UserId == userId)
                .FirstOrDefaultAsync();
            if(rentedBook == null)
            {
                return false;
            }
            return true;
        }
    }
}
