using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using Apiraiser.Models;
using Apiraiser.Mappings;
using Apiraiser.Helpers;
using Apiraiser.Services;
using Apiraiser.Enums;
using Apiraiser.Interfaces;

namespace Apiraiser.Helpers
{
    public class QueryDesigner
    {
        private ApiraiserQuery query { get; }

        public QueryDesigner()
        {
            query = new ApiraiserQuery();
        }

        public static QueryDesigner CreateDesigner(string schema = null, string table = null, QueryAction action = QueryAction.Null, string customQuery = null)
        {
            QueryDesigner designer = new QueryDesigner();
            if (table != null)
            {
                designer.query.TableName = table;
            }
            if (schema != null)
            {
                designer.query.SchemaName = schema;
            }
            if (action != QueryAction.Null)
            {
                designer.query.Action = action;
            }
            if (customQuery != null)
            {
                designer.query.CustomQuery = customQuery;
            }
            return designer;
        }




        public static QueryDesigner CreateCustomQueryDesigner(string customQuery, Dictionary<string, object> customParameters)
        {
            QueryDesigner designer = new QueryDesigner();
            designer.query.Action = QueryAction.Custom;
            designer.query.CustomQuery = customQuery;
            designer.query.CustomParameters = customParameters;
            return designer;
        }

        public QueryDesigner SelectAction(QueryAction action)
        {
            this.query.Action = action;
            return this;
        }

        public QueryDesigner SelectSchema(string schema)
        {
            this.query.SchemaName = schema;
            return this;
        }

        public QueryDesigner SelectTable(string table)
        {
            this.query.TableName = table;
            return this;
        }

        public QueryDesigner Limit(int limit)
        {
            this.query.Limit = limit;
            return this;
        }

        public QueryDesigner Offset(int offset)
        {
            this.query.Offset = offset;
            return this;
        }

        public QueryDesigner OrderBy(string orderBy)
        {
            this.query.OrderBy = orderBy;
            this.query.IsOrderDescending = false;
            return this;
        }

        public QueryDesigner OrderDescendingBy(string orderBy)
        {
            this.query.OrderBy = orderBy;
            this.query.IsOrderDescending = true;
            return this;
        }

        public QueryDesigner GroupBy(string groupBy)
        {
            this.query.GroupBy = groupBy;
            return this;
        }

        public QueryDesigner ApplySelectSettings(SelectSettings selectSettings)
        {
            if (selectSettings != null)
            {
                if (selectSettings.Limit > 0)
                {
                    this.Limit(selectSettings.Limit);
                }
                if (selectSettings.Offset > 0)
                {
                    this.Offset(selectSettings.Offset);
                }
                if (selectSettings.OrderBy != null && selectSettings.OrderBy.Length > 0)
                {
                    this.OrderBy(selectSettings.OrderBy);
                }
                if (selectSettings.OrderDescendingBy != null && selectSettings.OrderDescendingBy.Length > 0)
                {
                    this.OrderDescendingBy(selectSettings.OrderDescendingBy);
                }
                if (selectSettings.GroupBy != null && selectSettings.GroupBy.Length > 0)
                {
                    this.GroupBy(selectSettings.GroupBy);
                }
            }
            return this;
        }

        public QueryDesigner AddRow(Dictionary<string, object> row)
        {
            this.query.Rows ??= new List<Dictionary<string, object>>();
            this.query.Rows.Add(row);
            return this;
        }

        public QueryDesigner SetRows(List<Dictionary<string, object>> rows)
        {
            this.query.Rows = rows;
            return this;
        }

        public QueryDesigner AddOutputColumn(string column)
        {
            this.query.SelectedOutputColumns ??= new Dictionary<string, string>();
            this.query.SelectedOutputColumns.Add(column, column);
            return this;
        }

