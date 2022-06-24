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
    public class PermissionGroupMappingsService : BaseService
    {

        public PermissionGroupMappingsService(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<APIResult> GetPermissionGroupMappings()
        {
            APIResult cacheResult = await ServiceManager.Instance.GetService<MemoryCacheService>().Get("PermissionGroupMappings");
            if (cacheResult != null)
            {
                return cacheResult;
            }

            string tablePath = FileSystem.GetPathInConfigurations("Tables/Definitions/" + TableNames.PermissionGroupMappings.ToString() + ".json");
            string tableJson = FileSystem.ReadFile(tablePath);
            TableDefinition table = FileSystem.ReadJsonString<TableDefinition>(tableJson);


            List<ColumnInfo> columnDefinitions = await ServiceManager.Instance.GetService<DatabaseService>().GetDatabaseDriver().GetTableColumns(Schemas.System, TableNames.PermissionGroupMappings.ToString());

            List<Dictionary<string, object>> result = await QueryDesigner
                .CreateDesigner(schema: Schemas.System, table: TableNames.PermissionGroupMappings.ToString())
                .AddColumnDefinitions(columnDefinitions)
                .AddForeignTables(table.ForeignTables)
                .RunSelectQuery();

            APIResult newCacheResult = new APIResult()
            {
                Success = true,
                Message = "PermissionGroupMappings loaded successfully!",
                Data = result
            };

            ServiceManager.Instance.GetService<MemoryCacheService>().Set("PermissionGroupMappings", newCacheResult);

            return newCacheResult;
        }

        public async Task<APIResult> AddPermissionGroupMapping(Dictionary<string, object> data)
        {
            List<int> result = await QueryDesigner
                .CreateDesigner(schema: Schemas.System, table: TableNames.PermissionGroupMappings.ToString())
                .AddRow(data)
                .RunInsertQuery();

            ServiceManager.Instance.GetService<MemoryCacheService>().Remove("PermissionGroupMappings");

            return new APIResult()
            {
                Success = true,
                Message = "PermissionGroupMapping added successfully!",
                Data = result
            };
        }

        public async Task<APIResult> UpdatePermissionGroupMapping(int Id, Dictionary<string, object> data)
        {
            bool result = await QueryDesigner
                .CreateDesigner(schema: Schemas.System, table: TableNames.PermissionGroupMappings.ToString())
                .WhereEquals("Id", Id)
                .AddRow(data)
                .RunUpdateQuery();

            ServiceManager.Instance.GetService<MemoryCacheService>().Remove("PermissionGroupMappings");

            return new APIResult()
            {
                Success = true,
                Message = "PermissionGroupMapping updated successfully!",
                Data = result
            };
        }

        public async Task<APIResult> DeletePermissionGroupMapping(int Id)
        {
            bool result = await QueryDesigner
                .CreateDesigner(schema: Schemas.System, table: TableNames.PermissionGroupMappings.ToString())
                .WhereEquals("Id", Id)
                .RunDeleteQuery();

            ServiceManager.Instance.GetService<MemoryCacheService>().Remove("PermissionGroupMappings");

            return new APIResult()
            {
                Success = true,
                Message = "PermissionGroupMapping deleted successfully!",
                Data = result
            };
        }
    }
}