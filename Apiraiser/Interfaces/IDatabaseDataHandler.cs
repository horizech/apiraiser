using System;
using System.Collections.Generic;
using System.Text.Json;

using Apiraiser.Enums;
using Apiraiser.Models;

namespace Apiraiser.Interfaces
{
    public interface IDatabaseDataHandler
    {
        string GetDataTypeString(ColumnDataType type);
        ColumnDataType GetDataType(string type);
        Type GetSystemDataType(string type);
        object GetDataFromReader(object reader, string type, int index);
        object DeserializeValue(ColumnDataType type, object value);
        object DeserializeJson(ColumnDataType type, JsonElement value);
        JsonDocument SerializeJson(ColumnDataType type, object value);
        string SerializeJsonString(ColumnDataType type, object value);
        object FormatValue(ColumnDataType type, object value);

    }
}