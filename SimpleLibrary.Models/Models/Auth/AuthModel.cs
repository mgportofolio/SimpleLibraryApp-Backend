using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleLibrary.Models.Models.Auth
{
    public class AuthModel
    {
        public int UserId { get; set; }
        public string Token { get; set; }
        public string UserRole { get; set; }
    }
}
