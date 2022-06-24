using System;
using System.Collections.Generic;

using Apiraiser.Models;
using Apiraiser.Mappings;
using Apiraiser.Helpers;
using Apiraiser.Services;

namespace Apiraiser.Helpers
{
    public class Columns
    {
        // public static List<Dictionary<string, string>> PredefinedColumns = new List<Dictionary<string, string>>
        // {
        //     new Dictionary<string, string>
        //     {
        //         { "Name", "Id" },
        //         { "Description", "Id" }
        //     },
        //     new Dictionary<string, string>
        //     {
        //         { "Name", "CreatedOn" },
        //         { "Description", "Created On" }
        //     },
        //     new Dictionary<string, string>
        //     {
        //         { "Name", "CreatedBy" },
        //         { "Description", "Created By" }
        //     },
        //     new Dictionary<string, string>
        //     {
        //         { "Name", "LastUpdatedOn" },
        //         { "Description", "LastUpdated On" }
        //     },
        //     new Dictionary<string, string>
        //     {
        //         { "Name", "LastUpdatedBy" },
        //         { "Description", "LastUpdated By" }
        //     }
        // };

        public static ColumnsDefinition GetColumns(string ColumnsName)
        {
            string columnsPath = FileSystem.GetPathInConfigurations($"Columns/Definitions/{ColumnsName}.json");
            string columnsJson = FileSystem.ReadFile(columnsPath);
            return FileSystem.ReadJsonString<ColumnsDefinition>(columnsJson);
        }

        public static ColumnsDefinition PredefinedColumns
        {
            get
            {
                return GetColumns("Predefined");
            }
        }

        public static ColumnsDefinition AdditionalColumns
        {
            get
            {
                return GetColumns("Additional");
            }
        }

        public static ColumnsDefinition CreatedColumns
        {
            get
            {
                return GetColumns("Created");
            }
        }

        public static ColumnsDefinition UpdatedColumns
        {
            get
            {
                return GetColumns("Updated");
            }
        }
        public static void AppendCreatedInfo(Dictionary<string, object> columns, int id = 0)
        {
            ColumnsDefinition columnsDefinition = GetColumns("Created");
            foreach (ColumnInfo column in columnsDefinition.Columns)
            {
                if (column.Datatype == Enums.ColumnDataType.Integer)
                {
                    // It's Id
                    columns.Add(column.Name, id);
                }
                else if (column.Datatype == Enums.ColumnDataType.DateTime)
                {
                    // It's Date Time
                    columns.Add(column.Name, DateTime.UtcNow);
                }
            }
        }

        public static void AppendUpdatedInfo(Dictionary<string, object> columns, int id = 0)
        {
            ColumnsDefinition columnsDefinition = GetColumns("Updated");
            foreach (ColumnInfo column in columnsDefinition.Columns)
            {
                if (column.Datatype == Enums.ColumnDataType.Integer)
                {
                    // It's Id
                    columns.Add(column.Name, id);
                }
                else if (column.Datatype == Enums.ColumnDataType.DateTime)
                {
                    // It's Date Time
                    columns.Add(column.Name, DateTime.UtcNow);
                }
            }
        }
    }
}