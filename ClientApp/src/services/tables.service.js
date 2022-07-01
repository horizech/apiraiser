import { TablesApiProvider } from "../api_providers";

export const tablesService = {
  getTables,
  getTableColumns,
  getSystemTables,
  getSystemTableColumns,
  getApplicationTables,
  getApplicationTableColumns,
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

function getApplicationTables() {
  return TablesApiProvider.getApplicationTables();
}

function getApplicationTableColumns(table) {
  return TablesApiProvider.getApplicationTableColumns(table);
}

function getSystemTables() {
  return TablesApiProvider.getSystemTables();
}

function getSystemTableColumns(table) {
  return TablesApiProvider.getSystemTableColumns(table);
}

function getPredefinedColumns() {
  return TablesApiProvider.getPredefinedColumns();
}

function getTableRows(table) {
  return TablesApiProvider.getTableRows(table);
}
function insertRow(table, rows) {
  return TablesApiProvider.insertRow(table, rows);
}

function updateRow(table, rows) {
  return TablesApiProvider.updateRow(table, rows);
}

function deleteRow(table, id) {
  return TablesApiProvider.deleteRow(table, id);
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
