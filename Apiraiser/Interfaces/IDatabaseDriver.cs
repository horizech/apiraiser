using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Apiraiser.Models;
using Apiraiser.Mappings;
using Apiraiser.Enums;
using Apiraiser.Helpers;

namespace Apiraiser.Interfaces
{
    public interface IDatabaseDriver
    {
        IDatabaseDataHandler DataType { get; set; }

        IDatabaseConnection Connection { get; set; }

        Task<bool> SetSessionReplicationRole(string role);
        Task<bool> SetValWithMaxId(string schema, string table);
        Task<List<string>> GetTablesList(string schema);

        Task<bool> CreateSchema(string schema);

        Task<bool> CreateTable(string schema, string table, List<ColumnInfo> columns);

        Task<List<ForeignKeyInfo>> GetForeignKeysList(string schema, string table);

        Task<APIResult> AddColumn(string schema, string table, ColumnInfo column);

        Task<bool> RemoveColumn(string schema, string table, string column, bool removeDependants);

        Task<List<ColumnInfo>> GetTableColumns(string schema, string table);
        Task<APIResult> DeleteColumn(string schema, string table, string column);

        Task<object> RunQuery(ApiraiserQuery query);

        Task<List<int>> InsertRow(string schema, string table, Dictionary<string, object> parameters, List<ColumnInfo> columns);
        Task<List<int>> InsertRows(string schema, string table, List<Dictionary<string, object>> parameters);

        Task<List<Dictionary<string, object>>> GetRows(string schema, string table, List<AddForeignTables> AddForeignTables = null);

        Task<List<Dictionary<string, object>>> GetRowsByConditions(string schema, string table, List<QuerySearchItem> parameters, List<AddForeignTables> AddForeignTables = null);

        Task<bool> UpdateRows(string schema, string table, Dictionary<string, object> data, List<QuerySearchItem> parameters);

        Task<bool> DeleteRows(string schema, string table, List<QuerySearchItem> parameters);

        Task CreateDBIfNotExist(string database);
    }
}