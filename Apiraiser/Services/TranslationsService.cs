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
    public class TranslationsService : BaseService
    {

        public TranslationsService(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<APIResult> GetTranslations()
        {
            APIResult cacheResult = await ServiceManager.Instance.GetService<MemoryCacheService>().Get("Translations");
            if (cacheResult != null)
            {
                return cacheResult;
            }

            List<Dictionary<string, object>> result = await QueryDesigner
                .CreateDesigner(schema: Schemas.System, table: TableNames.Translations.ToString())
                .RunSelectQuery();

            APIResult newCacheResult = new APIResult()
            {
                Success = true,
                Message = "Translations loaded successfully!",
                Data = result
            };

            ServiceManager.Instance.GetService<MemoryCacheService>().Set("Translations", newCacheResult);

            return newCacheResult;

        }

        public async Task<APIResult> AddTranslation(Dictionary<string, object> data)
        {
            List<int> result = await QueryDesigner
                .CreateDesigner(schema: Schemas.System, table: TableNames.Translations.ToString())
                .AddRow(data)
                .RunInsertQuery();

            ServiceManager.Instance.GetService<MemoryCacheService>().Remove("Translations");

            return new APIResult()
            {
                Success = true,
                Message = "Translation added successfully!",
                Data = result
            };
        }

        public async Task<APIResult> UpdateTranslation(int id, Dictionary<string, object> data)
        {
            bool result = await QueryDesigner
                .CreateDesigner(schema: Schemas.System, table: TableNames.Translations.ToString())
                .WhereEquals("Id", id)
                .AddRow(data)
                .RunUpdateQuery();

            ServiceManager.Instance.GetService<MemoryCacheService>().Remove("Translations");

            return new APIResult()
            {
                Success = true,
                Message = "Translation updated successfully!",
                Data = result
            };
        }

        public async Task<APIResult> DeleteTranslation(int id)
        {
            bool result = await QueryDesigner
                .CreateDesigner(schema: Schemas.System, table: TableNames.Translations.ToString())
                .WhereEquals("Id", id)
                .RunDeleteQuery();

            ServiceManager.Instance.GetService<MemoryCacheService>().Remove("Translations");

            return new APIResult()
            {
                Success = true,
                Message = "Translation deleted successfully!",
                Data = result
            };
        }
    }
}