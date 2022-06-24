using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Threading.Tasks;

using Apiraiser.Models;
using Apiraiser.Exceptions;
using Apiraiser.Helpers;
using Apiraiser.Services;
using Apiraiser.Enums;
using Apiraiser.Interfaces;

namespace Apiraiser.Interfaces
{
    public interface IQueryExecuter
    {
        Task<IEnumerable<T>> Execute<T>(QueryBuilderOutput queryBuilderOutput);
        Task<IEnumerable<dynamic>> Execute(QueryBuilderOutput queryBuilderOutput);
        Task<bool> ExecuteNonQuery(QueryBuilderOutput queryBuilderOutput);
    }
}