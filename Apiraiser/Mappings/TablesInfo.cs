using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;


using Apiraiser.Models;
using Apiraiser.Enums;

namespace Apiraiser.Mappings
{
    public class TablesInfo
    {
        [JsonPropertyName("Name")]
        public string Name { get; set; }

        [JsonPropertyName("Tables")]
        public Dictionary<string, string> Tables { get; set; }
    }
}
