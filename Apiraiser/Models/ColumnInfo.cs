using System;
using System.Collections.Generic;

using System.Text.Json.Serialization;

using Apiraiser.Services;
using Apiraiser.Models;
using Apiraiser.Interfaces;
using Apiraiser.Enums;

namespace Apiraiser.Models
{
    public class ColumnInfo
    {
        public string Name { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ColumnDataType Datatype { get; set; }
        public bool IsRequired { get; set; }
        public bool IsUnique { get; set; }
        public bool IsForeignKey { get; set; }
        public string ForeignSchema { get; set; }
        public string ForeignTable { get; set; }
        public string ForeignName { get; set; }
        public object DefaultValue { get; set; }

        public ColumnInfo() { }
        public void Print()
        {
            ServiceManager.Instance.GetService<LogService>().Print(string.Format("Name: {0}, DataType: {1}, IsRequired: {2}, IsUnique: {3}, IsForeignKey: {4}, ForeignSchema: {5}, ForeignTable: {6}, ForeignName: {7}", Name, Datatype.ToString(), IsRequired, IsUnique, IsForeignKey, ForeignSchema, ForeignTable, ForeignName), LoggingLevel.All);
        }
    }

}