using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SimpleLibrary.Models.Models.Book;
using SimpleLibrary.Services.Services.Book;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SimpleLibraryApp.Controllers
{
    [Route("api/v1/book")]
    public class BookController : Controller
    {
        private readonly BookService _BookMan;

        public BookController(BookService bookService)
        {
            this._BookMan = bookService;
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet]
        public async Task<IActionResult> GetBooksAsync()
        {
            var res = await _BookMan.GetBooks();
            if(res.Status == false)
            {
                return BadRequest(res);
            }
            return Ok(res);
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookAsync(int id)
        {
            var res = await _BookMan.GetBook(id);
            if (res.Status == false)
            {
                return BadRequest(res);
            }
            return Ok(res);
        }

        // POST api/<controller>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateBook([FromBody]BookDetailModel book)
        {
            var res = await _BookMan.CreateBook(book);
            if (res.Status == false)
            {
                return BadRequest(res);
            }
            return Ok(res);
        }

        // PUT api/<controller>/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> EditBook(int id, [FromBody]BookDetailModel book)
        {
            var res = await _BookMan.UpdateBook(id, book);
            if (res.Status == false)
            {
                return BadRequest(res);
            }
            return Ok(res);
        }
    }
}
