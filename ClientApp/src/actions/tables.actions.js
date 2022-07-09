import { tablesConstants } from "../constants";
import { tablesService } from "../services";
import { alertActions } from ".";
import { history } from "../helpers";
import { toastActions } from "./toast.actions";

export const tablesActions = {
    getTables,
    acknowledgeGetTables,
    getTableColumns,
    getPredefinedColumns,
    acknowledgePredefinedColumns,
    getTableRows,
    insertRow,
    updateRow,
    deleteRow,
    acknowledgeGetTableColumns,
    acknowledgeGetRows,
    acknowledgeInsertRow,
    acknowledgeUpdateRow,
    acknowledgeDeleteRow,
    createTable,
    acknowledgeCreateTable,
    deleteColumn,
    acknowledgeDeleteColumn,
    addColumn,
    acknowledgeAddColumn,
};

function getTables(schema) {
    return (dispatch) => {
        dispatch(request());

        tablesService.getTables(schema).then(
            (result) => {
                if (result.Success) {
                    let tableslist = result.Data;
                    // login successful if there's a jwt token in the response
                    // store table details and jwt token in local storage to keep table logged in between page refreshes

                    localStorage.setItem(
                        "tables_list",
                        JSON.stringify(tableslist)
                    );

                    dispatch(success(tableslist));
                    // history.push('/');
                    // dispatch(alertActions.success("Signed in!"));
                } else {
                    dispatch(failure(result.Message));
                    dispatch(alertActions.error("Error", result.Message));
                }
            },
            (error) => {
                dispatch(failure(error));
                dispatch(alertActions.error("Error", error));
            }
        );
    };

    function request() {
        return { type: tablesConstants.GET_TABLES_LIST_REQUEST };
    }
    function success(tableslist) {
        return { type: tablesConstants.GET_TABLES_LIST_SUCCESS, tableslist };
    }
    function failure(error) {
        return { type: tablesConstants.GET_TABLES_LIST_FAILURE, error };
    }
}

function acknowledgeGetTables() {
    return (dispatch) => {
        dispatch(acknowledge());
    };

    function acknowledge() {
        return { type: tablesConstants.GET_TABLES_LIST_ACKNOWLEDGE };
    }
}

function getTableColumns(schema, tablename) {
    return (dispatch) => {
        dispatch(request());

        tablesService.getTableColumns(schema, tablename).then(
            (result) => {
                if (result.Success) {
                    let columns = result.Data;
                    // login successful if there's a jwt token in the response
                    // store table details and jwt token in local storage to keep table logged in between page refreshes
                    const table = {
                        name: tablename,
                        columns: columns,
                    };
                    // localStorage.setItem(
                    //     "current_table",
                    //     JSON.stringify(table)
                    // );

                    dispatch(success(table));
                    // history.push('/');
                    // dispatch(alertActions.success("Signed in!"));
                } else {
                    dispatch(failure(result.Message));
                    dispatch(alertActions.error("Error", result.Message));
                }
            },
            (error) => {
                dispatch(failure(error));
                dispatch(alertActions.error("Error", error));
            }
        );
    };

    function request() {
        return { type: tablesConstants.GET_TABLE_COLUMNS_REQUEST };
    }
    function success(table) {
        return { type: tablesConstants.GET_TABLE_COLUMNS_SUCCESS, table };
    }
    function failure(error) {
        return { type: tablesConstants.GET_TABLE_COLUMNS_FAILURE, error };
    }
}

function acknowledgeGetTableColumns() {
    return (dispatch) => {
        dispatch(acknowledge());
    };

    function acknowledge() {
        return { type: tablesConstants.GET_TABLE_COLUMNS_ACKNOWLEDGE };
    }
}

function getTableRows(schema, tablename) {
    return (dispatch) => {
        dispatch(request());
        tablesService.getTableRows(schema, tablename).then(
            (result) => {
                if (result.Success) {
                    let rows = result.Data;
                    // login successful if there's a jwt token in the response
                    // store table details and jwt token in local storage to keep table logged in between page refreshes
                    // localStorage.setItem(
                    //     "current_table_rows",
                    //     JSON.stringify(rows)
                    // );

                    dispatch(success(rows));
                    // history.push('/');
                    // dispatch(alertActions.success("Signed in!"));
                } else {
                    dispatch(failure(result.Message));
                    dispatch(alertActions.error("Error", result.Message));
                }
            },
            (error) => {
                dispatch(failure(error));
                dispatch(alertActions.error("Error", error));
            }
        );
    };

    function request() {
        return { type: tablesConstants.GET_TABLE_ROWS_REQUEST };
    }
    function success(rows) {
        return { type: tablesConstants.GET_TABLE_ROWS_SUCCESS, rows };
    }
    function failure(error) {
        return { type: tablesConstants.GET_TABLE_ROWS_FAILURE, error };
    }
}

