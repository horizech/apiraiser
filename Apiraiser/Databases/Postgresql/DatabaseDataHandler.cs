using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

using Npgsql;

using Apiraiser.Interfaces;
using Apiraiser.Enums;
using Apiraiser.Models;

namespace Apiraiser.Databases.Postgresql
{
    public class DatabaseDataHandler : IDatabaseDataHandler
    {
        public string GetDataTypeString(ColumnDataType type)
        {
            switch (type)
            {
                default:
                case ColumnDataType.Integer:
                    return "integer";
                case ColumnDataType.IntegerArray:
                    return "integer[]";
                case ColumnDataType.Decimal:
                    return "decimal";
                case ColumnDataType.Float:
                    return "real";
                case ColumnDataType.Boolean:
                    return "boolean";
                case ColumnDataType.BooleanArray:
                    return "boolean[]";
                case ColumnDataType.DateTime:
                    return "timestamp";
                case ColumnDataType.Json:
                    return "jsonb";
                case ColumnDataType.JsonArray:
                    return "jsonb[]";
                case ColumnDataType.Money:
                    return "money";
                case ColumnDataType.ShortText:
                    return "character varying(256)";
                case ColumnDataType.LongText:
                    return "text";
                case ColumnDataType.Image:
                    return "bytea";
            }
        }

        public ColumnDataType GetDataType(string type)
        {
            switch (type)
            {
                default:
                case "integer":
                    return ColumnDataType.Integer;
                case "integer[]":
                    return ColumnDataType.IntegerArray;
                case "decimal":
                    return ColumnDataType.Decimal;
                case "real":
                    return ColumnDataType.Float;
                case "boolean":
                    return ColumnDataType.Boolean;
                case "boolean[]":
                    return ColumnDataType.BooleanArray;
                case "timestamp":
                case "timestamp without time zone":
                    return ColumnDataType.DateTime;
                case "jsonb":
                    return ColumnDataType.Json;
                case "jsonb[]":
                    return ColumnDataType.JsonArray;
                case "money":
                    return ColumnDataType.Money;
                case "character varying(256)":
                    return ColumnDataType.ShortText;
                case "text":
                    return ColumnDataType.LongText;
                case "bytea":
                    return ColumnDataType.Image;
            }
        }

        public Type GetSystemDataType(string type)
        {
            switch (type)
            {
                default:
                case "integer":
                    return typeof(int);
                case "integer[]":
                    return typeof(int[]);
                case "decimal":
                    return typeof(decimal);
                case "real":
                    return typeof(double);
                case "boolean":
                    return typeof(bool);
                case "boolean[]":
                    return typeof(bool[]);
                case "timestamp":
                case "timestamp without time zone":
                    return typeof(DateTime);
                case "jsonb":
                    return typeof(object);
                case "jsonb[]":
                    return typeof(object[]);
                case "money":
                    return typeof(decimal);
                case "character varying(256)":
                    return typeof(string);
                case "text":
                    return typeof(string);
            }
        }

        public object GetDataFromReader(object dataReader, string type, int index)
        {
            try
            {
                NpgsqlDataReader reader = (NpgsqlDataReader)dataReader;
                switch (type)
                {
                    default:
                    case "integer":
                        return reader.GetFieldValue<Int32>(index);
                    case "integer[]":
                        return reader.GetFieldValue<Int32[]>(index);
                    case "decimal":
                        return reader.GetFieldValue<decimal>(index);
                    case "real":
                        return reader.GetFieldValue<double>(index);
                    case "boolean":
                        return reader.GetFieldValue<bool>(index);
                    case "boolean[]":
                        return reader.GetFieldValue<bool[]>(index);
                    case "timestamp":
                    case "timestamp without time zone":
                        return reader.GetFieldValue<DateTime>(index);
                    case "jsonb":
                        return reader.GetFieldValue<object>(index);
                    case "jsonb[]":
                        return reader.GetFieldValue<object[]>(index);
                    case "money":
                        return reader.GetFieldValue<decimal>(index);
                    case "character varying(256)":
                        return reader.GetFieldValue<string>(index);
                    case "text":
                        return reader.GetFieldValue<string>(index);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                switch (type)
                {
                    default:
                    case "integer":
                        return 0;
                    case "integer[]":
                        return null;
                    case "decimal":
                        return 0;
                    case "real":
                        return 0;
                    case "boolean":
                        return false;
                    case "boolean[]":
                        return null;
                    case "timestamp":
                    case "timestamp without time zone":
                        return null;
                    case "jsonb":
                        return null;
                    case "jsonb[]":
                        return null;
                    case "money":
                        return 0;
                    case "character varying(256)":
                        return null;
                    case "text":
                        return null;
                }
            }

        }

        public object DeserializeValue(ColumnDataType type, object value)
        {
            if (value == null)
            {
                return null;
            }
            else if (value is JsonElement)
            {
                return DeserializeJson(type, ((JsonElement)value));
            }
            else
            {
                return value;
            }
        }

        public object DeserializeJson(ColumnDataType type, JsonElement value)
        {
            var json = value.GetRawText();

