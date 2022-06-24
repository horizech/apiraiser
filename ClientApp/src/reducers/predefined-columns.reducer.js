import { tablesConstants } from '../constants';

// let tablesList = JSON.parse(localStorage.getItem('tables_list'));
let predefinedColumns = JSON.parse(localStorage.getItem('predefined_Columns'));

const initialState = {
    // loadingList: false,
    loadingpredefinedColumns: false,
    // list: tablesList || null,
    predefinedColumns: predefinedColumns || null,
};

export function predefinedColumnsReducer(state = initialState, action) {
    switch (action.type) {
        case tablesConstants.GET_PREDEFINED_COLUMNS_REQUEST:
            return {
                loadingpredefinedColumns: true,
            };
        case tablesConstants.GET_PREDEFINED_COLUMNS_SUCCESS:
            return {
                loadingpredefinedColumns: false,
                predefinedColumns: action.predefinedColumns
            };
        case tablesConstants.GET_PREDEFINED_COLUMNS_FAILURE:
            return {
                loadingpredefinedColumns: false,
                predefinedColumns: null
            };
        default:
            return state
    }
}