function acknowledgeGetRows() {
    return (dispatch) => {
        dispatch(acknowledge());
    };

    function acknowledge() {
        return { type: tablesConstants.GET_TABLE_ROWS_ACKNOWLEDGE };
    }
}

function getPredefinedColumns() {
    return (dispatch) => {
        dispatch(request());

        tablesService.getPredefinedColumns().then(
            (result) => {
                if (result.Success) {
                    let predefinedColumns = result.Data;
                    // login successful if there's a jwt token in the response
                    // store table details and jwt token in local storage to keep table logged in between page refreshes

                    localStorage.setItem(
                        "predefined_Columns",
                        JSON.stringify(predefinedColumns)
                    );

                    dispatch(success(predefinedColumns));
                    // history.push('/');
                    // dispatch(alertActions.success("Signed in!"));
                } else {
                    dispatch(failure(result.Message));
                    dispatch(alertActions.error("Error", result.Message));
                }
            },
            (error) => {
                dispatch(failure(error));
                dispatch(alertActions.error("Error", error));
            }
        );
    };

    function request() {
        return { type: tablesConstants.GET_PREDEFINED_COLUMNS_REQUEST };
    }
    function success(predefinedColumns) {
        return {
            type: tablesConstants.GET_PREDEFINED_COLUMNS_SUCCESS,
            predefinedColumns,
        };
    }
    function failure(error) {
        return { type: tablesConstants.GET_PREDEFINED_COLUMNS_FAILURE, error };
    }
}

function acknowledgePredefinedColumns() {
    return (dispatch) => {
        dispatch(acknowledge());
    };

    function acknowledge() {
        return { type: tablesConstants.GET_PREDEFINED_COLUMNS_ACKNOWLEDGE };
    }
}

function insertRow(schema, table, row) {
    return (dispatch) => {
        dispatch(request(row));
        tablesService.insertRow(schema, table, row).then(
            (result) => {
                if (result.Success) {
                    dispatch(success(row));
                    // history.push(link);
                    dispatch(toastActions.success("Row Inserted"));
                    // dispatch(getTableRow(table));
                    return "Row inserted";
                } else {
                    dispatch(failure(result.Message));
                    dispatch(alertActions.error("Error", result.Message));
                    return result.Message;
                }
            },
            (error) => {
                dispatch(failure(error.toString()));
                dispatch(alertActions.error("Error", error.toString()));
            }
        );
    };

    function request(row) {
        return { type: tablesConstants.INSERT_TABLE_ROWS_REQUEST, row };
    }
    function success(row) {
        return { type: tablesConstants.INSERT_TABLE_ROWS_SUCCESS, row };
    }
    function failure(error) {
        return { type: tablesConstants.INSERT_TABLE_ROWS_FAILURE, error };
    }
}

function acknowledgeInsertRow() {
    return (dispatch) => {
        dispatch(acknowledge());
    };

    function acknowledge() {
        return { type: tablesConstants.INSERT_TABLE_ROWS_ACKNOWLEDGE };
    }
}

function updateRow(schema, table, row) {
    return (dispatch) => {
        dispatch(request(row));

        tablesService.updateRow(schema, table, row).then(
            (row) => {
                dispatch(success(row));
                // history.push(link);
                dispatch(toastActions.success("Row Updated"));
                // dispatch(getTableRow(table));
                return "Row inserted";
            },
            (error) => {
                dispatch(failure(error.toString()));
                dispatch(alertActions.error("Error", error.toString()));
            }
        );
    };

    function request(row) {
        return { type: tablesConstants.UPDATE_TABLE_ROWS_REQUEST, row };
    }
    function success(row) {
        return { type: tablesConstants.UPDATE_TABLE_ROWS_SUCCESS, row };
    }
    function failure(error) {
        return { type: tablesConstants.UPDATE_TABLE_ROWS_FAILURE, error };
    }
}

function acknowledgeUpdateRow() {
    return (dispatch) => {
        dispatch(acknowledge());
    };

    function acknowledge() {
        return { type: tablesConstants.UPDATE_TABLE_ROWS_ACKNOWLEDGE };
    }
}

