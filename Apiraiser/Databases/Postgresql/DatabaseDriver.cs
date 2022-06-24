using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using Dapper;
using Npgsql;

using Apiraiser.Services;
using Apiraiser.Models;
using Apiraiser.Interfaces;
using Apiraiser.Enums;
using Apiraiser.Helpers;
using Apiraiser.Constants;
using Apiraiser.Exceptions;
using Apiraiser.Mappings;

namespace Apiraiser.Databases.Postgresql
{
    public class DatabaseDriver : IDatabaseDriver
    {
        public IDatabaseDataHandler DataType { get; set; }
        public IDatabaseConnection Connection { get; set; }

        public DatabaseDriver()
        {
            DataType = new DatabaseDataHandler();
            Connection = new DatabaseConnection();
        }

        public async Task<bool> SetSessionReplicationRole(string role)
        {
            if (Connection is null)
            {
                return false;
            }

            using (var conn = new NpgsqlConnection(Connection.GetDatabaseConnectionString()))
            {
                string query = String.Format(
                    "SET session_replication_role = '{0}';",
                    role
                );

                ServiceManager.Instance.GetService<LogService>().Print("Running query:\n" + query, LoggingLevel.Info);

                try
                {
                    await conn.ExecuteAsync(query);
                    return true;
                }
                catch (Exception e)
                {
                    ServiceManager.Instance.GetService<LogService>().Print("Database Error: " + e.Message, LoggingLevel.Errors);
                    return false;
                }
            }
        }

