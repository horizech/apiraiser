using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text.Json;

using Apiraiser.Interfaces;
using Apiraiser.Models;
using Apiraiser.Helpers;
using Apiraiser.Enums;
using Apiraiser.Services;


namespace Apiraiser.Databases.Postgresql
{
    public class QueryHelper : IQueryHelper
    {

        public string GetConditionString(ColumnCondition condition)
        {
            switch (condition)
            {
                default:
                case ColumnCondition.Equal:
                    return "=";
                case ColumnCondition.GreaterThan:
                    return ">";
                case ColumnCondition.LessThan:
                    return "<";
                case ColumnCondition.Includes:
                    return "= ANY";
            }
        }

        public string ApplyCase(object str, bool isCaseSensitive = true)
        {
            if (isCaseSensitive)
            {
                return str.ToString();
            }
            else
            {
                return string.Format("LOWER({0})", str.ToString());
            }
        }

        public string ApplyColumnCondition(ColumnDataType dataType, QuerySearchItem item)
        {
            string name = item.Name;
            object value = ServiceManager.Instance.GetService<DatabaseService>().GetDatabaseDriver().DataType.DeserializeValue(dataType, item.Value);

            if (value == null)
            {
                return string.Format("\"{0}\" IS NULL", name, GetConditionString(item.Condition));
            }

            if (value.GetType().IsArray)
            {
                switch (dataType)
                {
                    default:
                    case ColumnDataType.Integer:
                    case ColumnDataType.IntegerArray:
                        return string.Format("\"{0}\" {1} ({2})", name, GetConditionString(item.Condition), string.Join(", ", ((int[])value).Select(x => x.ToString())));

                    case ColumnDataType.Decimal:
                        return string.Format("\"{0}\" {1} ({2})", name, GetConditionString(item.Condition), string.Join(", ", ((decimal[])value).Select(x => x.ToString())));

                    case ColumnDataType.Float:
                        return string.Format("\"{0}\" {1} ({2})", name, GetConditionString(item.Condition), string.Join(", ", ((double[])value).Select(x => x.ToString())));

                    case ColumnDataType.Money:
                        return string.Format("\"{0}\" {1} ({2})", name, GetConditionString(item.Condition), string.Join(", ", ((decimal[])value).Select(x => x.ToString())));

                    case ColumnDataType.Boolean:
                    case ColumnDataType.BooleanArray:
                        return string.Format("\"{0}\" {1} ({2})", name, GetConditionString(item.Condition), string.Join(", ", ((object[])value).Select(x => x.ToString())));

                    case ColumnDataType.DateTime:
                        return string.Format("\"{0}\" {1} ({2})", name, GetConditionString(item.Condition), string.Join(", ", ((object[])value).Select(x => DateTime.Parse(x.ToString()).ToLongDateString())));

                    case ColumnDataType.Json:
                    case ColumnDataType.JsonArray:
                        return string.Format("\"{0}\" {1} ('{2}')", name, GetConditionString(item.Condition), ServiceManager.Instance.GetService<DatabaseService>().GetDatabaseDriver().DataType.SerializeJsonString(dataType, item.Value));

                    case ColumnDataType.ShortText:
                    case ColumnDataType.LongText:
                        return string.Format("{0} {1} ({2})",
                        ApplyCase(string.Format("\"{0}\"", name), item.CaseSensitive),
                        GetConditionString(item.Condition),
                        string.Join(",", ((string[])value).Select(x => ApplyCase(string.Format("'{0}'", x), item.CaseSensitive))));
                    case ColumnDataType.Image:
                        return string.Format("\"{0}\" {1} ({2})", name, GetConditionString(item.Condition), string.Join(", ", ((byte[])value).Select(x => x.ToString())));
                }
            }
            else
            {
                switch (dataType)
                {
                    default:
                    case ColumnDataType.Integer:
                    case ColumnDataType.Decimal:
                    case ColumnDataType.Float:
                    case ColumnDataType.Money:
                        return string.Format("\"{0}\" {1} {2}", name, GetConditionString(item.Condition), value);

                    case ColumnDataType.IntegerArray:
                        return string.Format("\"{0}\" {1} ({2})", name, GetConditionString(item.Condition), value);

                    case ColumnDataType.Boolean:
                        return string.Format("\"{0}\" {1} {2}", name, GetConditionString(item.Condition), value.ToString());

                    case ColumnDataType.BooleanArray:
                        return string.Format("\"{0}\" {1} ({2})", name, GetConditionString(item.Condition), value.ToString());

                    case ColumnDataType.DateTime:
                        return string.Format("\"{0}\" {1} {2}", name, GetConditionString(item.Condition), ((DateTime)value).ToLongDateString());

                    case ColumnDataType.Json:
                        return string.Format("\"{0}\" {1} '{2}'", name, GetConditionString(item.Condition), ServiceManager.Instance.GetService<DatabaseService>().GetDatabaseDriver().DataType.SerializeJsonString(dataType, item.Value));

                    case ColumnDataType.JsonArray:
                        return string.Format("\"{0}\" {1} ('{2}')", name, GetConditionString(item.Condition), ServiceManager.Instance.GetService<DatabaseService>().GetDatabaseDriver().DataType.SerializeJsonString(dataType, item.Value));

                    case ColumnDataType.ShortText:
                    case ColumnDataType.LongText:
                        return string.Format("{0} {1} {2}", ApplyCase(string.Format("\"{0}\"", name), item.CaseSensitive), GetConditionString(item.Condition), ApplyCase(string.Format("'{0}'", value), item.CaseSensitive));
                    case ColumnDataType.Image:
                        return string.Format("\"{0}\" {1} {2}", name, GetConditionString(item.Condition), value);
                }
            }
        }