function deleteRow(schema, table, id) {
    return (dispatch) => {
        dispatch(request(id));

        tablesService.deleteRow(schema, table, id).then(
            (user) => dispatch(success(id)),
            (error) => dispatch(failure(id, error.toString()))
        );
    };

    function request(id) {
        return { type: tablesConstants.DELETE_TABLE_ROWS_REQUEST, id };
    }
    function success(id) {
        return { type: tablesConstants.DELETE_TABLE_ROWS_SUCCESS, id };
    }
    function failure(id, error) {
        return { type: tablesConstants.DELETE_TABLE_ROWS_FAILURE, id, error };
    }
}

function acknowledgeDeleteRow() {
    return (dispatch) => {
        dispatch(acknowledge());
    };

    function acknowledge() {
        return { type: tablesConstants.DELETE_TABLE_ROWS_ACKNOWLEDGE };
    }
}

function createTable(table, ColumnsInfo) {
    return (dispatch) => {
        dispatch(request(ColumnsInfo));

        tablesService.createTable(table, ColumnsInfo).then(
            (result) => {
                if (result.Success) {
                    dispatch(success());
                    // history.push(link);
                    dispatch(toastActions.success("Table Created"));
                    // dispatch(getTableRows(table));
                    return "Table Created";
                } else {
                    dispatch(failure(result.Message));
                    dispatch(alertActions.error("Error", result.Message));
                    return result.Message;
                }
            },
            (error) => {
                dispatch(failure(error.toString()));
                dispatch(alertActions.error("Error", error.toString()));
            }
        );
    };

    function request(ColumnsInfo) {
        return { type: tablesConstants.CREATE_TABLE_REQUEST, ColumnsInfo };
    }
    function success(ColumnsInfo) {
        return { type: tablesConstants.CREATE_TABLE_SUCCESS, ColumnsInfo };
    }
    function failure(error) {
        return { type: tablesConstants.CREATE_TABLE_FAILURE, error };
    }
}

function acknowledgeCreateTable() {
    return (dispatch) => {
        dispatch(acknowledge());
    };

    function acknowledge() {
        return { type: tablesConstants.CREATE_TABLE_ACKNOWLEDGE };
    }
}

function deleteColumn(schema, table, Column) {
    return (dispatch) => {
        dispatch(request(Column));

        tablesService.deleteColumn(schema, table, Column).then(
            (result) => {
                if (result.Success) {
                    dispatch(success());
                    // history.push(link);
                    dispatch(toastActions.success("Column Deleted"));
                    // dispatch(getTableRows(table));
                    return "Column Deleted";
                } else {
                    dispatch(failure(result.Message));
                    dispatch(alertActions.error("Error", result.Message));
                    return result.Message;
                }
            },
            (error) => {
                dispatch(failure(error.toString()));
                dispatch(alertActions.error("Error", error.toString()));
            }
        );
    };

    function request(Column) {
        return { type: tablesConstants.DELETE_TABLE_COLUMNS_REQUEST, Column };
    }
    function success(Column) {
        return { type: tablesConstants.DELETE_TABLE_COLUMNS_SUCCESS, Column };
    }
    function failure(error) {
        return { type: tablesConstants.DELETE_TABLE_COLUMNS_FAILURE, error };
    }
}

function acknowledgeDeleteColumn() {
    return (dispatch) => {
        dispatch(acknowledge());
    };

    function acknowledge() {
        return { type: tablesConstants.DELETE_TABLE_COLUMNS_ACKNOWLEDGE };
    }
}

function addColumn(schema, table, ColumnsInfo) {
    return (dispatch) => {
        dispatch(request(ColumnsInfo));

        tablesService.addColumn(schema, table, ColumnsInfo).then(
            (result) => {
                if (result.Success) {
                    console.log(ColumnsInfo);
                    dispatch(success(ColumnsInfo));
                    dispatch(toastActions.success("Column Added"));
                    return "Column Added";
                } else {
                    dispatch(failure(result.Message));
                    dispatch(alertActions.error("Error", result.Message));
                    return result.Message;
                }
            },
            (error) => {
                dispatch(failure(error.toString()));
                dispatch(alertActions.error("Error", error.toString()));
            }
        );
    };

    function request(ColumnsInfo) {
        return { type: tablesConstants.ADD_TABLE_COLUMNS_REQUEST, ColumnsInfo };
    }
    function success(ColumnsInfo) {
        return { type: tablesConstants.ADD_TABLE_COLUMNS_SUCCESS, ColumnsInfo };
    }
    function failure(error) {
        return { type: tablesConstants.ADD_TABLE_COLUMNS_FAILURE, error };
    }
}

function acknowledgeAddColumn() {
    return (dispatch) => {
        dispatch(acknowledge());
    };

    function acknowledge() {
        return { type: tablesConstants.ADD_TABLE_COLUMNS_ACKNOWLEDGE };
    }
}
