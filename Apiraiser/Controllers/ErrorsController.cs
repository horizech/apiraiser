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
using System.Security.Claims;

namespace Apiraiser.Controllers
{
    [ApiController]
    [Route("API/[controller]")]
    public class ErrorsController : ControllerBase
    {
        private readonly ILogger<ErrorsController> _logger;

        public ErrorsController(ILogger<ErrorsController> logger)
        {
            _logger = logger;
        }

        [HttpGet("Error")]
        public APIResult Error(string errorCode, string errorMessage)
        {
            return new APIResult
            {
                Success = false,
                Data = null,
                ErrorCode = "AUTH001",
                Message = errorMessage
            };
        }
    }
}
