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
using Apiraiser.Filters;
using Apiraiser.Exceptions;
using Apiraiser.Interfaces;

namespace Apiraiser.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class APIController : ControllerBase
    {
        private readonly ILogger<APIController> _logger;

        public APIController(ILogger<APIController> logger)
        {
            _logger = logger;
        }


        [HttpGet]
        public APIResult Get()
        {
            return new APIResult()
            {
                Success = true,
                Message = "Server up and running"
            };
        }

        [Authorize]
        [SystemPermission("", "CanCreateTables")]
        [HttpPost("{schema}/CreateTable")]
        public async Task<APIResult> CreateTable(string schema, string table, List<ColumnInfo> columns)
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

            return await ServiceManager.Instance.GetService<TableService>().CreateTable(schema, table, columns);
        }

        [Authorize]
        [HttpGet("{schema}/GetTablesList")]
        public async Task<APIResult> GetTablesList(string schema)
        {
            return await ServiceManager.Instance.GetService<TableService>().GetTablesList(schema);
        }

        [Authorize]
        [HttpGet("GetPredefinedColumns")]
        public APIResult GetPredefinedColumns()
        {
            return ServiceManager.Instance.GetService<TableService>().GetPredefinedColumns();
        }

        [Authorize]
        [HttpGet("{schema}/{table}/Columns")]
        public async Task<APIResult> GetTableColumns(string schema, string table)
        {
            return await ServiceManager.Instance.GetService<TableService>().GetTableColumns(schema, table);
        }

        [Authorize]
        [SystemPermission("", "CanUpdateTables")]
        [HttpPost("{schema}/{table}/Column")]
        public async Task<APIResult> AddColumn(string schema, string table, ColumnInfo columnInfo)
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
                    return APIResult.GetSimpleFailureResult("Cannot add predefined column!");
                }

                Task<APIResult> createTask = ServiceManager.Instance.GetService<TableService>().AddColumn(schema, table, columnInfo);
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

        [Authorize]
        [SystemPermission("", "CanUpdateTables")]
        [HttpDelete("{schema}/{table}/Column/{column}")]
        public async Task<APIResult> DeleteColumn(string schema, string table, string column)
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

                Task<APIResult> createTask = ServiceManager.Instance.GetService<TableService>().DeleteColumn(schema, table, column);
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


        [Authorize]
        [TablePermission("", "", "CanRead", "CreatedBy")]
        [HttpGet("{schema}/{table}")]
        public async Task<APIResult> GetRows(string schema, string table, [FromQuery] string orderBy, [FromQuery] string orderDescendingBy, [FromQuery] string groupBy, [FromQuery] int limit = -1, [FromQuery] int offset = -1, int CreatedBy = 0, int LastUpdatedBy = 0)
        {
            if (table == null || table.Count() == 0)
            {
                return APIResult.GetSimpleFailureResult("Table is not valid!");
            }

            SelectSettings selectSettings = new SelectSettings
            {
                Limit = limit,
                Offset = offset,
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
            // return await ServiceManager.Instance.GetService<APIService>().GetRowsByConditions(schema, table, UserParameters, selectSettings);

            if (CreatedBy == 0 && LastUpdatedBy == 0)
            {
                return await ServiceManager.Instance.GetService<APIService>().GetRows(schema, table, selectSettings);
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

                return await ServiceManager.Instance.GetService<APIService>().GetRowsByConditions(schema, table, parameters, selectSettings);
            }
        }

        [Authorize]
        [TablePermission("", "", "CanRead", "CreatedBy")]
        [HttpGet("{schema}/{table}/{id}")]
        public async Task<APIResult> GetRow(string schema, string table, int id, int CreatedBy = 0, int LastUpdatedBy = 0)
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
            // return await ServiceManager.Instance.GetService<APIService>().GetRowsByConditions(schema, table, UserParameters);

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

            return await ServiceManager.Instance.GetService<APIService>().GetRowsByConditions(schema, table, parameters);
        }

        [Authorize]
        [TablePermission("", "", "CanRead", "parameters.CreatedBy")]
        [HttpPost("{schema}/{table}/GetRowsByConditions")]
        public async Task<APIResult> GetRowsByConditions(string schema, string table, List<QuerySearchItem> parameters)
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
            // return await ServiceManager.Instance.GetService<TableService>().GetRowsByConditions(schema, table, parameters);

            return await ServiceManager.Instance.GetService<APIService>().GetRowsByConditions(schema, table, parameters);
            //return await ServiceManager.Instance.GetService<TableService>().GetRowsByConditions(schema, table, parameters);
        }

        [Authorize]
        [TablePermission("", "", "CanWrite", insertPropertyName: "data.CreatedBy")]
        [HttpPost("{schema}/{table}")]
        public async Task<APIResult> InsertRow(string schema, string table, Dictionary<string, object> data)
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

            APIResult tableColumns = await ServiceManager.Instance.GetService<TableService>().GetTableColumns(schema, table);
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

            return await ServiceManager.Instance.GetService<APIService>().InsertRow(schema, table, data);
        }

        [Authorize]
        [TablePermission("", "", "CanUpdate", "CreatedBy", updatePropertyName: "data.LastUpdatedBy")]
        [HttpPut("{schema}/{table}/{id}")]
        public async Task<APIResult> UpdateRow(string schema, string table, int id, Dictionary<string, object> data, int CreatedBy = 0, int LastUpdatedBy = 0)
        {
            if (table == null || table.Count() == 0)
            {
                return APIResult.GetSimpleFailureResult("Table is not valid!");
            }

            if (data == null || data.Count() == 0)
            {
                return APIResult.GetSimpleFailureResult("Data is not valid!");
            }

            APIResult tableColumns = await ServiceManager.Instance.GetService<TableService>().GetTableColumns(schema, table);
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

            // return await ServiceManager.Instance.GetService<APIService>().UpdateRow(schema, table, request.Data, request.Parameters);

            return await ServiceManager.Instance.GetService<APIService>().UpdateRow(schema, table, request.Data, request.Parameters);
        }

        [Authorize]
        [TablePermission("", "", "CanDelete", "CreatedBy")]
        [HttpDelete("{schema}/{table}/{id}")]
        public async Task<APIResult> DeleteRow(string schema, string table, int id, int CreatedBy = 0, int LastUpdatedBy = 0)
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

            // return await ServiceManager.Instance.GetService<APIService>().DeleteRow(schema, table, parameters);

            return await ServiceManager.Instance.GetService<APIService>().DeleteRow(schema, table, parameters);
        }





        [Authorize]
        [TablePermission("", "", "CanWrite", insertPropertyName: "data.CreatedBy")]
        [HttpPost("{schema}/{table}/InsertRows")]
        public async Task<APIResult> InsertRows(string schema, string table, List<Dictionary<string, object>> data)
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

                Task<APIResult> insertTask = ServiceManager.Instance.GetService<TableService>().InsertRows(schema, table, data);
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

        [Authorize]
        [TablePermission("", "", "CanUpdate", "request.parameters.CreatedBy", updatePropertyName: "request.Data.LastUpdatedBy")]
        [HttpPut("{schema}/{table}/UpdateRows")]
        public async Task<APIResult> UpdateRows(string schema, string table, UpdateRequest request)
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

            // return await ServiceManager.Instance.GetService<TableService>().UpdateRows(schema, table, request.Data, request.Parameters);

            return await ServiceManager.Instance.GetService<TableService>().UpdateRows(schema, table, request.Data, request.Parameters);
        }


        [Authorize]
        [TablePermission("", "", "CanDelete", "parameters.CreatedBy")]
        [HttpDelete("{schema}/{table}/DeleteRows")]
        public async Task<APIResult> DeleteRows(string schema, string table, List<QuerySearchItem> parameters)
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
            // return await ServiceManager.Instance.GetService<TableService>().DeleteRows(schema, table, parameters);

            return await ServiceManager.Instance.GetService<TableService>().DeleteRows(schema, table, parameters);
        }


    }
}
