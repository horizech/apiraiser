using System;
using System.Collections.Generic;

namespace Apiraiser.Interfaces
{
    public interface IDatabaseConnection
    {

        bool SetDatabaseConnectionUsingEnvironment();

        string GetDatabaseConnectionString();

    }
}