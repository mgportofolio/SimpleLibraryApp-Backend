using BCrypt.Net;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleLibrary.Models.Models.User
{
    public class UserRegistrationModel
    {
        private string _userPassword;

        public string UserName { get; set; }
        public string UserPassword {
            set
            {
                _userPassword = value;
            }
            get
            {
                return BCrypt.Net.BCrypt.EnhancedHashPassword(_userPassword, BCrypt.Net.HashType.SHA256);
            }
        }
    }
}
