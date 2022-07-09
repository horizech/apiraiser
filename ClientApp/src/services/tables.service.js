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

function createTable(schema, table, ColumnsInfo) {
    return TablesApiProvider.createTable(schema, table, ColumnsInfo);
}

function deleteColumn(schema, table, Column) {
    return TablesApiProvider.deleteColumn(schema, table, Column);
}

function addColumn(schema, table, ColumnsInfo) {
    return TablesApiProvider.addColumn(schema, table, ColumnsInfo);
}
