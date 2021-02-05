using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SimpleLibrary.Models.Models.User;
using SimpleLibrary.Services.Services.User;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SimpleLibraryApp.Controllers
{
    [Route("api/v1/user")]
    public class UserController : Controller
    {
        private readonly UserService _UserMan;

        public UserController(UserService userService)
        {
            this._UserMan = userService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var res = await _UserMan.GetUsers();
            if (res.Status == false)
            {
                return BadRequest(res);
            }
            return Ok(res);
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var res = await _UserMan.GetUser(id);
            if (res.Status == false)
            {
                return BadRequest(res);
            }
            return Ok(res);
        }

        // POST api/<controller>
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody]UserRegistrationModel userRegistration)
        {
            var res = await _UserMan.CreateUser(userRegistration);
            if (res.Status == false)
            {
                return BadRequest(res);
            }
            return Ok(res);
        }
    }
}
