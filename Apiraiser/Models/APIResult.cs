using System;
using System.Collections.Generic;

using Apiraiser.Constants;
using Apiraiser.Services;
using Apiraiser.Enums;
using Apiraiser.Exceptions;
namespace Apiraiser.Models
{
    public class APIResult
    {
        public bool Success { get; set; }
        public string ErrorCode { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }

        public void Print()
        {
            ServiceManager.Instance.GetService<LogService>().Print(string.Format("Success: {0}, Message: {1}, Data: {2}", Success.ToString(), Message, Data.ToString()), LoggingLevel.All);
        }

        public static APIResult GetSimpleSuccessResult(string successMessage)
        {
            return new APIResult()
            {
                Success = true,
                Message = successMessage
            };
        }

        public static APIResult GetSimpleFailureResult(string failureMessage)
        {
            return new APIResult()
            {
                Success = false,
                Message = failureMessage
            };
        }

        public static APIResult GetSimpleErrorResult(string failureMessage, string errorCode)
        {
            return new APIResult()
            {
                Success = false,
                Message = failureMessage,
                ErrorCode = errorCode
            };
        }

        public static APIResult GetExceptionResult(Exception exception)
        {
            string errorCode = ExceptionHandler.GetExceptionErrorCode(exception);

            return APIResult.GetSimpleErrorResult(ErrorCodeMessages.Get(errorCode), errorCode);

        }
    }

}