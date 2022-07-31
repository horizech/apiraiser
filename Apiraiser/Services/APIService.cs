using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

using Apiraiser.Databases;
using Apiraiser.Models;
using Apiraiser.Helpers;
using Apiraiser.Constants;

using Microsoft.Extensions.Configuration;

namespace Apiraiser.Services
{
    public class APIService : BaseService
    {

        public APIService(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<APIResult> InsertRow(string schema, string table, Dictionary<string, object> data)
        {
            return await InsertRows(schema, table, new List<Dictionary<string, object>>() { data });
        }

        public async Task<APIResult> InsertRows(string schema, string table, List<Dictionary<string, object>> data)
        {
            List<int> result = await QueryDesigner
                .CreateDesigner(schema: schema, table: table)
                .SetRows(data)
                .RunInsertQuery();

            if ((result?.Count ?? 0) > 0)
            {
                return new APIResult()
                {
                    Success = true,
                    Message = "Row inserted Successfully!",
                    Data = result[0]
                };
            }
            else
            {
                return new APIResult()
                {
                    Success = false,
                    Message = "Error occured while inserting Row!",
                    Data = null
                };
            }
        }


        public async Task<APIResult> GetRows(string schema, string table, SelectSettings selectSettings = null)
        {
            List<Dictionary<string, object>> result = await QueryDesigner
                .CreateDesigner(schema: schema, table: table)
                .ApplySelectSettings(selectSettings)
                .RunSelectQuery();

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
                    Success = true,
                    Message = "Nothing found!",
                    Data = result
                };
            }
        }

        public async Task<APIResult> GetRowsByConditions(string schema, string table, List<QuerySearchItem> parameters, SelectSettings selectSettings = null)
        {
            List<Dictionary<string, object>> result = await QueryDesigner
                .CreateDesigner(schema: schema, table: table)
                .SetConditions(parameters)
                .ApplySelectSettings(selectSettings)
                .RunSelectQuery();

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
                    Success = true,
                    Message = "Nothing found!",
                    Data = result
                };
            }
        }

        public async Task<APIResult> UpdateRow(string schema, string table, Dictionary<string, object> data, List<QuerySearchItem> parameters)
        {
            bool result = await QueryDesigner
                .CreateDesigner(schema: schema, table: table)
                .AddRow(data)
                .SetConditions(parameters)
                .RunUpdateQuery();

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
                    Message = "Error occured while updating Row(s)!",
                    Data = null
                };
            }
        }

        public async Task<APIResult> DeleteRow(string schema, string table, List<QuerySearchItem> parameters)
        {
            bool result = await QueryDesigner
                .CreateDesigner(schema: schema, table: table)
                .SetConditions(parameters)
                .RunDeleteQuery();

            if (result == true)
            {
                return new APIResult()
                {
                    Success = true,
                    Message = "Row deleted Successfully!",
                    Data = null
                };
            }
            else
            {
                return new APIResult()
                {
                    Success = false,
                    Message = "Error occured while deleting Row!",
                    Data = null
                };
            }
        }
    }
}