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
    public class UserRolesService : BaseService
    {

        public UserRolesService(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<APIResult> GetUserRoles()
        {
            APIResult cacheResult = await ServiceManager.Instance.GetService<MemoryCacheService>().Get("UserRoles");
            if (cacheResult != null)
            {
                return cacheResult;
            }

            List<Dictionary<string, object>> result = await QueryDesigner
                .CreateDesigner(schema: Schemas.System, table: TableNames.UserRoles.ToString())
                .RunSelectQuery();

            APIResult newCacheResult = new APIResult()
            {
                Success = true,
                Message = "UserRoles loaded successfully!",
                Data = result
            };

            ServiceManager.Instance.GetService<MemoryCacheService>().Set("UserRoles", newCacheResult);

            return newCacheResult;

        }

        public async Task<APIResult> AddUserRole(Dictionary<string, object> data)
        {
            List<int> result = await QueryDesigner
                .CreateDesigner(schema: Schemas.System, table: TableNames.UserRoles.ToString())
                .AddRow(data)
                .RunInsertQuery();

            ServiceManager.Instance.GetService<MemoryCacheService>().Remove("UserRoles");

            return new APIResult()
            {
                Success = true,
                Message = "UserRole added successfully!",
                Data = result
            };
        }

        public async Task<APIResult> UpdateUserRole(int id, Dictionary<string, object> data)
        {
            bool result = await QueryDesigner
                .CreateDesigner(schema: Schemas.System, table: TableNames.UserRoles.ToString())
                .WhereEquals("Id", id)
                .AddRow(data)
                .RunUpdateQuery();

            ServiceManager.Instance.GetService<MemoryCacheService>().Remove("UserRoles");

            return new APIResult()
            {
                Success = true,
                Message = "UserRole updated successfully!",
                Data = result
            };
        }

        public async Task<APIResult> DeleteUserRole(int id)
        {
            bool result = await QueryDesigner
                .CreateDesigner(schema: Schemas.System, table: TableNames.UserRoles.ToString())
                .WhereEquals("Id", id)
                .RunDeleteQuery();

            ServiceManager.Instance.GetService<MemoryCacheService>().Remove("UserRoles");

            return new APIResult()
            {
                Success = true,
                Message = "UserRole deleted successfully!",
                Data = result
            };
        }
    }
}