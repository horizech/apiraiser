using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Apiraiser.Models;
using Apiraiser.Mappings;
using Apiraiser.Helpers;
using Apiraiser.Services;
using Apiraiser.Enums;
using Apiraiser.Interfaces;

namespace Apiraiser.Models
{
    public class QueryBuilderOutput
    {
        public string Script { get; set; }
        public Dictionary<string, object> Parameters { get; set; }

        public QueryBuilderOutput() { }

        public QueryBuilderOutput(string script, Dictionary<string, object> parameters = null)
        {
            Script = script;
            Parameters = parameters;
        }

    }
}