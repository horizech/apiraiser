using System;
using System.Collections.Generic;

using Apiraiser.Enums;

namespace Apiraiser.Constants
{
    public class ErrorCodeMessages
    {
        public static string Get(string errorCode)
        {
            switch (errorCode)
            {
                default: case "GENERIC": return null;
                case "DB001": return "Database: Not authorized";
                case "DB002": return "Database: Not connected";
                case "DB003": return "Database: Connection timeout";
                case "DB004": return "Database: Connection error";
                case "DB005": return "Database: Request timeout";
                case "DB510": return "Database: Table does not exist";
                case "DB520": return "Database: Column cannot be null";
                case "DB521": return "Database: Column not found";
                case "DB100": return "Database: No result";
            }
        }

        public static string Get(ErrorCode errorCode)
        {
            return Get(errorCode.ToString());
        }


    }
}