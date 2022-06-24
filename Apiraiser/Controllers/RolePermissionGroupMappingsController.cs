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
    public class RolePermissionGroupMappingsController : ControllerBase
    {
        private readonly ILogger<RolePermissionGroupMappingsController> _logger;

        public RolePermissionGroupMappingsController(ILogger<RolePermissionGroupMappingsController> logger)
        {
            _logger = logger;
        }

        [ApiraiserAuthorized]
        [HttpGet("GetRolePermissionGroupMappings")]
        public async Task<APIResult> GetRolePermissionGroupMappings()
        {
            return await ServiceManager.Instance.GetService<RolePermissionGroupMappingsService>().GetRolePermissionGroupMappings();

        }

        [ApiraiserAuthorized]
        [HttpPost("AddRolePermissionGroupMapping")]
        public async Task<APIResult> AddRolePermissionGroupMapping(Dictionary<string, object> data)
        {
            try
            {
                if (data == null || data.Count() == 0 || !data.ContainsKey("Role") || !data.ContainsKey("PermissionGroup") || !data.ContainsKey("IsSystem"))
                {
                    return APIResult.GetSimpleFailureResult("PermissionGroup must contain Role, PermissionGroup and IsSystem!");
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
                    APIResult result = await ServiceManager.Instance.GetService<RolePermissionGroupMappingsService>().AddRolePermissionGroupMapping(data);
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
        [HttpPut("UpdateRolePermissionGroupMapping")]
        public async Task<APIResult> UpdateRolePermissionGroupMapping(int Id, Dictionary<string, object> data)
        {
            try
            {
                if (data == null || data.Count() == 0 || !data.ContainsKey("Role") || !data.ContainsKey("PermissionGroup") || !data.ContainsKey("IsSystem"))
                {
                    return APIResult.GetSimpleFailureResult("PermissionGroup must contain Role, PermissionGroup and IsSystem!");
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
                    APIResult result = await ServiceManager.Instance.GetService<RolePermissionGroupMappingsService>().UpdateRolePermissionGroupMapping(Id, data);
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
        [HttpDelete("DeleteRolePermissionGroupMapping")]
        public async Task<APIResult> DeleteRolePermissionGroupMapping(int Id)
        {
            try
            {
                try
                {
                    APIResult result = await ServiceManager.Instance.GetService<RolePermissionGroupMappingsService>().DeleteRolePermissionGroupMapping(Id);
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
