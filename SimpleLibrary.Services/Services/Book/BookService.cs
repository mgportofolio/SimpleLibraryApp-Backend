using SimpleLibrary.Models.Common.Responses;
using SimpleLibrary.Models.Models.Book;
using SimpleLibraryDbContext.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using SimpleLibrary.Models.Enums;
using Microsoft.EntityFrameworkCore;
using SimpleLibrary.Models.Consts;

namespace SimpleLibrary.Services.Services.Book
{
    public class BookService
    {
        private readonly LibraryDb_DevContext _DbMan;

        public BookService(LibraryDb_DevContext dbContext)
        {
            this._DbMan = dbContext;
        }

        public async Task<DataResponse<List<BookModel>>> GetBooks()
        {
            var res = new DataResponse<List<BookModel>>();
            res.Data = new List<BookModel>();
            var query = from book in _DbMan.Books
                        join rent in _DbMan.Rents on book.BookId equals rent.BookId into ps
                        from newRent in ps.Where(Q => Q.ReturnedAt == null).DefaultIfEmpty()
                        select new BookModel
                        {
                            AuthorName = book.BookAuthor,
                            BookId = book.BookId,
                            BookName = book.BookName,
                            BookGenre = book.BookGenre,
                            BookDescription = book.BookDescription,
                            Availability = newRent == null ? true : newRent.ReturnedAt == null? false : true
                            //if rent = null, it's available, if rent not null, then check returned at, if returned at null then it's not available
                        };
            var books = await query.ToListAsync().ConfigureAwait(false);
            if(books.Count > 0)
            {
                res.Data = books;
                res.SetSuccessStatsus();
            }
            else
            {
                res.SetFailedtatsus();
                res.SetMessage("Books not found", (int)MessageTypeEnum.Warning);
            }
            return res;
        }

        public async Task<DataResponse<BookDetailModel>> GetBook(int id)
        {
            var res = new DataResponse<BookDetailModel>();
            res.Data = new BookDetailModel();
            var book = await this.GetPrivateBook(id);
            if (book != null)
            {
                res.Data = book;
                res.SetSuccessStatsus();
            }
            else
            {
                res.SetFailedtatsus();
                res.SetMessage("Book not found", (int)MessageTypeEnum.Warning);
            }
            return res;
        }

        public async Task<DataResponse<BookDetailModel>> UpdateBook(int id, BookDetailModel updatedBook)
        {
            var res = new DataResponse<BookDetailModel>();
            res.Data = new BookDetailModel();
            var book = await this._DbMan.Books.Where(Q => Q.BookId == id).FirstOrDefaultAsync();
            if (book == null)
            {
                res.SetFailedtatsus();
                res.SetMessage("Book not found", (int)MessageTypeEnum.Warning);
                return res;

            }

            book.BookAuthor = updatedBook.AuthorName;
            book.BookDescription = updatedBook.BookDescription;
            book.BookGenre = updatedBook.BookGenre;
            book.BookName = updatedBook.BookName;
            book.UpdatedAt = DateTime.Now;
            book.UpdatedBy = "SYSTEM";

            try
            {
                this._DbMan.Update(book);
                await this._DbMan.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                //ex should be loggeg to logging services
                res.SetMessage(CommonMessage.CommonErrorMessage, (int)MessageTypeEnum.Exception);
                res.SetFailedtatsus();
                return res;
            }
            res.Data = await this.GetPrivateBook(id);
            res.SetSuccessStatsus();
            res.SetMessage("Buku berhasil diupdate", (int)MessageTypeEnum.Success);

            return res;
        }

        public async Task<BaseResponse> CreateBook(BookDetailModel newBook)
        {
            var res = new BaseResponse();
            if(await IsBookExisted(newBook))
            {
                res.SetMessage("Buku sudah pernah dibuat", (int)MessageTypeEnum.Warning);
                res.SetFailedtatsus();
                return res;
            }

            try
            {
                this._DbMan.Books.Add(new SimpleLibraryDbContext.Entities.Book
                {
                    BookAuthor = newBook.AuthorName,
                    BookDescription = newBook.BookDescription,
                    BookGenre = newBook.BookGenre,
                    BookName = newBook.BookName,
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
            res.SetMessage("Buku berhasil dibuat", (int)MessageTypeEnum.Success);
            return res;
        }

        private async Task<BookDetailModel> GetPrivateBook(int id)
        {
            var book = await this._DbMan.Books
                .Where(Q => Q.BookId == id)
                .Select(Q => new BookDetailModel
                {
                    AuthorName = Q.BookAuthor,
                    BookId = Q.BookId,
                    BookName = Q.BookName,
                    BookGenre = Q.BookGenre,
                    BookDescription = Q.BookDescription
                })
                .FirstOrDefaultAsync();
            return book;
        }

        private async Task<bool> IsBookExisted(BookDetailModel book)
        {
            var existedBook = await this._DbMan.Books
                .Where(Q => Q.BookName == book.BookName && Q.BookGenre == book.BookGenre
                && Q.BookDescription == book.BookDescription && Q.BookAuthor == book.AuthorName)
                .FirstOrDefaultAsync();
            if(existedBook != null)
            {
                return true;
            }
            return false;
        }
    }
}