        public QueryDesigner AddOutputColumnWithAlias(string name, string alias)
        {
            this.query.SelectedOutputColumns ??= new Dictionary<string, string>();
            this.query.SelectedOutputColumns.Add(name, alias);
            return this;
        }

        public QueryDesigner SelectOutputColumns(dynamic columns)
        {
            this.query.SelectedOutputColumns = new Dictionary<string, string>();
            foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(columns))
            {
                string obj = propertyDescriptor.GetValue(columns);
                this.query.SelectedOutputColumns.Add(propertyDescriptor.Name, obj);
            }
            return this;
        }


        public QueryDesigner AddColumnDefinitions(List<ColumnInfo> columnInfos)
        {
            this.query.ColumnsDefinitions = columnInfos;
            return this;
        }

        public QueryDesigner AddForeignTables(List<AddForeignTables> addForeignTables)
        {
            this.query.AddForeignTables = addForeignTables;
            return this;
        }

        public QueryDesigner WhereEquals(string name, object value, bool caseSensitive = false)
        {
            this.query.Conditions ??= new List<QuerySearchItem>();
            this.query.Conditions.Add(
                new QuerySearchItem()
                {
                    Name = name,
                    Value = value,
                    CaseSensitive = caseSensitive,
                    Condition = Enums.ColumnCondition.Equal
                }
            );
            return this;
        }

        public QueryDesigner WhereIncludes(string name, object value, bool caseSensitive = false)
        {
            this.query.Conditions ??= new List<QuerySearchItem>();
            this.query.Conditions.Add(
                new QuerySearchItem()
                {
                    Name = name,
                    Value = value,
                    CaseSensitive = caseSensitive,
                    Condition = Enums.ColumnCondition.Includes
                }
            );
            return this;
        }

        public QueryDesigner WhereGreaterThan(string name, object value, bool caseSensitive = false)
        {
            this.query.Conditions ??= new List<QuerySearchItem>();
            this.query.Conditions.Add(
                new QuerySearchItem()
                {
                    Name = name,
                    Value = value,
                    CaseSensitive = caseSensitive,
                    Condition = Enums.ColumnCondition.GreaterThan
                }
            );
            return this;
        }

        public QueryDesigner WhereLessThan(string name, object value, bool caseSensitive = false)
        {
            this.query.Conditions ??= new List<QuerySearchItem>();
            this.query.Conditions.Add(
                new QuerySearchItem()
                {
                    Name = name,
                    Value = value,
                    CaseSensitive = caseSensitive,
                    Condition = Enums.ColumnCondition.LessThan
                }
            );
            return this;
        }

        public QueryDesigner SetConditions(List<QuerySearchItem> conditions)
        {
            this.query.Conditions = conditions;
            return this;
        }

        public QueryDesigner ClearConditions()
        {
            this.query.Conditions?.Clear();
            return this;
        }

        public async Task<IEnumerable<T>> ExecuteQuery<T>()
        {
            QueryBuilderOutput query = ServiceManager
                .Instance
                .GetService<DatabaseService>()
                .GetQueryBuilder()
                .Build(this.query);

            return await ServiceManager
                .Instance
                .GetService<DatabaseService>()
                .GetQueryExecuter()
                .Execute<T>(query);
        }

        public async Task<IEnumerable<dynamic>> ExecuteQuery()
        {
            QueryBuilderOutput query = ServiceManager
                .Instance
                .GetService<DatabaseService>()
                .GetQueryBuilder()
                .Build(this.query);

            return await ServiceManager
                .Instance
                .GetService<DatabaseService>()
                .GetQueryExecuter()
                .Execute(query);
        }

        public async Task<bool> ExecuteNonQuery()
        {
            QueryBuilderOutput query = ServiceManager
                .Instance
                .GetService<DatabaseService>()
                .GetQueryBuilder()
                .Build(this.query);

            return await ServiceManager
                .Instance
                .GetService<DatabaseService>()
                .GetQueryExecuter()
                .ExecuteNonQuery(query);
        }


