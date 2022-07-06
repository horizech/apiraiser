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
    public class AdministrationController : ControllerBase
    {
        private readonly ILogger<AdministrationController> _logger;

        public AdministrationController(ILogger<AdministrationController> logger)
        {
            _logger = logger;
        }

        [Authorize]
        [SystemPermission("CanCreateTables")]
        [HttpPost("CreateTable")]
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

            APIResult result = await ServiceManager.Instance.GetService<TableService>().CreateTable(Schemas.Administration, table, columns);
            if (result.Success)
            {
                ServiceManager.Instance.GetService<MemoryCacheService>().Remove(Schemas.Administration + "Tables");
            }
            return result;
        }

        [Authorize]
        [HttpGet("GetTablesList")]
        public async Task<APIResult> GetTablesList(string schema)
        {
            APIResult cacheResult = await ServiceManager.Instance.GetService<MemoryCacheService>().Get(schema + "Tables");
            if (cacheResult != null)
            {
                return cacheResult;
            }
            APIResult result = await ServiceManager.Instance.GetService<TableService>().GetTablesList(schema);
            ServiceManager.Instance.GetService<MemoryCacheService>().Set(schema + "Tables", result);
            return result;

        }

        [Authorize]
        [HttpGet("GetTableColumns")]
        public async Task<APIResult> GetTableColumns(string schema, string table)
        {
            APIResult cacheResult = await ServiceManager.Instance.GetService<MemoryCacheService>().Get(schema + table + "Columns");
            if (cacheResult != null)
            {
                return cacheResult;
            }
            APIResult result = await ServiceManager.Instance.GetService<TableService>().GetTableColumns(schema, table);
            ServiceManager.Instance.GetService<MemoryCacheService>().Set(schema + table + "Columns", result);
            return result;
        }

        [Authorize]
        [HttpGet("GetPredefinedColumns")]
        public APIResult GetPredefinedColumns()
        {
            return ServiceManager.Instance.GetService<TableService>().GetPredefinedColumns();
        }

        [Authorize]
        [SystemPermission("CanUpdateTables")]
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

                Task<APIResult> createTask = ServiceManager.Instance.GetService<TableService>().AddColumn(Schemas.Administration, table, columnInfo);
                try
                {
                    APIResult result = await createTask;
                    if (result.Success)
                    {
                        ServiceManager.Instance.GetService<MemoryCacheService>().Remove(Schemas.Administration + table + "Columns");
                    }
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

        [Authorize]
        [SystemPermission("CanUpdateTables")]
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

                Task<APIResult> deleteTask = ServiceManager.Instance.GetService<TableService>().DeleteColumn(Schemas.Administration, table, column);
                try
                {
                    APIResult result = await deleteTask;
                    if (result.Success)
                    {
                        ServiceManager.Instance.GetService<MemoryCacheService>().Remove(Schemas.Administration + table + "Columns");
                    }
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

        [Authorize]
        [TablePermission(Schemas.Administration, "", "CanRead", "parameters.CreatedBy")]
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
            // return await ServiceManager.Instance.GetService<TableService>().GetRowsByConditions(Schemas.Administration, table, parameters);

            return await ServiceManager.Instance.GetService<APIService>().GetRowsByConditions(Schemas.Administration, table, parameters);
        }


#nullable enable
        [Authorize]
        [TablePermission(Schemas.Administration, "", "CanRead", "CreatedBy")]
        [HttpGet("{table}")]
        public async Task<APIResult> GetRows(string table, [FromQuery] int? limit, [FromQuery] int? offset, [FromQuery] string? orderBy, [FromQuery] string? orderDescendingBy, [FromQuery] string? groupBy, int CreatedBy = 0, int LastUpdatedBy = 0)
        {
            if (table == null || table.Count() == 0)
            {
                return APIResult.GetSimpleFailureResult("Table is not valid!");
            }

            if (!limit.HasValue && !offset.HasValue && CreatedBy == 0 && LastUpdatedBy == 0)
            {
                APIResult cacheResult = await ServiceManager.Instance.GetService<MemoryCacheService>().Get(Schemas.Administration + table + "Data");
                if (cacheResult != null)
                {
                    return cacheResult;
                }
            }
            SelectSettings selectSettings = new SelectSettings
            {
                Limit = limit ?? -1,
                Offset = offset ?? -1,
                OrderBy = orderBy,
                OrderDescendingBy = orderDescendingBy,
                GroupBy = groupBy
            };

            // If user only data access
            // List<QuerySearchItem> UserParameters = new List<QuerySearchItem>{
            //     new QuerySearchItem(){
            //         Name = "CreatedBy",
            //         CaseSensitive = false,
            //         Condition = Enums.ColumnCondition.Equal,
            //         Value = Users.GetUserId(User)
            //     }
            // };
            // return await ServiceManager.Instance.GetService<APIService>().GetRowsByConditions(Schemas.Administration, table, UserParameters, selectSettings);

            APIResult result;
            if (CreatedBy == 0 && LastUpdatedBy == 0)
            {
                result = await ServiceManager.Instance.GetService<APIService>().GetRows(Schemas.Administration, table, selectSettings);
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

                result = await ServiceManager.Instance.GetService<APIService>().GetRowsByConditions(Schemas.Administration, table, parameters, selectSettings);
            }
            if (!limit.HasValue && !offset.HasValue && CreatedBy == 0 && LastUpdatedBy == 0)
            {
                ServiceManager.Instance.GetService<MemoryCacheService>().Set(Schemas.Administration + table + "Data", result);
            }
            return result;
        }
#nullable disable

        [Authorize]
        [TablePermission(Schemas.Administration, "", "CanRead", "CreatedBy")]
        [HttpGet("{table}/{id}")]
        public async Task<APIResult> GetRow(string table, int id, int CreatedBy = 0, int LastUpdatedBy = 0)
        {
            if (table == null || table.Count() == 0)
            {
                return APIResult.GetSimpleFailureResult("Table is not valid!");
            }

            if (id < 1)
            {
                return APIResult.GetSimpleFailureResult("Id is not valid!");
            }

            // If user only data access 
            // List<QuerySearchItem> UserParameters = new List<QuerySearchItem>{
            //     new QuerySearchItem(){
            //         Name = "CreatedBy",
            //         CaseSensitive = false,
            //         Condition = Enums.ColumnCondition.Equal,
            //         Value = Users.GetUserId(User)
            //     },
            //     new QuerySearchItem(){
            //         Name = "Id",
            //         CaseSensitive = false,
            //         Condition = Enums.ColumnCondition.Equal,
            //         Value = id
            //     }
            // };
            // return await ServiceManager.Instance.GetService<APIService>().GetRowsByConditions(Schemas.Administration, table, UserParameters);

            List<QuerySearchItem> parameters = new List<QuerySearchItem>{
                new QuerySearchItem(){
                    Name = "Id",
                    CaseSensitive = false,
                    Condition = Enums.ColumnCondition.Equal,
                    Value = id
                }
            };

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

            return await ServiceManager.Instance.GetService<APIService>().GetRowsByConditions(Schemas.Administration, table, parameters);
        }

        [Authorize]
        [TablePermission(Schemas.Administration, "", "CanWrite", insertPropertyName: "data.CreatedBy")]
        [HttpPost("{table}")]
        public async Task<APIResult> InsertRow(string table, Dictionary<string, object> data)
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

            APIResult tableColumns = await ServiceManager.Instance.GetService<TableService>().GetTableColumns(Schemas.Administration, table);
            if (!tableColumns.Success || tableColumns.Data == null)
            {
                return APIResult.GetSimpleFailureResult("Table is not valid!");
            }

            List<string> MissingColumns = (tableColumns.Data as List<ColumnInfo>).Where(column =>
                (!predefinedColumns.Contains(column.Name.ToLower()) && column.IsRequired && !data.ContainsKey(column.Name)) ? true : false).Select(x => x.Name).ToList();

            if (MissingColumns.Count > 0)
            {
                return APIResult.GetSimpleFailureResult("Data is missing the following columns: " + string.Join(", ", MissingColumns));
            }

            data.Keys.ToList().ForEach(key =>
            {
                if (predefinedColumns.Contains(key.ToLower()))
                {
                    ServiceManager.Instance.GetService<LogService>().Print(string.Format("Removing key: {0}", key), LoggingLevel.Info);
                    data.Remove(key);
                }
            });

            Columns.AppendCreatedInfo(data, Users.GetUserId(User));

            APIResult result = await ServiceManager.Instance.GetService<APIService>().InsertRow(Schemas.Administration, table, data);
            if (result.Success)
            {
                ServiceManager.Instance.GetService<MemoryCacheService>().Remove(Schemas.Administration + table + "Data");
            }
            return result;
        }

        [Authorize]
        [TablePermission(Schemas.Administration, "", "CanUpdate", "CreatedBy", updatePropertyName: "data.LastUpdatedBy")]
        [HttpPut("{table}/{id}")]
        public async Task<APIResult> UpdateRow(string table, int id, Dictionary<string, object> data, int CreatedBy = 0, int LastUpdatedBy = 0)
        {
            if (table == null || table.Count() == 0)
            {
                return APIResult.GetSimpleFailureResult("Table is not valid!");
            }

            if (data == null || data.Count() == 0)
            {
                return APIResult.GetSimpleFailureResult("Data is not valid!");
            }

            APIResult tableColumns = await ServiceManager.Instance.GetService<TableService>().GetTableColumns(Schemas.Administration, table);
            if (!tableColumns.Success || tableColumns.Data == null)
            {
                return APIResult.GetSimpleFailureResult("Table is not valid!");
            }

            UpdateRequest request = new UpdateRequest()
            {
                Parameters = new List<QuerySearchItem>{
                    new QuerySearchItem(){
                        Name = "Id",
                        CaseSensitive = false,
                        Condition = Enums.ColumnCondition.Equal,
                        Value = id
                    }
                },
                Data = data
            };

            if (CreatedBy > 0)
            {
                request.Parameters.Add(new QuerySearchItem()
                {
                    Name = "CreatedBy",
                    Value = CreatedBy,
                    Condition = ColumnCondition.Equal,
                    CaseSensitive = false
                });
            }

            if (LastUpdatedBy > 0)
            {
                request.Parameters.Add(new QuerySearchItem()
                {
                    Name = "LastUpdatedBy",
                    Value = LastUpdatedBy,
                    Condition = ColumnCondition.Equal,
                    CaseSensitive = false
                });
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

            // If user only data access
            // request.Parameters.Add(
            //     new QuerySearchItem()
            //     {
            //         Name = "CreatedBy",
            //         CaseSensitive = false,
            //         Condition = Enums.ColumnCondition.Equal,
            //         Value = Users.GetUserId(User)
            //     }
            // );

            // return await ServiceManager.Instance.GetService<APIService>().UpdateRow(Schemas.Administration, table, request.Data, request.Parameters);

            APIResult result = await ServiceManager.Instance.GetService<APIService>().UpdateRow(Schemas.Administration, table, request.Data, request.Parameters);
            if (result.Success)
            {
                ServiceManager.Instance.GetService<MemoryCacheService>().Remove(Schemas.Administration + table + "Data");
            }
            return result;
        }


        [Authorize]
        [TablePermission(Schemas.Administration, "", "CanDelete", "CreatedBy")]
        [HttpDelete("{table}/{id}")]
        public async Task<APIResult> DeleteRow(string table, int id, int CreatedBy = 0, int LastUpdatedBy = 0)
        {
            if (table == null || table.Count() == 0)
            {
                return APIResult.GetSimpleFailureResult("Table is not valid!");
            }

            if (id < 1)
            {
                return APIResult.GetSimpleFailureResult("Id is not valid!");
            }

            List<QuerySearchItem> parameters = new List<QuerySearchItem>(){
                new QuerySearchItem()
                    {
                        Name = "Id",
                        CaseSensitive = false,
                        Condition = Enums.ColumnCondition.Equal,
                        Value = id
                    }
            };

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

            // if user only data access
            // parameters.Add(
            //     new QuerySearchItem()
            //     {
            //         Name = "CreatedBy",
            //         CaseSensitive = false,
            //         Condition = Enums.ColumnCondition.Equal,
            //         Value = Users.GetUserId(User)
            //     }
            // );

            // return await ServiceManager.Instance.GetService<APIService>().DeleteRow(Schemas.Administration, table, parameters);

            APIResult result = await ServiceManager.Instance.GetService<APIService>().DeleteRow(Schemas.Administration, table, parameters);
            if (result.Success)
            {
                ServiceManager.Instance.GetService<MemoryCacheService>().Remove(Schemas.Administration + table + "Data");
            }
            return result;
        }
    }
}
