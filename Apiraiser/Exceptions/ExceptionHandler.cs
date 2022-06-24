using System;
using System.Collections.Generic;

using Apiraiser.Constants;
using Apiraiser.Enums;
using Apiraiser.Models;

namespace Apiraiser.Exceptions
{
    public class ExceptionHandler
    {
        public static string GetExceptionErrorCode(Exception exception)
        {
            string errorCode = ErrorCode.GENERIC.ToString();
            if (exception.GetType() == typeof(ApiraiserErrorCodeException))
            {
                errorCode = ((ApiraiserErrorCodeException)exception)?.ErrorCode?.ToString() ?? exception.Message;
            }
            return errorCode;
        }
    }

}