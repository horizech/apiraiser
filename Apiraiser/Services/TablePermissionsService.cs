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
    public class TablePermissionsService : BaseService
    {

        public TablePermissionsService(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<APIResult> GetTablePermissions()
        {
            APIResult cacheResult = await ServiceManager.Instance.GetService<MemoryCacheService>().Get(TableNames.TablePermissions.ToString());
            if (cacheResult != null)
            {
                return cacheResult;
            }

            List<Dictionary<string, object>> result = await QueryDesigner
                .CreateDesigner(schema: Schemas.System, table: TableNames.TablePermissions.ToString())
                .RunSelectQuery();

            APIResult newCacheResult = new APIResult()
            {
                Success = true,
                Message = "Table Permissions loaded successfully!",
                Data = result
            };

            ServiceManager.Instance.GetService<MemoryCacheService>().Set(TableNames.TablePermissions.ToString(), newCacheResult);

            return newCacheResult;

        }

        public async Task<APIResult> AddTablePermission(Dictionary<string, object> data)
        {
            List<int> result = await QueryDesigner
                .CreateDesigner(schema: Schemas.System, table: TableNames.TablePermissions.ToString())
                .AddRow(data)
                .RunInsertQuery();

            ServiceManager.Instance.GetService<MemoryCacheService>().Remove(TableNames.TablePermissions.ToString());

            return new APIResult()
            {
                Success = true,
                Message = "TablePermission added successfully!",
                Data = result
            };
        }

        public async Task<APIResult> UpdateTablePermission(string name, Dictionary<string, object> data)
        {
            bool result = await QueryDesigner
                .CreateDesigner(schema: Schemas.System, table: TableNames.TablePermissions.ToString())
                .WhereEquals("Name", name)
                .AddRow(data)
                .RunUpdateQuery();

            ServiceManager.Instance.GetService<MemoryCacheService>().Remove(TableNames.TablePermissions.ToString());

            return new APIResult()
            {
                Success = true,
                Message = "TablePermission updated successfully!",
                Data = result
            };
        }

        public async Task<APIResult> DeleteTablePermission(string name)
        {
            bool result = await QueryDesigner
                .CreateDesigner(schema: Schemas.System, table: TableNames.TablePermissions.ToString())
                .WhereEquals("Name", name)
                .RunDeleteQuery();

            ServiceManager.Instance.GetService<MemoryCacheService>().Remove(TableNames.TablePermissions.ToString());

            return new APIResult()
            {
                Success = true,
                Message = "TablePermission deleted successfully!",
                Data = result
            };
        }
    }
}