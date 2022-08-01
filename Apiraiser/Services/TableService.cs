using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

using Apiraiser.Enums;
using Apiraiser.Models;
using Apiraiser.Helpers;
using Apiraiser.Constants;

using Microsoft.Extensions.Configuration;

namespace Apiraiser.Services
{
    public class TableService : BaseService
    {

        public TableService(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<APIResult> CreateTable(string schema, string table, int userId, List<ColumnInfo> columns)
        {

            bool createTableResult = await ServiceManager
                .Instance
                .GetService<DatabaseService>()
                .GetDatabaseDriver()
                .CreateTable(schema, table, columns);

            List<Dictionary<string, object>> tablePermissions = new List<Dictionary<string, object>>();

            tablePermissions.AddRange(new List<Dictionary<string, object>>
                {
                    new Dictionary<string, object>{
                        { "Schema", schema },
                        { "Table", table },
                        { "Role", 1 },
                        { "UserAccessLevel", 1 },
                        { "CanRead", true },
                        { "CanWrite", true },
                        { "CanUpdate", true },
                        { "CanDelete", true },
                        { "CreatedOn", DateTime.UtcNow},
                        { "CreatedBy", userId}
                    },
                    new Dictionary<string, object>{
                        { "Schema", schema },
                        { "Table", table },
                        { "Role", 2 },
                        { "UserAccessLevel", 1 },
                        { "CanRead", true },
                        { "CanWrite", true },
                        { "CanUpdate", true },
                        { "CanDelete", true },
                        { "CreatedOn", DateTime.UtcNow},
                        { "CreatedBy", userId}
                    },
                    new Dictionary<string, object>{
                        { "Schema", schema },
                        { "Table", table },
                        { "Role", 3 },
                        { "UserAccessLevel", 2 },
                        { "CanRead", true },
                        { "CanWrite", schema == Schemas.Data },
                        { "CanUpdate", schema == Schemas.Data },
                        { "CanDelete", schema == Schemas.Data },
                        { "CreatedOn", DateTime.UtcNow},
                        { "CreatedBy", userId}
                    }
                }
            );

            await ServiceManager
                            .Instance
                            .GetService<DatabaseService>()
                            .GetDatabaseDriver()
                            .InsertRows(Schemas.Administration, TableNames.TablePermissions.ToString(), tablePermissions);


            return new APIResult()
            {
                Success = true,
                Message = "Table Created Successfully!",
                Data = null
            };
        }

        public async Task<APIResult> GetTablesList(string schema)
        {
            return new APIResult()
            {
                Success = true,
                Message = "Tables list loaded Successfully!",
                Data = await ServiceManager
                    .Instance
                    .GetService<DatabaseService>()
                    .GetDatabaseDriver()
                    .GetTablesList(schema)
            };
        }

        public async Task<APIResult> GetTableColumns(string schema, string table)
        {
            try
            {
                List<ColumnInfo> columns = await ServiceManager
                    .Instance
                    .GetService<DatabaseService>()
                    .GetDatabaseDriver()
                    .GetTableColumns(schema, table);

                if (columns == null || columns.Count == 0)
                {
                    return APIResult.GetSimpleFailureResult("Table not found!");
                }
                else
                {
                    return new APIResult()
                    {
                        Success = true,
                        Message = "Columns loaded Successfully!",
                        Data = columns
                    };
                }
            }
            catch (Exception e)
            {
                return APIResult.GetExceptionResult(e);
            }
        }

        public APIResult GetPredefinedColumns()
        {
            return new APIResult
            {
                Success = true,
                Message = "Predefined columns loaded successfully!",
                Data = Columns.PredefinedColumns
            };
        }

        public async Task<APIResult> AddColumn(string schema, string table, ColumnInfo columnInfo)
        {
            try
            {
                return await ServiceManager
                    .Instance
                    .GetService<DatabaseService>()
                    .GetDatabaseDriver()
                    .AddColumn(schema, table, columnInfo);
            }
            catch (Exception e)
            {
                return APIResult.GetExceptionResult(e);
            }
        }

        public async Task<APIResult> DeleteColumn(string schema, string table, string column)
        {
            try
            {
                return await ServiceManager
                    .Instance
                    .GetService<DatabaseService>()
                    .GetDatabaseDriver()
                    .DeleteColumn(schema, table, column);
            }
            catch (Exception e)
            {
                return APIResult.GetExceptionResult(e);
            }
        }

        public async Task<APIResult> InsertRows(string schema, string table, List<Dictionary<string, object>> data)
        {
            List<int> result = await ServiceManager
                    .Instance
                    .GetService<DatabaseService>()
                    .GetDatabaseDriver()
                    .InsertRows(schema, table, data);

            if ((result?.Count ?? 0) > 0)
            {
                return new APIResult()
                {
                    Success = true,
                    Message = "Row(s) inserted Successfully!",
                    Data = result
                };
            }
            else
            {
                return new APIResult()
                {
                    Success = false,
                    Message = "Error occured while inserting Row(s)!",
                    Data = null
                };
            }
        }


        public async Task<APIResult> GetRows(string schema, string table)
        {
            List<Dictionary<string, object>> result = await ServiceManager
                            .Instance
                            .GetService<DatabaseService>()
                            .GetDatabaseDriver()
                            .GetRows(schema, table);

            if ((result?.Count ?? 0) > 0)
            {
                return new APIResult()
                {
                    Success = true,
                    Message = "Rows loaded successfully!",
                    Data = result
                };
            }
            else
            {
                return new APIResult()
                {
                    Success = false,
                    Message = "Nothing found!",
                    Data = result
                };
            }
        }

        public async Task<APIResult> GetRowsByConditions(string schema, string table, List<QuerySearchItem> parameters)
        {
            List<Dictionary<string, object>> result = await ServiceManager
                            .Instance
                            .GetService<DatabaseService>()
                            .GetDatabaseDriver()
                            .GetRowsByConditions(schema, table, parameters);

            if ((result?.Count ?? 0) > 0)
            {
                return new APIResult()
                {
                    Success = true,
                    Message = "Rows loaded successfully!",
                    Data = result
                };
            }
            else
            {
                return new APIResult()
                {
                    Success = false,
                    Message = "Nothing found!",
                    Data = result
                };
            }
        }

        public async Task<APIResult> UpdateRows(string schema, string table, Dictionary<string, object> data, List<QuerySearchItem> parameters)
        {
            bool result = await ServiceManager
                    .Instance
                    .GetService<DatabaseService>()
                    .GetDatabaseDriver()
                    .UpdateRows(schema, table, data, parameters);

            if (result == true)
            {
                return new APIResult()
                {
                    Success = true,
                    Message = "Row(s) updated Successfully!",
                    Data = null
                };
            }
            else
            {
                return new APIResult()
                {
                    Success = false,
                    Message = "Error occured while updated Row(s)!",
                    Data = null
                };
            }
        }

        public async Task<APIResult> DeleteRows(string schema, string table, List<QuerySearchItem> parameters)
        {
            bool result = await ServiceManager
                    .Instance
                    .GetService<DatabaseService>()
                    .GetDatabaseDriver()
                    .DeleteRows(schema, table, parameters);

            if (result == true)
            {
                return new APIResult()
                {
                    Success = true,
                    Message = "Row(s) deleted Successfully!",
                    Data = null
                };
            }
            else
            {
                return new APIResult()
                {
                    Success = false,
                    Message = "Error occured while deleting Row(s)!",
                    Data = null
                };
            }
        }
    }
}
