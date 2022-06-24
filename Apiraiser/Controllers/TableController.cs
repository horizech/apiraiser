using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

using Apiraiser.Services;
using Apiraiser.Models;
using Apiraiser.Enums;
using Apiraiser.Constants;
using Apiraiser.Helpers;
using Apiraiser.Exceptions;
using Apiraiser.Interfaces;
using Apiraiser.Filters;

namespace Apiraiser.Controllers
{
    [ApiController]
    [Route("API/[controller]")]
    public class TableController : ControllerBase
    {
        private readonly ILogger<TableController> _logger;

        public TableController(ILogger<TableController> logger)
        {
            _logger = logger;
        }

        [ApiraiserAuthorized]
        [HttpPost("CreateTable")]
        [Authorize]
        public async Task<APIResult> CreateTable(string table, List<ColumnInfo> columns)
        {
            if (table == null || table.Count() == 0)
            {
                return APIResult.GetSimpleFailureResult("Table is not valid!");
            }


            bool UsingPredefinedColumns = false;
            List<string> predefinedColumns = Columns.PredefinedColumns.Descriptions.Select(x => x["Name"].ToLower()).ToList();

            if ((columns?.Count ?? 0) > 0)
            {
                columns.ForEach(x =>
                {
                    string name = x.Name.ToLower();
                    if (predefinedColumns.Contains(name))
                    {
                        UsingPredefinedColumns = true;
                    }
                });

                if (UsingPredefinedColumns)
                {
                    return APIResult.GetSimpleFailureResult("Using predefined column(s)!");
                }
            }

            return await ServiceManager.Instance.GetService<TableService>().CreateTable(Schemas.Application, table, columns);
        }

        [ApiraiserAuthorized]
        [HttpGet("GetTablesList")]
        [Authorize]
        public async Task<APIResult> GetTablesList()
        {
            return await ServiceManager.Instance.GetService<TableService>().GetTablesList(Schemas.Application);
        }

        [ApiraiserAuthorized]
        [HttpGet("GetTableColumns")]
        [Authorize]
        public async Task<APIResult> GetTableColumns(string table)
        {
            return await ServiceManager.Instance.GetService<TableService>().GetTableColumns(Schemas.Application, table);
        }


        [ApiraiserAuthorized]
        [HttpGet("GetPredefinedColumns")]
        [Authorize]
        public APIResult GetPredefinedColumns()
        {
            return ServiceManager.Instance.GetService<TableService>().GetPredefinedColumns();
        }

