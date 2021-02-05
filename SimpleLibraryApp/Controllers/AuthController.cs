using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SimpleLibrary.Models.Models.Auth;
using SimpleLibrary.Services.Services.Auth;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SimpleLibraryApp.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : Controller
    {
        private readonly IAuthService _AuthMan;

        public AuthController(IAuthService authenticationService)
        {
            this._AuthMan = authenticationService;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody]AuthLoginModel auth)
        {
            var res = await this._AuthMan.Login(auth);
            if(res.Status == false)
            {
                return BadRequest(res);
            }
            return Ok(res);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> MakeAdmin(int id)
        {
            var res = await this._AuthMan.MakeAdmin(id);
            if(res.Status == false)
            {
                return BadRequest(res);
            }
            return Ok(res);
        }
    }
}
