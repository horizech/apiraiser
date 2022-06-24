using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;


using System.Text.Json.Serialization;

using Apiraiser.Models;
using Apiraiser.Enums;

namespace Apiraiser.Mappings
{
    public class ColumnsDefinition
    {

        [JsonPropertyName("Name")]
        public string Name { get; set; }

        // [JsonConverter(typeof(List<JsonStringEnumConverter>))]
        [JsonPropertyName("Columns")]
        public List<ColumnInfo> Columns { get; set; }

        [JsonPropertyName("Descriptions")]
        public List<Dictionary<string, string>> Descriptions { get; set; }
    }
}
