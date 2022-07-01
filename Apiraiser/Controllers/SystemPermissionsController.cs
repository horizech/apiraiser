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
    public class SystemPermissionsController : ControllerBase
    {
        private readonly ILogger<SystemPermissionsController> _logger;

        public SystemPermissionsController(ILogger<SystemPermissionsController> logger)
        {
            _logger = logger;
        }

        [TablePermission(Schemas.System, "SystemPermissions", "CanRead")]
        [HttpGet("GetSystemPermissions")]
        public async Task<APIResult> GetSystemPermissions()
        {
            return await ServiceManager.Instance.GetService<SystemPermissionsService>().GetSystemPermissions();
        }

        [TablePermission(Schemas.System, "SystemPermissions", "CanWrite")]
        [HttpPost("AddSystemPermission")]
        public async Task<APIResult> AddSystemPermission(Dictionary<string, object> data)
        {
            try
            {
                if (data == null || data.Count() == 0 || !data.ContainsKey("Role") || !data.ContainsKey("CanCreateTables") || !data.ContainsKey("CanUpdateTables") || !data.ContainsKey("CanDeleteTables"))
                {
                    return APIResult.GetSimpleFailureResult("System Permission must contain Role, CanCreateTables, CanUpdateTables and CanDeleteTables!");
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
                    APIResult result = await ServiceManager.Instance.GetService<SystemPermissionsService>().AddSystemPermission(data);
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

        [TablePermission(Schemas.System, "SystemPermissions", "CanUpdate")]
        [HttpPut("UpdateSystemPermission")]
        public async Task<APIResult> UpdateSystemPermission(int id, Dictionary<string, object> data)
        {
            try
            {
                if (data == null || data.Count() == 0 || !data.ContainsKey("Role") || !data.ContainsKey("CanCreateTables") || !data.ContainsKey("CanUpdateTables") || !data.ContainsKey("CanDeleteTables"))
                {
                    return APIResult.GetSimpleFailureResult("System Permission must contain Role, CanCreateTables, CanUpdateTables and CanDeleteTables!");
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
                    APIResult result = await ServiceManager.Instance.GetService<SystemPermissionsService>().UpdateSystemPermission(id, data);
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

        [TablePermission(Schemas.System, "SystemPermissions", "CanDelete")]
        [HttpDelete("DeleteSystemPermission")]
        public async Task<APIResult> DeleteSystemPermission(int id)
        {
            try
            {
                try
                {
                    APIResult result = await ServiceManager.Instance.GetService<SystemPermissionsService>().DeleteSystemPermission(id);
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
