﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

using Apiraiser.Services;
using Apiraiser.Models;
using Apiraiser.Helpers;
using Apiraiser.Constants;
using Apiraiser.Filters;

namespace Apiraiser.Controllers
{
    [ApiController]
    [Route("API/[controller]")]
    public class ApiraiserController : ControllerBase
    {
        private readonly ILogger<ApiraiserController> _logger;

        public ApiraiserController(ILogger<ApiraiserController> logger)
        {
            _logger = logger;
        }

        [HttpGet("IsInitialized")]
        public async Task<APIResult> IsInitialized()
        {
            return await ServiceManager.Instance.GetService<ApiraiserService>().IsInitialized();
        }

        [HttpPost("Initialize")]
        public async Task<APIResult> Initialize(LoginRequest loginDetails)
        {
            if (loginDetails == null)
            {
                return APIResult.GetSimpleFailureResult("Credentials not valid!");
            }
            if (loginDetails.Username == null || loginDetails.Username.Length < 6)
            {
                return APIResult.GetSimpleFailureResult("Username should be at least 6 characters!");
            }

            if (loginDetails.Password == null || loginDetails.Password.Length == 0)
            {
                return APIResult.GetSimpleFailureResult("password is not valid!");
            }

            return await ServiceManager.Instance.GetService<ApiraiserService>().Initialize(loginDetails.Username, loginDetails.Password);
        }

        [ApiraiserAuthorized]
        [HttpGet("GetAllActions")]
        public APIResult GetAllActions()
        {
            Assembly asm = Assembly.GetEntryAssembly(); //.GetCallingAssembly() ;// Assembly.GetAssembly(typeof(MyWebDll.MvcApplication));

            var controlleractionlist1 = asm.GetTypes();

            var controlleractionlist = controlleractionlist1
                .Where(type => typeof(Microsoft.AspNetCore.Mvc.ControllerBase).IsAssignableFrom(type))
                .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
                .Where(m => !m.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), true).Any())
                .Select(x => new { Controller = string.Join("", x.DeclaringType.Name.Split("Controller")), Action = x.Name, ReturnType = x.ReturnType.Name, Attributes = String.Join(",", x.GetCustomAttributes().Select(a => a.GetType().Name.Replace("Attribute", ""))) })
                .OrderBy(x => x.Controller).ThenBy(x => x.Action).ToList();

            return new APIResult
            {
                Data = controlleractionlist,
                Success = true,
                Message = "Actions loaded successfully!"
            };
        }

        [ApiraiserAuthorized]
        [HttpGet("GetTablesList")]
        [Authorize]
        public async Task<APIResult> GetTablesList()
        {
            return await ServiceManager.Instance.GetService<TableService>().GetTablesList(Schemas.System);
        }

        [ApiraiserAuthorized]
        [HttpGet("GetTableColumns")]
        [Authorize]
        public async Task<APIResult> GetTableColumns(string table)
        {
            return await ServiceManager.Instance.GetService<TableService>().GetTableColumns(Schemas.System, table);
        }


        [HttpGet("GetInfo")]
        public APIResult GetInfo()
        {
            return new APIResult()
            {
                Success = true,
                Data = new Dictionary<string, object>
                {
                    {"Apiraiser Version", "0.2.0"},
                    {".Net Version", "net5"}
                }
            };
        }
    }
}
