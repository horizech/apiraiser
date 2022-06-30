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
    public class SystemPermissionsService : BaseService
    {

        public SystemPermissionsService(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<APIResult> GetSystemPermissions()
        {
            APIResult cacheResult = await ServiceManager.Instance.GetService<MemoryCacheService>().Get(TableNames.SystemPermissions.ToString());
            if (cacheResult != null)
            {
                return cacheResult;
            }

            List<Dictionary<string, object>> result = await QueryDesigner
                .CreateDesigner(schema: Schemas.System, table: TableNames.SystemPermissions.ToString())
                .RunSelectQuery();

            APIResult newCacheResult = new APIResult()
            {
                Success = true,
                Message = "System Permissions loaded successfully!",
                Data = result
            };

            ServiceManager.Instance.GetService<MemoryCacheService>().Set(TableNames.SystemPermissions.ToString(), newCacheResult);

            return newCacheResult;

        }

        public async Task<APIResult> AddSystemPermission(Dictionary<string, object> data)
        {
            List<int> result = await QueryDesigner
                .CreateDesigner(schema: Schemas.System, table: TableNames.SystemPermissions.ToString())
                .AddRow(data)
                .RunInsertQuery();

            ServiceManager.Instance.GetService<MemoryCacheService>().Remove(TableNames.SystemPermissions.ToString());

            return new APIResult()
            {
                Success = true,
                Message = "SystemPermission added successfully!",
                Data = result
            };
        }

        public async Task<APIResult> UpdateSystemPermission(string name, Dictionary<string, object> data)
        {
            bool result = await QueryDesigner
                .CreateDesigner(schema: Schemas.System, table: TableNames.SystemPermissions.ToString())
                .WhereEquals("Name", name)
                .AddRow(data)
                .RunUpdateQuery();

            ServiceManager.Instance.GetService<MemoryCacheService>().Remove(TableNames.SystemPermissions.ToString());

            return new APIResult()
            {
                Success = true,
                Message = "SystemPermission updated successfully!",
                Data = result
            };
        }

        public async Task<APIResult> DeleteSystemPermission(string name)
        {
            bool result = await QueryDesigner
                .CreateDesigner(schema: Schemas.System, table: TableNames.SystemPermissions.ToString())
                .WhereEquals("Name", name)
                .RunDeleteQuery();

            ServiceManager.Instance.GetService<MemoryCacheService>().Remove(TableNames.SystemPermissions.ToString());

            return new APIResult()
            {
                Success = true,
                Message = "SystemPermission deleted successfully!",
                Data = result
            };
        }
    }
}