            if (value.ValueKind == JsonValueKind.Array)
            {
                switch (type)
                {
                    default:
                    case ColumnDataType.Integer:
                        return JsonSerializer.Deserialize<int[]>(json);
                    case ColumnDataType.IntegerArray:
                        return JsonSerializer.Deserialize<int[]>(json);
                    case ColumnDataType.Decimal:
                        return JsonSerializer.Deserialize<decimal[]>(json);
                    case ColumnDataType.Float:
                        return JsonSerializer.Deserialize<double[]>(json);
                    case ColumnDataType.Boolean:
                        return JsonSerializer.Deserialize<bool[]>(json);
                    case ColumnDataType.BooleanArray:
                        return JsonSerializer.Deserialize<bool[]>(json);
                    case ColumnDataType.DateTime:
                        return (JsonSerializer.Deserialize<string[]>(json)).Select(x => DateTime.Parse(x));
                    case ColumnDataType.Json:
                        return JsonSerializer.Deserialize<JsonElement[]>(json);
                    case ColumnDataType.JsonArray:
                        return JsonSerializer.Deserialize<JsonElement[]>(json);
                    case ColumnDataType.Money:
                        return JsonSerializer.Deserialize<decimal[]>(json);
                    case ColumnDataType.ShortText:
                        return JsonSerializer.Deserialize<string[]>(json);
                    case ColumnDataType.LongText:
                        return JsonSerializer.Deserialize<string[]>(json);
                    case ColumnDataType.Image:
                        return JsonSerializer.Deserialize<byte[]>(json);
                }
            }
            else
            {
                switch (type)
                {
                    default:
                    case ColumnDataType.Integer:
                        return JsonSerializer.Deserialize<int>(json);
                    case ColumnDataType.IntegerArray:
                        return JsonSerializer.Deserialize<int[]>(json);
                    case ColumnDataType.Decimal:
                        return JsonSerializer.Deserialize<decimal>(json);
                    case ColumnDataType.Float:
                        return JsonSerializer.Deserialize<double>(json);
                    case ColumnDataType.Boolean:
                        return JsonSerializer.Deserialize<bool>(json);
                    case ColumnDataType.BooleanArray:
                        return JsonSerializer.Deserialize<bool[]>(json);
                    case ColumnDataType.DateTime:
                        return DateTime.Parse(JsonSerializer.Deserialize<string>(json));
                    case ColumnDataType.Json:
                        return JsonSerializer.Deserialize<JsonElement>(json);
                    case ColumnDataType.JsonArray:
                        return JsonSerializer.Deserialize<JsonElement[]>(json);
                    case ColumnDataType.Money:
                        return JsonSerializer.Deserialize<decimal>(json);
                    case ColumnDataType.ShortText:
                        return JsonSerializer.Deserialize<string>(json);
                    case ColumnDataType.LongText:
                        return JsonSerializer.Deserialize<string>(json);
                    case ColumnDataType.Image:
                        return JsonSerializer.Deserialize<byte[]>(json);
                }
            }

        }

        public JsonDocument SerializeJson(ColumnDataType type, object value)
        {
            return JsonDocument.Parse(JsonSerializer.SerializeToUtf8Bytes(value.ToString()));
        }

        public string SerializeJsonString(ColumnDataType type, object value)
        {
            return JsonSerializer.Serialize(value.ToString());
        }

        public object FormatValue(ColumnDataType type, object value)
        {
            Type valueType = value.GetType();
            if (valueType.IsArray)
            {
                switch (type)
                {
                    default:
                    case ColumnDataType.Integer:
                        return value;
                    case ColumnDataType.IntegerArray:
                        return value;
                    case ColumnDataType.Decimal:
                        return value;
                    case ColumnDataType.Float:
                        return value;
                    case ColumnDataType.Boolean:
                        return value;
                    case ColumnDataType.BooleanArray:
                        return value;
                    case ColumnDataType.DateTime:
                        return "'" + value + "'";
                    case ColumnDataType.Json:
                        return value;
                    case ColumnDataType.JsonArray:
                        return value;
                    case ColumnDataType.Money:
                        return value;
                    case ColumnDataType.ShortText:
                        return "'" + value + "'";
                    case ColumnDataType.LongText:
                        return "'" + value + "'";
                    case ColumnDataType.Image:
                        return value;
                }
            }
            else
            {
                switch (type)
                {
                    default:
                    case ColumnDataType.Integer:
                        return value;
                    case ColumnDataType.IntegerArray:
                        return value;
                    case ColumnDataType.Decimal:
                        return value;
                    case ColumnDataType.Float:
                        return value;
                    case ColumnDataType.Boolean:
                        return value;
                    case ColumnDataType.BooleanArray:
                        return value;
                    case ColumnDataType.DateTime:
                        return "'" + value + "'";
                    case ColumnDataType.Json:
                        return value;
                    case ColumnDataType.JsonArray:
                        return value;
                    case ColumnDataType.Money:
                        return value;
                    case ColumnDataType.ShortText:
                        return "'" + value + "'";
                    case ColumnDataType.LongText:
                        return "'" + value + "'";
                    case ColumnDataType.Image:
                        return value;
                }
            }
        }

    }
}