using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using Dapper;
using Npgsql;

using Apiraiser.Models;
using Apiraiser.Exceptions;
using Apiraiser.Helpers;
using Apiraiser.Services;
using Apiraiser.Enums;
using Apiraiser.Interfaces;

namespace Apiraiser.Databases.Postgresql
{
    public class QueryExecuter : IQueryExecuter
    {
        public async Task<IEnumerable<T>> Execute<T>(QueryBuilderOutput queryBuilderOutput)
        {
            IDatabaseConnection connection = ServiceManager.Instance.GetService<DatabaseService>().GetDatabaseConnection();
            if (connection is null)
            {
                throw new ApiraiserErrorCodeException(ErrorCode.DB002);
            }

            ServiceManager.Instance.GetService<LogService>().Print(string.Format("Running query:\n{0}", queryBuilderOutput.Script), LoggingLevel.Info);
            using (var conn = new NpgsqlConnection(connection.GetDatabaseConnectionString()))
            {
                try
                {
                    if ((queryBuilderOutput.Parameters?.Count ?? 0) > 0)
                    {
                        Type outputType = typeof(T);
                        if (outputType == typeof(Dictionary<string, object>))
                        {
                            IEnumerable<dynamic> data = await conn.QueryAsync<dynamic>(queryBuilderOutput.Script, queryBuilderOutput.Parameters); //.ToDictionary<string, object>(kvp => kvp.Key, kvp => kvp.Value).ToList(); //.ToList();
                            // List<Dictionary<string, object>> dict = data
                            //         .Select(x => ((IDictionary<string, object>)x)
                            //         .ToDictionary(kvp => kvp.Key, kvp => kvp.Value)).ToList();
                            // return Castings.CastObject<IEnumerable<T>>(dict);
                            return Castings.GetDictionaryFromDapperDynamicIEnumerable<T>(data);
                        }
                        else if (outputType == typeof(System.Boolean))
                        {
                            // IEnumerable<T> data = await conn.QueryAsync<T>(queryBuilderOutput.Script, queryBuilderOutput.Parameters); //.ToDictionary<string, object>(kvp => kvp.Key, kvp => kvp.Value).ToList(); //.ToList();
                            // return data;
                            int rowsAffected = await conn.ExecuteAsync(queryBuilderOutput.Script, queryBuilderOutput.Parameters); //.ToDictionary<string, object>(kvp => kvp.Key, kvp => kvp.Value).ToList(); //.ToList();
                            IEnumerable<T> data = new List<T> { (T)Convert.ChangeType((rowsAffected > 0), typeof(T)) };
                            return data;
                        }
                        else
                        {
                            IEnumerable<T> data = await conn.QueryAsync<T>(queryBuilderOutput.Script, queryBuilderOutput.Parameters); //.ToDictionary<string, object>(kvp => kvp.Key, kvp => kvp.Value).ToList(); //.ToList();
                            return data;
                        }
                    }
                    else
                    {
                        if (typeof(T) == typeof(Dictionary<string, object>))
                        {
                            IEnumerable<dynamic> data = await conn.QueryAsync<dynamic>(queryBuilderOutput.Script); //.ToDictionary<string, object>(kvp => kvp.Key, kvp => kvp.Value).ToList(); //.ToList();
                            // List<Dictionary<string, object>> dict = data
                            //         .Select(x => ((IDictionary<string, object>)x)
                            //         .ToDictionary(kvp => kvp.Key, kvp => kvp.Value)).ToList();
                            // return Castings.CastObject<IEnumerable<T>>(dict);
                            return Castings.GetDictionaryFromDapperDynamicIEnumerable<T>(data);
                        }
                        else
                        {
                            IEnumerable<T> data = await conn.QueryAsync<T>(queryBuilderOutput.Script); //.ToDictionary<string, object>(kvp => kvp.Key, kvp => kvp.Value).ToList(); //.ToList();
                            return data;
                        }
                    }
                }
                catch (Exception e)
                {
                    HandleException(e);
                    return null;
                }
            }
        }

        public async Task<IEnumerable<dynamic>> Execute(QueryBuilderOutput queryBuilderOutput)
        {
            IDatabaseConnection connection = ServiceManager.Instance.GetService<DatabaseService>().GetDatabaseConnection();
            if (connection is null)
            {
                throw new ApiraiserErrorCodeException(ErrorCode.DB002);
            }

            ServiceManager.Instance.GetService<LogService>().Print(string.Format("Running query:\n{0}", queryBuilderOutput.Script), LoggingLevel.Info);
            using (var conn = new NpgsqlConnection(connection.GetDatabaseConnectionString()))
            {
                try
                {
                    if ((queryBuilderOutput.Parameters?.Count ?? 0) > 0)
                    {
                        IEnumerable<dynamic> data = await conn.QueryAsync(queryBuilderOutput.Script, queryBuilderOutput.Parameters); //.ToDictionary<string, object>(kvp => kvp.Key, kvp => kvp.Value).ToList(); //.ToList();
                        return data;
                    }
                    else
                    {
                        IEnumerable<dynamic> data = await conn.QueryAsync(queryBuilderOutput.Script); //.ToDictionary<string, object>(kvp => kvp.Key, kvp => kvp.Value).ToList(); //.ToList();
                        return data;
                    }
                }
                catch (Exception e)
                {
                    HandleException(e);
                    return null;
                }
            }
        }

        public async Task<bool> ExecuteNonQuery(QueryBuilderOutput queryBuilderOutput)
        {
            IDatabaseConnection connection = ServiceManager.Instance.GetService<DatabaseService>().GetDatabaseConnection();
            if (connection is null)
            {
                throw new ApiraiserErrorCodeException(ErrorCode.DB002);
            }

            ServiceManager.Instance.GetService<LogService>().Print(string.Format("Running query:\n{0}", queryBuilderOutput.Script), LoggingLevel.Info);
            using (var conn = new NpgsqlConnection(connection.GetDatabaseConnectionString()))
            {
                try
                {
                    if ((queryBuilderOutput.Parameters?.Count ?? 0) > 0)
                    {
                        await conn.ExecuteAsync(queryBuilderOutput.Script, queryBuilderOutput.Parameters); //.ToDictionary<string, object>(kvp => kvp.Key, kvp => kvp.Value).ToList(); //.ToList();
                        return true;
                    }
                    else
                    {
                        await conn.ExecuteAsync(queryBuilderOutput.Script); //.ToDictionary<string, object>(kvp => kvp.Key, kvp => kvp.Value).ToList(); //.ToList();
                        return true;
                    }
                }
                catch (Exception e)
                {
                    HandleException(e);
                    return false;
                }
            }
        }

        private void HandleException(Exception e)
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

            if (errorCode == ErrorCode.DB521)
            {
                // column not found
                message = errorCode.GetMessage() + ": " + e.Message.Split('\"')[1];
            }
            ServiceManager.Instance.GetService<LogService>().Print("Database Error: " + e.Message, LoggingLevel.Errors);
            ServiceManager.Instance.GetService<DatabaseService>().GetDatabaseErrorHandler().ThrowApiraiserException(errorCode, message);
        }
    }
}