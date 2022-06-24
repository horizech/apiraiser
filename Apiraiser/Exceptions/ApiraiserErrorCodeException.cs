using System;
using System.Collections.Generic;

using Apiraiser.Constants;
using Apiraiser.Enums;
using Apiraiser.Models;

namespace Apiraiser.Exceptions
{
    public class ApiraiserErrorCodeException : Exception
    {
        public ErrorCode? ErrorCode { get; set; }

        public ApiraiserErrorCodeException() { }
        public ApiraiserErrorCodeException(ErrorCode errorCode) : base(errorCode.ToString())
        {
            ErrorCode = errorCode;
        }

        public ApiraiserErrorCodeException(ErrorCode errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }

        public ApiraiserErrorCodeException(string message) : base(message) { }
        public ApiraiserErrorCodeException(string message, System.Exception inner) : base(message, inner) { }
        protected ApiraiserErrorCodeException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

    }

}