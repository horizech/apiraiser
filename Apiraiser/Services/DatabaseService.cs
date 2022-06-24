using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

using Apiraiser.Enums;
using Apiraiser.Databases;
using Apiraiser.Interfaces;

namespace Apiraiser.Services
{
    public class DatabaseService : BaseService
    {
        public DatabaseService(IConfiguration configuration) : base(configuration)
        {
        }

        /// <summary>
        /// Query Builder
        /// </summary>
        private IQueryBuilder builder;


        /// <summary>
        /// Query Executer
        /// </summary>
        private IQueryExecuter executer;

        /// <summary>
        /// Query Helper
        /// </summary>
        private IQueryHelper helper { get; set; }

        /// <summary>
        /// Database Instance
        /// </summary>
        private IDatabaseDriver database;

        /// <summary>
        /// Database Error Handler
        /// </summary>
        private IDatabaseErrorHandler errorHandler;

        /// <summary>
        /// Set Query Builder
        /// </summary>
        /// <param name="type">Database Type</param>
        /// <returns>Select and initialize Query Builder</returns>
        public IQueryBuilder SetQueryBuilder(DatabaseTypes type)
        {
            switch (type)
            {
                default:
                case DatabaseTypes.Postgresql:
                    builder = new Databases.Postgresql.QueryBuilder();
                    break;
            }
            return builder;
        }

        /// <summary>
        /// Get Query Builder
        /// </summary>
        /// <returns>Currently selected Query Builder if exists</returns>
        public IQueryBuilder GetQueryBuilder()
        {
            return builder;
        }

        /// <summary>
        /// Set Query Executer
        /// </summary>
        /// <param name="type">Database Type</param>
        /// <returns>Select and initialize Query Executer</returns>
        public IQueryExecuter SetQueryExecuter(DatabaseTypes type)
        {
            switch (type)
            {
                default:
                case DatabaseTypes.Postgresql:
                    executer = new Databases.Postgresql.QueryExecuter();
                    break;
            }
            return executer;
        }

        /// <summary>
        /// Get Query Executer
        /// </summary>
        /// <returns>Currently selected Query Executer if exists</returns>
        public IQueryExecuter GetQueryExecuter()
        {
            return executer;
        }

        /// <summary>
        /// Set Query Helper
        /// </summary>
        /// <param name="type">Database Type</param>
        /// <returns>Select and initialize Query Helper</returns>
        public IQueryHelper SetQueryHelper(DatabaseTypes type)
        {
            switch (type)
            {
                default:
                case DatabaseTypes.Postgresql:
                    helper = new Databases.Postgresql.QueryHelper();
                    break;
            }
            return helper;
        }

        /// <summary>
        /// Get Query Helper
        /// </summary>
        /// <returns>Currently selected Query Helper if exists</returns>
        public IQueryHelper GetQueryHelper()
        {
            return helper;
        }

        /// <summary>
        /// Set Database Driver
        /// </summary>
        /// <param name="type">Database Type</param>
        /// <returns>Select and initialize Database Driver</returns>
        public IDatabaseDriver SetDatabaseDriver(DatabaseTypes type)
        {
            switch (type)
            {
                default:
                case DatabaseTypes.Postgresql:
                    database = new Databases.Postgresql.DatabaseDriver();
                    break;
            }
            return database;
        }

        /// <summary>
        /// Get Database Driver
        /// </summary>
        /// <returns>Currently selected Database Driver if exists</returns>
        public IDatabaseDriver GetDatabaseDriver()
        {
            return database;
        }

        /// <summary>
        /// Set Database Error Handler
        /// </summary>
        /// <param name="type">Database type</param>
        /// <returns>Select and initialize Database Error Handler</returns>
        public IDatabaseErrorHandler SetDatabaseErrorHandler(DatabaseTypes type)
        {
            switch (type)
            {
                default:
                case DatabaseTypes.Postgresql:
                    errorHandler = new Databases.Postgresql.DatabaseErrorHandler();
                    break;
            }
            return errorHandler;
        }

        /// <summary>
        /// Get Database Error Handler
        /// </summary>
        /// <returns>Currently selected Database Error Handler if exists</returns>
        public IDatabaseErrorHandler GetDatabaseErrorHandler()
        {
            return errorHandler;
        }

        /// <summary>
        /// Get Database Connection
        /// </summary>
        /// <returns>Currently selected Database if exists</returns>
        public IDatabaseConnection GetDatabaseConnection()
        {
            if (database is null)
            {
                return null;
            }
            return database.Connection;
        }

        /// <summary>
        /// Get Database Types
        /// </summary>
        /// <returns>All data types as a list</returns>
        public List<string> GetDataTypes()
        {
            return Enum.GetNames(typeof(ColumnDataType)).ToList();
        }

    }

}