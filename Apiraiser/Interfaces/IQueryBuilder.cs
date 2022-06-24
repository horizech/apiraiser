using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Apiraiser.Models;
using Apiraiser.Enums;
using Apiraiser.Helpers;

namespace Apiraiser.Interfaces
{
    public interface IQueryBuilder
    {
        QueryBuilderOutput Build(ApiraiserQuery query, int level = 0, Dictionary<string, object> parameters = null);
    }
}