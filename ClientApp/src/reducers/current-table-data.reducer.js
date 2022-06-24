import { tablesConstants } from '../constants';

let tablesList = JSON.parse(localStorage.getItem('tables_list'));
let currentTableRows = JSON.parse(localStorage.getItem('current_table_rows'));
const initialState = {
    loadingList: null,
    loadingCurrentTableRows: null,
    list: tablesList || null,
    currentTableRows: currentTableRows || null,
    insertingRow: null,
    insertedRowSuccess: null,
    updatingRow: null,
    updatedRowSuccess: null,
    deletingRow: null,
    deletedRowSuccess: null,
};

export function currentTableDataReducer(state = initialState, action) {
    switch (action.type) {
        
        case tablesConstants.GET_TABLE_ROWS_ACKNOWLEDGE:
            return {
                ...state,
                loadingCurrentTableRows: null
            };
        case tablesConstants.GET_TABLE_ROWS_REQUEST:
            return {
                ...state,
                loadingCurrentTableRows: true,
                currentTableRows: null
            };
        case tablesConstants.GET_TABLE_ROWS_SUCCESS:
            return {
                ...state,
                loadingCurrentTableRows: false,
                currentTableRows: action.rows
            };
        case tablesConstants.GET_TABLE_ROWS_FAILURE:
            return {
                ...state,
                loadingCurrentTableRows: false,
                currentTableRows: null
            };

        case tablesConstants.INSERT_TABLE_ROWS_ACKNOWLEDGE:
            return {
                ...state,
                insertingRow: null,
                insertedRowSuccess: null
            };
        case tablesConstants.INSERT_TABLE_ROWS_REQUEST:
            return {
                ...state,
                insertingRow: true,
                insertedRowSuccess: null
            };
        case tablesConstants.INSERT_TABLE_ROWS_SUCCESS:
            return {
                ...state,
                insertingRow: false,
                insertedRowSuccess: true
            };
        case tablesConstants.INSERT_TABLE_ROWS_FAILURE:
            return {
                ...state,
                insertingRow: false,
                insertedRowSuccess: false
            };

        case tablesConstants.UPDATE_TABLE_ROWS_ACKNOWLEDGE:
            return {
                ...state,
                updatingRow: null,
                updatedRowSuccess: null
            };
        case tablesConstants.UPDATE_TABLE_ROWS_REQUEST:
            return {
                ...state,
                updatingRow: true,
                updatedRowSuccess: null
            };
        case tablesConstants.UPDATE_TABLE_ROWS_SUCCESS:
            return {
                ...state,
                updatingRow: false,
                updatedRowSuccess: true
            };
        case tablesConstants.UPDATE_TABLE_ROWS_FAILURE:
            return {
                ...state,
                updatingRow: false,
                updatedRowSuccess: false
            };

        case tablesConstants.DELETE_TABLE_ROWS_ACKNOWLEDGE:
            return {
                ...state,
                deletingRow: null,
                deletedRowSuccess: null
            };
        case tablesConstants.DELETE_TABLE_ROWS_REQUEST:
            return {
                ...state,
                deletingRow: true,
                deletedRowSuccess: null
            };
        case tablesConstants.DELETE_TABLE_ROWS_SUCCESS:
            return {
                ...state,
                deletingRow: false,
                deletedRowSuccess: true
            };
        case tablesConstants.DELETE_TABLE_ROWS_FAILURE:
            return {
                ...state,
                deletingRow: false,
                deletedRowSuccess: false
            };
        default:
        return state
    }
}