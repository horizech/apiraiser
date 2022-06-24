using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.Configuration;

using Apiraiser.Constants;
using Apiraiser.Enums;
using Apiraiser.Models;

namespace Apiraiser.Services
{
    public class EnvironmentService : BaseService
    {

        public EnvironmentService(IConfiguration configuration) : base(configuration)
        {
        }

        public object GetEnvironmentVariable(string variableName, object defaultValue)
        {
            try
            {
                object value = Environment.GetEnvironmentVariable(variableName);
                if (value == null)
                {
                    return defaultValue;
                }
                else
                {
                    return value;
                }
            }
            catch (Exception e)
            {
                ServiceManager.Instance.GetService<LogService>().Print(e.Message, LoggingLevel.Errors);
                return defaultValue;
            }
        }

    }

}