        public string ApplyColumnConditionWithoutParam(ColumnDataType dataType, QuerySearchItem item, int? level = null)
        {
            string name = item.Name;
            object value = ServiceManager.Instance.GetService<DatabaseService>().GetDatabaseDriver().DataType.DeserializeValue(dataType, item.Value);

            if (value == null)
            {
                return string.Format("\"{0}\" IS NULL", name, GetConditionString(item.Condition));
            }
            else
            {
                if (level.HasValue)
                {
                    return string.Format("\"{0}\" {1} (@{0}{2})", name, GetConditionString(item.Condition), level.Value);
                }
                else
                {
                    return string.Format("\"{0}\" {1} (@{0})", name, GetConditionString(item.Condition));
                }

            }
        }


        public string GetColumnConditionItem(ColumnDataType dataType, QuerySearchItem item)
        {
            return string.Format("\"{0}\" {1} @{0}", item.Name, GetConditionString(item.Condition));
        }

        public void PutColumnConditionParameterInCollection(ColumnDataType dataType, QuerySearchItem item, object collection)
        {
            string name = item.Name;
            object value = ServiceManager.Instance.GetService<DatabaseService>().GetDatabaseDriver().DataType.DeserializeValue(dataType, item.Value);

            if (value.GetType().IsArray)
            {
                // ((Npgsql.NpgsqlParameterCollection)collection).AddWithValue(item.Name, NpgsqlTypes.NpgsqlDbType.Array, value);

                switch (dataType)
                {
                    default:
                    case ColumnDataType.Integer:
                        ((Npgsql.NpgsqlParameterCollection)collection).AddWithValue(item.Name, NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Integer, value);
                        break;
                    case ColumnDataType.IntegerArray:
                        ((Npgsql.NpgsqlParameterCollection)collection).AddWithValue(item.Name, NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Integer, value);
                        break;

                    case ColumnDataType.Decimal:
                        ((Npgsql.NpgsqlParameterCollection)collection).AddWithValue(item.Name, NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Double, value);
                        break;

                    case ColumnDataType.Float:
                        ((Npgsql.NpgsqlParameterCollection)collection).AddWithValue(item.Name, NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Real, value);
                        break;

                    case ColumnDataType.Money:
                        ((Npgsql.NpgsqlParameterCollection)collection).AddWithValue(item.Name, NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Money, value);
                        break;

                    case ColumnDataType.Boolean:
                        ((Npgsql.NpgsqlParameterCollection)collection).AddWithValue(item.Name, NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Boolean, value);
                        break;

                    case ColumnDataType.DateTime:
                        ((Npgsql.NpgsqlParameterCollection)collection).AddWithValue(item.Name, NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Date, value);
                        break;

                    case ColumnDataType.Json:
                        ((Npgsql.NpgsqlParameterCollection)collection).AddWithValue(item.Name, NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Jsonb, value);
                        break;

                    case ColumnDataType.ShortText:
                        ((Npgsql.NpgsqlParameterCollection)collection).AddWithValue(item.Name, NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Varchar, value);
                        break;

                    case ColumnDataType.LongText:
                        ((Npgsql.NpgsqlParameterCollection)collection).AddWithValue(item.Name, NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Text, value);
                        break;
                    case ColumnDataType.Image:
                        ((Npgsql.NpgsqlParameterCollection)collection).AddWithValue(item.Name, NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Bytea, value);
                        break;

                }
            }
            else
            {
                switch (dataType)
                {
                    default:
                    case ColumnDataType.Integer:
                        ((Npgsql.NpgsqlParameterCollection)collection).AddWithValue(item.Name, NpgsqlTypes.NpgsqlDbType.Integer, value);
                        break;
                    case ColumnDataType.IntegerArray:
                        ((Npgsql.NpgsqlParameterCollection)collection).AddWithValue(item.Name, NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Integer, value);
                        break;

                    case ColumnDataType.Decimal:
                        ((Npgsql.NpgsqlParameterCollection)collection).AddWithValue(item.Name, NpgsqlTypes.NpgsqlDbType.Double, value);
                        break;

                    case ColumnDataType.Float:
                        ((Npgsql.NpgsqlParameterCollection)collection).AddWithValue(item.Name, NpgsqlTypes.NpgsqlDbType.Real, value);
                        break;

                    case ColumnDataType.Money:
                        ((Npgsql.NpgsqlParameterCollection)collection).AddWithValue(item.Name, NpgsqlTypes.NpgsqlDbType.Money, value);
                        break;

                    case ColumnDataType.Boolean:
                        ((Npgsql.NpgsqlParameterCollection)collection).AddWithValue(item.Name, NpgsqlTypes.NpgsqlDbType.Boolean, value);
                        break;

                    case ColumnDataType.BooleanArray:
                        ((Npgsql.NpgsqlParameterCollection)collection).AddWithValue(item.Name, NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Boolean, value);
                        break;

                    case ColumnDataType.DateTime:
                        ((Npgsql.NpgsqlParameterCollection)collection).AddWithValue(item.Name, NpgsqlTypes.NpgsqlDbType.Date, value);
                        break;

                    case ColumnDataType.Json:
                        ((Npgsql.NpgsqlParameterCollection)collection).AddWithValue(item.Name, NpgsqlTypes.NpgsqlDbType.Jsonb, value);
                        break;

                    case ColumnDataType.JsonArray:
                        ((Npgsql.NpgsqlParameterCollection)collection).AddWithValue(item.Name, NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Jsonb, value);
                        break;

                    case ColumnDataType.ShortText:
                        ((Npgsql.NpgsqlParameterCollection)collection).AddWithValue(item.Name, NpgsqlTypes.NpgsqlDbType.Varchar, value);
                        break;

                    case ColumnDataType.LongText:
                        ((Npgsql.NpgsqlParameterCollection)collection).AddWithValue(item.Name, NpgsqlTypes.NpgsqlDbType.Text, value);
                        break;
                    case ColumnDataType.Image:
                        ((Npgsql.NpgsqlParameterCollection)collection).AddWithValue(item.Name, NpgsqlTypes.NpgsqlDbType.Bytea, value);
                        break;

                }
            }
        }

