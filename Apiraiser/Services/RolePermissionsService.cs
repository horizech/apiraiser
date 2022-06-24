using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

using Apiraiser.Databases;
using Apiraiser.Models;
using Apiraiser.Helpers;
using Apiraiser.Constants;
using Apiraiser.Enums;
using Apiraiser.Mappings;

using Microsoft.Extensions.Configuration;

namespace Apiraiser.Services
{
    public class RolePermissionsService : BaseService
    {

        public RolePermissionsService(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<APIResult> GetRolePermissions()
        {
            APIResult cacheResult = await ServiceManager.Instance.GetService<MemoryCacheService>().Get("RolePermissions");
            if (cacheResult != null)
            {
                return cacheResult;
            }

            string tablePath = FileSystem.GetPathInConfigurations("Tables/Definitions/" + TableNames.RolePermissions.ToString() + ".json");
            string tableJson = FileSystem.ReadFile(tablePath);
            TableDefinition table = FileSystem.ReadJsonString<TableDefinition>(tableJson);

            List<ColumnInfo> columnDefinitions = await ServiceManager.Instance.GetService<DatabaseService>().GetDatabaseDriver().GetTableColumns(Schemas.System, TableNames.RolePermissions.ToString());

            List<Dictionary<string, object>> result = await QueryDesigner
                .CreateDesigner(schema: Schemas.System, table: TableNames.RolePermissions.ToString())
                .AddColumnDefinitions(columnDefinitions)
                .AddForeignTables(table.ForeignTables)
                .RunSelectQuery();

            APIResult newCacheResult = new APIResult()
            {
                Success = true,
                Message = "RolePermissions loaded successfully!",
                Data = result
            };

            ServiceManager.Instance.GetService<MemoryCacheService>().Set("RolePermissions", newCacheResult);

            return newCacheResult;
        }

        public async Task<APIResult> AddRolePermission(Dictionary<string, object> data)
        {
            List<Dictionary<string, object>> existingRows = await QueryDesigner
                .CreateDesigner(schema: Schemas.System, table: TableNames.RolePermissions.ToString())
                .WhereEquals("Role", data["Role"])
                .WhereEquals("Permission", data["Permission"])
                .RunSelectQuery();
            if (existingRows != null && existingRows.Count > 0)
            {
                return new APIResult()
                {
                    Success = false,
                    Message = "RolePermission with Role and Permission already exist!",
                    Data = null
                };
            }

            List<int> result = await QueryDesigner
                .CreateDesigner(schema: Schemas.System, table: TableNames.RolePermissions.ToString())
                .AddRow(data)
                .RunInsertQuery();

            ServiceManager.Instance.GetService<MemoryCacheService>().Remove("RolePermissions");

            return new APIResult()
            {
                Success = true,
                Message = "RolePermission added successfully!",
                Data = result
            };
        }

        public async Task<APIResult> UpdateRolePermission(int Id, Dictionary<string, object> data)
        {
            List<Dictionary<string, object>> existingRows = await QueryDesigner
                .CreateDesigner(schema: Schemas.System, table: TableNames.RolePermissions.ToString())
                .WhereEquals("Role", data["Role"])
                .WhereEquals("Permission", data["Permission"])
                .RunSelectQuery();
            if (existingRows != null && existingRows.Count > 0 && (int)existingRows[0]["Id"] != Id)
            {
                return new APIResult()
                {
                    Success = false,
                    Message = "RolePermission with Role and Permission already exist!",
                    Data = null
                };
            }

            bool result = await QueryDesigner
                .CreateDesigner(schema: Schemas.System, table: TableNames.RolePermissions.ToString())
                .WhereEquals("Id", Id)
                .AddRow(data)
                .RunUpdateQuery();

            ServiceManager.Instance.GetService<MemoryCacheService>().Remove("RolePermissions");

            return new APIResult()
            {
                Success = true,
                Message = "RolePermission updated successfully!",
                Data = result
            };
        }

        public async Task<APIResult> DeleteRolePermission(int Id)
        {
            bool result = await QueryDesigner
                .CreateDesigner(schema: Schemas.System, table: TableNames.RolePermissions.ToString())
                .WhereEquals("Id", Id)
                .RunDeleteQuery();

            ServiceManager.Instance.GetService<MemoryCacheService>().Remove("RolePermissions");

            return new APIResult()
            {
                Success = true,
                Message = "RolePermission deleted successfully!",
                Data = result
            };
        }
    }
}