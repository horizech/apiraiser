import { TablesApiProvider } from "../api_providers";

export const tablesService = {
    getTables,
    getTableColumns,
    getPredefinedColumns,
    getTableRows,
    insertRow,
    updateRow,
    deleteRow,
    createTable,
    deleteColumn,
    addColumn,
};

function getTables(schema) {
    return TablesApiProvider.getTables(schema);
}

function getTableColumns(schema, table) {
    return TablesApiProvider.getTableColumns(schema, table);
}

function getPredefinedColumns() {
    return TablesApiProvider.getPredefinedColumns();
}

function getTableRows(schema, table) {
    return TablesApiProvider.getTableRows(schema, table);
}

function insertRow(schema, table, rows) {
    return TablesApiProvider.insertRow(schema, table, rows);
}

function updateRow(schema, table, rows) {
    return TablesApiProvider.updateRow(schema, table, rows);
}

function deleteRow(schema, table, id) {
    return TablesApiProvider.deleteRow(schema, table, id);
}

function createTable(table, ColumnsInfo) {
    return TablesApiProvider.createTable(table, ColumnsInfo);
}

function deleteColumn(table, Column) {
    return TablesApiProvider.deleteColumn(table, Column);
}

function addColumn(table, ColumnsInfo) {
    return TablesApiProvider.addColumn(table, ColumnsInfo);
}
