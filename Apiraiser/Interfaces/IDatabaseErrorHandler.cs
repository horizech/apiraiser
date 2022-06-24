using System;
using System.Collections.Generic;
using System.Text.Json;

using Apiraiser.Enums;
using Apiraiser.Models;

namespace Apiraiser.Interfaces
{
    public interface IDatabaseErrorHandler
    {
        ErrorCode GetErrorCode(string errorMessage);
        ErrorCode GetErrorCode(Exception errorException);
        void ThrowApiraiserException(string errorMessage);
        void ThrowApiraiserException(Exception errorException);
        void ThrowApiraiserException(ErrorCode errorCode, string errorMessage);
        void ThrowApiraiserExceptionWithCustomMessage(string errorMessage);
    }
}