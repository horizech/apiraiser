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


namespace Apiraiser.Databases.Postgresql
{
    public class DatabaseErrorHandler : IDatabaseErrorHandler
    {
        public DatabaseErrorHandler()
        {
        }

        public ErrorCode GetErrorCode(string errorMessage)
        {
            if (errorMessage.Contains("42P01") || errorMessage.Contains("DB510"))
            {
                return ErrorCode.DB510;
            }
            if (errorMessage.Contains("23502") || errorMessage.Contains("DB520"))
            {
                return ErrorCode.DB520;
            }
            if (errorMessage.Contains("42703") || errorMessage.Contains("DB521"))
            {
                return ErrorCode.DB521;
            }
            return ErrorCode.DB004;
        }

        public ErrorCode GetErrorCode(Exception errorException)
        {
            return GetErrorCode(errorException.Message);
        }

        public void ThrowApiraiserException(string errorMessage)
        {
            throw new ApiraiserErrorCodeException(GetErrorCode(errorMessage));
        }

        public void ThrowApiraiserException(Exception errorException)
        {
            ThrowApiraiserException(errorException.Message);
        }

        public void ThrowApiraiserException(ErrorCode errorCode, string errorMessage)
        {
            throw new ApiraiserErrorCodeException(errorCode, errorMessage);
        }

        public void ThrowApiraiserExceptionWithCustomMessage(string errorMessage)
        {
            throw new ApiraiserErrorCodeException(errorMessage);
        }



    }
}