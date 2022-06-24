using System;
using System.Collections.Generic;

namespace Apiraiser.Enums
{
    public enum QueryAction
    {
        Null,
        CreateDatabase,
        CreateSchema,
        CreateTable,
        SelectRows,
        SelectCount,
        InsertRows,
        UpdateRows,
        DeleteRows,
        Custom
    }

}