        public async Task<object> RunQuery()
        {
            IDatabaseDriver driver = ServiceManager
                .Instance
                .GetService<DatabaseService>()
                .GetDatabaseDriver();

            if (this.query.Action == QueryAction.SelectRows)
            {

                if ((this.query.Conditions?.Count ?? 0) > 0)
                {
                    return await driver.GetRowsByConditions(this.query.SchemaName, this.query.TableName, this.query.Conditions, this.query.AddForeignTables);
                }
                else
                {
                    return await driver.GetRows(this.query.SchemaName, this.query.TableName, this.query.AddForeignTables);
                }
            }
            else if (this.query.Action == QueryAction.InsertRows)
            {
                return await driver.InsertRows(this.query.SchemaName, this.query.TableName, this.query.Rows);
            }
            else if (this.query.Action == QueryAction.UpdateRows)
            {
                return await driver.UpdateRows(this.query.SchemaName, this.query.TableName, this.query.Rows?[0] ?? null, this.query.Conditions);
            }
            else if (this.query.Action == QueryAction.DeleteRows)
            {
                return await driver.DeleteRows(this.query.SchemaName, this.query.TableName, this.query.Conditions);
            }
            else
            {
                return null;
            }
        }

        public async Task<List<Dictionary<string, object>>> RunSelectQuery()
        {
            if ((this.query.ColumnsDefinitions?.Count ?? 0) < 1)
            {
                this.AddColumnDefinitions(await ServiceManager.Instance.GetService<DatabaseService>().GetDatabaseDriver().GetTableColumns(this.query.SchemaName, this.query.TableName));
            }

            if ((this.query.Conditions?.Count ?? 0) > 0)
            {

                this.query.Conditions = this.query.Conditions.Select(parameter =>
                {
                    parameter.Value = ServiceManager
                            .Instance
                            .GetService<DatabaseService>()
                            .GetDatabaseDriver()
                            .DataType
                            .DeserializeValue(this.query.ColumnsDefinitions.Find(c => c.Name.ToLower().Equals(parameter.Name.ToString().ToLower())).Datatype, parameter.Value);
                    return parameter;
                }).ToList();

                OrderBy("Id");
                // .OrderDescendingBy("TableName")
                // .Limit(5)
                // .Offset(5);

            }

            SelectAction(QueryAction.SelectRows);

            List<Dictionary<string, object>> result = (await ExecuteQuery<Dictionary<string, object>>()).ToList();

            List<string> imageColumns = this.query.ColumnsDefinitions.Where(column => column.Datatype == ColumnDataType.Image).Select(x => x.Name).ToList();

            if (imageColumns.Count > 0)
            {
                for (int i = 0; i < result.Count; i++)
                {
                    foreach (string imageColumn in imageColumns)
                    {
                        if (result[i][imageColumn] != null)
                        {
                            result[i][imageColumn] = (result[i][imageColumn] as byte[]).Select(x => (int)x).ToArray();
                        }
                    }
                }
            }
            return result;

        }

        public async Task<List<int>> RunInsertQuery()
        {
            return await ServiceManager
                .Instance
                .GetService<DatabaseService>()
                .GetDatabaseDriver()
                .InsertRows(this.query.SchemaName, this.query.TableName, this.query.Rows);
        }

        public async Task<bool> RunUpdateQuery()
        {
            return await ServiceManager
                .Instance
                .GetService<DatabaseService>()
                .GetDatabaseDriver()
                .UpdateRows(this.query.SchemaName, this.query.TableName, this.query.Rows?[0] ?? null, this.query.Conditions);
        }

        public async Task<bool> RunDeleteQuery()
        {
            return await ServiceManager
                .Instance
                .GetService<DatabaseService>()
                .GetDatabaseDriver()
                .DeleteRows(this.query.SchemaName, this.query.TableName, this.query.Conditions);
        }

    }
}