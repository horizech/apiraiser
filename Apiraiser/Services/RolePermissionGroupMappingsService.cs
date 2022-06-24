using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

using Apiraiser.Databases;
using Apiraiser.Models;
using Apiraiser.Helpers;
using Apiraiser.Constants;
using Apiraiser.Enums;

using Microsoft.Extensions.Configuration;

namespace Apiraiser.Services
{
    public class RolePermissionGroupMappingsService : BaseService
    {

        public RolePermissionGroupMappingsService(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<APIResult> GetRolePermissionGroupMappings()
        {
            APIResult cacheResult = await ServiceManager.Instance.GetService<MemoryCacheService>().Get("RolePermissionGroupMappings");
            if (cacheResult != null)
            {
                return cacheResult;
            }

            List<Dictionary<string, object>> result = await QueryDesigner
                .CreateDesigner(schema: Schemas.System, table: TableNames.RolePermissionGroupMappings.ToString())
                .RunSelectQuery();

            APIResult newCacheResult = new APIResult()
            {
                Success = true,
                Message = "RolePermissionGroupMappings loaded successfully!",
                Data = result
            };

            ServiceManager.Instance.GetService<MemoryCacheService>().Set("RolePermissionGroupMappings", newCacheResult);

            return newCacheResult;
        }

        public async Task<APIResult> AddRolePermissionGroupMapping(Dictionary<string, object> data)
        {
            List<int> result = await QueryDesigner
                .CreateDesigner(schema: Schemas.System, table: TableNames.RolePermissionGroupMappings.ToString())
                .AddRow(data)
                .RunInsertQuery();

            ServiceManager.Instance.GetService<MemoryCacheService>().Remove("RolePermissionGroupMappings");

            return new APIResult()
            {
                Success = true,
                Message = "RolePermissionGroupMapping added successfully!",
                Data = result
            };
        }

        public async Task<APIResult> UpdateRolePermissionGroupMapping(int id, Dictionary<string, object> data)
        {
            bool result = await QueryDesigner
                .CreateDesigner(schema: Schemas.System, table: TableNames.RolePermissionGroupMappings.ToString())
                .WhereEquals("Id", id)
                .AddRow(data)
                .RunUpdateQuery();

            ServiceManager.Instance.GetService<MemoryCacheService>().Remove("RolePermissionGroupMappings");

            return new APIResult()
            {
                Success = true,
                Message = "RolePermissionGroupMapping updated successfully!",
                Data = result
            };
        }

        public async Task<APIResult> DeleteRolePermissionGroupMapping(int id)
        {
            bool result = await QueryDesigner
                .CreateDesigner(schema: Schemas.System, table: TableNames.RolePermissionGroupMappings.ToString())
                .WhereEquals("Id", id)
                .RunDeleteQuery();

            ServiceManager.Instance.GetService<MemoryCacheService>().Remove("RolePermissionGroupMappings");

            return new APIResult()
            {
                Success = true,
                Message = "RolePermissionGroupMapping deleted successfully!",
                Data = result
            };
        }
    }
}