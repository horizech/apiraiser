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
    public class UserRolesController : ControllerBase
    {
        private readonly ILogger<UserRolesController> _logger;

        public UserRolesController(ILogger<UserRolesController> logger)
        {
            _logger = logger;
        }

        [ApiraiserAuthorized]
        [HttpGet("GetUserRoles")]
        public async Task<APIResult> GetUserRoles()
        {
            return await ServiceManager.Instance.GetService<UserRolesService>().GetUserRoles();
        }

        [ApiraiserAuthorized]
        [HttpPost("AddUserRole")]
        public async Task<APIResult> AddUserRole(Dictionary<string, object> data)
        {
            try
            {
                if (data == null || data.Count() == 0 || !data.ContainsKey("User") || !data.ContainsKey("Role"))
                {
                    return APIResult.GetSimpleFailureResult("UserRole must contain User and Role!");
                }

                List<string> predefinedColumns = Columns.PredefinedColumns.Descriptions.Select(x => x["Name"].ToLower()).ToList();

                for (int i = 0; i < data.Count; i++)
                {
                    data.Keys.ToList().ForEach(key =>
                    {
                        if (predefinedColumns.Contains(key.ToLower()))
                        {
                            ServiceManager.Instance.GetService<LogService>().Print(string.Format("Removing key: {0}", key), LoggingLevel.Info);
                            data.Remove(key);
                        }
                    });
                }

                Columns.AppendCreatedInfo(data, Users.GetUserId(User));

                try
                {
                    APIResult result = await ServiceManager.Instance.GetService<UserRolesService>().AddUserRole(data);
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

        [ApiraiserAuthorized]
        [HttpPut("UpdateUserRole")]
        public async Task<APIResult> UpdateUserRole(int id, Dictionary<string, object> data)
        {
            try
            {
                if (data == null || data.Count() == 0 || !data.ContainsKey("User") || !data.ContainsKey("Role"))
                {
                    return APIResult.GetSimpleFailureResult("UserRole must contain User and Role!");
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

                Columns.AppendUpdatedInfo(data, Users.GetUserId(User));

                try
                {
                    APIResult result = await ServiceManager.Instance.GetService<UserRolesService>().UpdateUserRole(id, data);
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

        [ApiraiserAuthorized]
        [HttpDelete("DeleteUserRole")]
        public async Task<APIResult> DeleteUserRole(int id)
        {
            try
            {
                try
                {
                    APIResult result = await ServiceManager.Instance.GetService<UserRolesService>().DeleteUserRole(id);
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
