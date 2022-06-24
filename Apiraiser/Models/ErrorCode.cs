using System;
using System.Collections.Generic;

using Apiraiser.Constants;

namespace Apiraiser.Enums
{
    public enum ErrorCode
    {
        GENERIC,
        DB001,
        DB002,
        DB003,
        DB004,
        DB005,

        // Table not found
        DB510,

        // Column not nullable
        DB520,
        // Column not found
        DB521,

        DB100
    }

    public static class Extensions
    {
        public static string GetMessage(this ErrorCode errorCode)
        {
            return ErrorCodeMessages.Get(errorCode);
        }
    }
}