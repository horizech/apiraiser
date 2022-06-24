using System;
using System.Collections.Generic;

using Apiraiser.Services;
using Apiraiser.Models;
using Apiraiser.Interfaces;
using Apiraiser.Enums;

namespace Apiraiser.Models
{
    public class UpdateRequest
    {
        public Dictionary<string, object> Data { get; set; }
        public List<QuerySearchItem> Parameters { get; set; }

    }
}