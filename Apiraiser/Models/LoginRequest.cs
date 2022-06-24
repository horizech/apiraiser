using System;
using System.Collections.Generic;

using Apiraiser.Services;
using Apiraiser.Enums;
using Apiraiser.Models;

namespace Apiraiser.Models
{
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

}