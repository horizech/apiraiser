using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Claims;

using Apiraiser.Models;
using Apiraiser.Enums;

namespace Apiraiser.Mappings
{
    public class JWTClaim
    {


        [JsonPropertyName("Table")]
        public string Table { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        [JsonPropertyName("ClaimType")]
        public JWTClaimTypes ClaimType { get; set; }

        [JsonPropertyName("Column")]
        public string Column { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        [JsonPropertyName("IncludeAs")]
        public JWTClaimIncludeTypes IncludeAs { get; set; }

    }
}
