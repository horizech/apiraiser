using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

using Apiraiser.Models;
using Apiraiser.Enums;

namespace Apiraiser.Interfaces
{
    public interface IQueryHelper
    {
        string GetConditionString(ColumnCondition condition);
        string ApplyCase(object str, bool isCaseSensitive = true);
        string ApplyColumnCondition(ColumnDataType dataType, QuerySearchItem item);
        string ApplyColumnConditionWithoutParam(ColumnDataType dataType, QuerySearchItem item, int? level = null);
        string GetColumnConditionItem(ColumnDataType dataType, QuerySearchItem item);
        void PutColumnConditionParameterInCollection(ColumnDataType dataType, QuerySearchItem item, object collection);
        object GetColumnConditionParameter(ColumnDataType dataType, QuerySearchItem item);

    }
}
