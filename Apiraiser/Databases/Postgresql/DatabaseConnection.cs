using System;
using System.Collections.Generic;

using Apiraiser.Interfaces;
using Apiraiser.Services;
using Apiraiser.Models;
using Apiraiser.Enums;
using Apiraiser.Constants;

namespace Apiraiser.Databases.Postgresql
{
    public class DatabaseConnection : IDatabaseConnection
    {
        public string Host { get; set; }
        public string User { get; set; }
        public string Database { get; set; }
        public string Password { get; set; }
        public string Port { get; set; }

        public DatabaseConnection()
        {

        }

        public DatabaseConnection(string host, string user, string dbName, string password, string port)
        {
            this.Host = host;
            this.User = user;
            this.Database = dbName;
            this.Password = password;
            this.Port = port;
        }

        public bool SetDatabaseConnectionUsingEnvironment()
        {
            bool errorsFound = false;

            Host = (string)ServiceManager.Instance.GetService<EnvironmentService>().GetEnvironmentVariable(Config.DatabaseHost, null);
            if (Host == null || Host.Length == 0)
            {
                ServiceManager.Instance.GetService<LogService>().Print("Database Host not set!", LoggingLevel.Errors);
                errorsFound = true;
            }

            Port = (string)ServiceManager.Instance.GetService<EnvironmentService>().GetEnvironmentVariable(Config.DatabasePort, null);
            if (Port == null || Port.Length == 0)
            {
                ServiceManager.Instance.GetService<LogService>().Print("Database Port not set!", LoggingLevel.Errors);
                errorsFound = true;
            }

            Database = (string)ServiceManager.Instance.GetService<EnvironmentService>().GetEnvironmentVariable(Config.DatabaseName, null);
            if (Database == null || Database.Length == 0)
            {
                ServiceManager.Instance.GetService<LogService>().Print("Database Name not set!", LoggingLevel.Errors);
                errorsFound = true;
            }

            User = (string)ServiceManager.Instance.GetService<EnvironmentService>().GetEnvironmentVariable(Config.DatabaseUsername, null);
            if (User == null || User.Length == 0)
            {
                ServiceManager.Instance.GetService<LogService>().Print("Database Username not set!", LoggingLevel.Errors);
                errorsFound = true;
            }

            Password = (string)ServiceManager.Instance.GetService<EnvironmentService>().GetEnvironmentVariable(Config.DatabasePassword, null);
            if (Password == null || Password.Length == 0)
            {
                ServiceManager.Instance.GetService<LogService>().Print("Database Password not set!", LoggingLevel.Errors);
                errorsFound = true;
            }

            if (!errorsFound)
            {
                return true;
            }
            else
            {
                ServiceManager.Instance.GetService<LogService>().Print("Could not create Database connection!", LoggingLevel.Errors);
                return false;
            }
        }

        public string GetDatabaseConnectionString()
        {
            return
                String.Format(
                    "Server={0};Username={1};Database={2};Port={3};Password={4};SSLMode=Disable",
                    Host,
                    User,
                    Database,
                    Port,
                    Password);
        }


    }
}