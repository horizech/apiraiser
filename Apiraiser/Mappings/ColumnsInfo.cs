using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;


using Apiraiser.Models;
using Apiraiser.Enums;

namespace Apiraiser.Mappings
{
    public class ColumnsInfo
    {
        [JsonPropertyName("Info")]
        public Dictionary<string, string> Columns { get; set; }
    }
}
