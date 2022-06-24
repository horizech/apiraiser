using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

using Apiraiser.Services;
using Apiraiser.Models;
using Apiraiser.Enums;
using Apiraiser.Constants;
using Apiraiser.Helpers;
using Apiraiser.Interfaces;
using Apiraiser.Exceptions;
using Apiraiser.Filters;

namespace Apiraiser.Controllers
{
    [ApiController]
    [Route("API/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }

        [HttpPost("Signup")]
        public async Task<APIResult> Signup(SignupRequest user)
        {
            if (user == null)
            {
                return APIResult.GetSimpleFailureResult("Credentials not valid!");
            }
            if (user.Username == null || user.Username.Length < 6)
            {
                return APIResult.GetSimpleFailureResult("Username should be at least 6 characters!");
            }

            if (user.Password == null || user.Password.Length == 0)
            {
                return APIResult.GetSimpleFailureResult("password is not valid!");
            }

            return await ServiceManager.Instance.GetService<UserService>().Signup(user);
        }

        [HttpPost("Login")]
        public async Task<APIResult> Login(LoginRequest loginDetails)
        {
            if (loginDetails == null)
            {
                return APIResult.GetSimpleFailureResult("Credentials not valid!");
            }
            if ((loginDetails.Username == null || loginDetails.Username.Length < 6) && (loginDetails.Email == null || loginDetails.Email.Length < 6))
            {
                return APIResult.GetSimpleFailureResult("You should provide Username or email!");
            }

            if (loginDetails.Password == null || loginDetails.Password.Length == 0)
            {
                return APIResult.GetSimpleFailureResult("password is not valid!");
            }

            return await ServiceManager.Instance.GetService<UserService>().Login(loginDetails.Username, loginDetails.Email, loginDetails.Password);
        }

        [HttpGet("AuthLogin")]
        [Authorize]
        public async Task<APIResult> AuthLogin()
        {
            return await ServiceManager.Instance.GetService<UserService>().AuthLogin(Users.GetUserId(User));
        }

    }
}
