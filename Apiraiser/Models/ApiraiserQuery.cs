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
    public class ApiraiserQuery
    {
        public QueryAction Action { get; set; }
        public string SchemaName { get; set; }
        public string TableName { get; set; }
        public string CustomQuery { get; set; }
        public Dictionary<string, object> CustomParameters { get; set; }
        public List<Dictionary<string, object>> Rows { get; set; }
        public List<QuerySearchItem> Conditions { get; set; }
        public List<ColumnInfo> ColumnsDefinitions { get; set; }
        public Dictionary<string, string> SelectedOutputColumns { get; set; }
        public List<AddForeignTables> AddForeignTables { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public string OrderBy { get; set; }
        public bool IsOrderDescending { get; set; }
        public string GroupBy { get; set; }
        public ApiraiserQuery() { }

    }
}