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
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;

        public UsersController(ILogger<UsersController> logger)
        {
            _logger = logger;
        }

        [TablePermission(Schemas.System, "Users", "CanRead")]
        [HttpGet("GetUserRoles/{userId}")]
        public async Task<APIResult> GetUserRoles(int userId)
        {
            if (userId < 1)
            {
                return APIResult.GetSimpleFailureResult("User Id is not vaild!");
            }

            return await ServiceManager.Instance.GetService<UsersService>().GetUserRoles(userId);
        }

        [TablePermission(Schemas.System, "Users", "CanRead")]
        [HttpGet("GetUser/{Id}")]
        public async Task<APIResult> GetUser(int Id)
        {
            return await ServiceManager.Instance.GetService<UsersService>().GetUser(Id);

        }

        [TablePermission(Schemas.System, "Users", "CanRead")]
        [HttpGet("GetUsers")]
        public async Task<APIResult> GetUsers()
        {
            return await ServiceManager.Instance.GetService<UsersService>().GetUsers();

        }

        [TablePermission(Schemas.System, "Users", "CanWrite")]
        [HttpPost("AddUser")]
        public async Task<APIResult> AddUser(SignupRequest user)
        {
            try
            {
                return await ServiceManager.Instance.GetService<UsersService>().AddUser(user);
            }
            catch (ApiraiserErrorCodeException e)
            {
                return APIResult.GetSimpleFailureResult(e.Message);
            }
            catch (Exception e)
            {
                return APIResult.GetSimpleFailureResult(e.Message);
            }
        }

        [TablePermission(Schemas.System, "Users", "CanUpdate")]
        [HttpPut("UpdateUser")]
        public async Task<APIResult> UpdateUser(int Id, Dictionary<string, object> data)
        {
            try
            {
                if (data == null || data.Count() == 0)
                {
                    return APIResult.GetSimpleFailureResult("Nothing to update!");
                }

                List<string> predefinedColumns = Columns.PredefinedColumns.Descriptions.Select(x => x["Name"].ToLower()).ToList();

                data.Keys.ToList().ForEach(key =>
                {
                    if (predefinedColumns.Contains(key.ToLower()))
                    {
                        ServiceManager.Instance.GetService<LogService>().Print(string.Format("Removing key: {0}", key), LoggingLevel.Info);
                        data.Remove(key);
                    }
                });

                data.Add("LastUpdatedOn", DateTime.UtcNow);

                try
                {
                    APIResult result = await ServiceManager.Instance.GetService<UsersService>().UpdateUser(Id, data);
                    return result;
                }
                catch (Exception e)
                {
                    IDatabaseErrorHandler handler = ServiceManager.Instance.GetService<DatabaseService>().GetDatabaseErrorHandler();
                    ErrorCode errorCode = handler.GetErrorCode(e.Message);
                    if (errorCode == ErrorCode.DB520)
                    {
                        // It's a null value column constraint violation
                        return APIResult.GetSimpleFailureResult(errorCode.GetMessage() + ": " + e.Message.Split('\"')[1]);
                    }
                    else
                    {
                        return APIResult.GetSimpleFailureResult(e.Message);
                    }
                }
            }
            catch (ApiraiserErrorCodeException e)
            {
                return APIResult.GetSimpleFailureResult(e.Message);
            }
            catch (Exception e)
            {
                return APIResult.GetSimpleFailureResult(e.Message);
            }
        }

        [TablePermission(Schemas.System, "Users", "CanDelete")]
        [HttpDelete("DeleteUser")]
        public async Task<APIResult> DeleteUser(int Id)
        {
            try
            {
                try
                {
                    APIResult result = await ServiceManager.Instance.GetService<UsersService>().DeleteUser(Id);
                    return result;
                }
                catch (Exception e)
                {
                    IDatabaseErrorHandler handler = ServiceManager.Instance.GetService<DatabaseService>().GetDatabaseErrorHandler();
                    ErrorCode errorCode = handler.GetErrorCode(e.Message);
                    if (errorCode == ErrorCode.DB520)
                    {
                        // It's a null value column constraint violation
                        return APIResult.GetSimpleFailureResult(errorCode.GetMessage() + ": " + e.Message.Split('\"')[1]);
                    }
                    else
                    {
                        return APIResult.GetSimpleFailureResult(e.Message);
                    }
                }
            }
            catch (ApiraiserErrorCodeException e)
            {
                return APIResult.GetSimpleFailureResult(e.Message);
            }
            catch (Exception e)
            {
                return APIResult.GetSimpleFailureResult(e.Message);
            }
        }


    }
}
