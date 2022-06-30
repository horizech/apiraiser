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
    public class TablePermissionsController : ControllerBase
    {
        private readonly ILogger<TablePermissionsController> _logger;

        public TablePermissionsController(ILogger<TablePermissionsController> logger)
        {
            _logger = logger;
        }

        [TablePermission(Schemas.System, "TablePermissions", "CanRead")]
        [HttpGet("GetTablePermissions")]
        public async Task<APIResult> GetTablePermissions()
        {
            return await ServiceManager.Instance.GetService<TablePermissionsService>().GetTablePermissions();
        }

        [TablePermission(Schemas.System, "TablePermissions", "CanWrite")]
        [HttpPost("AddTablePermission")]
        public async Task<APIResult> AddTablePermission(Dictionary<string, object> data)
        {
            try
            {
                if (data == null || data.Count() == 0 || !data.ContainsKey("Schema") || !data.ContainsKey("Table") || !data.ContainsKey("Role") || !data.ContainsKey("UserAccessLevel") || !data.ContainsKey("CanRead") || !data.ContainsKey("CanWrite") || !data.ContainsKey("CanUpdate") || !data.ContainsKey("CanDelete"))
                {
                    return APIResult.GetSimpleFailureResult("Table Permission must contain Schema, Table, Role, UserAccessLevel, CanRead, CanWrite, CanUpdate and CanDelete!");
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
                    APIResult result = await ServiceManager.Instance.GetService<TablePermissionsService>().AddTablePermission(data);
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

        [TablePermission(Schemas.System, "TablePermissions", "CanUpdate")]
        [HttpPut("UpdateTablePermission")]
        public async Task<APIResult> UpdateTablePermission(int id, Dictionary<string, object> data)
        {
            try
            {
                if (data == null || data.Count() == 0 || !data.ContainsKey("Schema") || !data.ContainsKey("Table") || !data.ContainsKey("Role") || !data.ContainsKey("UserAccessLevel") || !data.ContainsKey("CanRead") || !data.ContainsKey("CanWrite") || !data.ContainsKey("CanUpdate") || !data.ContainsKey("CanDelete"))
                {
                    return APIResult.GetSimpleFailureResult("Table Permission must contain Schema, Table, Role, UserAccessLevel, CanRead, CanWrite, CanUpdate and CanDelete!");
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
                    APIResult result = await ServiceManager.Instance.GetService<TablePermissionsService>().UpdateTablePermission(id, data);
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

        [TablePermission(Schemas.System, "TablePermissions", "CanDelete")]
        [HttpDelete("DeleteTablePermission")]
        public async Task<APIResult> DeleteTablePermission(int id)
        {
            try
            {
                try
                {
                    APIResult result = await ServiceManager.Instance.GetService<TablePermissionsService>().DeleteTablePermission(id);
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
