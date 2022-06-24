import { tablesConstants } from '../constants';

let tablesList = JSON.parse(localStorage.getItem('tables_list'));
let currentTableColumns = JSON.parse(localStorage.getItem('current_table'));

const initialState = {
    loadingList: null,
    loadingCurrentTable: null,
    list: tablesList || null,
    currentTableColumns: currentTableColumns || null,
    deletingColumn: null,
    deletedColumnSuccess: null,
    addingColumn: null,
    addedColumnSuccess: null
};

export function currentTableInfoReducer(state = initialState, action) {
    switch (action.type) {
        case tablesConstants.GET_TABLE_COLUMNS_ACKNOWLEDGE:
            return {
                ...state,
                loadingCurrentTable: null,
            };
        case tablesConstants.GET_TABLE_COLUMNS_REQUEST:
            return {
                ...state,
                loadingCurrentTable: true,
            };
        case tablesConstants.GET_TABLE_COLUMNS_SUCCESS:
            return {
                ...state,
                loadingCurrentTable: false,
                currentTableColumns: action.table
            };
        case tablesConstants.GET_TABLE_COLUMNS_FAILURE:
            return {
                ...state,
                loadingCurrentTable: false,
                currentTableColumns: null
            };
        case tablesConstants.DELETE_TABLE_COLUMNS_ACKNOWLEDGE:
            return {
                ...state,
                deletingColumn: null,
                deletedColumnSuccess: null
            };
        case tablesConstants.DELETE_TABLE_COLUMNS_REQUEST:
            return {
                ...state,
                deletingColumn: true,
                deletedColumnSuccess: null
            };
        case tablesConstants.DELETE_TABLE_COLUMNS_SUCCESS:
            return {
                ...state,
                deletingColumn: false,
                deletedColumnSuccess: true
            };
        case tablesConstants.DELETE_TABLE_COLUMNS_FAILURE:
            return {
                ...state,
                deletingColumn: false,
                deletedColumnSuccess: false
            };
        case tablesConstants.ADD_TABLE_COLUMNS_ACKNOWLEDGE:
            return {
                ...state,
                addingColumn: null,
                addedColumnSuccess: null
            };
        case tablesConstants.ADD_TABLE_COLUMNS_REQUEST:
            return {
                ...state,
                addingColumn: true,
                addedColumnSuccess: null
            };
        case tablesConstants.ADD_TABLE_COLUMNS_SUCCESS:
            return {
                ...state,
                addingColumn: false,
                addedColumnSuccess: true
            };
        case tablesConstants.ADD_TABLE_COLUMNS_FAILURE:
            return {
                ...state,
                addingColumn: false,
                addedColumnSuccess: false
            };
        default:
            return state
    }
}