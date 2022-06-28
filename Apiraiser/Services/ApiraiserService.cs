using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

using Apiraiser.Models;
using Apiraiser.Helpers;
using Apiraiser.Constants;
using Apiraiser.Mappings;
using Apiraiser.Enums;
using Apiraiser.Interfaces;

using Microsoft.Extensions.Configuration;

namespace Apiraiser.Services
{
    public class ApiraiserService : BaseService
    {

        public ApiraiserService(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<APIResult> IsInitialized()
        {
            try
            {
                List<string> tables = await ServiceManager
                    .Instance
                    .GetService<DatabaseService>()
                    .GetDatabaseDriver()
                    .GetTablesList(Schemas.System);

                if (tables == null || tables.Count == 0 || tables.IndexOf(TableNames.Users.ToString()) < 0)
                {
                    return APIResult.GetSimpleFailureResult("Apiraiser is not initialized!");
                }
                else
                {
                    return APIResult.GetSimpleSuccessResult("Apiraiser is initialized!");
                }
            }
            catch (Exception e)
            {
                return APIResult.GetExceptionResult(e);
            }

        }

        public async Task<APIResult> Initialize(string username, string email, string password)
        {
            List<string> tables = await ServiceManager
                .Instance
                .GetService<DatabaseService>()
                .GetDatabaseDriver()
                .GetTablesList(Schemas.System);


            if (tables == null || tables.Count == 0 || tables.IndexOf(TableNames.Users.ToString()) < 0)
            {
                await ServiceManager
                    .Instance
                    .GetService<DatabaseService>()
                    .GetDatabaseDriver()
                    .SetSessionReplicationRole("replica");


                await ServiceManager
                    .Instance
                    .GetService<DatabaseService>()
                    .GetDatabaseDriver()
                    .CreateSchema(Schemas.Application);

                await ServiceManager
                    .Instance
                    .GetService<DatabaseService>()
                    .GetDatabaseDriver()
                    .CreateSchema(Schemas.System);

                // Create tables from Tables Json file in 
                string tablesInfoPath = FileSystem.GetPathInConfigurations(@"Tables/Info.json");
                string tablesInfoJson = FileSystem.ReadFile(tablesInfoPath);
                TablesInfo tablesInfo = FileSystem.ReadJsonString<TablesInfo>(tablesInfoJson);
                if ((tablesInfo?.Tables?.Count ?? 0) > 0)
                {
                    foreach (KeyValuePair<string, string> tableName in tablesInfo.Tables)
                    {
                        string tablePath = FileSystem.GetPathInConfigurations(string.Format(@"Tables/Definitions/{0}.json", tableName.Value));
                        string tableJson = FileSystem.ReadFile(tablePath);
                        TableDefinition table = FileSystem.ReadJsonString<TableDefinition>(tableJson);

                        if (table.Name == null)
                        {
                            ServiceManager
                                .Instance
                                .GetService<LogService>()
                                .Print("Invalid Table in Tables Configuration!", Enums.LoggingLevel.Errors);
                        }
                        else if (table.Columns == null || table.Columns.Count == 0)
                        {
                            ServiceManager
                                .Instance
                                .GetService<LogService>()
                                .Print(string.Format("Table: {0} does not contain any columns!", table.Name), Enums.LoggingLevel.Errors);
                        }
                        else
                        {
                            if (table.AddAdditionalColumns)
                            {
                                table.Columns.AddRange(Columns.AdditionalColumns.Columns);
                            }
                            await ServiceManager
                                .Instance
                                .GetService<DatabaseService>()
                                .GetDatabaseDriver()
                                .CreateTable(Schemas.System, table.Name, table.Columns);
                            if ((table.DefaultRows?.Count ?? 0) > 0)
                            {
                                if (table.AddAdditionalColumns)
                                {
                                    foreach (Dictionary<string, object> row in table.DefaultRows)
                                    {
                                        Columns.AppendCreatedInfo(row, 1);
                                    }
                                }
                                await ServiceManager
                                    .Instance
                                    .GetService<DatabaseService>()
                                    .GetDatabaseDriver()
                                    .InsertRows(Schemas.System, table.Name, table.DefaultRows);
                            }
                            else if (table.Name.Equals("Users"))
                            {
                                List<Dictionary<string, object>> users = new List<Dictionary<string, object>>{
                                    new Dictionary<string, object>
                                    {
                                        { "Username", username },
                                        { "Email", email },
                                        { "Password", Hash.Create(password) },
                                        { "CreatedOn", DateTime.UtcNow}
                                    }
                                };

                                await ServiceManager
                                    .Instance
                                    .GetService<DatabaseService>()
                                    .GetDatabaseDriver()
                                    .InsertRows(Schemas.System, TableNames.Users.ToString(), users);

                            }
                            // Setval
                            await ServiceManager
                                .Instance
                                .GetService<DatabaseService>()
                                .GetDatabaseDriver()
                                .SetValWithMaxId(Schemas.System, table.Name);
                        }
                    };
                }

                await ServiceManager
                    .Instance
                    .GetService<DatabaseService>()
                    .GetDatabaseDriver()
                    .SetSessionReplicationRole("origin");

                return APIResult.GetSimpleSuccessResult("Apiraiser initialized successfully!");
            }
            else
            {
                return APIResult.GetSimpleFailureResult("Apiraiser is already initialized!");
            }
        }

    }
}