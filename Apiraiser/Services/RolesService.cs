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
    public class RolesService : BaseService
    {

        public RolesService(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<APIResult> GetRoles()
        {
            APIResult cacheResult = await ServiceManager.Instance.GetService<MemoryCacheService>().Get("Roles");
            if (cacheResult != null)
            {
                return cacheResult;
            }

            string tablePath = FileSystem.GetPathInConfigurations("Tables/Definitions/" + TableNames.Roles.ToString() + ".json");
            string tableJson = FileSystem.ReadFile(tablePath);
            TableDefinition table = FileSystem.ReadJsonString<TableDefinition>(tableJson);


            List<ColumnInfo> columnDefinitions = await ServiceManager.Instance.GetService<DatabaseService>().GetDatabaseDriver().GetTableColumns(Schemas.System, TableNames.Roles.ToString());

            List<Dictionary<string, object>> result = await QueryDesigner
                .CreateDesigner(schema: Schemas.System, table: TableNames.Roles.ToString())
                .AddColumnDefinitions(columnDefinitions)
                .AddForeignTables(table.ForeignTables)
                .RunSelectQuery();

            APIResult newCacheResult = new APIResult()
            {
                Success = true,
                Message = "Roles loaded successfully!",
                Data = result
            };

            ServiceManager.Instance.GetService<MemoryCacheService>().Set("Roles", newCacheResult);

            return newCacheResult;
        }

        public async Task<APIResult> AddRole(Dictionary<string, object> data)
        {
            List<int> result = await QueryDesigner
                .CreateDesigner(schema: Schemas.System, table: TableNames.Roles.ToString())
                .AddRow(data)
                .RunInsertQuery();

            ServiceManager.Instance.GetService<MemoryCacheService>().Remove("Roles");

            return new APIResult()
            {
                Success = true,
                Message = "Role added successfully!",
                Data = result
            };
        }

        public async Task<APIResult> UpdateRole(int id, Dictionary<string, object> data)
        {
            bool result = await QueryDesigner
                .CreateDesigner(schema: Schemas.System, table: TableNames.Roles.ToString())
                .WhereEquals("Id", id)
                .AddRow(data)
                .RunUpdateQuery();

            ServiceManager.Instance.GetService<MemoryCacheService>().Remove("Roles");

            return new APIResult()
            {
                Success = true,
                Message = "Role updated successfully!",
                Data = result
            };
        }

        public async Task<APIResult> DeleteRole(int id)
        {
            bool result = await QueryDesigner
                .CreateDesigner(schema: Schemas.System, table: TableNames.Roles.ToString())
                .WhereEquals("Id", id)
                .RunDeleteQuery();

            ServiceManager.Instance.GetService<MemoryCacheService>().Remove("Roles");

            return new APIResult()
            {
                Success = true,
                Message = "Role deleted successfully!",
                Data = result
            };
        }
    }
}