        public async Task<bool> SetValWithMaxId(string schema, string table)
        {
            if (Connection is null)
            {
                return false;
            }

            try
            {
                await QueryDesigner.CreateCustomQueryDesigner($"SELECT setval('\"{schema}\".\"{table}_Id_seq\"'::regclass, (SELECT MAX(\"Id\") FROM \"{schema}\".\"{table}\"))", null)
                .ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {
                ServiceManager.Instance.GetService<LogService>().Print("Database Error: " + e.Message, LoggingLevel.Errors);
                return false;
            }
        }

        public async Task<List<string>> GetTablesList(string schema)
        {
            // Three methods to get the job done

            // Method 1: Using Query Designer
            QueryDesigner designer = QueryDesigner
                .CreateDesigner("information_schema", "tables", QueryAction.SelectRows)
                .AddOutputColumnWithAlias("table_name", "TableName")
                .WhereEquals("table_schema", schema, true);
            // .OrderDescendingBy("TableName")
            // .Limit(5)
            // .Offset(5);

            List<string> tables = (await designer.ExecuteQuery<string>()).ToList();
            return tables;

            // Method 2: Manual Apiraiser Query generation and execution
            // ApiraiserQuery query = new ApiraiserQuery()
            // {
            //     SchemaName = "information_schema",
            //     TableName = "tables",
            //     Action = QueryAction.SelectRows,
            //     SelectedOutputColumns = new Dictionary<string, string> {
            //             { "table_name", "TableName" }
            //         },
            //     Conditions = new List<QuerySearchItem>{
            //             new QuerySearchItem{ Name = "table_schema", Value = schema, Condition = ColumnCondition.Equal, CaseSensitive = true}
            //         }
            // };

            // IQueryBuilder builder = ServiceManager
            //     .Instance
            //     .GetService<DatabaseService>()
            //     .GetQueryBuilder();

            // IQueryExecuter executer = ServiceManager
            //     .Instance
            //     .GetService<DatabaseService>()
            //     .GetQueryExecuter();

            // List<string> tables = (await executer.Execute<string>(builder.Build(query))).ToList();
            // return tables;

            // Method 3: Using custom query and execution
            // if (Connection is null)
            // {
            //     throw new ApiraiserErrorCodeException(ErrorCode.DB002);
            // }

            // using (var conn = new NpgsqlConnection(Connection.GetDatabaseConnectionString()))
            // {
            //     string query = "SELECT table_name AS \"TableName\" FROM information_schema.tables\n\tWHERE table_schema = @Schema";

            //     ServiceManager.Instance.GetService<LogService>().Print("Running query:\n" + query, LoggingLevel.Info);
            //     try
            //     {
            //         IEnumerable<string> tables = await conn.QueryAsync<string>(query, new { Schema = schema });
            //         if ((tables?.Count() ?? 0) > 0)
            //         {
            //             return tables.ToList();
            //         }
            //         else
            //         {
            //             throw new ApiraiserErrorCodeException(ErrorCode.DB006);
            //         }
            //     }
            //     catch (Exception e)
            //     {
            //         ServiceManager.Instance.GetService<LogService>().Print("Database Error: " + e.Message, LoggingLevel.Errors);
            //         throw new ApiraiserErrorCodeException(ErrorCode.DB004);
            //     }
            // }
        }


        public async Task<bool> CreateSchema(string schema)
        {
            if (Connection is null)
            {
                return false;
            }

            using (var conn = new NpgsqlConnection(Connection.GetDatabaseConnectionString()))
            {
                string query = String.Format(
                    "CREATE SCHEMA IF NOT EXISTS \"{0}\" ;",
                    schema
                );

                ServiceManager.Instance.GetService<LogService>().Print("Running query:\n" + query, LoggingLevel.Info);

                try
                {
                    await conn.ExecuteAsync(query);
                    return true;
                }
                catch (Exception e)
                {
                    ServiceManager.Instance.GetService<LogService>().Print("Database Error: " + e.Message, LoggingLevel.Errors);
                    return false;
                }
            }
        }

        public async Task<bool> CreateTable(string schema, string table, List<ColumnInfo> columns)
        {
            if (Connection is null)
            {
                return false;
            }

            List<ColumnInfo> completeColumns = new List<ColumnInfo>();

            if ((columns?.Count ?? 0) > 0)
            {
                completeColumns = columns.ToArray().ToList();
            }

            if (schema.Equals(Schemas.Application))
            {
                completeColumns.AddRange(Columns.AdditionalColumns.Columns);
            }

            using (var conn = new NpgsqlConnection(Connection.GetDatabaseConnectionString()))
            {
                string query = String.Format(
                        "CREATE TABLE IF NOT EXISTS \"{0}\".\"{1}\" (\n" +
                        "\t\"Id\" SERIAL PRIMARY KEY NOT NULL, \n" +
                        "\t{2}\n" +
                        "); ",
                        schema,
                        table,
                        string.Join(",\n\t", completeColumns.Select(x => "\"" + x.Name + "\" " + DataType.GetDataTypeString(x.Datatype) + (x.IsUnique ? " UNIQUE" : "") + (x.IsRequired ? " NOT NULL" : " NULL") + (x.IsForeignKey ? " REFERENCES \"" + x.ForeignSchema + "\".\"" + x.ForeignTable + "\"(\"" + x.ForeignName + "\")" : "")))
                    );

                ServiceManager.Instance.GetService<LogService>().Print("Running query:\n" + query, LoggingLevel.Info);
                try
                {
                    await conn.ExecuteAsync(query);
                    return true;
                }
                catch (Exception e)
                {
                    ServiceManager.Instance.GetService<LogService>().Print("Database Error: " + e.Message, LoggingLevel.Errors);
                    return false;
                }
            }
        }

        public async Task<List<ForeignKeyInfo>> GetForeignKeysList(string schema, string table)
        {
            if (Connection is null)
            {
                return null;
            }

            using (var conn = new NpgsqlConnection(Connection.GetDatabaseConnectionString()))
            {
                // string query = String.Format(
                //     "select\n" +
                //     "\tcl.relname as \"ParentTable\",\n" +
                //     "\tatt.attname as \"ParentColumn\",\n" +
                //     "\tatt2.attname as \"ChildColumn\",\n" +
                //     "\tconname as \"ConnectionName\"\n" +
                //     "from\n" +
                //     "\t( select\n" +
                //     "\t\tunnest(con1.conkey) as \"parent\",\n" +
                //     "\t\tunnest(con1.confkey) as \"child\",\n" +
                //     "\t\tcon1.confrelid,\n" +
                //     "\t\tcon1.conrelid,\n" +
                //     "\t\tcon1.conname\n" +
                //     "\tfrom\n" +
                //     "\t\tpg_class cl\n" +
                //     "\t\tjoin pg_namespace ns on cl.relnamespace = ns.oid\n" +
                //     "\t\tjoin pg_constraint con1 on con1.conrelid = cl.oid\n" +
                //     "\twhere\n" +
                //     "\t\tcl.relname = '{1}'\n" +
                //     "\t\tand ns.nspname = '{0}'\n" +
                //     "\t\tand con1.contype = 'f'\n" +
                //     "\t) con\n" +
                //     "\tjoin pg_attribute att on\n" +
                //     "\t\tatt.attrelid = con.confrelid and att.attnum = con.child\n" +
                //     "\tjoin pg_class cl on\n" +
                //     "\t\tcl.oid = con.confrelid\n" +
                //     "\tjoin pg_attribute att2 on\n" +
                //     "\t\tatt2.attrelid = con.conrelid and att2.attnum = con.parent ;",
                //     schema,
                //     table
                // );

                string query = String.Format(
                        "SELECT\n" +
                        "\ttc.table_schema AS \"ChildSchema\",\n" +
                        "\ttc.table_name AS \"ChildTable\",\n" +
                        "\tkcu.column_name AS \"ChildColumn\",\n" +
                        "\tccu.table_schema AS \"ParentSchema\",\n" +
                        "\tccu.table_name AS \"ParentTable\",\n" +
                        "\tccu.column_name AS \"ParentColumn\",\n" +
                        "\ttc.constraint_name AS \"ConnectionName\"\n" +
                        "FROM\n" +
                        "\tinformation_schema.table_constraints AS tc\n" +
                        "\tJOIN information_schema.key_column_usage AS kcu\n" +
                        "\tON tc.constraint_name = kcu.constraint_name\n" +
                        "\tAND tc.table_schema = kcu.table_schema\n" +
                        "\tJOIN information_schema.constraint_column_usage AS ccu\n" +
                        "\tON ccu.constraint_name = tc.constraint_name\n" +
                        "\t-- AND ccu.table_schema = tc.table_schema\n" +
                        "WHERE\n" +
                        "\ttc.constraint_type = 'FOREIGN KEY' AND tc.table_schema = '{0}' AND tc.table_name = '{1}'; ",
                        schema, table
                    );

                ServiceManager.Instance.GetService<LogService>().Print("Running query:\n" + query, LoggingLevel.Info);

                try
                {
                    IEnumerable<ForeignKeyInfo> foreignKeys = await conn.QueryAsync<ForeignKeyInfo>(query);
                    return foreignKeys.ToList();
                }
                catch (Exception e)
                {
                    ServiceManager.Instance.GetService<LogService>().Print("Database Error: " + e.Message, LoggingLevel.Errors);
                    return null;
                }
            }

        }

        public async Task<APIResult> AddColumn(string schema, string table, ColumnInfo column)
        {
            string query = String.Format(
                "ALTER TABLE \"{0}\".\"{1}\"\n" +
                "\tADD \"" + column.Name + "\" " + DataType.GetDataTypeString(column.Datatype) + (column.IsUnique ? " UNIQUE" : "") + (column.IsRequired ? " NOT NULL" : " NULL") +
                (column.DefaultValue != null ? " DEFAULT " + (ServiceManager
                        .Instance
                        .GetService<DatabaseService>()
                        .GetDatabaseDriver()
                        .DataType.
                        FormatValue(column.Datatype, ServiceManager
                            .Instance
                            .GetService<DatabaseService>()
                            .GetDatabaseDriver()
                            .DataType
                            .DeserializeValue(column.Datatype, column.DefaultValue)
                        )) : "") +
                (column.IsForeignKey ? " REFERENCES \"" + column.ForeignSchema + "\".\"" + column.ForeignTable + "\"(\"" + column.ForeignName + "\")" : "") + " ;",
                schema,
                table
            );

            ServiceManager.Instance.GetService<LogService>().Print("Running query:\n" + query, LoggingLevel.Info);

            try
            {
                await QueryDesigner
                    .CreateCustomQueryDesigner(query, null)
                    .ExecuteNonQuery();

                return new APIResult
                {
                    Success = true,
                    Message = "Column added successfully!",
                    Data = null
                };
            }
            catch (ApiraiserErrorCodeException e)
            {
                return new APIResult
                {
                    Success = false,
                    Message = e.Message,
                    ErrorCode = e.ErrorCode.ToString()
                };
            }

            catch (Exception e)
            {
                IDatabaseErrorHandler handler = ServiceManager.Instance.GetService<DatabaseService>().GetDatabaseErrorHandler();
                ErrorCode errorCode = handler.GetErrorCode(e.Message);
                string message = "";

                if (errorCode == ErrorCode.DB510)
                {
                    // The table does not exist
                    message = errorCode.GetMessage() + ": " + e.Message.Split('\"')[1];
                }

                if (errorCode == ErrorCode.DB520)
                {
                    // It's a null value column constraint violation
                    message = errorCode.GetMessage() + ": " + e.Message.Split('\"')[1];
                }

                ServiceManager.Instance.GetService<LogService>().Print("Database Error: " + e.Message, LoggingLevel.Errors);

                return new APIResult
                {
                    Success = false,
                    Message = message,
                    ErrorCode = errorCode.ToString()
                };
            }
        }

        public async Task<bool> RemoveColumn(string schema, string table, string column, bool removeDependants)
        {
            if (Connection is null)
            {
                return false;
            }
            using (var conn = new NpgsqlConnection(Connection.GetDatabaseConnectionString()))
            {
                string query = String.Format(
                    "ALTER TABLE \"{0}\".\"{1}\"\n" +
                    "\tDROP COLUMN \"{2}\"{3} ;",
                    schema,
                    table,
                    column,
                    (removeDependants ? " CASCADE" : "")
                );

                ServiceManager.Instance.GetService<LogService>().Print("Running query:\n" + query, LoggingLevel.Info);

                try
                {
                    int rows = await conn.ExecuteAsync(query);
                    return true;
                }
                catch (Exception e)
                {
                    ServiceManager.Instance.GetService<LogService>().Print("Database Error: " + e.Message, LoggingLevel.Errors);
                    return false;
                }
            }
        }

        public async Task<List<ColumnInfo>> GetTableColumns(string schema, string table)
        {
            string query =
                        "SELECT\n" +
                        "\t\"pg_attribute\".attname as \"Name\",\n" +
                        "\tpg_catalog.format_type(\"pg_attribute\".atttypid, \"pg_attribute\".atttypmod) as \"Datatype\",\n" +
                        "\tnot(\"pg_attribute\".attnotnull) AS \"IsRequired\"\n" +
                        "FROM\n" +
                        "\tpg_catalog.pg_attribute \"pg_attribute\"\n" +
                        "WHERE\n" +
                        "\t\"pg_attribute\".attnum > 0\n" +
                        "\tAND NOT \"pg_attribute\".attisdropped\n" +
                        "\tAND \"pg_attribute\".attrelid = (\n" +
                        "\t\tSELECT \"pg_class\".oid\n" +
                        "\t\tFROM pg_catalog.pg_class \"pg_class\"\n" +
                        "\t\t\tLEFT JOIN pg_catalog.pg_namespace \"pg_namespace\" ON \"pg_namespace\".oid = \"pg_class\".relnamespace\n" +
                        "\t\tWHERE\n" +
                        "\t\t\t\"pg_namespace\".nspname = @Schema\n" +
                        "\t\t\tAND \"pg_class\".relname = @Table\n" +
                        ");";
            Dictionary<string, object> parameters = new Dictionary<string, object>{
                    {"Schema", schema},
                    {"Table", table }
                };

            ServiceManager.Instance.GetService<LogService>().Print("Running query:\n" + query, LoggingLevel.Info);

            IEnumerable<dynamic> data = await QueryDesigner
                .CreateCustomQueryDesigner(query, parameters)
                .ExecuteQuery();

            List<ForeignKeyInfo> foreignKeyInfos = await GetForeignKeysList(schema, table);

            List<ColumnInfo> d = data
                .Select(x => ((IDictionary<string, object>)x).ToDictionary(kvp => kvp.Key, kvp => kvp.Value))
                .Select(x =>
                {
                    bool isRequired;
                    bool.TryParse(x["IsRequired"].ToString(), out isRequired);
                    ColumnInfo columnInfo = new ColumnInfo()
                    {
                        Name = x["Name"].ToString(),
                        Datatype = DataType.GetDataType(x["Datatype"].ToString()),
                        IsRequired = !isRequired
                    };
                    List<ForeignKeyInfo> foreignKeys = foreignKeyInfos.Where(x => x.ChildColumn.Equals(columnInfo.Name)).ToList();
                    if (foreignKeys != null && foreignKeys.Count > 0)
                    {
                        columnInfo.ForeignSchema = foreignKeys[0].ParentSchema;
                        columnInfo.ForeignTable = foreignKeys[0].ParentTable;
                        columnInfo.ForeignName = foreignKeys[0].ParentColumn;
                        columnInfo.IsForeignKey = true;
                    }

                    return columnInfo;

                })
                .ToList();

            return d;

        }

        public async Task<APIResult> DeleteColumn(string schema, string table, string column)
        {
            string query = String.Format(
                        "ALTER TABLE \"{0}\".\"{1}\"\n" +
                        "\tDROP COLUMN \"{2}\";",
                        schema,
                        table,
                        column
                    );

            Dictionary<string, object> parameters = new Dictionary<string, object>{
                    {"Schema", schema},
                    {"Table", table },
                    {"Column", column}
                };

            ServiceManager.Instance.GetService<LogService>().Print("Running query:\n" + query, LoggingLevel.Info);

            try
            {
                IEnumerable<dynamic> data = await QueryDesigner
                    .CreateCustomQueryDesigner(query, parameters)
                    .ExecuteQuery();

                return new APIResult
                {
                    Success = true,
                    Message = "Column deleted successfully!",
                    Data = data
                };
            }
            catch (ApiraiserErrorCodeException e)
            {
                return new APIResult
                {
                    Success = false,
                    Message = e.Message,
                    ErrorCode = e.ErrorCode.ToString()
                };
            }

            catch (Exception e)
            {
                IDatabaseErrorHandler handler = ServiceManager.Instance.GetService<DatabaseService>().GetDatabaseErrorHandler();
                ErrorCode errorCode = handler.GetErrorCode(e.Message);
                string message = "";

                if (errorCode == ErrorCode.DB510)
                {
                    // The table does not exist
                    message = errorCode.GetMessage() + ": " + e.Message.Split('\"')[1];
                }

                if (errorCode == ErrorCode.DB520)
                {
                    // It's a null value column constraint violation
                    message = errorCode.GetMessage() + ": " + e.Message.Split('\"')[1];
                }

                ServiceManager.Instance.GetService<LogService>().Print("Database Error: " + e.Message, LoggingLevel.Errors);

                return new APIResult
                {
                    Success = false,
                    Message = message,
                    ErrorCode = errorCode.ToString()
                };
            }
        }

        public async Task<object> RunQuery(ApiraiserQuery query)
        {
            if (query.Action == QueryAction.SelectRows)
            {
                if ((query.Conditions?.Count ?? 0) > 0)
                {
                    return await GetRowsByConditions(query.SchemaName, query.TableName, query.Conditions, query.AddForeignTables);
                }
                else
                {
                    return await GetRows(query.SchemaName, query.TableName, query.AddForeignTables);
                }
            }
            else if (query.Action == QueryAction.InsertRows)
            {
                return await InsertRows(query.SchemaName, query.TableName, query.Rows);
            }
            else if (query.Action == QueryAction.UpdateRows)
            {
                return await UpdateRows(query.SchemaName, query.TableName, query.Rows?[0] ?? null, query.Conditions);
            }
            else if (query.Action == QueryAction.DeleteRows)
            {
                return await DeleteRows(query.SchemaName, query.TableName, query.Conditions);
            }
            else
            {
                return null;
            }
        }

        public async Task<List<int>> InsertRow(string schema, string table, Dictionary<string, object> data, List<ColumnInfo> columns = null)
        {
            if (Connection is null)
            {
                return null;
            }

            if (columns == null)
            {
                columns = await GetTableColumns(schema, table);
            }
            if ((columns?.Count ?? 0) < 1)
            {
                return null;
            }

            data = data
                .ToDictionary(
                    x => x.Key,
                    x =>
                        ServiceManager
                        .Instance
                        .GetService<DatabaseService>()
                        .GetDatabaseDriver()
                        .DataType
                        .DeserializeValue(columns.Find(c => c.Name.ToLower().Equals(x.Key.ToString().ToLower())).Datatype, x.Value)
                );

            List<string> columnNames = columns.Select(x => x.Name).ToList();

            columnNames.ForEach(column =>
            {
                data.Keys.ToList().ForEach(key =>
                {
                    if (column.ToLower().Equals(key.ToLower()) && !column.Equals(key))
                    {
                        object value = data[key];
                        data.Remove(key);
                        data.Add(column, value);
                    }
                });
            });

            List<int> result = new List<int>();

            using (var conn = new NpgsqlConnection(Connection.GetDatabaseConnectionString()))
            {
                string valuesList = string.Format(
                        "({0})",
                        string.Join(", ", data.Select(x => "@" + x.Key))
                    );

                string query = String.Format(
                        "INSERT INTO \"{0}\".\"{1}\" ({2}) VALUES \n {3} RETURNING \"Id\";",
                        schema,
                        table,
                        string.Join(", ", data.Select(x => string.Format("\"{0}\"", x.Key))),
                        valuesList //string.Join(", ", valuesList)
                    );

                ServiceManager.Instance.GetService<LogService>().Print("Running query:\n" + query, LoggingLevel.Info);

                try
                {
                    IEnumerable<int> rows = await conn.QueryAsync<int>(query, data);
                    return rows.ToList();

                }
                catch (Exception e)
                {
                    IDatabaseErrorHandler handler = ServiceManager.Instance.GetService<DatabaseService>().GetDatabaseErrorHandler();
                    ErrorCode errorCode = handler.GetErrorCode(e.Message);
                    if (errorCode == ErrorCode.DB520)
                    {
                        // It's a null value column constraint violation
                        handler.ThrowApiraiserExceptionWithCustomMessage(errorCode.GetMessage() + ": " + e.Message.Split('\"')[1]);
                    }
                    ServiceManager.Instance.GetService<LogService>().Print("Database Error: " + e.Message, LoggingLevel.Errors);
                    return null;
                }
            }
        }

        public async Task<List<int>> InsertRows(string schema, string table, List<Dictionary<string, object>> data)
        {
            if (Connection is null)
            {
                return null;
            }

            List<ColumnInfo> columns = await GetTableColumns(schema, table);
            if ((columns?.Count ?? 0) < 1)
            {
                return null;
            }

            List<string> columnNames = columns.Select(x => x.Name).ToList();

            for (int i = 0; i < data.Count; i++)
            {
                // Fix any JSON like objects
                data[i] = data[i]
                .ToDictionary(
                    x => x.Key,
                    x =>
                        ServiceManager
                        .Instance
                        .GetService<DatabaseService>()
                        .GetDatabaseDriver()
                        .DataType
                        .DeserializeValue(columns.Find(c => c.Name.ToLower().Equals(x.Key.ToString().ToLower())).Datatype, x.Value)
                );

                // Remove any wrong case key
                columnNames.ForEach(column =>
                {
                    data[i].Keys.ToList().ForEach(key =>
                    {
                        if (column.ToLower().Equals(key.ToLower()) && !column.Equals(key))
                        {
                            object value = data[i][key];
                            data[i].Remove(key);
                            data[i].Add(column, value);
                        }
                    });
                });
            }

            List<int> rowIds = (await QueryDesigner.CreateDesigner(schema, table, QueryAction.InsertRows)
            .SetRows(data)
            .AddColumnDefinitions(columns)
            .ExecuteQuery<int>()).ToList();

            return rowIds;
        }

        // public List<Dictionary<string, object>> GetRows(string schema, string table)
        // {
        //     if (Connection is null)
        //     {
        //         return null;
        //     }

        //     List<ColumnInfo> columns = GetTableColumns(schema, table);
        //     if ((columns?.Count ?? 0) < 1)
        //     {
        //         return null;
        //     }

        //     using (var conn = new NpgsqlConnection(Connection.GetDatabaseConnectionString()))
        //     {
        //         List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();

        //         ServiceManager.Instance.GetService<LogService>().Print("Opening connection", LoggingLevel.Info);
        //         conn.Open();

        //         try
        //         {
        //             string query = String.Format(
        //                 "SELECT * FROM \"{0}\".\"{1}\";",
        //                 schema,
        //                 table
        //             );

        //             ServiceManager.Instance.GetService<LogService>().Print("Running query:\n" + query, LoggingLevel.Info);

        //             using (var command = new NpgsqlCommand(query, conn))
        //             {
        //                 try
        //                 {
        //                     // Not implemeted in .Net Core yet
        //                     // if ((parameters?.Count ?? 0)  > 0)
        //                     // {
        //                     //     foreach (QuerySearchItem parameter in parameters)
        //                     //     {
        //                     //         // Method 1
        //                     //         // ServiceManager.Instance.GetService<DatabaseService>().GetQueryHelper().PutColumnConditionParameter(columns.Find(c => c.Name.ToLower().Equals(parameter.Name.ToLower())).Datatype, parameter, command.Parameters);

        //                     //         // Method 2
        //                     //         object value = ServiceManager.Instance.GetService<DatabaseService>().GetQueryHelper().GetColumnConditionParameter(columns.Find(c => c.Name.ToLower().Equals(parameter.Name.ToLower())).Datatype, parameter);
        //                     // command.Parameters.AddWithValue(parameter.Name, ((object)value) ?? DBNull.Value);
        //                     //     }
        //                     // }

        //                     var reader = command.ExecuteReader();

        //                     while (reader.Read())
        //                     {
        //                         Dictionary<string, object> row = new Dictionary<string, object>();
        //                         for (int i = 0; i < reader.FieldCount; i++)
        //                         {
        //                             // Object column = reader[i];
        //                             // Type fieldType = reader.GetFieldType(i);
        //                             row.Add(reader.GetName(i), DataType.GetDataFromReader(reader, reader.GetDataTypeName(i), i));
        //                             ServiceManager.Instance.GetService<LogService>().Print(
        //                                 string.Format(
        //                                     "Column Info: {0} = {1}",
        //                                     row.Last().Key,
        //                                     row.Last().Value?.ToString() ?? "null"
        //                                 ),
        //                                 LoggingLevel.Info
        //                             );
        //                         }
        //                         result.Add(row);
        //                     }
        //                 }
        //                 catch (Exception e)
        //                 {
        //                     ServiceManager.Instance.GetService<LogService>().Print("Database Error: " + e.Message, LoggingLevel.Errors);
        //                 }
        //             }
        //         }
        //         catch (Exception e)
        //         {
        //             ServiceManager.Instance.GetService<LogService>().Print("Database Error: " + e.Message, LoggingLevel.Errors);
        //         }

        //         conn.Close();
        //         ServiceManager.Instance.GetService<LogService>().Print("Connection closed", LoggingLevel.Info);
        //         return result;
        //     }
        // }


        public async Task<List<Dictionary<string, object>>> GetRows(string schema, string table, List<AddForeignTables> AddForeignTables = null)
        {
            QueryDesigner designer = QueryDesigner
                .CreateDesigner(schema, table, QueryAction.SelectRows)
                .AddForeignTables(AddForeignTables)
                .OrderBy("Id");

            List<Dictionary<string, object>> result = (await designer.ExecuteQuery<Dictionary<string, object>>()).ToList();
            return result;
        }

        public async Task<List<Dictionary<string, object>>> GetRowsByConditions(string schema, string table, List<QuerySearchItem> parameters, List<AddForeignTables> AddForeignTables = null)
        {
            List<ColumnInfo> columns = await GetTableColumns(schema, table);

            parameters = parameters.Select(parameter =>
            {
                parameter.Value = ServiceManager
                        .Instance
                        .GetService<DatabaseService>()
                        .GetDatabaseDriver()
                        .DataType
                        .DeserializeValue(columns.Find(c => c.Name.ToLower().Equals(parameter.Name.ToString().ToLower())).Datatype, parameter.Value);
                return parameter;
            }).ToList();

            QueryDesigner designer = QueryDesigner
                .CreateDesigner(schema, table, QueryAction.SelectRows)
                .SetConditions(parameters)
                .AddForeignTables(AddForeignTables)
                .OrderBy("Id");
            // .OrderDescendingBy("TableName")
            // .Limit(5)
            // .Offset(5);

            List<Dictionary<string, object>> result = (await designer.ExecuteQuery<Dictionary<string, object>>()).ToList();
            return result;
        }

        public async Task<bool> UpdateRows(string schema, string table, Dictionary<string, object> data, List<QuerySearchItem> parameters)
        {
            if (Connection is null)
            {
                return false;
            }

            List<ColumnInfo> columns = await GetTableColumns(schema, table);
            if ((columns?.Count ?? 0) < 1)
            {
                return false;
            }

            List<string> columnNames = columns.Select(x => x.Name).ToList();

            // Check if any parameter is not part of table columns
            foreach (QuerySearchItem item in parameters)
            {
                if (!columns.Any(x => x.Name.ToLower().Equals(item.Name.ToLower())))
                {
                    return false;
                }
            }

            // Fix any JSON like objects in data
            data = data
            .ToDictionary(
                x => x.Key,
                x =>
                    ServiceManager
                    .Instance
                    .GetService<DatabaseService>()
                    .GetDatabaseDriver()
                    .DataType
                    .DeserializeValue(columns.Find(c => c.Name.ToLower().Equals(x.Key.ToString().ToLower())).Datatype, x.Value)
            );

            // Fix any JSON like objects in parameters
            if ((parameters?.Count ?? 0) > 0)
            {
                for (int i = 0; i < parameters.Count; i++)
                {
                    parameters[i].Value =
                        ServiceManager
                        .Instance
                        .GetService<DatabaseService>()
                        .GetDatabaseDriver()
                        .DataType
                        .DeserializeValue(columns.Find(c => c.Name.ToLower().Equals(parameters[i].Name.ToString().ToLower())).Datatype, parameters[i].Value);
                }
            }

            // Remove any wrong case key
            columnNames.ForEach(column =>
            {
                data.Keys.ToList().ForEach(key =>
                {
                    if (column.ToLower().Equals(key.ToLower()) && !column.Equals(key))
                    {
                        object value = data[key];
                        data.Remove(key);
                        data.Add(column, value);
                    }
                });
            });

            List<bool> rowsUpdated = (await QueryDesigner.CreateDesigner(schema, table, QueryAction.UpdateRows)
            .AddRow(data)
            .AddColumnDefinitions(columns)
            .SetConditions(parameters)
            .ExecuteQuery<bool>()).ToList();

            return rowsUpdated.Count > 0;
        }

        public async Task<bool> DeleteRows(string schema, string table, List<QuerySearchItem> parameters)
        {
            if (Connection is null)
            {
                return false;
            }

            List<ColumnInfo> columns = await GetTableColumns(schema, table);
            if ((columns?.Count ?? 0) < 1)
            {
                return false;
            }

            using (var conn = new NpgsqlConnection(Connection.GetDatabaseConnectionString()))
            {

                string query = String.Format(
                    "DELETE FROM \"{0}\".\"{1}\"",
                    schema,
                    table
                );

                if ((parameters?.Count ?? 0) > 0)
                {
                    foreach (QuerySearchItem item in parameters)
                    {
                        if (columns.Count(x => x.Name.ToLower().Equals(item.Name.ToLower())) < 1)
                        {
                            return false;
                        }
                    }

                    query = query + String.Format(
                        "\r\nWHERE {0};",
                        // Use this when NpgsqlParameterCollection properly gets implemented
                        // string.Join(" AND ", parameters.Select(x => ServiceManager.Instance.GetService<DatabaseService>().GetQueryHelper().GetColumnConditionItem(columns.Find(c => c.Name.ToLower().Equals(x.Name.ToLower())).Datatype, x)))
                        string.Join(" AND ", parameters.Select(x => ServiceManager.Instance.GetService<DatabaseService>().GetQueryHelper().ApplyColumnCondition(columns.Find(c => c.Name.ToLower().Equals(x.Name.ToLower())).Datatype, x)))
                    );
                }
                else
                {
                    query = query + ";";
                }
                ServiceManager.Instance.GetService<LogService>().Print("Running query:\n" + query, LoggingLevel.Info);

                Dictionary<string, object> param = parameters.ToDictionary(
                    x => x.Name,
                    x =>
                        ServiceManager
                        .Instance
                        .GetService<DatabaseService>()
                        .GetDatabaseDriver()
                        .DataType
                        .DeserializeValue(columns.Find(c => c.Name.ToLower().Equals(x.Name.ToString().ToLower())).Datatype, x.Value)
                );

                try
                {
                    int rows = await conn.ExecuteAsync(query, param); //.ToDictionary<string, object>(kvp => kvp.Key, kvp => kvp.Value).ToList(); //.ToList();
                    return rows > 0;
                }
                catch (Exception e)
                {
                    ServiceManager.Instance.GetService<LogService>().Print("Database Error: " + e.Message, LoggingLevel.Errors);
                    return false;
                }
            }
        }

        public async Task CreateDBIfNotExist(string database)
        {
            if (Connection is null)
            {
                return;
            }
            using (var conn = new NpgsqlConnection(Connection.GetDatabaseConnectionString()))
            {
                string query = "DROP TABLE IF EXISTS public.inventory ;";
                ServiceManager.Instance.GetService<LogService>().Print("Running query:\n" + query, LoggingLevel.Info);

                try
                {
                    await conn.QueryAsync(query);
                }
                catch (Exception e)
                {
                    ServiceManager.Instance.GetService<LogService>().Print("Database Error: " + e.Message, LoggingLevel.Errors);
                }

                query = "CREATE TABLE inventory(id serial PRIMARY KEY, name VARCHAR(50), quantity INTEGER) ;";
                ServiceManager.Instance.GetService<LogService>().Print("Running query:\n" + query, LoggingLevel.Info);

                try
                {
                    await conn.QueryAsync(query);
                }
                catch (Exception e)
                {
                    ServiceManager.Instance.GetService<LogService>().Print("Database Error: " + e.Message, LoggingLevel.Errors);
                }

                /*
                ServiceManager.Instance.GetService<LogService>().Print("Opening connection", LoggingLevel.Info);
                conn.Open();
                try
                {
                    
                    string query = "INSERT INTO inventory (name, quantity) VALUES (@n1, @q1), (@n2, @q2), (@n3, @q3)";

                    ServiceManager.Instance.GetService<LogService>().Print("Running query:\n" + query, LoggingLevel.Info);

                    using (var command = new NpgsqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("n1", "banana");
                        command.Parameters.AddWithValue("q1", 150);
                        command.Parameters.AddWithValue("n2", "orange");
                        command.Parameters.AddWithValue("q2", 154);
                        command.Parameters.AddWithValue("n3", "apple");
                        command.Parameters.AddWithValue("q3", 100);

                        int nRows = command.ExecuteNonQuery();
                        ServiceManager.Instance.GetService<LogService>().Print(String.Format("Number of rows inserted={0}", nRows), LoggingLevel.Info);
                    }

                }
                catch (Exception e)
                {
                    ServiceManager.Instance.GetService<LogService>().Print("Database Error: " + e.Message, LoggingLevel.Errors);
                }


                conn.Close();
                ServiceManager.Instance.GetService<LogService>().Print("Connection closed", LoggingLevel.Info);
                */
            }
            ServiceManager.Instance.GetService<LogService>().Print("Finished createDBIfNotExist function!", LoggingLevel.Info);
        }

    }
}