        public object GetColumnConditionParameter(ColumnDataType dataType, QuerySearchItem item)
        {
            string name = item.Name;
            object value = ServiceManager.Instance.GetService<DatabaseService>().GetDatabaseDriver().DataType.DeserializeValue(dataType, item.Value);

            if (value.GetType().IsArray)
            {
                switch (dataType)
                {
                    default:
                    case ColumnDataType.Integer:
                    case ColumnDataType.IntegerArray:
                        return (int[])value;

                    case ColumnDataType.Decimal:
                        return (decimal[])value;

                    case ColumnDataType.Float:
                        return (double[])value;

                    case ColumnDataType.Money:
                        return (decimal[])value;

                    case ColumnDataType.Boolean:
                    case ColumnDataType.BooleanArray:
                        return (bool[])value;

                    case ColumnDataType.DateTime:
                        return ((object[])value).Select(x => DateTime.Parse(x.ToString()));

                    case ColumnDataType.Json:
                    case ColumnDataType.JsonArray:
                        return item.Value;

                    case ColumnDataType.ShortText:
                    case ColumnDataType.LongText:
                        return (string[])value;
                    case ColumnDataType.Image:
                        return item.Value;
                }
            }
            else
            {
                switch (dataType)
                {
                    default:
                    case ColumnDataType.Integer:
                        return (int)value;

                    case ColumnDataType.IntegerArray:
                        return (int[])value;

                    case ColumnDataType.Decimal:
                        return (decimal)value;

                    case ColumnDataType.Float:
                        return (double)value;

                    case ColumnDataType.Money:
                        return (decimal)value;

                    case ColumnDataType.Boolean:
                        return (bool)value;

                    case ColumnDataType.BooleanArray:
                        return (bool[])value;

                    case ColumnDataType.DateTime:
                        return DateTime.Parse(value.ToString());

                    case ColumnDataType.Json:
                    case ColumnDataType.JsonArray:
                        return item.Value;

                    case ColumnDataType.ShortText:
                    case ColumnDataType.LongText:
                        return (string)value;
                    case ColumnDataType.Image:
                        return item.Value;
                }
            }
        }

    }
}
