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
                    .GetTablesList(Schemas.Administration);

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

        public async Task<APIResult> Initialize(string username, string email, string password, string template)
        {
            List<string> tables = await ServiceManager
                .Instance
                .GetService<DatabaseService>()
                .GetDatabaseDriver()
                .GetTablesList(Schemas.Administration);


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
                    .CreateSchema(Schemas.Data);

                await ServiceManager
                    .Instance
                    .GetService<DatabaseService>()
                    .GetDatabaseDriver()
                    .CreateSchema(Schemas.Administration);

                // Create tables from Tables Json file in 
                string appTemplatesPath = FileSystem.GetPathInConfigurations(@"Tables/Templates.json");
                string appTemplatesJson = FileSystem.ReadFile(appTemplatesPath);
                AppTemplates appTemplates = FileSystem.ReadJsonString<AppTemplates>(appTemplatesJson);

                List<Dictionary<string, object>> tablePermissions = new List<Dictionary<string, object>>();
                TablesInfo tablesInfo = appTemplates.Templates.First(x => x.Name == template);

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
                                .CreateTable(table.Schema, table.Name, table.Columns);
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
                                    .InsertRows(table.Schema, table.Name, table.DefaultRows);
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
                                    .InsertRows(table.Schema, TableNames.Users.ToString(), users);

                            }
                            // Setval
                            await ServiceManager
                                .Instance
                                .GetService<DatabaseService>()
                                .GetDatabaseDriver()
                                .SetValWithMaxId(table.Schema, table.Name);
                        }

                        tablePermissions.AddRange(new List<Dictionary<string, object>>{
                            new Dictionary<string, object>{
                            { "Schema", table.Schema },
                            { "Table", table.Name },
                            { "Role", 1 },
                            { "UserAccessLevel", 1 },
                            { "CanRead", true },
                            { "CanWrite", true },
                            { "CanUpdate", true },
                            { "CanDelete", true },
                            { "CreatedOn", DateTime.UtcNow},
                            { "CreatedBy", 1}
                        },
                        new Dictionary<string, object>{
                            { "Schema", table.Schema },
                            { "Table", table.Name },
                            { "Role", 2 },
                            { "UserAccessLevel", 1 },
                            { "CanRead", true },
                            { "CanWrite", true },
                            { "CanUpdate", true },
                            { "CanDelete", true },
                            { "CreatedOn", DateTime.UtcNow},
                            { "CreatedBy", 1}
                        },
                        new Dictionary<string, object>{
                            { "Schema", table.Schema },
                            { "Table", table.Name },
                            { "Role", 3 },
                            { "UserAccessLevel", 2 },
                            { "CanRead", true },
                            { "CanWrite", table.Schema == Schemas.Data },
                            { "CanUpdate", table.Schema == Schemas.Data },
                            { "CanDelete", table.Schema == Schemas.Data },
                            { "CreatedOn", DateTime.UtcNow},
                            { "CreatedBy", 1}
                        }
                        }
                        );
                    };

                    await ServiceManager
                                    .Instance
                                    .GetService<DatabaseService>()
                                    .GetDatabaseDriver()
                                    .InsertRows(Schemas.Administration, TableNames.TablePermissions.ToString(), tablePermissions);
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