using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpleLibrary.Models.Models.Rent;
using SimpleLibrary.Services.Services.Rent;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SimpleLibraryApp.Controllers
{
    [ApiController]
    [Route("api/v1/rent")]
    public class RentController : Controller
    {
        private readonly RentService _RentMan;

        public RentController(RentService rentService)
        {
            this._RentMan = rentService;
        }

        [Authorize(Roles = "User, Admin")]
        [HttpGet]
        public async Task<IActionResult> GetRent()
        {
            var userId = HttpContext.Items[ClaimTypes.NameIdentifier].ToString();
            var role = HttpContext.Items[ClaimTypes.Role].ToString();
            if (role == "Admin")
            {
                var resAdmin = await _RentMan.GetRentHistory();
                if (resAdmin.Status == false)
                {
                    return NotFound(resAdmin);
                }
                return Ok(resAdmin);
            }

            if (role == "User")
            {
                var isValid = int.TryParse(userId, out int id);
                if(isValid)
                {
                    var resUser = await _RentMan.GetRentHistory(id);
                    if (resUser.Status == false)
                    {
                        return NotFound(resUser);
                    }
                    return Ok(resUser);
                }
            }
            return Ok();
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<IActionResult> Rent([FromBody]RentRequestModel rent)
        {
            var userId = HttpContext.Items[ClaimTypes.NameIdentifier].ToString();
            var res = await _RentMan.RentBook(rent.BookId, userId);
            if (res.Status == false)
            {
                return BadRequest(res);
            }
            return Ok(res);
        }

        [Authorize(Roles = "User")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Return(int id)
        {
            var userId = HttpContext.Items[ClaimTypes.NameIdentifier].ToString();
            var res = await _RentMan.ReturnBook(id, userId);
            if (res.Status == false)
            {
                return BadRequest(res);
            }
            return Ok(res);
        }
    }
}
