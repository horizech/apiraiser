import { tablesConstants } from '../constants';

let tablesList = JSON.parse(localStorage.getItem('tables_list'));

const initialState = {
    loadedTablesSuccess: null,
    loadingTablesList: false,
    tableslist: tablesList || null,
    createdTable: null,
    creatingTable: null,
    createdTableSuccess: false

};

export function tablesReducer(state = initialState, action) {
    switch (action.type) {
        case tablesConstants.GET_TABLES_LIST_ACKNOWLEDGE:
                return {
                    ...state,
                    loadingTablesList: false,
                    loadedTablesSuccess: null
                };
        case tablesConstants.GET_TABLES_LIST_REQUEST:
            return {
                ...state,
                loadingTablesList: true,
                loadedTablesSuccess: null
            };
        case tablesConstants.GET_TABLES_LIST_SUCCESS:
            return {
                ...state,
                loadingTablesList: false,
                tableslist: action.tableslist,
                loadedTablesSuccess: true
                
            };
        case tablesConstants.GET_TABLES_LIST_FAILURE:
            return {
                ...state,
                loadingTablesList: false,
                tableslist: null,
                loadedTablesSuccess: false
            };
            case tablesConstants.CREATE_TABLE_ACKNOWLEDGE:
                return {
                    ...state,
                    creatingTable: null,
                    createdTableSuccess: null
                };
            case tablesConstants.CREATE_TABLE_REQUEST:
                return {
                    ...state,
                    creatingTable: true,
                    createdTableSuccess: null
                };
            case tablesConstants.CREATE_TABLE_SUCCESS:
                return {
                    ...state,
                    creatingTable: false,
                    createdTable: action.table,
                    createdTableSuccess: true
                };
            case tablesConstants.CREATE_TABLE_FAILURE:
                return {
                    ...state,
                    creatingTable: false,
                    createdTable: null,
                    createdTableSuccess: false
                };
        default:
            return state
    }
}