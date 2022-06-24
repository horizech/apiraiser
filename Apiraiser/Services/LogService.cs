using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.Configuration;

using Apiraiser.Databases;
using Apiraiser.Enums;
using Apiraiser.Constants;

namespace Apiraiser.Services
{
    public class LogService : BaseService
    {

        public LogService(IConfiguration configuration) : base(configuration)
        {
        }

        private LoggingLevel loggingLevel { get; set; }

        public LoggingLevel GetLoggingLevel()
        {
            return loggingLevel;
        }

        public void SetLoggingLevel(LoggingLevel loggingLevel)
        {
            this.loggingLevel = loggingLevel;
        }

        public void SetLoggingLevelFromEnvironment()
        {
            this.loggingLevel = (LoggingLevel)Enum.Parse(
                typeof(LoggingLevel),
                (string)ServiceManager.Instance.GetService<EnvironmentService>().GetEnvironmentVariable(Config.LoggingLevel, Config.DefaultLoggingLevel)
            );
        }

        public void Print(object text, LoggingLevel loggingLevel)
        {
            if ((int)GetLoggingLevel() >= (int)loggingLevel)
            {
                Console.WriteLine(text.ToString());
            }
        }

    }

}