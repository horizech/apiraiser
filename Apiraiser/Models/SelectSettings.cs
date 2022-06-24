using System;
using System.Collections.Generic;

using Apiraiser.Constants;
using Apiraiser.Services;
using Apiraiser.Enums;
using Apiraiser.Exceptions;
namespace Apiraiser.Models
{
    public class SelectSettings
    {
        public int Limit { get; set; }
        public int Offset { get; set; }
        public string OrderBy { get; set; }
        public string OrderDescendingBy { get; set; }
        public string GroupBy { get; set; }

        public SelectSettings() { }
        public SelectSettings(int limit = -1, int offset = -1, string orderBy = null, string orderDescendingBy = null, string groupBy = null)
        {
            Limit = limit;
            Offset = offset;
            OrderBy = orderBy;
            OrderDescendingBy = orderDescendingBy;
            GroupBy = groupBy;
        }
    }

}