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
    public class SettingsService : BaseService
    {

        public SettingsService(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<APIResult> GetSetting(string key)
        {
            List<Dictionary<string, object>> result = await QueryDesigner
                .CreateDesigner(schema: Schemas.System, table: TableNames.Settings.ToString())
                .WhereEquals("Key", Constants.Settings.DefaultRoleOnSignup, true)
                .RunSelectQuery();

            if ((result?.Count ?? 0) > 0)
            {
                return new APIResult()
                {
                    Success = true,
                    Message = "Setting loaded successfully!",
                    Data = result[0]
                };
            }
            else
            {
                return new APIResult()
                {
                    Success = false,
                    Message = "Setting not found!",
                    Data = null
                };
            }
        }

        public async Task<APIResult> GetSettings()
        {
            List<Dictionary<string, object>> result = await QueryDesigner
                .CreateDesigner(schema: Schemas.System, table: TableNames.Settings.ToString())
                .RunSelectQuery();

            return new APIResult()
            {
                Success = true,
                Message = "Settings loaded successfully!",
                Data = result
            };
        }

        public async Task<APIResult> AddSetting(Dictionary<string, object> data)
        {
            List<int> result = await QueryDesigner
                .CreateDesigner(schema: Schemas.System, table: TableNames.Settings.ToString())
                .AddRow(data)
                .RunInsertQuery();

            return new APIResult()
            {
                Success = true,
                Message = "Setting added successfully!",
                Data = result
            };
        }

        public async Task<APIResult> UpdateSetting(string key, Dictionary<string, object> data)
        {
            bool result = await QueryDesigner
                .CreateDesigner(schema: Schemas.System, table: TableNames.Settings.ToString())
                .WhereEquals("Key", key)
                .AddRow(data)
                .RunUpdateQuery();

            return new APIResult()
            {
                Success = true,
                Message = "Setting updated successfully!",
                Data = result
            };
        }

        public async Task<APIResult> DeleteSetting(string key)
        {
            bool result = await QueryDesigner
                .CreateDesigner(schema: Schemas.System, table: TableNames.Settings.ToString())
                .WhereEquals("Key", key)
                .RunDeleteQuery();

            return new APIResult()
            {
                Success = true,
                Message = "Setting deleted successfully!",
                Data = result
            };
        }
    }
}