        [ApiraiserAuthorized]
        [HttpPost("AddColumn")]
        public async Task<APIResult> AddColumn(string table, ColumnInfo columnInfo)
        {
            try
            {
                if (table == null || table.Count() == 0)
                {
                    return APIResult.GetSimpleFailureResult("Table is not valid!");
                }
                if (columnInfo == null || string.IsNullOrEmpty(columnInfo.Name) || columnInfo.Datatype < 0)
                {
                    return APIResult.GetSimpleFailureResult("Column information is not valid!");
                }

                List<string> predefinedColumns = Columns.PredefinedColumns.Descriptions.Select(x => x["Name"].ToLower()).ToList();

                if (predefinedColumns.Contains(columnInfo.Name))
                {
                    return APIResult.GetSimpleFailureResult("Cannot delete predefined column!");
                }

                Task<APIResult> createTask = ServiceManager.Instance.GetService<TableService>().AddColumn(Schemas.Application, table, columnInfo);
                try
                {
                    APIResult result = await createTask;
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
        [HttpDelete("DeleteColumn")]
        public async Task<APIResult> DeleteColumn(string table, string column)
        {
            try
            {
                if (table == null || table.Count() == 0)
                {
                    return APIResult.GetSimpleFailureResult("Table is not valid!");
                }
                if (column == null || column.Count() == 0)
                {
                    return APIResult.GetSimpleFailureResult("Data is not valid!");
                }

                List<string> predefinedColumns = Columns.PredefinedColumns.Descriptions.Select(x => x["Name"].ToLower()).ToList();

                if (predefinedColumns.Contains(column))
                {
                    return APIResult.GetSimpleFailureResult("Cannot delete predefined column!");
                }

                Task<APIResult> createTask = ServiceManager.Instance.GetService<TableService>().DeleteColumn(Schemas.Application, table, column);
                try
                {
                    APIResult result = await createTask;
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
        [ApiraiserUserAccessLevel(insertPropertyName: "data.CreatedBy")]
        [Authorize]
        [HttpPost("InsertRows")]
        public async Task<APIResult> InsertRows(string table, List<Dictionary<string, object>> data)
        {
            try
            {
                if (table == null || table.Count() == 0)
                {
                    return APIResult.GetSimpleFailureResult("Table is not valid!");
                }
                if (data == null || data.Count() == 0)
                {
                    return APIResult.GetSimpleFailureResult("Data is not valid!");
                }

                List<string> predefinedColumns = Columns.PredefinedColumns.Descriptions.Select(x => x["Name"].ToLower()).ToList();

                for (int i = 0; i < data.Count; i++)
                {
                    data[i].Keys.ToList().ForEach(key =>
                    {
                        if (predefinedColumns.Contains(key.ToLower()))
                        {
                            ServiceManager.Instance.GetService<LogService>().Print(string.Format("Removing key: {0}", key), LoggingLevel.Info);
                            data[i].Remove(key);
                        }
                    });
                }

                for (int i = 0; i < data.Count; i++)
                {
                    data[i].Keys.ToList().ForEach(key =>
                    {
                        if (predefinedColumns.Contains(key.ToLower()))
                        {
                            ServiceManager.Instance.GetService<LogService>().Print(string.Format("Removing key: {0}", key), LoggingLevel.Info);
                            data[i].Remove(key);
                        }
                    });

                    Columns.AppendCreatedInfo(data[i], Users.GetUserId(User));

                }

                Task<APIResult> insertTask = ServiceManager.Instance.GetService<TableService>().InsertRows(Schemas.Application, table, data);
                try
                {
                    APIResult result = await insertTask;
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
        [ApiraiserUserAccessLevel("CreatedBy")]
        [HttpGet("GetRows")]
        [Authorize]
        public async Task<APIResult> GetRows(string table, int CreatedBy = 0, int LastUpdatedBy = 0)
        {
            if (table == null || table.Count() == 0)
            {
                return APIResult.GetSimpleFailureResult("Table is not valid!");
            }

            // If user only data access
            // List<QuerySearchItem> UserParameters = new List<QuerySearchItem>{
            //     new QuerySearchItem(){
            //         Name = "CreatedBy",
            //         CaseSensitive = false,
            //         Condition = Enums.ColumnCondition.Equal,
            //         Value = Users.GetUserId(User)
            //     }
            // };
            // return await ServiceManager.Instance.GetService<TableService>().GetRowsByConditions(Schemas.Application, table, UserParameters);
            if (CreatedBy == 0 && LastUpdatedBy == 0)
            {
                return await ServiceManager.Instance.GetService<TableService>().GetRows(Schemas.Application, table);
            }
            else
            {
                List<QuerySearchItem> parameters = new List<QuerySearchItem>();
                if (CreatedBy > 0)
                {
                    parameters.Add(new QuerySearchItem()
                    {
                        Name = "CreatedBy",
                        Value = CreatedBy,
                        Condition = ColumnCondition.Equal,
                        CaseSensitive = false
                    });
                }
                if (LastUpdatedBy > 0)
                {
                    parameters.Add(new QuerySearchItem()
                    {
                        Name = "LastUpdatedBy",
                        Value = LastUpdatedBy,
                        Condition = ColumnCondition.Equal,
                        CaseSensitive = false
                    });
                }
                return await ServiceManager.Instance.GetService<TableService>().GetRowsByConditions(Schemas.Application, table, parameters);
            }
        }

        [ApiraiserAuthorized]
        [ApiraiserUserAccessLevel("parameters.CreatedBy")]
        [Authorize]
        [HttpPost("GetRowsByConditions")]
        public async Task<APIResult> GetRowsByConditions(string table, List<QuerySearchItem> parameters)
        {
            if (table == null || table.Count() == 0)
            {
                return APIResult.GetSimpleFailureResult("Table is not valid!");
            }

            // If user only data access
            // bool doesUserParamExist = false;

            // for (int i = 0; i < parameters.Count; i++)
            // {
            //     if (parameters[i].Name.ToLower().Equals("createdby"))
            //     {
            //         parameters[i].Name = "CreatedBy";
            //         parameters[i].Value = Users.GetUserId(User);
            //         parameters[i].CaseSensitive = false;
            //         parameters[i].Condition = Enums.ColumnCondition.Equal;
            //         doesUserParamExist = true;
            //     }
            // }

            // if (!doesUserParamExist)
            // {
            //     parameters.Add(
            //         new QuerySearchItem()
            //         {
            //             Name = "CreatedBy",
            //             CaseSensitive = false,
            //             Condition = Enums.ColumnCondition.Equal,
            //             Value = Users.GetUserId(User)
            //         }
            //     );
            // }
            // return await ServiceManager.Instance.GetService<TableService>().GetRowsByConditions(Schemas.Application, table, parameters);

            return await ServiceManager.Instance.GetService<TableService>().GetRowsByConditions(Schemas.Application, table, parameters);
        }

        [ApiraiserAuthorized]
        [ApiraiserUserAccessLevel("request.Parameters.CreatedBy", updatePropertyName: "request.Data.LastUpdatedBy")]
        [Authorize]
        [HttpPut("UpdateRows")]
        public async Task<APIResult> UpdateRows(string table, UpdateRequest request)
        {
            if (table == null || table.Count() == 0)
            {
                return APIResult.GetSimpleFailureResult("Table is not valid!");
            }
            if (request == null || request.Data == null || request.Data.Count() == 0)
            {
                return APIResult.GetSimpleFailureResult("Data is not valid!");
            }

            List<string> predefinedColumns = Columns.PredefinedColumns.Descriptions.Select(x => x["Name"].ToLower()).ToList();

            request.Data.Keys.ToList().ForEach(key =>
            {
                if (predefinedColumns.Contains(key.ToLower()))
                {
                    ServiceManager.Instance.GetService<LogService>().Print(string.Format("Removing key: {0}", key), LoggingLevel.Info);
                    request.Data.Remove(key);
                }
            });

            Columns.AppendUpdatedInfo(request.Data, Users.GetUserId(User));

            if (request.Parameters == null)
            {
                request.Parameters = new List<QuerySearchItem>();
            }

            // If user only read access
            // bool doesUserParamExist = false;

            // for (int i = 0; i < request.Parameters.Count; i++)
            // {
            //     if (request.Parameters[i].Name.ToLower().Equals("createdby"))
            //     {
            //         request.Parameters[i].Name = "CreatedBy";
            //         request.Parameters[i].Value = Users.GetUserId(User);
            //         request.Parameters[i].CaseSensitive = false;
            //         request.Parameters[i].Condition = Enums.ColumnCondition.Equal;
            //         doesUserParamExist = true;
            //     }
            // }

            // if (!doesUserParamExist)
            // {
            //     request.Parameters.Add(
            //         new QuerySearchItem()
            //         {
            //             Name = "CreatedBy",
            //             CaseSensitive = false,
            //             Condition = Enums.ColumnCondition.Equal,
            //             Value = Users.GetUserId(User)
            //         }
            //     );
            // }

            // return await ServiceManager.Instance.GetService<TableService>().UpdateRows(Schemas.Application, table, request.Data, request.Parameters);

            return await ServiceManager.Instance.GetService<TableService>().UpdateRows(Schemas.Application, table, request.Data, request.Parameters);
        }


        [ApiraiserAuthorized]
        [ApiraiserUserAccessLevel("parameters.CreatedBy")]
        [HttpDelete("DeleteRows")]
        public async Task<APIResult> DeleteRows(string table, List<QuerySearchItem> parameters)
        {
            if (table == null || table.Count() == 0)
            {
                return APIResult.GetSimpleFailureResult("Table is not valid!");
            }

            // If user only read access
            // bool doesUserParamExist = false;

            // for (int i = 0; i < parameters.Count; i++)
            // {
            //     if (parameters[i].Name.ToLower().Equals("createdby"))
            //     {
            //         parameters[i].Name = "CreatedBy";
            //         parameters[i].Value = Users.GetUserId(User);
            //         parameters[i].CaseSensitive = false;
            //         parameters[i].Condition = Enums.ColumnCondition.Equal;
            //         doesUserParamExist = true;
            //     }
            // }

            // if (!doesUserParamExist)
            // {
            //     parameters.Add(
            //         new QuerySearchItem()
            //         {
            //             Name = "CreatedBy",
            //             CaseSensitive = false,
            //             Condition = Enums.ColumnCondition.Equal,
            //             Value = Users.GetUserId(User)
            //         }
            //     );
            // }
            // return await ServiceManager.Instance.GetService<TableService>().DeleteRows(Schemas.Application, table, parameters);

            return await ServiceManager.Instance.GetService<TableService>().DeleteRows(Schemas.Application, table, parameters);
        }
    }
}
